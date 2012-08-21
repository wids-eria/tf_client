using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Properties for one trigger
/// </summary>
public class Trigger{
	public bool isTriggered{get; protected set;}
	public string TriggerTitle{get; protected set;}
	public string TriggerDescription{get; protected set;}
	public float progress{get; protected set;}
	public int WaitTime {get; protected set;}
	public bool isDone{get; protected set;}
	public object TriggerTask{get; protected set;}
	
	//null trigger
	public Trigger(){
	}
	
	//default trigger
	public Trigger(string name, string description, string objective_type){
		TriggerTitle = name;
		TriggerDescription = description;
		isTriggered = false;
		progress = 0f;
		WaitTime = 2;
		isDone = false;
		try{
			//TODO:create interface to handle task, so that later can cast this
			
			TriggerTask = System.Reflection.Assembly.GetExecutingAssembly().CreateInstance(objective_type);
		}catch(System.Exception ex){
			Debug.LogWarning(ex);
		}
	}
	
	
	//Update Trigger from server
	public Trigger(bool Triggered, string name, string description, float percentDone, int WaitSeconds, bool isDONE, object task){
		isTriggered = Triggered;
		TriggerTitle = name;
		TriggerDescription = description;
		progress = percentDone;
		WaitTime= WaitSeconds;
		isDone = isDONE;
		try{
			TriggerTask = task;
		}catch(System.Exception ex){
			Debug.LogWarning(ex);
		}
	}
	
	//fetch trigger data
	public virtual string[] GetTriggerDetails(){
		return new string[]{TriggerTitle, TriggerDescription};
	}
	
	//fetch trigger status
	public virtual string[] GetTriggerStatus(){
		return new string[]{
			string.Format("{0}", isTriggered?"Yes":"No"),
			progress.ToString(),
			WaitTime.ToString()
		};
	}
	
//	protected void addProgress(ref Trigger t,float num){
//		t.progress += num;
//	}
	
	public virtual void DoAction(){
		
	}
}
