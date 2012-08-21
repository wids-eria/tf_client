using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Class for issuing web requests.
/// </summary>
public static class WebRequests : System.Object
{
	/// <summary>
	/// Gets the authenticated parameters.
	/// </summary>
	/// <value>
	/// The authenticated parameters.
	/// </value>
	public static Dictionary<string, string> authenticatedParameters {
		get {
			return new Dictionary<string, string> { { "auth_token", UserData.current.auth_token } };
		}
	}
	
	/// <summary>
	/// Gets the authenticated god mode parameters.
	/// </summary>
	/// <value>
	/// The authenticated god mode parameters.
	/// </value>
	public static Dictionary<string, string> authenticatedGodModeParameters {
		get {
			Dictionary<string, string> ret = authenticatedParameters;
			ret.Add("god_mode", "iddqd");
			return ret;
		}
	}
	
	/// <summary>
	/// The URL server.
	/// </summary>
	public static string urlServer = GamePrefs.GetServerURL();
	
	/// <summary>
	/// Gets the URL to register a new player.
	/// </summary>
	/// <value>
	/// The URL to register a new player.
	/// </value>
	public static string urlRegisterNewPlayer {
		get {
			return string.Format("{0}/users/sign_up", urlServer);
		}
	}
	
	/// <summary>
	/// Gets the URL for the login.
	/// </summary>
	/// <value>
	/// The URL for the login.
	/// </value>
	public static string urlLogin {
		get {
			return string.Format("{0}/users/authenticate_for_token.json", urlServer);
		}
	}
	
	/// <summary>
	/// Gets the URL for the world list.
	/// </summary>
	/// <value>
	/// The URL for the world list.
	/// </value>
	public static string urlGetWorldList {
		get {
			return string.Format(
				"{0}/worlds.json",
				urlServer
			);
		}
	}
		
	/// <summary>
	/// Gets the URL for the world.
	/// </summary>
	/// <value>
	/// The URL for the world.
	/// </value>
	public static string urlGetWorld {
		get {
			return string.Format(
				"{0}/worlds/{1}.json",
				urlServer,
				UserData.worldNumber
			);
		}
	}
	
	/// <summary>
	/// Gets the URL for the player.
	/// </summary>
	/// <value>
	/// The URL for the player.
	/// </value>
	public static string urlGetUserPlayers(int userId) {
		return string.Format("{0}/users/{1}/players.json",urlServer,userId);
	}
	public static string urlGetUserPlayers() {
		return string.Format("{0}/users/{1}/players.json",urlServer,UserData.current.id);
	}
	public static string urlGetUserPlayerId(int userId, int playerId) {
		return string.Format("{0}/users/{1}/players/{2}.json",urlServer,userId,playerId);
	}
	
	public static string urlGetWorldPlayers {
		get { return string.Format("{0}/worlds/{1}/players.json", urlServer,UserData.worldNumber); }
	}
	
	public static string urlGetCurrentPlayer {
		get { return string.Format("{0}/users/{1}/players/{2}.json", urlServer,UserData.current.id, GameManager.use.currentPlayer.id); }
	}
	
	public static string urlGetPlayerInWorld( int worldId, int playerId ) {
		return string.Format("{0}/worlds/{1}/players/{2}.json", urlServer, worldId, playerId );
	}
	
	public static string urlDeletePlayer( int worldId, int playerId ) {
		return string.Format( "{0}/worlds/{1}/players/{2}.json", urlServer, worldId, playerId );
	}
	
	/// <summary>
	/// Gets the URL for megatiles.
	/// </summary>
	/// <value>
	/// The URL for megatiles.
	/// </value>
	public static string urlGetMegatiles {
		get {
			return string.Format(
				"{0}/worlds/{1}/megatiles.json",
				urlServer,
				UserData.worldNumber
			);
		}
	}
	
	public static string GetMegatileURL(int idMegatile) {
		return string.Format( 
			"{0}/worlds/{1}/megatiles/{2}.json",
			urlServer,
			UserData.worldNumber,
			idMegatile);
	}
	
	public static string urlPurchaseMegatile {
		get {
			return string.Format(
				"{0}/worlds/{1}/megatiles/{2}/buy.json",
				urlServer,
				UserData.worldNumber,
				InputManager.use.GetSelectedMegatileIds()[0]
			);
		}
	}
		
	public static string urlSurveyMegatile {
		get {
			return string.Format(
				"{0}/worlds/{1}/megatiles/{2}/surveys.json",
				urlServer,
				UserData.worldNumber,
				InputManager.use.GetSelectedMegatileIds()[0]
			);
		}
	}	
			
	/// <summary>
	/// Gets the URL for resource tiles.
	/// </summary>
	/// <value>
	/// The URL for resource tiles.
	/// </value>
	public static string urlGetResourceTiles {
		get {
			return string.Format(
				"{0}/worlds/{1}/resource_tiles.json",
				urlServer,
				UserData.worldNumber
			);
		}
	}
	
	/// <summary>
	/// Gets the URL to resource tile.
	/// </summary>
	/// <returns>
	/// The URL to resource tile.
	/// </returns>
	/// <param name='id'>
	/// Identifier.
	/// </param>
	public static string GetURLToResourceTile(int id)
	{
		return string.Format(
			"{0}/worlds/{1}/resource_tiles/{2}.json",
			urlServer,
			UserData.worldNumber,
			id
		);
	}
	public static string GetResourceTileURL(int id) {
		return string.Format(
			"{0}/worlds/{1}/resource_tiles/{2}/",
			urlServer,
			UserData.worldNumber,
			id
		);
	}
	
	/// <summary>
	/// Gets the URL for resource tile permissions.
	/// </summary>
	/// <value>
	/// The URL for resource tile permissions.
	/// </value>
	public static string urlGetResourceTilePermissions {
		get {
			return string.Format(
				"{0}/permitted_actions.json",
				urlGetResourceTiles.Substring(0, urlGetResourceTiles.LastIndexOf("."[0]))
			);
		}
	}
	
	/// <summary>
	/// Gets the URL to get the selected megatile by id.
	/// </summary>
	/// <value>
	/// The URL to get selected megatile.
	/// </value>
	public static string urlGetSelectedMegatile {
		get {
			return string.Format(
				"{0}/worlds/{1}/megatiles/{2}.json",
				urlServer,
				UserData.worldNumber,
				InputManager.use.GetSelectedMegatileIds()[0] //<< -- HACK: We may want to do all, or a sort...
			);
		}
	}
	
	/// <summary>
	/// Gets the URL put player turn done.
	/// </summary>
	/// <value>
	/// The URL put player turn done.
	/// </value>
	public static string urlPutPlayerTurnDone {
		get {
			return string.Format("{0}/worlds/{1}/players/submit_turn.json",urlServer,UserData.worldNumber);
		}
	}
	
	/// <summary>
	/// Gets the state of the URL get turn, which contains time left and state.
	/// </summary>
	/// <value>
	/// The state of the URL get turn.
	/// </value>
	public static string urlGetTurnState {
		get {
			return string.Format("{0}/worlds/{1}/turn_state.json",urlServer,UserData.worldNumber);
		}
	}
	
	/// <summary>
	/// Adds the megatile region to parameters.
	/// </summary>
	/// <param name='region'>
	/// Region.
	/// </param>
	/// <param name='parameters'>
	/// Parameters.
	/// </param>
	public static void AddMegatileRegionToParameters(Region region, Dictionary<string, string> parameters)
	{
		// these have to fall on exact values
		region.EncapsulateMegatiles();
		parameters.Add("x_min", region.left.ToString());
		parameters.Add("x_max", region.right.ToString());
		parameters.Add("y_min", Megatile.ToServerY(region.top).ToString());
		parameters.Add("y_max", Megatile.ToServerY(region.bottom).ToString());
	}
	
	/// <summary>
	/// Gets or sets the login status.
	/// </summary>
	/// <value>
	/// The login status.
	/// </value>
	public static string loginStatus { get; set; }
	
	public static IEnumerator	GetWorldList( AWebCoroutine handler ) {
        loginStatus = "";

		handler.Request = new HTTP.Request( "Get", urlGetWorldList );
		handler.Request.AddParameters( WebRequests.authenticatedParameters );
		handler.Request.Send();
		while( !handler.IsDone ) {
			yield return 0;
		}
		
		Func<string, WorldsData>	worldsDataDecodeDelegate = JSONDecoder.Decode<WorldsData>;
		IAsyncResult ar = worldsDataDecodeDelegate.BeginInvoke( handler.ResponseText, null, null );
		while( !ar.IsCompleted ) {
			yield return 0;
		}
		
		LoginGUI.worlds = worldsDataDecodeDelegate.EndInvoke( ar );
	}

	public static IEnumerator	GetWorldList() {
		// get world list
        loginStatus = "";

		WWW www = WWWX.Get( urlGetWorldList, WebRequests.authenticatedParameters );
		yield return www;

		Func<string, WorldsData>	worldsDataDecodeDelegate = JSONDecoder.Decode<WorldsData>;
		IAsyncResult ar = worldsDataDecodeDelegate.BeginInvoke( www.text, null, null );
		while( !ar.IsCompleted ) {
			yield return 0;
		}
		
		LoginGUI.worlds = worldsDataDecodeDelegate.EndInvoke( ar );
	}
	
	public static IEnumerator	GetPlayerData( AWebCoroutine handler ) {
		// get world list
        //loginStatus = "";
		handler.Request = new HTTP.Request( "Get", urlGetUserPlayers() );
		handler.Request.acceptGzip = false;
		handler.Request.AddParameters( authenticatedGodModeParameters );
		handler.Request.Send();
		while( !handler.Request.isDone ) {
			Debug.Log( "GetPlayerData: " + handler.Request.isDone );
			yield return 0;
		}
		
		Func<string, PlayersData>	playersDataDecodeDelegate = JSONDecoder.Decode<PlayersData>;
		IAsyncResult ar = playersDataDecodeDelegate.BeginInvoke( handler.ResponseText, null, null );
		while( !ar.IsCompleted ) {
			yield return 0;
		}
		
		LoginGUI.playerData = playersDataDecodeDelegate.EndInvoke( ar );
	}
	
	/// <summary>
	/// Downloads the player data.
	/// </summary>
	/// <returns>
	/// The player data for.
	/// </returns>
	/// <param name='player'>
	/// Player.
	/// </param>
	public static IEnumerator DownloadPlayerData(Player player)
	{
		HTTP.Request request = new HTTP.Request( "Get", urlGetUserPlayers() );
		request.AddParameters( authenticatedParameters );
		request.Send();
		
		while( !request.isDone ) {
			yield return 0;
		}
		
		Func<string, PlayersData>	playersDataDecodeDelegate = JSONDecoder.Decode<PlayersData>;
		IAsyncResult ar = playersDataDecodeDelegate.BeginInvoke( request.response.Text, null, null );
		while( !ar.IsCompleted ) {
			yield return 0;
		}
		
		PlayersData playersData = playersDataDecodeDelegate.EndInvoke( ar );
				
		// get the player from the list of players for the selected the world
		player.data = playersData.GetPlayerWithWorldId(UserData.worldNumber);
	}
	
	public static IEnumerator RefreshWorldData(AWebCoroutine handler)
	{
		handler.Request = new HTTP.Request( "Get", urlGetWorld );
		handler.Request.AddParameters( authenticatedParameters );
		handler.Request.Send();
		
		while( !handler.RequestIsDone ) {
			yield return 0;
		}

		if( handler.Request.ProducedError ) {
            Debug.LogError( handler.Request.Error );
			yield break;
		}
		
		Func<string, WorldsData>	asyncDelegate = JSONDecoder.Decode<WorldsData>;
		IAsyncResult ar = asyncDelegate.BeginInvoke( handler.ResponseText, null, null );
		while( !ar.IsCompleted ) {
			yield return 0;
		}
		
		WorldsData worlds = asyncDelegate.EndInvoke( ar );
		GameManager.worldData = worlds.world;
	}
	
	/// <summary>
	/// Gets or sets the tile download progress.
	/// </summary>
	/// <value>
	/// The tile download progress.
	/// </value>
	public static float tileDownloadProgress { get; set; }
	
	/// <summary>
	/// Downloads the world.
	/// </summary>
	/// <returns>
	/// The world.
	/// </returns>
	public static IEnumerator DownloadWorld()
	{
		HTTP.Request request = new HTTP.Request( "Get", WebRequests.urlGetWorld );
		request.AddParameters( authenticatedParameters );
		request.Send();
		
		while( !request.isDone ) {
			yield return 0;
		}
		
		if( request.ProducedError ) {
			Debug.LogError( request.Error );
			yield break;
		}
		
		Func<string, WorldsData>	asyncDelegate = JSONDecoder.Decode<WorldsData>;
		IAsyncResult ar = asyncDelegate.BeginInvoke( request.response.Text, null, null );
		while( !ar.IsCompleted ) {
			yield return 0;
		}
		
		WorldData world = asyncDelegate.EndInvoke( ar ).world;

		Debug.Log (string.Format ("This is the tilesize from world: {0}", world.megatile_height));
		TerrainManager.worldRegion = new Region(0, world.width-1, world.height-1, 0);
		Megatile.SetMegatileSize(world.megatile_height); // NOTE: just assumes square tile size
			
		// move terrain and camera to center
		float x, z;
		TerrainManager.worldRegion.GetCenter(out x, out z);
		CameraRig.use.JumpTo(new Vector3(x, 0f, z));
		TerrainManager.use.MoveTerrainToCenterOfView();
			
		// tell everyone the world is loaded
		MessengerAM.Send(new MessageWorldLoaded());
		//Messenger.Broadcast(kWorldLoaded);
	}
	
	public static IEnumerator	PostJoinWorld( int worldId, string playerClass ) {
		HTTP.Request request = new HTTP.Request( "Post", string.Format("{0}/worlds/{1}/players.json", urlServer, worldId) );
		request.AddHeader( "type", playerClass );
		request.AddParameters( WebRequests.authenticatedGodModeParameters );
		request.Send();
		
		while( !request.isDone ) {
			yield return 0;
		}
	}
	
	public static void	JoinWorld( int worldId, string playerClass ) {
		if( worldId < 0 ) {
			throw new ArgumentException( string.Format("WorldId( {0} ) is invalid", worldId) );
		}
		
		HTTP.JSON playerT = new HTTP.JSON( "{ \"player\": {\"type\": \"" + playerClass + "\"}}" );
		//WWW www = WWWX.Post( string.Format("{0}/worlds/{1}/players.json", urlServer, worldId), playerT, authenticatedGodModeParameters );
		
		HTTP.Request request = new HTTP.Request( "Post", string.Format("{0}/worlds/{1}/players.json", urlServer, worldId) );
		request.SetText( playerT );
		request.AddParameters( authenticatedGodModeParameters );
		request.Send();
		while( !request.isDone ) {
		}
		Debug.Log(request.response.Text);
		
		request = new HTTP.Request( "Get", urlGetUserPlayers() );
		request.AddParameters( authenticatedGodModeParameters );
		request.Send();
		while( !request.isDone ) {
		}
	}
	
	//public static void	JoinWorld( int worldId, Player.CharacterClass playerClass ) {
	//	JoinWorld( worldId, playerClass.ToString() );
	//}
}