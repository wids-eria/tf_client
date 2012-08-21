using UnityEngine;
using System.Collections;

/*
/// <summary>
/// base action request class 
/// </summary>
public abstract class ActionRequest
{
	public string time;
	public int costAction;
	public int costRequest;
	public Player requester;
	public Megatile tile;
	public PlayerAction action;
	
	/// <summary>
	/// Create a new instance and initialize is time 
	/// </summary>
	public ActionRequest(PlayerAction action, Player requester, Megatile tile, int costAction, int costRequest)
	{
		this.action = action;
		this.requester = requester;
		this.tile = tile;
		this.costAction = costAction;
		this.costRequest = costRequest;
		this.time = System.DateTime.Now.ToString("g");
	}
	
	/// <summary>
	/// Have the doer perform the request
	/// </summary>
	/// <param name="doer">
	/// A <see cref="Player"/>
	/// </param>
	/// <returns>
	/// A <see cref="System.Boolean"/> true if the request was succesfully performed; otherwise false
	/// </returns>
	public virtual bool PerformRequest(Player doer)
	{
		requester.actionRequestList.Remove(this);
		doer.cash += costRequest;
		return true;
	}
	
	/// <summary>
	/// Remove the request from the requester's list 
	/// </summary>
	protected void ConcludeRequest()
	{
		
	}
}

/// <summary>
/// bulldoze action request class 
/// </summary>
public class ActionRequestBulldoze : ActionRequest
{
	/// <summary>
	/// Constructor 
	/// </summary>
	/// <param name="action">
	/// A <see cref="PlayerAction"/>
	/// </param>
	/// <param name="player">
	/// A <see cref="Player"/>
	/// </param>
	/// <param name="tile">
	/// A <see cref="Megatile"/>
	/// </param>
	/// <param name="costAction">
	/// A <see cref="System.Int32"/>
	/// </param>
	/// <param name="costRequest">
	/// A <see cref="System.Int32"/>
	/// </param>
	public ActionRequestBulldoze(PlayerAction action, Player player, Megatile tile, int costAction, int costRequest) : base(action, player, tile, costAction, costRequest)
	{
		
	}
	
	/// <summary>
	/// Have the doer perform the request
	/// </summary>
	/// <param name="doer">
	/// A <see cref="Player"/>
	/// </param>
	/// <returns>
	/// A <see cref="System.Boolean"/> true if the request was succesfully performed; otherwise false
	/// </returns>
	public override bool PerformRequest(Player doer)
	{
		// make it so
		bool ret = (doer.GetActionByType(action.GetType()) as PlayerActionBulldoze).Bulldoze(requester, doer, tile);
		if (ret) base.PerformRequest(doer);
		return ret;
	}
}

/// <summary>
/// build stucture action request class 
/// </summary>
public class ActionRequestBuildStructure : ActionRequest
{
	public float density;
	public float intensity;
	
	/// <summary>
	/// Constructor 
	/// </summary>
	/// <param name="action">
	/// A <see cref="PlayerAction"/>
	/// </param>
	/// <param name="player">
	/// A <see cref="Player"/>
	/// </param>
	/// <param name="tile">
	/// A <see cref="Megatile"/>
	/// </param>
	/// <param name="costAction">
	/// A <see cref="System.Int32"/>
	/// </param>
	/// <param name="costRequest">
	/// A <see cref="System.Int32"/>
	/// </param>
	public ActionRequestBuildStructure(PlayerAction action, Player player, Megatile tile, int costAction, int costRequest, float density, float intensity) : base(action, player, tile, costAction, costRequest)
	{
		this.density = density;
		this.intensity = intensity;
	}
	
	/// <summary>
	/// Have the doer perform the request
	/// </summary>
	/// <param name="doer">
	/// A <see cref="Player"/>
	/// </param>
	/// <returns>
	/// A <see cref="System.Boolean"/> true if the request was succesfully performed; otherwise false
	/// </returns>
	public override bool PerformRequest(Player doer)
	{
		// make it so
		bool ret = (doer.GetActionByType(action.GetType()) as PlayerActionBuildStructures).Build(requester, doer, tile, density, intensity);
		if (ret) base.PerformRequest(doer);
		return ret;
	}
}

/// <summary>
/// clear cut action request class 
/// </summary>
public class ActionRequestClearcut : ActionRequest
{
	/// <summary>
	/// Constructor 
	/// </summary>
	/// <param name="action">
	/// A <see cref="PlayerAction"/>
	/// </param>
	/// <param name="player">
	/// A <see cref="Player"/>
	/// </param>
	/// <param name="tile">
	/// A <see cref="Megatile"/>
	/// </param>
	/// <param name="costAction">
	/// A <see cref="System.Int32"/>
	/// </param>
	/// <param name="costRequest">
	/// A <see cref="System.Int32"/>
	/// </param>
	public ActionRequestClearcut(PlayerAction action, Player player, Megatile tile, int costAction, int costRequest) : base(action, player, tile, costAction, costRequest)
	{
		
	}
	
	/// <summary>
	/// Have the doer perform the request
	/// </summary>
	/// <param name="doer">
	/// A <see cref="Player"/>
	/// </param>
	/// <returns>
	/// A <see cref="System.Boolean"/> true if the request was succesfully performed; otherwise false
	/// </returns>
	public override bool PerformRequest(Player doer)
	{
		// make it so
		bool ret = (doer.GetActionByType(action.GetType()) as PlayerActionHarvestTimber).Clearcut(requester, doer, tile);
		if (ret) base.PerformRequest(doer);
		return ret;
	}
}

/// <summary>
/// restore land action request class 
/// </summary>
public class ActionRequestRestoreLand : ActionRequest
{
	/// <summary>
	/// Constructor 
	/// </summary>
	/// <param name="action">
	/// A <see cref="PlayerAction"/>
	/// </param>
	/// <param name="player">
	/// A <see cref="Player"/>
	/// </param>
	/// <param name="tile">
	/// A <see cref="Megatile"/>
	/// </param>
	/// <param name="costAction">
	/// A <see cref="System.Int32"/>
	/// </param>
	/// <param name="costRequest">
	/// A <see cref="System.Int32"/>
	/// </param>
	public ActionRequestRestoreLand(PlayerAction action, Player player, Megatile tile, int costAction, int costRequest) : base(action, player, tile, costAction, costRequest)
	{
		
	}
	
	/// <summary>
	/// Have the doer perform the request
	/// </summary>
	/// <param name="doer">
	/// A <see cref="Player"/>
	/// </param>
	/// <returns>
	/// A <see cref="System.Boolean"/> true if the request was succesfully performed; otherwise false
	/// </returns>
	public override bool PerformRequest(Player doer)
	{
		// make it so
		bool ret = (doer.GetActionByType(action.GetType()) as PlayerActionRestoreLand).Restore(requester, doer, tile);
		if (ret) base.PerformRequest(doer);
		return ret;
	}
}

/// <summary>
/// survey land action request class 
/// </summary>
public class ActionRequestSurveyLand : ActionRequest
{
	/// <summary>
	/// Constructor 
	/// </summary>
	/// <param name="action">
	/// A <see cref="PlayerAction"/>
	/// </param>
	/// <param name="player">
	/// A <see cref="Player"/>
	/// </param>
	/// <param name="tile">
	/// A <see cref="Megatile"/>
	/// </param>
	/// <param name="costAction">
	/// A <see cref="System.Int32"/>
	/// </param>
	/// <param name="costRequest">
	/// A <see cref="System.Int32"/>
	/// </param>
	public ActionRequestSurveyLand(PlayerAction action, Player player, Megatile tile, int costAction, int costRequest) : base(action, player, tile, costAction, costRequest)
	{
		
	}
	
	/// <summary>
	/// Have the doer perform the request
	/// </summary>
	/// <param name="doer">
	/// A <see cref="Player"/>
	/// </param>
	/// <returns>
	/// A <see cref="System.Boolean"/> true if the request was succesfully performed; otherwise false
	/// </returns>
	public override bool PerformRequest(Player doer)
	{
		// make it so
		bool ret = (doer.GetActionByType(action.GetType()) as PlayerActionSurveyLand).Survey(requester, doer, tile);
		if (ret) base.PerformRequest(doer);
		return ret;
	}
}

/// <summary>
/// manage forest action request class 
/// </summary>
public class ActionRequestManageForest : ActionRequest
{
	/// <summary>
	/// Constructor 
	/// </summary>
	/// <param name="action">
	/// A <see cref="PlayerAction"/>
	/// </param>
	/// <param name="player">
	/// A <see cref="Player"/>
	/// </param>
	/// <param name="tile">
	/// A <see cref="Megatile"/>
	/// </param>
	/// <param name="costAction">
	/// A <see cref="System.Int32"/>
	/// </param>
	/// <param name="costRequest">
	/// A <see cref="System.Int32"/>
	/// </param>
	public ActionRequestManageForest(PlayerAction action, Player player, Megatile tile, int costAction, int costRequest) : base(action, player, tile, costAction, costRequest)
	{
		
	}
	
	/// <summary>
	/// Have the doer perform the request
	/// </summary>
	/// <param name="doer">
	/// A <see cref="Player"/>
	/// </param>
	/// <returns>
	/// A <see cref="System.Boolean"/> true if the request was succesfully performed; otherwise false
	/// </returns>
	public override bool PerformRequest(Player doer)
	{
		// make it so
		bool ret = (doer.GetActionByType(action.GetType()) as PlayerActionManageForest).Manage(requester, doer, tile);
		if (ret) base.PerformRequest(doer);
		return ret;
	}
}

/// <summary>
/// create park action request class 
/// </summary> 
public class ActionRequestCreatePark : ActionRequest
{
	public float density;
	
	/// <summary>
	/// Constructor 
	/// </summary>
	/// <param name="action">
	/// A <see cref="PlayerAction"/>
	/// </param>
	/// <param name="player">
	/// A <see cref="Player"/>
	/// </param>
	/// <param name="tile">
	/// A <see cref="Megatile"/>
	/// </param>
	/// <param name="costAction">
	/// A <see cref="System.Int32"/>
	/// </param>
	/// <param name="costRequest">
	/// A <see cref="System.Int32"/>
	/// </param>
	public ActionRequestCreatePark(PlayerAction action, Player player, Megatile tile, int costAction, int costRequest, float density) : base(action, player, tile, costAction, costRequest)
	{
		this.density = density;
	}
	
	/// <summary>
	/// Have the doer perform the request
	/// </summary>
	/// <param name="doer">
	/// A <see cref="Player"/>
	/// </param>
	/// <returns>
	/// A <see cref="System.Boolean"/> true if the request was succesfully performed; otherwise false
	/// </returns>
	public override bool PerformRequest(Player doer)
	{
		// make it so
		bool ret = (doer.GetActionByType(action.GetType()) as PlayerActionCreatePark).Create(requester, doer, tile, density);
		if (ret) base.PerformRequest(doer);
		return ret;
	}
}

/// <summary>
/// create atv trail action request class 
/// </summary>
public class ActionRequestCreateATVTrail : ActionRequest
{
	/// <summary>
	/// Constructor 
	/// </summary>
	/// <param name="action">
	/// A <see cref="PlayerAction"/>
	/// </param>
	/// <param name="player">
	/// A <see cref="Player"/>
	/// </param>
	/// <param name="tile">
	/// A <see cref="Megatile"/>
	/// </param>
	/// <param name="costAction">
	/// A <see cref="System.Int32"/>
	/// </param>
	/// <param name="costRequest">
	/// A <see cref="System.Int32"/>
	/// </param>
	public ActionRequestCreateATVTrail(PlayerAction action, Player player, Megatile tile, int costAction, int costRequest) : base(action, player, tile, costAction, costRequest)
	{
		
	}
	
	/// <summary>
	/// Have the doer perform the request
	/// </summary>
	/// <param name="doer">
	/// A <see cref="Player"/>
	/// </param>
	/// <returns>
	/// A <see cref="System.Boolean"/> true if the request was succesfully performed; otherwise false
	/// </returns>
	public override bool PerformRequest(Player doer)
	{
		// make it so
		bool ret = (doer.GetActionByType(action.GetType()) as PlayerActionCreateATVTrails).CreateTrails(requester, doer, tile);
		if (ret) base.PerformRequest(doer);
		return ret;
	}
}
*/