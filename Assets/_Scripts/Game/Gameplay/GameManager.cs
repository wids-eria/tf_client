using UnityEngine;
using System.Collections;

/// <summary>
/// Game manager
/// </summary>
public class GameManager : MonoBehaviour
{
	public const string kWorldDataChanged = "WorldDataChanged";
	public const string kTimberCountIncreased = "TimberCountIncreased";
	
	
	/// <summary>
	/// The player prefab.
	/// </summary>
	public Player[] playerPrefabs;
	
	/// <summary>
	/// Gets or sets the current player.
	/// </summary>
	/// <value>
	/// The current player.
	/// </value>
	public Player currentPlayer { get; private set; }
	
	/// <summary>
	/// Gets or sets the singleton.
	/// </summary>
	/// <value>
	/// The singleton.
	/// </value>
	public static GameManager use { get; private set; }
	
	protected static WebCoroutine<WorldData> putWorldData = new WebCoroutine<WorldData>(PutWorldData);
	
	
	public static WorldData worldData { 
		get {
			return m_worldData;
		} 
		set { 
			if (m_worldData != null && m_worldData.timber_count < value.timber_count) {
				Messenger<int>.Broadcast(kTimberCountIncreased,value.timber_count-m_worldData.timber_count);
			}
			m_worldData = value;
			Messenger<WorldData>.Broadcast(kWorldDataChanged,m_worldData); 
		}
	}
		
	protected static WorldData m_worldData;
	
	public static EconomyController economyController;
	
	
	/// <summary>
	/// Initialize
	/// </summary>
	void Awake()
	{
		if (use != null) {
			Destroy(use.gameObject);
		}
		use = this;
		
		// TODO: for testing purposes, ensure we can just launch game level if not formally logged in
		if (UserData.current == null) {
			UserData.current = new UserData();
		}
		
	}
	/// <summary>
	/// Instantiate player.
	/// </summary>
	void Start()
	{	
		// fetch the player data
		Player.CharacterClass characterClass = GamePrefs.GetCharacterClass();
		foreach (Player p in playerPrefabs) {
			if (p.characterClass != characterClass) {
				continue;
			}
			currentPlayer = Instantiate(p) as Player;
			break;
		}
		StartCoroutine(GetPlayerDataAndStartGame());
		StartCoroutine(GetWorldData());
		StartCoroutine(InvalidateWorldData());
	}
	
	static IEnumerator PutWorldData(WorldData data, AWebCoroutine handler)
	{
		string json = "{\"world\":{\"timber_count\":"+data.timber_count+"}";
		handler.Request = new HTTP.Request( "Put", WWWX.AppendParametersToUrl(WebRequests.urlGetWorld, WebRequests.authenticatedGodModeParameters) );
		handler.Request.SetText( json );
		handler.Request.Send();
		
		while(!handler.IsDone) {
			yield return 0;
		}
		Debug.Log(handler.ResponseText);
	}
	
	IEnumerator InvalidateWorldData()
	{
		yield return new WaitForSeconds(10f);
		worldDataIsValid = false;
		StartCoroutine(InvalidateWorldData());
	}
	
	/// <summary>
	/// Gets the player data and start game.
	/// </summary>
	/// <returns>
	/// The player data and start game.
	/// </returns>
	IEnumerator GetPlayerDataAndStartGame()
	{
		yield return StartCoroutine(WebRequests.DownloadPlayerData(currentPlayer));
		InputManager.use.isProcessingInput = true;
		GameGUIManager.use.enabled = true;
		//Init the money controller
		if (economyController == null) economyController = new EconomyController();
		//init syncQuest machanism
		Messenger.Broadcast("LoadGameState");
	}
	
	protected WWW	worldDataWWW = null;
	
	public bool	worldDataIsValid {
		get {
			return worldDataWWW.progress < 1.0f;
		}
		
		set {
			if( value == false ) {
				worldDataWWW = null;
				StartCoroutine( GetWorldData() );
			}
		}
	}

	IEnumerator GetWorldData() //If-Modified-Since RFC2822
	{
		worldDataWWW = WWWX.Get(WebRequests.urlGetWorld, WebRequests.authenticatedParameters);
		while (!worldDataWWW.isDone) yield return worldDataWWW;
		//Debug.Log(string.Format("World {0} Updated",worldData.id));
		try {
			// get world data
			worldData = JSONDecoder.Decode<WorldsData>(worldDataWWW.text).world;
			//Debug.Log(worldDataWWW.text);
		}
		catch (JsonException e) {
			GameGUIManager.use.SetErrorMessage("Error parsing world data.");
			Debug.LogError(worldDataWWW.text);
			Debug.LogError(e);
		}
	}	
}