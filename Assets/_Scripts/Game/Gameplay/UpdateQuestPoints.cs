using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UpdateQuestPoints : MonoBehaviour {
	
	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake(){
		Messenger<Player.CharacterClass>.AddListener("UpdatePoints", UpdateQuestPointsFunc);
	}
	/// <summary>
	/// Raises the destroy event.
	/// </summary>
	void OnDestroy(){
		Messenger<Player.CharacterClass>.RemoveListener("UpdatePoints", UpdateQuestPointsFunc);
	}
	
	/// <summary>
	/// Updates the quest points func.
	/// </summary>
	/// <param name='role'>
	/// Role.
	/// </param>
	protected void UpdateQuestPointsFunc(Player.CharacterClass role){
		StartCoroutine(UpdateUserPoints(role));
	}	
	
	/// <summary>
	/// Updates the user points.
	/// </summary>
	/// <returns>
	/// The user points.
	/// </returns>
	/// <param name='role'>
	/// Role.
	/// </param>
	protected IEnumerator UpdateUserPoints(Player.CharacterClass role){
		string url = WebRequests.urlGetUserPlayers(GameManager.worldData.GetUserIdFromRole(role));
		WWW www = WWWX.Get(url,WebRequests.authenticatedGodModeParameters);
		while(!www.isDone){yield return www;}
		
		PlayersData playersData = JSONDecoder.Decode<PlayersData>(www.text);	
		PlayerData data = playersData.GetPlayerWithWorldId(GameManager.worldData.id);
		
		//int point = QuestManager.Instance.questPoints;
		int point = GameManager.use.currentPlayer.questPoints;
		
		string json = "{\"player\":{\"quest_points\":" + point + "}}";
		
		www = WWWX.Put(WebRequests.urlGetUserPlayerId(data.user_id,data.id),json,WebRequests.authenticatedGodModeParameters);
		while(!www.isDone) {
			yield return www;
		}
		
		Debug.Log ("Points updated");
		yield break;
	}

}