using UnityEngine;
using System.Collections;

public class EconomyController
{
	public Player player 		{ get { return (GameManager.use != null) ? GameManager.use.currentPlayer : null; }}
	
	public int balance 			{ get { return player.balance; }}
	public int pendingBalance 	{ get { return player.pendingBalance; }}
	
	public int timberCount		{ get { return GameManager.worldData.timber_count; }}
	
	public const int timberRate = 10;
	
	
	protected delegate void PlayerDataHandler(ref PlayerData data, int amount);
	protected WebCoroutine<Player.CharacterClass,int,PlayerDataHandler> ModifyPlayerData;
	protected WebCoroutine<int> ModifyTimberCount;
	
	public EconomyController()
	{
		ModifyPlayerData = new WebCoroutine<Player.CharacterClass,int,PlayerDataHandler>(ChangePlayerData);
		ModifyTimberCount = new WebCoroutine<int>(ChangeTimberCount);
	}
	
	#region Public Utilities
	
	public bool IsBalanceAvailable(int amount) {
		return IsBalanceAvailable(player.data,amount);
	}
	
	public bool IsBalanceAvailable(Player.CharacterClass role, int amount) {
		throw new System.NotImplementedException();
//		return false;
	}
	
	protected bool IsBalanceAvailable(PlayerData data, int amount) {
		return data.balance >= amount;
	}
	
	public bool IsTimberAvailable(int amount)
	{
		return timberCount >= amount;
	}
	
	public bool SpendMoney(int amount) {
		if (IsBalanceAvailable(amount)) { 
			ModifyPlayerData.Start(GameManager.use,player.characterClass, -amount, AddBalance);
			return true;
		} else {
			return false;
		}
	}
	public bool SpendMoney(Player.CharacterClass role, int amount) {
		if (IsBalanceAvailable( role, amount ) ) { 
			ModifyPlayerData.Start(GameManager.use,role, -amount, AddBalance);
			return true;
		} else {
			return false;
		}
	}
	public void IncreasePendingBalance(int amount) {
		ModifyPlayerData.Start(GameManager.use,player.characterClass, amount, AddPendingBalance);
	}
	public void IncreasePendingBalance(Player.CharacterClass role, int amount) {
		ModifyPlayerData.Start(GameManager.use,role,amount,AddPendingBalance);
	}
	
	public bool UseTimber(int amount)
	{
		bool hasEnough = IsTimberAvailable(amount);
		if (hasEnough) {
			ModifyTimberCount.Start(GameManager.use,-amount);
		}
		return hasEnough;
	}
	
	#endregion
	
	/// <summary>
	/// PlayerDataHandler delegate method; adds to the balance
	/// </summary>
	/// <param name='data'>
	/// Data.
	/// </param>
	/// <param name='amount'>
	/// Amount.
	/// </param>
	protected void AddBalance( ref PlayerData data, int amount ) {
		data.balance += amount;
	}
	
	/// <summary>
	/// PlayerDataHandler delegate; adds 'ammount' to the pending_balance
	/// </summary>
	/// <param name='data'>
	/// Data.
	/// </param>
	/// <param name='amount'>
	/// Amount.
	/// </param>
	protected void AddPendingBalance( ref PlayerData data, int amount ) {
		data.pending_balance += amount;
	}

	protected IEnumerator ChangePlayerData(Player.CharacterClass role, int amount, PlayerDataHandler DataHandler, AWebCoroutine handler)
	{
		while(GameManager.worldData == null){
			WWW www2 = WWWX.Get(WebRequests.urlGetWorld, WebRequests.authenticatedParameters);
			while(!www2.isDone){www2 = WWWX.Get(WebRequests.urlGetWorld, WebRequests.authenticatedParameters);}
			GameManager.worldData = JSONDecoder.Decode<WorldData>(www2.text);
		}
		
		//string url = WebRequests.urlGetUserPlayerId(GameManager.worldData.GetUserIdFromRole(role),GameManager.worldData.GetPlayerIdFromRole(role));
		string url = WebRequests.urlGetUserPlayers(GameManager.worldData.GetUserIdFromRole(role));
		
		WWW www = WWWX.Get(url,WebRequests.authenticatedGodModeParameters);
		while(!www.isDone) {
			yield return www;
		}
		Debug.Log(www.text);
		//Does not contains balance and pending balance.....check wat server returns
		
		//Get the specific role we want
		PlayersData playersData = JSONDecoder.Decode<PlayersData>(www.text);
		PlayerData data = playersData.GetPlayerWithWorldId(GameManager.worldData.id);
		
		//Adjust the balance
		DataHandler(ref data,amount);
	
		string json = "{\"player\":{\"balance\":" + data.balance +",\"pending_balance\":" + data.pending_balance +"}}";
		
		Debug.Log(json);
		Debug.Log(WebRequests.urlGetUserPlayerId(data.user_id,data.id));
		
		www = WWWX.Put(WebRequests.urlGetUserPlayerId(data.user_id,data.id),json,WebRequests.authenticatedGodModeParameters);
		while(!www.isDone) {
			yield return 0;
		}
		
		//Conclude the Put
		IEnumerator e = ConcludePut(handler); 
		yield return e.Current;
		while(e.MoveNext()) yield return e.Current;
		
		
	}
	
	protected IEnumerator ChangeTimberCount(int amount, AWebCoroutine handler)
	{
		WWW www = WWWX.Get(WebRequests.urlGetWorld,WebRequests.authenticatedParameters);
		while(!www.isDone) {
			yield return 0;
		}
		WorldData data = JSONDecoder.Decode<WorldsData>(www.text).world;
		data.timber_count += amount;
		
		//Send new timber count
		string json = "{\"world\":{\"timber_count\":" + data.timber_count +"}}";
		www = WWWX.Put(WebRequests.urlGetWorld,json,WebRequests.authenticatedGodModeParameters);
		while(!www.isDone) {
			yield return 0;
		}
		//Refresh World Data
		GameManager.worldData.timber_count = data.timber_count;
		GameManager.use.worldDataIsValid = false;
		
	}

	protected IEnumerator ConcludePut(AWebCoroutine handler)
	{
		//Finished, update the client player
		WWW www = WWWX.Get(WebRequests.urlGetUserPlayers(),WebRequests.authenticatedParameters);
		while (!www.isDone) {
			yield return 0;
		}

		Debug.Log(www.text);
		
		PlayersData playersData = JSONDecoder.Decode<PlayersData>(www.text);
		if( playersData == null ) {
			yield break;
		}
		PlayerData d = playersData.GetPlayerWithWorldId(GameManager.worldData.id);
		
		if( d == null ) {
			yield break;
		}
		
		player.data = d;
		yield break;
	}
	
	//override
	public IEnumerator ConcludePutAfterTurn(AWebCoroutine a)
	{
		//Finished, update the client player
		WWW www = WWWX.Get(WebRequests.urlGetUserPlayers(),WebRequests.authenticatedParameters);
		while (!www.isDone) {
			yield return 0;
		}

		Debug.Log(www.text);
		
		PlayersData playersData = JSONDecoder.Decode<PlayersData>(www.text);
		if( playersData == null ) {
			yield break;
		}
		PlayerData d = playersData.GetPlayerWithWorldId(GameManager.worldData.id);
		
		if( d == null ) {
			yield break;
		}
		
		player.data = d;
		yield break;
	}
}

