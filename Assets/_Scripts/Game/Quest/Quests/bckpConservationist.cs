//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//
//#region Quests
//
///// <summary>
///// Quest survey area.
///// </summary>
//public class QuestSurveyArea : Quest
//{
//	public QuestSurveyArea() : base( QuestConstants.kConservationistNames[0], Player.CharacterClass.Conservationist )
//	{
//		AddObjective (new ObjectiveSurveyArea ("Survey Land", 10, this));
//		QuestsToUnlock.Add (QuestConstants.kConservationistNames[1]);
//		Description = "Survey area for conservation of ecosystem.";
//		PointValue = 3;
//	}
//}
///// <summary>
///// Quest protect area.
///// </summary>
//public class QuestProtectArea : Quest
//{
//	public QuestProtectArea() : base (QuestConstants.kConservationistNames[1], Player.CharacterClass.Conservationist)
//	{
//		AddObjective (new ObjectiveProtectArea ("Protect Land", 10, this));
//		QuestsToUnlock.Add (QuestConstants.kConservationistNames[2]);
//		Description = "Protect area with Martens";
//		PointValue = 3;
//	}
//}
///// <summary>
///// Quest request outpost.
///// </summary>
//public class QuestRequestOutpost : Quest
//{
//	public QuestRequestOutpost() : base (QuestConstants.kConservationistNames[2], Player.CharacterClass.Conservationist)
//	{
//		AddObjective (new ObjectiveRequestOutpost ("Request Outpost", 10, this));
//		QuestsToUnlock.Add (QuestConstants.kConservationistNames[3]);
//		Description = "Request outposts for animal conservation.";
//		PointValue = 3;
//	}
//}
///// <summary>
///// Quest restore area.
///// </summary>
//public class QuestRestoreArea : Quest
//{
//	public QuestRestoreArea() : base (QuestConstants.kConservationistNames[3], Player.CharacterClass.Conservationist)
//	{
//		AddObjective (new ObjectiveRequestOutpost ("Restore Land", 10, this));
//		QuestsToUnlock.Add (QuestConstants.kConservationistNames[4]);
//		Description = "Restore land for animal habitat.";
//		PointValue = 3;
//	}
//}
///// <summary>
///// Quest fulfill survey.
///// </summary>
//public class QuestFulfillSurvey : Quest
//{
//	public QuestFulfillSurvey() : base (QuestConstants.kConservationistNames[4], Player.CharacterClass.Conservationist)
//	{
//		AddObjective (new ObjectiveRequestOutpost ("Fulfill Survey", 10, this));
//		Description = "Fulfill the surveys for conservation.";
//		PointValue = 3;
//	}
//}
///// <summary>
///// Quest marten pop.
///// </summary>
//public class QuestMartenPop : Quest
//{
//	public QuestMartenPop() : base (QuestConstants.kConservationistNames[5],Player.CharacterClass.Conservationist)
//	{
//		int pop = GameManager.worldData.marten_suitable_tile_count;
//		AddObjective(new ObjectiveIncreaseMartenPop("Increase Habitat",pop,pop+100,this));
//		Description  = "Locate suitable Marten Habitats and replicate the conditions to increase the suitable habitat for the" +
//			"American Martens to live on.";
//		PointValue = 15;
//	}
//}
//
//#endregion
//
//#region Objectives
///// <summary>
///// Objective survey area.
///// </summary>
//public class ObjectiveSurveyArea : ObjectiveStoreInt<int>
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
//	public ObjectiveSurveyArea (string name, int goal, Quest o) : base(name,"LandSurveyed_Count",o)
//	{
//		behaviour = new ObjectiveBehaviour<int>(0,goal,Check,Store);
//	}
//
//	public override string Description {
//		get {
//			return string.Format ("Survey {0} acre(s) of land for conservation.", 
//				behaviour.GoalValue, behaviour.Description);
//		}
//	}
//}
///// <summary>
///// Objective protect area.
///// </summary>
//public class ObjectiveProtectArea : ObjectiveStoreInt<int>
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
//	public ObjectiveProtectArea (string name, int goal, Quest o) : base (name, "LandProtected_Count", o)
//	{
//		behaviour = new ObjectiveBehaviour<int>(0, goal, Check, Store);
//	}
//	
//	public override string Description{
//		get {
//			return string.Format ("Protect {0} acre(s) of land from being harmed. ",
//				behaviour.GoalValue);
//		}
//	}
//}
///// <summary>
///// Objective request outpost.
///// </summary>
//public class ObjectiveRequestOutpost : ObjectiveStoreInt<int>
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
//	public ObjectiveRequestOutpost (string name, int goal, Quest o) : base (name, "OutpostRequested", o)
//	{
//		behaviour = new ObjectiveBehaviour<int>(0, goal, Check, Store);
//	}
//	
//	public override string Description{
//		get {
//			return string.Format ("Request {0} acre(s) of outpost to be built. ",
//				behaviour.GoalValue);
//		}
//	} 
//}
///// <summary>
///// Objective restore area.
///// </summary>
//public class ObjectiveRestoreArea : ObjectiveStoreInt<int>
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
//	public ObjectiveRestoreArea (string name, int goal, Quest o) : base (name, "LandRestored_Count", o)
//	{
//		behaviour = new ObjectiveBehaviour<int>(0, goal, Check, Store);
//	}
//	
//	public override string Description{
//		get {
//			return string.Format ("Restore {0} acre(s) of land for ecosystem conservation. ",
//				behaviour.GoalValue);
//		}
//	} 
//}
///// <summary>
///// Objective fulfill survey.
///// </summary>
//public class ObjectiveFulfillSurvey : ObjectiveStoreInt<int>
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
//	public ObjectiveFulfillSurvey (string name, int goal, Quest o) : base (name, "SurveyRequested_Count", o)
//	{
//		behaviour = new ObjectiveBehaviour<int>(0, goal, Check, Store);
//	}
//	
//	public override string Description{
//		get {
//			return string.Format ("Fulfill {0} survey request(s) for the other 2 player classes.",
//				behaviour.GoalValue);
//		}
//	} 
//}
///// <summary>
///// Objective increase marten pop.
///// </summary>
//public class ObjectiveIncreaseMartenPop : ObjectiveStoreInt<WorldData>
//{
//	protected bool Check(int val, int goal)
//	{
//		return val >= goal;
//	}
//	protected void Store(ref int current, WorldData newWorldData)
//	{
//		current = newWorldData.marten_suitable_tile_count;	
//	}
//	public ObjectiveIncreaseMartenPop(string name, int startPop, int goalPop, Quest o) : base(name,GameManager.kWorldDataChanged,o)
//	{
//		behaviour = new ObjectiveBehaviour<int, WorldData>(startPop,goalPop,Check,Store);
//	}
//	
//	public override string Description {
//		get {
//			return string.Format( "Increase the Marten suitable living area by {0}. \nCurrent Progress: {1}", 
//				behaviour.GoalValue-behaviour.CurrentValue, behaviour.Description);
//		}
//	}
//}
//
//
//#endregion