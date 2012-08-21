using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// World data.
/// </summary>
public class WorldData : System.Object
{
	public int id { get; set; }
	public string name { get; set; }
	public int year_start { get; set; }
	public int year_current { get; set; }
	public int height { get; set; }
	public int width { get; set; }
	public int megatile_width { get; set; }
	public int megatile_height { get; set; }
	public string created_at { get; set; }
	public string updated_at { get; set; }
	public string turn_started_at { get; set; }
	public int current_turn {get; set; }
	public int timber_count {get; set; }
	public int human_population {get; set; }
	public int livable_tiles_count {get; set;}
	public int marten_suitable_tile_count {get; set;}
	public PlayerData[] players { get; set; }
	
	public string	GetClassNameForUser( int userId ) {
		foreach( PlayerData player in players ) {
			if( player.user_id == userId ) {
				return player.type;
			}
		}
		
		return "";
	}
	
	public Player.CharacterClass	GetClassForUser( int userId ) {
		return Player.GetCharacterClass( GetClassNameForUser(userId) );
	}
	public int GetPlayerIdFromRole( Player.CharacterClass role ) {
		foreach(PlayerData data in players) {
			if (data.type == Player.GetCharacterClassName(role)) {
				return data.id;
			}
		}
		Debug.LogWarning(string.Format("Role `{0}' does not exist in world `{1}'.",role.ToString(),GameManager.worldData.id));
		return -1;
	}
	public int GetUserIdFromRole( Player.CharacterClass role) {
		foreach(PlayerData data in players) {
			if (data.type == Player.GetCharacterClassName(role)) {
				return data.user_id;
			}
		}
		Debug.LogWarning(string.Format("User with class `{0}' does not exist in world `{1}'.",role.ToString(),GameManager.worldData.id));
		return -1;
	}
	
	public bool	GetClassForUser( int userId, ref Player.CharacterClass c ) {
		string className = GetClassNameForUser( userId );
		if( className.Length <= 0 ) {
			return false;
		}
		
		c = Player.GetCharacterClass( className );
		return true;
	}
}


/// <summary>
/// Worlds data.
/// </summary>
public class WorldsData : System.Object
{
	/// <summary>
	/// Gets or sets the world.
	/// </summary>
	/// <value>
	/// The world.
	/// </value>
	public WorldData world { get; set; }
	/// <summary>
	/// Gets or sets the worlds.
	/// </summary>
	/// <value>
	/// The worlds.
	/// </value>
	public WorldData[] worlds { get; set; }
	
	public void		FilterWorldsWith( System.Func<int, WorldData, bool> pred, int userId, List<WorldData> filteredWorlds ) {
		filteredWorlds.Clear();
		
		foreach( WorldData data in worlds ) {
			if( !pred(userId, data) ) {
				continue;
			}
			
			filteredWorlds.Add( data );
		}
	}
	
	public static bool	ClearWorldOfPlayers( int worldId ) {
		int prevWorld = UserData.worldNumber;
		
		UserData.worldNumber = worldId;

		HTTP.Request request = null;
		try {
			request = new HTTP.Request( "Get", WebRequests.urlGetWorld );
			request.AddParameters( WebRequests.authenticatedGodModeParameters );
			request.Send();
			while( !request.isDone ) {}
			
			if( request.ProducedError ) {
				Debug.Log( string.Format("Unable to find or delete players on world {0} (harmless)", worldId) );
				return true;
			}
		} catch {
			//Debug.LogError( www.error );
			return false;
		}
		
		WorldsData data = null;
		try {
			data = JSONDecoder.Decode<WorldsData>( request.response.Text );
		} catch( JsonException e ) {
			Debug.LogError( e );
			return false;
		}
		
		/////////
		foreach( PlayerData p in data.world.players ) {
			request = new HTTP.Request( "Delete", WebRequests.urlDeletePlayer(p.world_id, p.id) );
			request.AddParameters( WebRequests.authenticatedGodModeParameters );
			request.Send();
			while( !request.isDone ) {}
			
			if( request.ProducedError ) {
				Debug.LogError( request.Error );
				return false;
			}
		}
		/////////
		
		try {
			request = new HTTP.Request( "Get", WebRequests.urlGetWorld );
			request.AddParameters( WebRequests.authenticatedGodModeParameters );
			request.Send();
			
			while( !request.isDone ) {}

			if( request.ProducedError ) {
				Debug.LogError( request.Error );
				return false;
			}
		} catch {
			//Debug.LogError( www.error );
			return false;
		}
		
		UserData.worldNumber = prevWorld;
		
		try {
			data = JSONDecoder.Decode<WorldsData>( request.response.Text );
		} catch( JsonException e ) {
			Debug.LogError( e );
			return false;
		}

		return data.world.players == null || data.world.players.Length <= 0;
	}
	
	#region FILTER FUNCTIONS
	public static bool JoinableWorldsPassFilter( int userId, WorldData worldData ) {
		if( worldData.players == null ) {
			return false;
		}
		
		if( worldData.players.Length <= 0 ) {
			return false;
		}
		
		if( worldData.players.Length < 3 ) {
			return true;
		}
		
		if( worldData.players.Length >= 3 ) {
			foreach( PlayerData pd in worldData.players ) {
				if( pd.user_id == UserData.current.id ) {
					return true;
				}
			}
		}
		
		return false;
	}
	
	public static bool EmptyWorldsPassFilter( int userId, WorldData worldData ) { 
		if( worldData.players == null ) {
			return true;
		}
		
		if( worldData.players.Length <= 0 ) {
			return true;
		}
		
		return false;
	}
	#endregion
}