using System;
using System.Net;
using System.Net.Sockets;

public class Client {
	protected TcpClient		caller = new TcpClient();
	protected NetworkStream	stream;
	protected byte[]		receiveBuffer = new byte[ 1000 ];
	
	public Action<byte[]>	OnReceive;
	public Action			OnConnect;
	
	public void	ConnectTo( string addr, int port ) {
		caller.BeginConnect( addr, port, new AsyncCallback(OnConnectHandler), null );
	}
	
	protected void	OnConnectHandler( IAsyncResult ar ) {
		caller.EndConnect( ar );
		stream = caller.GetStream();
		
		if( OnConnect != null ) {
			OnConnect();
		}
		
		Receive();
	}
	
	public void	Disconnect() {
		stream.Close();
		caller.Close();
	}
	
	public void	Send( byte[] data ) {
		stream.BeginWrite( data, 0, data.Length, new AsyncCallback(EndSend), null );
	}
	
	protected void	EndSend( IAsyncResult ar ) {
		stream.EndWrite( ar );
	}
	
	public void	Receive() {
		stream.BeginRead( receiveBuffer, 0, receiveBuffer.Length, new AsyncCallback(EndReceive), null );
	}
	
	protected void	EndReceive( IAsyncResult ar ) {
		int nBytes = stream.EndRead( ar );
		if( nBytes <= 0 ) {
			return;
		}

		if( OnReceive != null ) {
			OnReceive( receiveBuffer );
		}
		
		Receive();
	}
}