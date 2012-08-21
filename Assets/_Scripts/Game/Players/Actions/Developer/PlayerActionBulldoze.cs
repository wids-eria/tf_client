using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// An action for bulldozing a section of land.
/// </summary>
public class PlayerActionBulldoze : PlayerAction
{
	/// <summary>
	/// Modified fields.
	/// </summary>
	internal class ModifiedFields {
		public readonly int landcover_class_code = (int)LandCoverType.DevelopedOpen;
		public readonly bool outpost = false;
		public readonly int housing_capacity = 0;
		public readonly int num_2_inch_diameter_trees = 0;
		public readonly int num_4_inch_diameter_trees = 0;
		public readonly int num_6_inch_diameter_trees = 0;
		public readonly int num_8_inch_diameter_trees = 0;
		public readonly int num_10_inch_diameter_trees = 0;
		public readonly int num_12_inch_diameter_trees = 0;
		public readonly int num_14_inch_diameter_trees = 0;
		public readonly int num_16_inch_diameter_trees = 0;
		public readonly int num_18_inch_diameter_trees = 0;
		public readonly int num_20_inch_diameter_trees = 0;
		public readonly int num_22_inch_diameter_trees = 0;
		public readonly int num_24_inch_diameter_trees = 0;
	}
	
	/// <summary>
	/// Initialize the tile modifications.
	/// </summary>
	void Awake()
	{
		m_tileModifications.resource_tile = new ModifiedFields();
	}
	
	/// <summary>
	/// Gets the do it WWW object.
	/// </summary>
	/// <returns>
	/// The do it WWW object.
	/// </returns>
	/// <param name='selection'>
	/// Selection.
	/// </param>
	override protected WWW GetDoItWWW(ResourceTileSelection selection)
	{
		string url = string.Format("{0}/worlds/{1}/resource_tiles/bulldoze.json", WebRequests.urlServer, UserData.worldNumber);
		return WWWX.Post(url, selection.ToJson(), WebRequests.authenticatedParameters);
	}
	
	/// <summary>
	/// Determines whether this action is permitted on the specified tile.
	/// </summary>
	/// <returns>
	/// <c>true</c> if this action is permitted on the specified tile; otherwise, <c>false</c>.
	/// </returns>
	/// <param name='tile'>
	/// The resource tile in question.
	/// </param>
	public override bool IsPermittedOnResourceTile(ResourceTile tile)
	{
		return !(
			tile.zoneType != ZoneType.Residential ||
			tile.baseCoverType == BaseCoverType.Excluded ||
			tile.baseCoverType == BaseCoverType.Unknown ||
			tile.baseCoverType == BaseCoverType.Water ||
			tile.housingOccupants > 0 ||
			(tile.baseCoverType == BaseCoverType.Developed && tile.housingCapacity == 0 && !tile.hasOutpost)
		);
	}
}