using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ProjectorActions : ProjectorMapSet<PlayerAction> 
{
	public ProjectorActions(string name) : base(name)
	{
		m_defaultValue = new DataMapAction();
		// NOTE : --- I DON'T LIKE newing all this crap (maybe a different way to do it?)
		Dictionary<Type,DataMapBase> m_actionMap = new Dictionary<Type, DataMapBase>() {
			{ typeof(PlayerActionPanCamera), new DataMapEmpty() },
			{ typeof(PlayerActionBuildHousing), new DataMapDesirability()  },
		};
		
		//Get the player actions and populate the dictionary
		foreach (PlayerAction p in Player.current.actions) {
			if (m_actionMap.ContainsKey(p.GetType())) {
				Add(p,m_actionMap[p.GetType()]);
			} else {
				Add(p,m_defaultValue);
			}
		}
		m_actionMap.Clear();
		SetActiveKey(Player.current.actions[0]); //<--- Ted: HACK
		SetProjecting(true);
	}
}

public class ProjectorSelection : ProjectorMapSet<ProjectorSelection.SelectionType>
{
	public enum SelectionType {
		Normal,
		Inverted,
	}
	public ProjectorSelection(string name) : base(name)
	{
		m_defaultKey = SelectionType.Normal;
		Add(SelectionType.Normal,	new DataMapSelection());
		Add(SelectionType.Inverted, new DataMapInvertedSelection());
		SetActiveKey(SelectionType.Normal);
	}
}


public class ProjectorGrid : ProjectorMapSet<GridDisplayFlags>
{
	public ProjectorGrid(string name, int megaTileSize) : base(name) {
		renderOrder = 5;
		projectionStyle = ProjectionStyle.Grid;
		Add(GridDisplayFlags.Megatile, new DataMapBase(
			DataMapController.GetProjectorPack(projectionStyle).baseTexture, 
			Vector2.one*(activeProjector.orthographicSize*2f/Megatile.size),
			new Vector2(-1f/3f,-1f/3f)));
		Add(GridDisplayFlags.Tile, 
			new DataMapBase(DataMapController.GetProjectorPack(projectionStyle).baseTexture, Vector2.one*(activeProjector.orthographicSize*2f)));
		SetActiveKey(GridDisplayFlags.Megatile);
		SetProjecting(true);
		activeProjector.transform.localPosition += new Vector3(0f,0.5f,0f);
	}
}

public class ProjectorCloud : ProjectorBase
{
	public ProjectorCloud(string name) : base(name) {
		renderOrder = -1;
		projectionStyle = ProjectionStyle.Cloud;
		activeMap = new DataMapBase(DataMapController.GetProjectorPack(projectionStyle).baseTexture, Vector2.one*0.25f);
		SetProjectionMap(activeMap);
	}
}

