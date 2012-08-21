using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class TriggerDatabase : Trigger
{
	//storage for all triggers
	public Dictionary<string, Trigger> TriggerDictionary{get; private set;}
	
	//default
	public TriggerDatabase(){
		TriggerDictionary = new Dictionary<string,Trigger>();
	}
	
	//add trigger
	public bool AddTrigger(Trigger content){
		try{
			TriggerDictionary.Add(content.TriggerTitle, content);
			return true;
		}catch (System.Exception e){
			Debug.Log(e);
			return false;
		}
	}
	
	public bool ExecuteTrigger(string target){
		if(!TriggerDictionary.ContainsKey(target)){
			return false;
		}
		Trigger t;
		TriggerDictionary.TryGetValue(target,out t);
		if(t.isTriggered && t.progress>=100f)
			return false;
		else{
//			addProgress(ref t, 10f);
			t.DoAction();
			return true;
		}
		
	}
	

	
}

