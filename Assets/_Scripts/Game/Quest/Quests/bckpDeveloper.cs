//using System;
//using System.Collections.Generic;
//using UnityEngine;
//
//
//
//#region Quests
//
//
//public class QuestDevelopLand : Quest
//{
//	public QuestDevelopLand () : base( QuestConstants.kDeveloperNames[0], Player.CharacterClass.Developer )
//	{
//		AddObjective (new ObjectiveDevelopLand ("Bulldoze", 10, this));
//		QuestsToUnlock.Add (QuestConstants.kDeveloperNames[1]);
//		Description = "Send the bulldozers to work over several parcels of land to prepare them for Housing.";
//		PointValue = 3;
//	}
//}
//
//public class QuestBuildHousing : Quest
//{
//	public QuestBuildHousing () : base (QuestConstants.kDeveloperNames[1], Player.CharacterClass.Developer)
//	{
//		AddObjective( new ObjectiveBuildHousing("Place Homes", 10, this));
//		QuestsToUnlock.Add(QuestConstants.kDeveloperNames[2]);
//		Description = "Erect homes on Developed lands to provide new housing for interested citizens.";
//		PointValue = 3;
//	}
//}
//public class QuestOutpostProcess : Quest
//{
//	public QuestOutpostProcess () : base (QuestConstants.kDeveloperNames[2], Player.CharacterClass.Developer)
//	{
//		AddObjective( new ObjectiveBuildOutpost("Build Outpost", 1, this));
//		AddObjective( new ObjectiveRequestSurveys("Request Surveys", 25, this));
//		AddObjective( new ObjectivePurchaseSurveys("Purchase Surveys", 25, this));
//		Description = "Build an Outpost that allows the Conservationist to collect survey data. Survey data can be used to " +
//			"give Desirability information about locations, making the task of populating Housing units easier.";
//		PointValue = 4;
//	}
//}
//
//public class QuestHousesAndPop : Quest
//{
//	public QuestHousesAndPop() : base (QuestConstants.kDeveloperNames[3], Player.CharacterClass.Developer)
//	{
//		AddObjective( new ObjectiveBuildHousing("VacationCentral",50,this));
//		AddObjective( new ObjectiveBuildCapacity("Housing Capacity",1500,this));
//		AddObjective( new ObjectiveTotalPopulation("Big 5,000",5000,this));
//	}
//}
//
//public class QuestTotalPopulation : Quest
//{
//	public QuestTotalPopulation() : base (QuestConstants.kDeveloperNames[4], Player.CharacterClass.Developer)
//	{
//		int startPop = GameManager.worldData.human_population;
//		AddObjective(new ObjectiveIncreaseInitialPopulation("Increase population", startPop, startPop+100, this));
//	}
//}
//
//
//
//
//#endregion
//
//#region Objectives
//
//public class ObjectiveDevelopLand : ObjectiveStoreInt<int>
//{
//	protected bool Check(int val, int goal)
//	{
//		return val >= goal;
//	}
//	protected void Store(ref int current, int num)
//	{
//		current += num;
//	}
//	
//	public ObjectiveDevelopLand (string name, int goal, Quest o) : base(name,"LandBulldozed_Count",o)
//	{
//		behaviour = new ObjectiveBehaviour<int>(0,goal,Check,Store);
//	}
//
//	public override string Description {
//		get {
//			return string.Format ("Bulldoze {0} acre(s) of land for development. (Development Requires Residential Zoning)\nCurrent Progress: {1}", 
//				behaviour.GoalValue, behaviour.Description);
//		}
//	}
//}
//
//public class ObjectiveBuildHousing : ObjectiveStoreInt<int>
//{
//	protected string m_description;
//	protected bool Check(int val, int goal)
//	{
//		return val >= goal;
//	}
//	protected void Store(ref int current, int num)
//	{
//		current += num;
//	}
//	public ObjectiveBuildHousing( string name, int goal, Quest o) : base(name,"HousingBuilt_Count",o)
//	{
//		behaviour = new ObjectiveBehaviour<int,int>(0,goal,Check,Store);
//		m_description = "Build any type of housing unit on {0} acres of developed land.\nCurrent Progress: {1}";
//	}
//	
//	public override string Description {
//		get {
//			return string.Format( m_description, behaviour.GoalValue, behaviour.Description);
//		}
//	}
//}
//
//public class ObjectiveBuildCapacity : ObjectiveStoreInt<int>
//{
//	protected string m_description;
//	protected bool Check(int val, int goal)
//	{
//		return val >= goal;
//	}
//	protected void Store(ref int current, int num)
//	{
//		current += num;
//	}
//	public ObjectiveBuildCapacity( string name, int goal, Quest o) : base(name, "HousingBuilt", o)
//	{
//		behaviour = new ObjectiveBehaviour<int, int>(0,goal,Check,Store);
//		m_description = "Build enough new Housing that will provide space for {0} new residents. \nCurrent Progress: {1}";
//	}
//	public ObjectiveBuildCapacity( string name, int start, int goal, Quest o) : base(name, "CapacityChanged", o)
//	{
//		behaviour = new ObjectiveBehaviour<int, int>(start,goal,Check,Store);
//		m_description = "Build upon current housing capacities providing space for a total of {0} residents. \nCurrent Progress: {1}";
//	}
//	public override string Description {
//		get {
//			return string.Format(m_description,behaviour.CurrentValue,behaviour.Description);
//		}
//	}
//}
//
//public class ObjectiveTotalPopulation : ObjectiveStoreInt<WorldData>
//{
//	protected bool Check(int val, int goal)
//	{
//		return val >= goal;
//	}
//	protected void Store(ref int current, WorldData data)
//	{
//		current = data.human_population;
//	}
//	public ObjectiveTotalPopulation(string name, int goal, Quest o) : base(name, GameManager.kWorldDataChanged, o)
//	{
//		behaviour = new ObjectiveBehaviour<int,WorldData>(0,goal,Check,Store);
//	}
//	public override string Description {
//		get {
//			return string.Format("Reach a total population of {0} residents. Status: {1}",behaviour.CurrentValue,behaviour.Description);
//		}
//	}
//}
//
//public class ObjectiveIncreasePopulation : Objective<int,WorldData>
//{
//	protected int m_lastPop;
//	
//	protected bool Check(int val, int goal)
//	{
//		return val >= goal;
//	}
//	protected void Store(ref int deltaPop, WorldData data)
//	{
//		deltaPop = data.human_population - m_lastPop;
//		m_lastPop = data.human_population;
//	}
//	public ObjectiveIncreasePopulation(string name,int startPop, int goalIncrease, Quest o) : base(name,GameManager.kWorldDataChanged, o)
//	{
//		behaviour = new ObjectiveBehaviour<int, WorldData>(0,goalIncrease,Check,Store);
//		m_lastPop = startPop;
//	}
//	public override string Description {
//		get {
//			return string.Format("Achieve an increase of population of {0} residents in a single turn.", behaviour.GoalValue);
//		}
//	}
//}
//
//public class ObjectiveIncreaseInitialPopulation : ObjectiveStoreInt<WorldData>
//{
//	protected bool Check(int val, int goal)
//	{
//		return val >= goal;
//	}
//	protected void Store(ref int current, WorldData newWorldData)
//	{
//		current = newWorldData.human_population;	
//	}
//	public ObjectiveIncreaseInitialPopulation(string name, int startPop, int goalPop, Quest o) : base(name,GameManager.kWorldDataChanged,o)
//	{
//		behaviour = new ObjectiveBehaviour<int, WorldData>(startPop,goalPop,Check,Store);
//	}
//	
//	public override string Description {
//		get {
//			return string.Format( "Increase the number of residents by {0}. \nCurrent Population: {1}", 
//				behaviour.GoalValue-behaviour.CurrentValue, behaviour.Description);
//		}
//	}
//}
//
//public class ObjectiveBuildOutpost : ObjectiveStoreInt<int>
//{
//	protected bool Check(int val, int goal)
//	{
//		return val >= goal;
//	}
//	protected void Store(ref int current, int num)
//	{
//		current += num;
//	}
//	public ObjectiveBuildOutpost(string name, int goal, Quest o) : base(name, "OutpostBuilt_Count",o)
//	{
//		behaviour = new ObjectiveBehaviour<int, int>(0,goal,Check,Store);	
//	}
//	public override string Description {
//		get {
//			return string.Format("Build {0} Outpost(s) from which the Conservationist can begin collecting survey data.",
//				behaviour.GoalValue,behaviour.Description);
//		}
//	}
//}
//
//
//
//
//#endregion
//
//
