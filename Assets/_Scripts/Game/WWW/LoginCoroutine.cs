using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class LoginCoroutine : WebCoroutine<string, string> {	
	public LoginCoroutine() {
		AddExecutionHandler( OnExecuteStage1 );
		AddExecutionHandler( OnExecuteStage2 );
		AddExecutionHandler( OnExecuteStage3 );
		AddExecutionHandler( OnExecuteStage4 );
	}
	
	protected IEnumerator OnExecuteStage1( string username, string password, AWebCoroutine self ) {
		// ensure user name was entered
		if (string.IsNullOrEmpty(username)) {
			//loginStatus = "You must enter a valid email address.";
			yield break;
		}
		// ensure a password was entered
		if (string.IsNullOrEmpty(password)) {
			//loginStatus = "You must enter a password.";
			yield break;
		}
		
		// try to log in
		//loginStatus = "Logging in...";
		
		Dictionary<string, string> dict = DataStructureLibrary< Dictionary<string, string> >.Instance.CheckOut();
		dict.Clear();
		
		dict.Add( "email", username );
		dict.Add( "password", password );
		
		HTTP.JSON json = new HTTP.JSON();
		json.Data = JsonMapper.ToJson( dict );

		self.Request = new HTTP.Request( "Get", WebRequests.urlLogin );
		self.Request.SetText( json );
		
		DataStructureLibrary< Dictionary<string, string> >.Instance.Return( dict );
		
		self.Request.Send();

		while( !self.Request.isDone ) {
			yield return 0;
		}
		
		if (self.Request.ProducedError) {
			self.cancelled = true;
			WebRequests.loginStatus = "Invalid username or password.";
			yield break;
		}
		yield break;
	}
	
	protected IEnumerator	OnExecuteStage2( string username, string password, AWebCoroutine self ) {
		Func<string, UserData>	userDataDecodeDelegate = JSONDecoder.Decode<UserData>;
		IAsyncResult ar = userDataDecodeDelegate.BeginInvoke( self.ResponseText, null, null );
		while( !ar.IsCompleted ) {
			yield return 0;
		}
		UserData.current = userDataDecodeDelegate.EndInvoke( ar );
		
		// make sure the player is valid
		if (UserData.current == null) {
            //loginStatus = "Invalid login";
			self.cancelled = true;
			WebRequests.loginStatus = "Invalid username or password.";
			yield break;
		}
       
		// login is successful!
		//loginStatus = "";
		
		// cache player user name and server
		GamePrefs.SetUser( username );
		GamePrefs.SetServerURL( WebRequests.urlServer );
	}
	
	protected IEnumerator	OnExecuteStage3( string username, string password, AWebCoroutine self ) {
		LoginGUI.worlds = null;
		LoginGUI.playerData = null;

		self.Request = new HTTP.Request( "Get", WebRequests.urlGetWorldList );
		self.Request.AddParameters( WebRequests.authenticatedParameters );
		self.Request.Send();
	
		while( !self.Request.isDone ) {
			yield return 0;
		}

		Func<string, WorldsData>	worldsDataDecodeDelegate = JSONDecoder.Decode<WorldsData>;
		IAsyncResult ar = worldsDataDecodeDelegate.BeginInvoke( self.ResponseText, null, null );
		while( !ar.IsCompleted ) {
			yield return 0;
		}
		
		LoginGUI.worlds = worldsDataDecodeDelegate.EndInvoke( ar );
	}
	
	protected IEnumerator	OnExecuteStage4( string username, string password, AWebCoroutine self ) {
		// get world list
        //loginStatus = "";
		self.Request = new HTTP.Request( "Get", WebRequests.urlGetUserPlayers() );
		self.Request.AddParameters( WebRequests.authenticatedParameters );
		self.Request.Send();
		
		while( !self.Request.isDone ) {
			yield return 0;
		}
		
		// deserialize the login JSON text
		Func<string, PlayersData>	playersDataDecodeDelegate = JSONDecoder.Decode<PlayersData>;
		IAsyncResult ar = playersDataDecodeDelegate.BeginInvoke( self.ResponseText, null, null );
		while( !ar.IsCompleted ) {
			yield return 0;
		}
		
		LoginGUI.playerData = playersDataDecodeDelegate.EndInvoke( ar );
	}
}