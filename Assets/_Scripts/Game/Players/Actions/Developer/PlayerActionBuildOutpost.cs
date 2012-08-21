using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// An action for building research outposts.
/// </summary>
public class PlayerActionBuildOutpost : PlayerAction
{
	/// <summary>
	/// Modified fields.
	/// </summary>
	internal class ModifiedFields {
		public readonly bool outpost = true;
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
			tile.hasOutpost ||
			tile.baseCoverType != BaseCoverType.Developed ||
			tile.zoneType != ZoneType.Residential ||
			tile.housingCapacity > 0 ||
			tile.housingOccupants > 0
		);
	}
	
	/// <summary>
	/// Initialize the tile modifications.
	/// </summary>
	void Awake()
	{
		m_tileModifications.resource_tile = new ModifiedFields();
	}
	
	protected override WWW GetDoItWWW (ResourceTileSelection selection)
	{
		string url = string.Format("{0}build_outpost.json",WebRequests.GetResourceTileURL(selection[selection.Count()-1]));
		Debug.Log(url);
		return WWWX.Post(url,WebRequests.authenticatedParameters);
	}
	protected override int CalculateCost (ResourceTileLite[] tiles)
	{
		int count = 0;
		foreach (ResourceTileLite tile in tiles) {
			if (!tile.outpostRequested) {
				count += cost;
			} else {
				//Discount if survey was requested
				count += cost/2;
			}
		}
		return count;
	}
}