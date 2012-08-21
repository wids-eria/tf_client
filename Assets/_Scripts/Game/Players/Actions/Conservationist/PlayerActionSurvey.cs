using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// An action for surveying a chunk of land.
/// </summary>
public class PlayerActionSurvey : PlayerAction
{
	internal class ModifiedFields 
	{
		public readonly bool is_surveyed = true;
	}
	
	void Awake()
	{
		m_tileModifications.resource_tile = new ModifiedFields();
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
		//return TerrainManager.use.IsWorldPositionInRadiusOfAnyOutpost(tile.GetSimpleCenterPoint()) && !tile.is_surveyed;
		return (!tile.is_surveyed && tile.can_be_surveyed);
	}
	
	protected override int CalculateCost (ResourceTileLite[] tiles)
	{
		int count = 0;
		foreach (ResourceTileLite tile in tiles) {
			if (!tile.surveyRequested) {
				count += cost;
			} else {
				//Discount if survey was requested
				count += cost/2;
			}
		}
		return count;
	}
}