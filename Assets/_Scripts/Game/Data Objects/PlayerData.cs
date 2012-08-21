using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Player data about the players of the world
/// </summary>
public class PlayerData : System.Object
{
	public int id { get; set; }
	public int user_id { get; set; }
	public string name { get; set; }
	public string type { get; set; }
	public int world_id { get; set; }
	public string world_name { get; set; }
	public int balance { get; set; }
	public Megatile[] megatiles { get; set; }
	public int quest_points {get; set;}
	public int pending_balance {get; set;}
	public QuestContainer quests {get; set;}
}

public class QuestContainer{
		public Quest[] CompletedQuest;
		public Quest[] LockedQuest;
		public Quest[] UnlockedQuest;
//		public List<Quest> CompletedQuest;
//		public List<Quest> LockedQuest;
//		public List<Quest> UnlockedQuest;
}



/// <summary>
/// Players data.
/// </summary>
public class PlayersData : System.Object
{
	/// <summary>
	/// Gets or sets the players.
	/// </summary>
	/// <value>
	/// The players.
	/// </value>
	public PlayerData[] players { get; set; }
	
	/// <summary>
	/// Gets the world names.
	/// </summary>
	/// <returns>
	/// The world names.
	/// </returns>
	public string[] GetWorldNames()
	{
		string[] worldNames = new string[ players.Length ];
		PlayerData player = null;
		for( int ix = 0; ix < players.Length; ++ix ) {
			player = players[ ix ];
			worldNames[ix] = player.world_name;
		}
		return worldNames;
	}
	
	/// <summary>
	/// Gets the player with world identifier.
	/// </summary>
	/// <returns>
	/// The player with world identifier.
	/// </returns>
	/// <param name='world_id'>
	/// World_id.
	/// </param>
	public PlayerData GetPlayerWithWorldId( int world_id )
	{
		foreach(PlayerData player in players) {
			if (player.world_id == world_id) {
				return player;
			}
		}
		return null;
	}
	
	public PlayerData GetPlayerWithRole( Player.CharacterClass role )
	{
		foreach(PlayerData player in players) {
			if (player.type == Player.GetCharacterClassName(role)) {
				return player;
			}
		}
		return null;
	}
	
	public PlayerData GetPlayerWithRole( string role )
	{
		foreach(PlayerData player in players) {
			if (player.type == role) {
				return player;
			}
		}
		return null;
	}
	
	/// <summary>
	/// Gets or sets the <see cref="PlayersData"/> at the specified index.
	/// </summary>
	/// <param name='index'>
	/// Index.
	/// </param>
	public PlayerData this[int index] {
		get { return this.players[index]; }
		set { this.players[index] = value; }
	}
}