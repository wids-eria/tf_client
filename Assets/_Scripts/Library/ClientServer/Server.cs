using System;
using System.Net;
using System.Net.Sockets;

public class Server {
	protected IPAddress		address;
	protected TcpListener	listener;
	protected Socket		socket;
	protected byte[]		receiveBuffer = new byte[ 1000 ];
	
	public Action<byte[]>	OnReceive = null;
	public Action			OnConnect = null;
	
	public Server() {
	}
	
	public	void	StartListenTo( string addr, int port ) {
		address = IPAddress.Parse( addr );
		listener = new TcpListener( address, port );
		
		listener.Start();
		
		listener.BeginAcceptSocket( new AsyncCallback(OnConnectHandler), null );
	}
	
	public void		OnConnectHandler( IAsyncResult ar ) {
		socket = listener.EndAcceptSocket( ar );
		
		if( OnConnect != null ) {
			OnConnect();
		}
		
		Receive();
	}
	
	public void		Disconnect() {
	}
	
	protected void	Receive() {
		socket.BeginReceive( receiveBuffer, 0, receiveBuffer.Length, SocketFlags.None, new AsyncCallback(EndReceive), null );
	}
	
	protected void	EndReceive( IAsyncResult ar ) {
		try {
			int nBytes;
			nBytes = socket.EndReceive( ar );
			if( nBytes <= 0 )
			{
				Disconnect();
				return;
			}
			
			// uncompress Data if we are on a compressed socket
            //if (m_Compressed)
            //{                        
            //    byte[] buf = Decompress(m_ReadBuffer, nBytes);
            //    base.FireOnReceive(buf, buf.Length);
            // for compression debug statistics
            	//base.FireOnInComingCompressionDebug(this, m_ReadBuffer, nBytes, buf, buf.Length);
            //}
            //else
            //{
            	//Console.WriteLine("Socket OnReceive: " + System.Text.Encoding.UTF8.GetString(m_ReadBuffer, 0, nBytes));                        
                // Raise the receive event
            //    base.FireOnReceive(m_ReadBuffer, nBytes);
            //}
				
			if( OnReceive != null ) {
				OnReceive( receiveBuffer );
			}
				
			// Setup next Receive Callback
			//if (this.Connected)
				Receive();
		}
		catch(ObjectDisposedException)
		{
			//object already disposed, just exit
			return;
		}
		catch (System.IO.IOException ex)
		{
			Console.WriteLine("\nSocket Exception: " + ex.Message);
			Disconnect();
		}
	}
	
	public void	Send( byte[] data ) {
		socket.BeginSend( data, 0, data.Length, SocketFlags.None, new AsyncCallback(EndSend), null );
	}
	
	protected void	EndSend( IAsyncResult ar ) {
		socket.EndSend( ar );
	}
}