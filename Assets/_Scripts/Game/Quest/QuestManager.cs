using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class QuestManager {
	private static QuestManager	instance = null;
	
	public static QuestManager	Instance {
		get {
			if( instance == null ) {
				instance = new QuestManager();
			}
			return instance;
		}
	}
	
	protected Dictionary<string, Quest> lockedQuests = new Dictionary<string, Quest>();
	protected Dictionary<string, Quest> unlockedQuests = new Dictionary<string, Quest>();
	protected Dictionary<string, Quest> completedQuests = new Dictionary<string, Quest>();
	
	[HideInInspector]
	public int questPoints=0;
	
	protected QuestManager() {
		//
		Debug.Log(GamePrefs.GetCharacterClass());
		
		
		
		//Quest for timber company
		if(GamePrefs.GetCharacterClass().Equals (Player.CharacterClass.TimberCompany)){
			//AddUnlockedQuest( new QuestHarvest20Years());
			//AddUnlockedQuest( new QuestHarvestAway() );
			//AddUnlockedQuest( new QuestHarvestPine() );
			//AddUnlockedQuest( new QuestAquireSurveys() );
			//AddUnlockedQuest( new QuestPlantTrees() );
			AddUnlockedQuest(new QuestTimberDemo());
		}
		
		//Quest for developer
		if(GamePrefs.GetCharacterClass().Equals (Player.CharacterClass.Developer)){
			AddUnlockedQuest(new QuestTotalPopulation());
			AddUnlockedQuest( new QuestDevelopLand() );
			AddLockedQuest( new QuestBuildHousing() );
			AddLockedQuest( new QuestOutpostProcess() );
		}
		
		//Quest for Conservationist
		if(GamePrefs.GetCharacterClass().Equals(Player.CharacterClass.Conservationist))
		{
			AddUnlockedQuest(new QuestMartenPop());
			AddUnlockedQuest(new QuestSurveyArea());
			AddLockedQuest(new QuestProtectArea());
			AddLockedQuest(new QuestRequestOutpost());
			AddLockedQuest(new QuestRestoreArea());
			AddLockedQuest(new QuestFulfillSurvey());
		}
		
		//string jsonStr = JsonMapper.ToJson(lockedQuests);
		//Debug.Log (jsonStr);
	}
	
	
	
	protected void	AddUnlockedQuest( Quest quest ) {
		unlockedQuests.Add( quest.Name, quest );
	}
	protected void AddLockedQuest( Quest quest ) {
		lockedQuests.Add( quest.Name, quest );
	}
	
	
	public void	OnAwake() {
		Messenger<string>.AddListener( "OnUnlockQuest", OnUnlockQuest );
		Messenger<string[]>.AddListener( "OnUnlockQuests", OnUnlockQuests );
		Messenger<string>.AddListener( "OnQuestComplete", OnQuestComplete );
		Messenger<string[]>.AddListener( "OnQuestsComplete", OnQuestsComplete );
		Messenger.AddListener("DestroyCompletedQuests", DestroyAllCompletedQuest);
		
		foreach( KeyValuePair<string, Quest> kv in unlockedQuests ) {
			kv.Value.OnAwake();
		}
	}
	
	public void OnDestroy() {
		foreach( KeyValuePair<string, Quest> kv in unlockedQuests ) {
			kv.Value.OnDestroy();
		}
		
		Messenger<string>.RemoveListener( "OnUnlockQuest", OnUnlockQuest );
		Messenger<string[]>.RemoveListener( "OnUnlockQuests", OnUnlockQuests );
		Messenger<string>.RemoveListener( "OnQuestComplete", OnQuestComplete );
		Messenger<string[]>.RemoveListener( "OnQuestsComplete", OnQuestsComplete );
		Messenger.RemoveListener("DestroyCompletedQuests", DestroyAllCompletedQuest);
	}
	
	protected void	OnUnlockQuest( string name ) {
		lockedQuests.CopyTo( name, unlockedQuests );
		//Initialize the quest
		unlockedQuests[name].OnAwake();	
		//=============Temp Place to save game state=============
		Messenger.Broadcast("SaveGameState");
	}
	
	protected void	OnUnlockQuests( string[] names ) {
		foreach( string name in names ) {
			OnUnlockQuest( name );
		}
	}
	
	protected void	OnQuestComplete( string name ) {
		unlockedQuests.CopyTo( name, completedQuests );
		questPoints += completedQuests[name].PointValue;
		completedQuests[name].OnDestroy();
		Debug.Log( string.Format("Quest {0} has been completed!", name) );
		//=============Temp Place to save game state=============
		Messenger.Broadcast("SaveGameState");
	}
	
	protected void	OnQuestsComplete( string[] names ) {
		foreach( string name in names ) {
			OnQuestComplete( name );
		}
	}
	
	public Quest[]	GetUnlockedQuests() {
		return Enumerable.ToArray<Quest>( unlockedQuests.Values );
	}
	
	public string[]	GetUnlockedQuestsNames() {
		return Enumerable.ToArray<string>( unlockedQuests.Keys );
	}
	
	public Quest[]	GetCompletedQuests() {
		return Enumerable.ToArray<Quest>( completedQuests.Values );
	}
	
	public Quest[] GetViewableQuests() {
		List<Quest> temp = new List<Quest>();
		temp.AddRange(completedQuests.Values);
		temp.AddRange(unlockedQuests.Values);
		return temp.ToArray();
	}
	
	public string[] GetCompletedQuestsNames(){
		return Enumerable.ToArray<string>( completedQuests.Keys);
	}
	
	protected void DestroyAllCompletedQuest(){
		
		foreach( KeyValuePair<string, Quest> Completekv in completedQuests){
			Completekv.Value.OnDestroy();
		}
	}
	
	public void ModifyQuestList(Dictionary<string,Quest> locked, Dictionary<string,Quest> unlocked, Dictionary<string,Quest> completed){		

		//remove duplicate quests that is in completed
		foreach(Quest q in completed.Values){
			if(lockedQuests.ContainsKey(q.Name)){
				Messenger<string>.Broadcast("OnUnlockQuest",q.Name);
			}
			if(unlockedQuests.ContainsKey(q.Name)){
				Messenger<string>.Broadcast("OnQuestComplete", q.Name);
			}
		}
		//remove duplicate quests that is in unlocked
		foreach(Quest q in unlocked.Values){
			if(lockedQuests.ContainsKey(q.Name)){
				Messenger<string>.Broadcast("OnUnlockQuest",q.Name);
			}
		}
		//unlock new quest after the last completed quest
		foreach(Quest q in completed.Values){
			//main quest does not have questToUnlock, so we need to prevent that.
			if(q.QuestsToUnlock == null){
				break;
			}
			foreach(string qUnlock in q.QuestsToUnlock){
				if(lockedQuests.ContainsKey(qUnlock)){
					Messenger<string>.Broadcast("OnUnlockQuest",q.Name);
				}
			}
		}
		
	}
	
	public Quest[]	GetLockedQuests() {
		return Enumerable.ToArray<Quest>( lockedQuests.Values );
	}
	
//	IEnumerator UpdateQuestPointToServer(){
//		WWW www = WWWX.Put("");
//	}
}