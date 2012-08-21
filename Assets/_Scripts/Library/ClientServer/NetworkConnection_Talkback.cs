using UnityEngine;
using System;
using System.Collections;

public class NetworkConnection_Talkback : ClientNetworkConnection {
	Server serverConnection = new Server();
	
	protected override void	Awake() {
		serverConnection.OnConnect += OnConnect;
		serverConnection.OnReceive += OnServerReceive;
		
		base.Awake();
		
		StartCoroutine( StartListening() );
	}
	
	IEnumerator StartListening() {
		string addr = NetworkUtils.DetermineLocalAddress();
		serverConnection.StartListenTo( addr, 2001 );
		yield return new WaitForSeconds(0);
	}
	
	protected void	OnServerReceive( byte[] data ) {
		Debug.Log( "OnServerReceive..." );
		serverConnection.Send( data );
	}
	
	protected void	OnServerConnect() {
		Debug.Log( "Server connected..." );
	}
}