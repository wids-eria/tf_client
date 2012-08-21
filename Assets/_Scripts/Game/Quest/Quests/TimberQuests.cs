using System;
using System.Collections.Generic;
using UnityEngine;

#region Quests


public class QuestHarvestAway : Quest
{
	public QuestHarvestAway () : base( QuestConstants.kTimberNames[0], Player.CharacterClass.TimberCompany )
	{
		AddObjective (new ObjectiveDoHarvest ("Clear Cut", CutType.Clearcut));
		AddObjective (new ObjectiveDoHarvest ("Diameter Limit Cut", CutType.DiameterLimitCut));
		AddObjective (new ObjectiveDoHarvest ("Q-Ratio Cut", CutType.QRatioCut));
		Description = "Use the 'Harvest' action to collect Timber using your arsenal of harvesting strategies.";
		PointValue = 2;
	}
}

public class QuestHarvestPine : Quest
{
	public QuestHarvestPine () : base ( QuestConstants.kTimberNames[1], Player.CharacterClass.TimberCompany )
	{
		AddObjective (new ObjectiveHarvestAmount("10,000 Board-Feet",0,10000));
		Description = "Locate and Harvest 10,000 board-feet of saw-log timber.";
		PointValue = 4;
	}
}

public class QuestAquireSurveys : Quest
{
	public QuestAquireSurveys() : base ( QuestConstants.kTimberNames[2],  Player.CharacterClass.TimberCompany | Player.CharacterClass.Developer )
	{
		AddObjective (new ObjectiveRequestSurveys("Request Surveys",5));
		AddObjective (new ObjectivePurchaseSurveys("Buy Surveys",5));
		Description = "Request Survey Data from the Conservationist and Purchase data that has been collected.";
		PointValue = 4;
	}
}

public class QuestPlantTrees : Quest
{
	public QuestPlantTrees() : base ( QuestConstants.kTimberNames[3], Player.CharacterClass.TimberCompany)
	{
		AddObjective (new ObjectivePlantTrees("Plant Trees", 20));
		Description = "Find suitable land to plant seeds and contribute to the tree population.";
		PointValue = 2;
	}
}

public class QuestHarvest20Years : Quest
{
	public QuestHarvest20Years() : base (QuestConstants.kTimberNames[4], Player.CharacterClass.TimberCompany){
		int timber = GameManager.worldData.timber_count;
		Debug.Log(timber);
		AddObjective (new ObjectiveHarvest20Years("Harvest 20 years", timber, timber + 200));
		Description = "Run a harvest operation for 20 years that yields X Timber";
		PointValue =15;
	}
}

public class QuestTimberDemo : Quest //================================================================================= This is the  demo quest
{
	public QuestTimberDemo () : base( QuestConstants.kTimberNames[0], Player.CharacterClass.TimberCompany )
	{
		AddObjective(new ObjectiveBuyLand());
		AddObjective ( new ObjectiveSurveyLand());
		AddObjective (new ObjectiveDoHarvest ("Clear Cut", CutType.Clearcut));
		
		Description = "Use the 'Harvest' action to collect Timber using your arsenal of harvesting strategies.";
		PointValue = 2;
	}
}

#endregion

#region Objectives

public class ObjectiveDoHarvest : ObjectiveSimple<CutType>
{
	protected bool Check(CutType val, CutType goal)
	{
		return val == goal;
	}
	// TED - This seems like a horrible way to do this...
	protected bool AutoPass (CutType val, CutType goal)
	{
		return true;
	}

	public ObjectiveDoHarvest (string name, CutType cutType) : base(name,"TimberHarvested")
	{
		behaviour = new ObjectiveBehaviour<CutType>(CutType.Clearcut,cutType,Check);
	}

	public ObjectiveDoHarvest () : base("Harvest", "TimberHarvested")
	{
		behaviour = new ObjectiveBehaviour<CutType>(CutType.Clearcut,CutType.Clearcut,AutoPass);
	}
	
	public override string Description {
		get {
			return string.Format ("Perform a {0} on any parcel(s) of land possible.", Name);
		}
	}
}

public class ObjectiveHarvestAmount : ObjectiveStoreInt<Harvest.Products>
{
	public bool Check (int val, int goal)
	{
		return val >= goal;
	}
	public void Store (ref int curVal, Harvest.Products val)
	{
		curVal += (int)Math.Round(val.sawtimberVolume);
	}
	
	public ObjectiveHarvestAmount (string name, int startTimber, int goalTimber) : base(name, "HarvestProducts")
	{
		behaviour = new ObjectiveBehaviour<int,Harvest.Products>(startTimber,goalTimber,Check,Store);
	}
	public override string Description {
		get {
			return string.Format("Collect {0} board-feet of any type of timber.\nCurrent progress: {1}", behaviour.GoalValue, behaviour.Description);
		}
	}
}

//TODO: Replace second int with the Type that gets sent with survey data (probably a list of SurveyData objects)
public class ObjectiveRequestSurveys : ObjectiveStoreInt<int>
{
	public bool Check ( int val, int goal)
	{
		return val >= goal;
	}
	public void Store ( ref int current, int surveys)
	{
		current = Mathf.Clamp(current+surveys,0,behaviour.GoalValue);
	}
	
	public ObjectiveRequestSurveys(string name, int goal) : base(name, "SurveyRequested_Count") {
		behaviour = new ObjectiveBehaviour<int, int>(0,goal,Check,Store);
	}
	public override string Description {
		get {
			return string.Format("Ask the Conservationist to Survey {0} acres of surveyable land.\nCurrent progress: {1}", behaviour.GoalValue, behaviour.Description);
		}
	}
}

//TODO: Replace second int with the Type that gets sent with survey data (probably a list of SurveyData objects)
public class ObjectivePurchaseSurveys : ObjectiveStoreInt<int>
{
	public bool Check (int val, int goal)
	{
		return val >= goal;
	}
	public void Store (ref int current, int surveys)
	{
		current = Mathf.Clamp(current+surveys,0,behaviour.GoalValue);
	}
	public ObjectivePurchaseSurveys(string name, int goal) : base (name, "SurveyDataBought_Count") {
		behaviour = new ObjectiveBehaviour<int, int>(0,goal,Check,Store);
	}
	public override string Description {
		get {
			return string.Format("Purchase Survey Data from {0} acres of land that the Conservationist has surveyed.\n Current Progress: {1}", 
				behaviour.GoalValue, behaviour.Description);
		}
	}
}

//TODO: Replace second int with the Type that gets sent when a player plants trees (probably TreePlant objects?)
public class ObjectivePlantTrees : ObjectiveStoreInt<int>
{
	public bool Check(int val, int goal)
	{
		return val >= goal;
	}
	public void Store( ref int current, int acresPlanted)
	{
		current = Mathf.Clamp(current+acresPlanted,0,behaviour.GoalValue);
	}
	public ObjectivePlantTrees(string name, int goal) : base (name, "TreesPlanted_Count") {
		behaviour = new ObjectiveBehaviour<int, int>(0,goal,Check,Store);
	}
	public override string Description {
		get {
			return string.Format("Plant any type of trees on {0} acres of land.\nCurrent Progress: {1}",behaviour.GoalValue,behaviour.Description);
		}
	}
}

public class ObjectiveHarvest20Years : Objective<int,int>
{
	int timberThisTurn = 0;
	Queue<int> timberHistory = new Queue<int>();
	int historyLength = 5;
	
	public override void OnAwake ()
	{
		Messenger.AddListener("TurnOver",OnTurnOver);
		base.OnAwake();
	}
	public override void OnDestroy ()
	{
		Messenger.RemoveListener("TurnOver",OnTurnOver);
		base.OnDestroy();
	}
	
	public bool Check(int val, int goal)
	{
		return val >= goal && timberHistory.Count >= historyLength;
	}
	public void Store( ref int current, int data)
	{
		current = data;
	}
	
	protected void OnTurnOver()
	{
		int newCount = 0;
		timberHistory.Enqueue(timberThisTurn);
		while(timberHistory.Count > historyLength) {
			timberHistory.Dequeue();
		}
		timberThisTurn = 0; //<--- THis might be a HACK
		CheckComplete(newCount);
	}
	protected override void CheckComplete (int val)
	{
		timberThisTurn += val;
		if( IsComplete() ) {
			return;
		}
		int count = 0;
		foreach(int i in timberHistory) {
			count += i;
		}
		isComplete = behaviour.Check( count );
		if( IsComplete() ) {
			Messenger<IObjective>.Broadcast("OnCompleteObjective", this);;
		}
	}
	
	public ObjectiveHarvest20Years(string name, int initStat, int goal) : base (name, GameManager.kTimberCountIncreased) {
		behaviour = new ObjectiveBehaviour<int,int> (initStat ,goal,Check,Store);
		timberThisTurn = initStat;
	}
	public override string Description {
		get {
			return string.Format("Harvest {0} timber(s) in 20 years. \nCurrent Progress: {1}",behaviour.GoalValue,behaviour.Description);
		}
	}
}

public class ObjectiveBuyLand : ObjectiveSimple<bool>
{
	// TED - This seems like a horrible way to do this...
	protected bool AutoPass (bool val, bool goal)
	{
		return true;
	}

	public ObjectiveBuyLand () : base("Own your land", "LandBought")
	{
		behaviour = new ObjectiveBehaviour<bool>(false,true,AutoPass);
	}
	
	public override string Description {
		get {
			return string.Format ("Buy a land. ");
		}
	}
}

public class ObjectiveSurveyLand : ObjectiveSimple<bool>
{
	// TED - This seems like a horrible way to do this...
	protected bool AutoPass (bool val, bool goal)
	{
		return true;
	}

	public ObjectiveSurveyLand () : base("Survey Land", "LandSurveyed")
	{
		behaviour = new ObjectiveBehaviour<bool>(false,true,AutoPass);
	}
	
	public override string Description {
		get {
			return string.Format ("Survey a land");
		}
	}
}

/*

public class ObjectiveHarvestStrategy : Objective<Harvest,Harvest.Products>
{
	public bool Check(Harvest val, Harvest goal)
	{
		return true;
	}
	public void Store( ref Harvest current, Harvest.Products change )
	{
		
	}
	public ObjectiveHarvestStrategy(string name, Quest o, int goalPeriod, int goalYield) : base (name, "HarvestProducts")
	
}*/


#endregion


