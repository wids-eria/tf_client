using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// A map that displays nothing (fully transparent)
/// </summary>
public class DataMapEmpty : DataMapBase
{
	public DataMapEmpty() : base() {
		name = "Empty";
	}
}

/// <summary>
/// Shows what zoning type a resource tile is
/// </summary>
public class DataMapZoning : DataMap
{
	public DataMapZoning() : base(FilterMode.Point) {
		name = "Zoning";
	}
	public override Color GetColorFromTile (ResourceTileLite tile) {
		switch (tile.zone) {
		case ZoneType.None:
			return GetColor(MappingColor.NoZoning);
		case ZoneType.Protected:
			return GetColor(MappingColor.Protected);
		case ZoneType.Residential:
			return GetColor(MappingColor.Residential);
		default:
			return GetColor(MappingColor.Empty);
		}
	}
}

/// <summary>
/// Map for showing the player's selection
/// </summary>
public class DataMapSelection : DataMap
{
	protected readonly Color m_highlight = new Color(1,1,1,0.3f);
	
	protected List<int> tileIds = new List<int>();
	
	public DataMapSelection() : base(FilterMode.Point) {
		name = "Selection";
		Messenger<ResourceTileSelection>.AddListener(InputManager.kTileSelectionChanged,OnSelectionChanged);
		Messenger<Region>.RemoveListener(TerrainManager.kLoadedNewTiles,PaintRegion);
		FloodFill();
	}
	~DataMapSelection() {
		Messenger<ResourceTileSelection>.RemoveListener(InputManager.kTileSelectionChanged,OnSelectionChanged);
	}
	public void OnSelectionChanged(ResourceTileSelection selection)
	{
		List<int> ids = new List<int>(selection.resource_tile_ids);
		if (tileIds.Count == ids.Count) {
			return;
		}
		if (ids.Count <= 0) {
			FloodFill();
		} else {
			if (tileIds.Count < ids.Count) {
				tileIds = ids;
			}
			TerrainManager.Status status;
			ResourceTileLite[] tiles = TerrainManager.GetResourceTileCacheGroup(out status,tileIds.ToArray());
			if (status == TerrainManager.Status.Succeeded) {
				Region region = new Region(tiles);
				region.ToTerrainRegion(out region);
				PaintRegion(region);
			}
		}
		tileIds = ids;
	}
	public override Color GetColorFromTile (ResourceTileLite tile)
	{
		bool test = InputManager.use.resourceTileSelection.Contains(tile.id);
		return (test) ? GetColor(MappingColor.Highlight) : GetColor(MappingColor.Empty);
	}
	protected virtual void FloodFill() {
		TextureHelpers.FloodFill(texture,GetColor(MappingColor.Empty),TextureHelpers.ApplyMode.Apply);
	}
}

/// <summary>
/// Data map blackening out everything but the player selection
/// </summary>
public class DataMapInvertedSelection : DataMapSelection
{
	public DataMapInvertedSelection() : base() {
		name = "InvertedSelection";
	}
	protected override void FloodFill() {
		TextureHelpers.FloodFill(texture,GetColor(MappingColor.Disabled),TextureHelpers.ApplyMode.Apply);
		SetFilterMode(FilterMode.Bilinear);
	}
	public override Color GetColorFromTile (ResourceTileLite tile)
	{
		bool test = InputManager.use.resourceTileSelection.Contains(tile.id);
		return (test) ? GetColor(MappingColor.Empty) : GetColor(MappingColor.Disabled);
	}
}

/*
public class DataMapMegaGrid : ProjectionMap
{
	public DataMapMegaGrid(Projector projector) : base(null, Vector2.one*(projector.orthographicSize*2f/Megatile.size)) {
	}
}*/


/// <summary>
/// Base class that simple shows wether or not an action can be perfromed on a tile
/// </summary>
public class DataMapAction : DataMap
{
	public DataMapAction() : base() {
		name = "Action";
		Messenger.AddListener(TerrainManager.kRefreshAll, Refresh);
	}
	public override Color GetColorFromTile (ResourceTileLite tile) {
		//int cmp = 1 << Player.current.permissionBitmaskByActionType[GameGUIManager.use.currentAction.GetType()];
		return (tile.IsActionPermitted(InputManager.use.currentAction)) ? GetColor(MappingColor.Empty) : GetColor(MappingColor.Disabled);
	}
}

/*
public class DataMapSurvey : DataMapAction
{
	public override Color GetColorFromTile (ResourceTileLite tile)
	{
		Color watch = base.GetColorFromTile(tile);
		if (watch == GetColor(MappingColor.Disabled)) {
			return watch;
		}
		if (!tile.isSurveyed) {
			
		}
		return watch;
	}
}*/

public class DataMapDesirability : DataMap
{
	public DataMapDesirability() : base() {
		name = "Desirability";
	}
	public override Color GetColorFromTile (ResourceTileLite tile)
	{
		return Color.Lerp(GetColor(MappingColor.Undesirable),GetColor(MappingColor.Desirable),tile.desirability);
	}
}

public class DataMapOwnership : DataMap
{
	public DataMapOwnership() {
		name = "Ownership";
	}
	public override Color GetColorFromTile (ResourceTileLite tile)
	{
		if ( tile.idOwner == Player.current.id ) {
			return GetColor(MappingColor.UserOwned);
		} else {
			if ( tile.idOwner != -1) {
				return GetColor(MappingColor.OtherOwned);
			} else {
				return GetColor(MappingColor.Unowned);
			}
		}
	}
}














