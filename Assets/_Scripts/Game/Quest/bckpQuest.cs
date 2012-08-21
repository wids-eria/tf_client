//using UnityEngine;
//using System;
//using System.Collections.Generic;
//
//[Serializable]
//public class Quest
//{
//	public class RedundentCallException : Exception {
//		public RedundentCallException() {
//		}
//		
//		public RedundentCallException( string msg ) : base( msg ) {
//		}
//	}
//	
//	//Server Compatible Values
//	public string 					Name;
//	public Player.CharacterClass 	Role;
//	public Player 					Owner;
//	public string					Description;
//	public int 						PointValue;
//	public List<string>				QuestsToUnlock = new List<string>();
//	
//	private List<IObjective>		objectives = new List<IObjective>();
//	
//	
//	//public static readonly Message<string>	OnQuestComplete = new Message<string>( "OnQuestComplete" );
//	
//	public Quest( string name, Player.CharacterClass r ) {
//		Name = name;
//		Role = r;
//	}
//	
//	public virtual void OnAwake() {	
//		foreach( IObjective obj in objectives ) {
//			obj.OnAwake();
//		}
//	}
//
//	public virtual void OnDestroy() {
//		foreach( IObjective obj in objectives ) {
//			obj.forceObjectiveComplete();
//			obj.OnDestroy();
//		}
//	}
//
//	public float 	Progress {
//		get {
//			float count = 0.0f;
//			foreach( IObjective objective in objectives ) {
//				count += objective.Progress;
//				/*
//				if( objective.IsComplete() ) {
//					count += 1.0f;
//				}*/
//			}
//			return count / objectives.Count;
//		}
//	}
//
//	public bool 	IsComplete {
//		get {
//			foreach( IObjective obj in objectives ) {
//				if( !obj.IsComplete() ) {
//					return false;
//				}
//			}
//			
//			return true;
//		}
//	}
//	
//	public List<IObjective>	GetObjectives() {
//		return objectives;
//	}
//
//	public void	AddObjective( IObjective obj ) {
//		objectives.Add( obj );
//	}
//	
//	private int debugQuestCompleteCount = 0;
//	public void	OnObjectiveComplete( string name ) {// Could turn this into a delegate if need be
//		if( !IsComplete ) {
//			return;
//		}
//		
//		++debugQuestCompleteCount;
//		if( debugQuestCompleteCount > 1 ) {
//			throw new RedundentCallException( string.Format("Quest {0} attempted to call OnQuestComplete more than once", Name) );
//		}
//		
//		Messenger<string>.Broadcast("OnQuestComplete", Name );
//		Messenger<string>.Broadcast("UpdateServerOnQuest", Name);
//		Messenger<Player.CharacterClass>.Broadcast("UpdatePoints", Role);
//		if (QuestsToUnlock != null && QuestsToUnlock.Count > 0) {
//			Messenger<string[]>.Broadcast("OnUnlockQuests", QuestsToUnlock.ToArray());
//		}
//	}
//	
//	public Quest(string nm, Player.CharacterClass character, Player n, string des, List<IObjective> obj, int point){
//		Name = nm;
//		Role = character;
//		Owner = n;
//		Description = des;
//		objectives = obj;
//		PointValue = point;
//	}
//	
//}