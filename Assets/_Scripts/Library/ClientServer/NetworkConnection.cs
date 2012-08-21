using UnityEngine;
using System;

public abstract class NetworkConnection : MonoBehaviour {
	protected static ClientNetworkConnection		instance = null;
	public static ClientNetworkConnection		Instance {
		get {
			if( instance == null ) {
				GameObject go = new GameObject( "NetworkConnection" );
				instance = go.AddComponent<NetworkConnection_Talkback>();
			}
			return instance;
		}
	}
	
	public void	Destroy() {
		Destroy( gameObject );
	}
	
	public abstract	void	ConnectTo( string addr, int port );
	
	public abstract  void	Send( byte[] data );
	
	public abstract Action		OnConnect {
		get;
		set;
	}
}