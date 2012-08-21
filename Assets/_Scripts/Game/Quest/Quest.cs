using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class Quest
{
	public class RedundentCallException : Exception {
		public RedundentCallException() {
		}
		
		public RedundentCallException( string msg ) : base( msg ) {
		}
	}
	
	//Server Compatible Values
	public string 					Name;
	public Player.CharacterClass 	Role;
	public string					Description;
	public int 						PointValue;
	public List<string>				QuestsToUnlock = new List<string>();
	
	private List<IObjective>		objectives = new List<IObjective>();
	
	
	//public static readonly Message<string>	OnQuestComplete = new Message<string>( "OnQuestComplete" );
	
	public Quest() {
	}
	
	public Quest( string name, Player.CharacterClass r ) {
		Name = name;
		Role = r;
	}
	
	public virtual void OnAwake() {	
		foreach( IObjective obj in objectives ) {
			obj.OnAwake();
		}
		Messenger<IObjective>.AddListener("OnCompleteObjective", checkObjectiveOnList);
	}

	public virtual void OnDestroy() {
		foreach( IObjective obj in objectives ) {
			obj.forceObjectiveComplete();
			obj.OnDestroy();
		}
		Messenger<IObjective>.RemoveListener("OnCompleteObjective", checkObjectiveOnList);
	}
	
	/// <summary>
	/// The server progress which contains the progress in the server of each quests.
	/// </summary>
	protected float ServerProgress = 0.0f;
	
	public float 	Progress {
		get {
			float count = 0.0f;
			foreach( IObjective objective in objectives ) {
				count += objective.Progress;
				/*
				if( objective.IsComplete() ) {
					count += 1.0f;
				}*/
			}
			
			if(ProgressFlag == true)
				return 1.0f;
			if (objectives.Count <= 0){
				return ServerProgress;
			}
			return count / objectives.Count;
		}
		
		set {
			ServerProgress = value;
		}
	}

	public bool 	IsComplete {
		get {
			foreach(string name in QuestManager.Instance.GetCompletedQuestsNames()){
				if(name.Contains(Name)){
					ProgressFlag = true;
					return true;
				}
			}
			
			
			foreach( IObjective obj in objectives ) {
				if( !obj.IsComplete() ) {
					return false;
				}
			}
			
			if(this.Progress < 1f){
				return false;
			}
			
			return true;

		}
		set {
		}
	}
	
	/// <summary>
	/// The progress flag which indicates the server has the quest completed.
	/// </summary>
	protected bool ProgressFlag = false;
	
	public List<IObjective>	GetObjectives() {
		return objectives;
	}

	public void	AddObjective( IObjective obj ) {
		objectives.Add( obj );
	}
	
	/// <summary>
	/// Checks the objective on list.
	/// </summary>
	/// <param name='objective'>
	/// Objective.
	/// </param>
	public void checkObjectiveOnList(IObjective objective){
		foreach(IObjective obj in objectives){
			if(obj.Equals(objective)){
				OnObjectiveComplete(objective.Name);
			}
		}
	}
	
	private int debugQuestCompleteCount = 0;
	public void	OnObjectiveComplete( string name ) {// Could turn this into a delegate if need be
		if( !IsComplete ) {
			return;
		}
		
		++debugQuestCompleteCount;
		if( debugQuestCompleteCount > 1 ) {
			throw new RedundentCallException( string.Format("Quest {0} attempted to call OnQuestComplete more than once", Name) );
		}
		
		Messenger<string>.Broadcast("OnQuestComplete", Name );
		Messenger<Player.CharacterClass>.Broadcast("UpdatePoints", Role);
		if (QuestsToUnlock != null && QuestsToUnlock.Count > 0) {
			Messenger<string[]>.Broadcast("OnUnlockQuests", QuestsToUnlock.ToArray());
		}
	}
	
	public Quest(string nm, Player.CharacterClass character, string des, List<IObjective> obj, int point){
		Name = nm;
		Role = character;
		Description = des;
		objectives = obj;
		PointValue = point;
	}
	
}