using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Format for Trigger handler.
/// </summary>
public delegate void TriggerHandler(object o, TriggerArgs e);

/// <summary>
/// Declaration of trigger arguement, adding trigger into dictionary
/// </summary>
public class TriggerArgs : EventArgs
{
	public readonly Dictionary<string, object> TriggerDictionary;
	public LinkedListNode<object> node;
	public TriggerArgs(object item)
	{
		node = new LinkedListNode<object>(item);
	}
}

/// <summary>
/// List of actions available
/// </summary>
public class TriggerAction
{
	public void FlyAway(object o , TriggerArgs e){
		Debug.Log("Lets Fly!!");
	}
	
	public void DoSomethingAwsome (object o, TriggerArgs e){
		Debug.Log (e);
	}
}

/// <summary>
/// Trigger parser.
/// </summary>
public class TriggerParser : MonoBehaviour {
	protected static event TriggerHandler EventTrigger;
	
	public void addTrigger(string[] tokens){
		EventTrigger += new TriggerHandler(this.SelectProperAction(tokens[0]));
	}
	
	/// <summary>
	/// Selects the proper action.
	/// </summary>
	/// <returns>
	/// The proper action.
	/// </returns>
	/// <param name='option'>
	/// Option.
	/// </param>
	protected TriggerHandler SelectProperAction(string option){
		TriggerAction T_Action = new TriggerAction();
		if(option == "fly"){
			return T_Action.FlyAway;
		}
		return null;
	}
	
}
