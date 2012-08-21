using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class SaveQuest : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void Awake(){
		Messenger.AddListener("SaveGameState", SyncQuestOnCloud);
		Messenger.AddListener("LoadGameState", LoadGameState);
	}
	
	void OnDestroy(){
		Messenger.RemoveListener("SaveGameState", SyncQuestOnCloud);
		Messenger.RemoveListener("LoadGameState", LoadGameState);
	}
	
	
	public void SyncQuestOnCloud(){
		StartCoroutine(SyncGameStateToServer());
	}
	
	public void LoadGameState(){
		StartCoroutine(UpdateClientOnQuest());
	}
	
	protected struct QuestPackage{
		public PlayerData player;
	}
	
//	public class QuestContainer{
//		public Quest[] CompletedQuest;
//		public Quest[] LockedQuest;
//		public Quest[] UnlockedQuest;
//	}
	
	protected IEnumerator SyncGameStateToServer(){
		/*
		WWW www = WWWX.Get(WebRequests.urlGetCurrentPlayer, WebRequests.authenticatedParameters);
		while (!www.isDone){
			yield return www;
		}
		
		Dictionary<string, Quest> locked, unlocked, completed;
		QuestPackage qp = JSONDecoder.DecodeValue<QuestPackage>(www.text);
		QuestContainer QuestDic = qp.player.quests;
		
		//load quests from server (buffer area)
		#region
		unlocked = new Dictionary<string,Quest>();
		if(QuestDic.UnlockedQuest != null && QuestDic.UnlockedQuest.Length != 0){
			//Messenger<Dictionary<string, Quest>>.Broadcast("UnlockedQuestFromServer", QuestDic["UnlockedQuest"]);
			foreach(Quest q in QuestDic.UnlockedQuest){
				unlocked.Add(q.Name,q);
			}
		}
		
		locked = new Dictionary<string,Quest>();
		
		completed = new Dictionary<string, Quest>();
		if(QuestDic.CompletedQuest != null && QuestDic.CompletedQuest.Length != 0){
			//Messenger<Dictionary<string,Quest>>.Broadcast("CompletedQuestFromServer", QuestDic["CompletedQuest"]);
			foreach(Quest q in QuestDic.CompletedQuest){
				completed.Add (q.Name, q);
			}
		}
		#endregion
		*/
		
		QuestContainer QuestDic = new QuestContainer();
		
		/*
		//compare client's quests and add to server list
		#region
		Quest[] Questlist = QuestManager.Instance.GetUnlockedQuests();
		Quest[] CompareList = QuestManager.Instance.GetCompletedQuests();
		foreach(Quest q in CompareList){
			if(unlocked.ContainsKey(q.Name)){
				unlocked.Remove(q.Name);
			}
		}
		foreach(Quest q in Questlist){
			if(!unlocked.ContainsKey(q.Name)){
				unlocked.Add(q.Name,q);
			}
		}
		List<Quest> questList = new List<Quest>();
		foreach(Quest q in unlocked.Values){
			questList.Add (q);
		}
		QuestDic.UnlockedQuest = questList.ToArray();
		questList.Clear ();
		
		Questlist = QuestManager.Instance.GetLockedQuests();
		foreach(Quest q in Questlist){
			locked.Add(q.Name, q);
		}
		foreach(Quest q in locked.Values){
			questList.Add (q);
		}
		QuestDic.LockedQuest = questList.ToArray();
		questList.Clear ();
		
		Questlist = QuestManager.Instance.GetCompletedQuests();
		foreach(Quest q in Questlist){
			if(!completed.ContainsKey(q.Name)){
				completed.Add(q.Name, q);
			}
		}
		foreach(Quest q in completed.Values){
			questList.Add (q);
		}
		QuestDic.CompletedQuest = questList.ToArray();
		questList.Clear();
		#endregion
		
		*/
		
		#region
		Quest[] Questlist = QuestManager.Instance.GetUnlockedQuests();
		List<Quest> questList = new List<Quest>();
		foreach(Quest q in Questlist){
			questList.Add (q);
		}
		QuestDic.UnlockedQuest = questList.ToArray();
		questList.Clear ();
		
		Questlist = QuestManager.Instance.GetLockedQuests();
		foreach(Quest q in Questlist){
			questList.Add(q);
		}
		QuestDic.LockedQuest = questList.ToArray();
		questList.Clear ();
		
		Questlist = QuestManager.Instance.GetCompletedQuests();
		foreach(Quest q in Questlist){
			questList.Add (q);
		}
		QuestDic.CompletedQuest = questList.ToArray();
		questList.Clear();
		#endregion
		
		string jsonUrl = JsonMapper.ToJson(QuestDic);
		string goServerUrl= "{\"player\":{\"quests\":" + jsonUrl.ToString() + "}}";
		
		WWW www = WWWX.Put (WebRequests.urlGetCurrentPlayer, goServerUrl, WebRequests.authenticatedGodModeParameters);
		while(!www.isDone){
			yield return www;
		}
		
		yield return new WaitForSeconds(10);
		StartCoroutine(UpdateClientOnQuest());
		
		yield break;
	}
	
	
	protected IEnumerator UpdateClientOnQuest(){
		WWW www = WWWX.Get(WebRequests.urlGetCurrentPlayer, WebRequests.authenticatedParameters);
		while (!www.isDone){
			yield return www;
		}
		
		Dictionary<string, Quest> locked, unlocked, completed;
		QuestPackage qp = JSONDecoder.Decode<QuestPackage>(www.text);
		QuestContainer QuestDic = qp.player.quests;
		
		if(QuestDic == null || QuestDic.CompletedQuest == null && QuestDic.UnlockedQuest == null && QuestDic.LockedQuest == null ){
			yield break;
		}
		
		//load quests from server (buffer area)
		#region
		unlocked = new Dictionary<string,Quest>();
		if(QuestDic.UnlockedQuest != null && QuestDic.UnlockedQuest.Length != 0){
			foreach(Quest q in QuestDic.UnlockedQuest){
				unlocked.Add(q.Name,q);
			}
		}
		
		locked = new Dictionary<string,Quest>();
		if(QuestDic.LockedQuest != null && QuestDic.LockedQuest.Length != 0){
			foreach(Quest q in QuestDic.LockedQuest){
				locked.Add(q.Name,q);
			}
		}
		
		completed = new Dictionary<string, Quest>();
		if(QuestDic.CompletedQuest != null && QuestDic.CompletedQuest.Length != 0){
			foreach(Quest q in QuestDic.CompletedQuest){
				completed.Add (q.Name, q);
			}
		}
		#endregion
		
		QuestManager.Instance.ModifyQuestList(locked, unlocked, completed);
		
		yield break;
		
	}
	
}
