using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ClientNetworkConnection : NetworkConnection {
	protected Client 				connection = new Client();
	protected Dictionary<int, ServerCommandDecoderBase>	decoders = new Dictionary<int, ServerCommandDecoderBase>();
	
	protected virtual void	Awake() {
		connection.OnConnect += OnConnect;

		connection.OnReceive += OnReceive;
	}
	
	protected virtual void	Start() {
	}
	
	// This is wrong - will fix soon
	public override	void	ConnectTo( string addr, int port ) {
		StartCoroutine( OpenConnection(addr, port) );
	}
	
	protected IEnumerator OpenConnection( string addr, int port ) {
		connection.ConnectTo( addr, port );
		yield return new WaitForSeconds(0);
	}
	
	//protected void	AddCodec( ServerCommandDeclaration codec ) {
	//	ServerCommandDecoderBase decoder = FindDecoder( codec.NumArguments );
	//	decoder.AddCodec( codec );
	//}
	
	protected ServerCommandDecoderBase	FindDecoder( int numParams ) {
		return decoders[ numParams ];
	}
	
	public override  void	Send( byte[] data ) {
		Debug.Log( "ClientNetworkConnection.Send..." );
		connection.Send( data );
	}
	
	//protected void	OnReceive( byte[] data ) {
	//}
	
	public override Action	OnConnect {
		get {
			return connection.OnConnect;
		}
		
		set {
			connection.OnConnect = value;
		}
	}
	
	protected void	OnReceive( byte[] data ) {	
	}
}