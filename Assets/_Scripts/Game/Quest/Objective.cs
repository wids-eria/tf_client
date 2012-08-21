using UnityEngine;
using System;


public delegate void	Storer<StoreT,SentT>(ref StoreT curValue, SentT message);
public delegate bool 	Comparer<StoreT>(StoreT valA, StoreT valB);

/// <summary>
/// Takes a Storer and Comparer; Store should update the current value, Compare should be the completion requirements
/// </summary>
public class ObjectiveBehaviour<StoreT,SentT> {
	
	public StoreT CurrentValue { get{ return m_current;}}
	public StoreT GoalValue	   { get{ return m_goal;}}
	
	private StoreT m_current;
	private StoreT m_goal;
	
	protected Storer<StoreT,SentT>	Store;
	protected Comparer<StoreT>		Compare;
	
	
	public ObjectiveBehaviour( StoreT start, StoreT goal, Comparer<StoreT> comparer, Storer<StoreT,SentT> storer) {
		m_current = start;
		m_goal = goal;
		Compare = comparer;
		Store = storer;
	}
	
	public bool	Check( SentT val ) {
		Store(ref m_current, val);
		return Compare(CurrentValue, GoalValue);
	}
	
	public virtual string Description {
		get {
			return string.Format("{0}/{1}", CurrentValue, GoalValue);
		}
	}
}

/// <summary>
/// Simplified version of the objective behaviour; assumes that the message type sent is the same as the store type
/// </summary>
public class ObjectiveBehaviour<StoreT> : ObjectiveBehaviour<StoreT,StoreT> {
	public ObjectiveBehaviour( StoreT start, StoreT goal, Comparer<StoreT> comparer, Storer<StoreT,StoreT> storer) : base(start,goal,comparer,storer) {
	}
	public ObjectiveBehaviour( StoreT start, StoreT goal, Comparer<StoreT> comparer) : base(start,goal,comparer,null) {
		Store = StoreRecieved;
	}
	protected void StoreRecieved(ref StoreT current, StoreT val)
	{
		current = val;
	}
}


public abstract class IObjective {
	public virtual string 	Description {
		get {
			return "";
		}
	}
	public virtual float	Progress {
		get {
			return IsComplete() ? 1f : 0f;
		}
	}
	
	public string			Name;

	public abstract bool	IsComplete();
	public abstract	void 	OnAwake();
	public abstract void 	OnDestroy();
	public abstract void 	forceObjectiveComplete();
}


/// <summary>
/// Long form objective; listens for given message to broadcast, and interprets it using 
/// ObjectiveBehaviour (behaviour)'s Store and Compare delegates. (Think of the store 
/// method as a message translator)
/// 
/// StoreT = type that we want the behaviour to store
/// SentT = type of message parameter that will be sent from a message broadcast
/// </summary>
public class Objective<StoreT,SentT> : IObjective {// where StoreT : IComparable {
	[SerializeField]
	protected string 		notificationMessage;
	

	protected bool			isComplete = false;
	protected ObjectiveBehaviour<StoreT,SentT>	behaviour;
	
	public Objective( string name, string msgName ) {
		Name = name;
		notificationMessage = msgName;
	}
	
	public Objective( string name, string msgName, ObjectiveBehaviour<StoreT,SentT> ob ) {
		Name = name;
		notificationMessage = msgName;
		behaviour = ob;
	}
	
	public override bool	IsComplete() {
		return isComplete;
	}
	
	public override void OnAwake() {
		Messenger<SentT>.AddListener( notificationMessage, CheckComplete );
	}

	public override void OnDestroy() {
		Messenger<SentT>.RemoveListener( notificationMessage, CheckComplete );
	}
	
	protected virtual void CheckComplete( SentT val ) {
		if( IsComplete() ) {
			return;
		}
		
		isComplete = behaviour.Check( val );
		if( IsComplete() ) {
			Messenger<IObjective>.Broadcast("OnCompleteObjective", this);
			//
			OnDestroy();
		}
	}
	
	public override string 	Description {
		get {
			return string.Format( "{0} {1}", Name, behaviour.Description );
		}
	}
	
	public override void forceObjectiveComplete(){
		isComplete = true;
	}
	
}

/// <summary>
/// Simplified version of Objective; assumes that the Sent type and Storage type are the same
/// </summary>
public class ObjectiveSimple<StoreT> : Objective<StoreT,StoreT> where StoreT : IComparable {
	public ObjectiveSimple( string name, string msgName) : base (name,msgName) {
	}
}

/// <summary>
/// An objective that we know will have a percentage of completion based on its objective behaviour
/// </summary>
public class ObjectiveStoreInt<SentT> : Objective<int,SentT> {
	public ObjectiveStoreInt( string name, string msgName) : base (name,msgName) {
	}
	public override float Progress {
		get {
			return ( Mathf.Clamp01((float)behaviour.CurrentValue / (float)behaviour.GoalValue));
		}
	}
}
public class ObjectiveStoreFloat<SentT> : Objective<float,SentT> {
	public ObjectiveStoreFloat( string name, string msgName) : base (name,msgName) {
	}
	public override float Progress {
		get {
			return ( Mathf.Clamp01(behaviour.CurrentValue / behaviour.GoalValue));
		}
	}
}