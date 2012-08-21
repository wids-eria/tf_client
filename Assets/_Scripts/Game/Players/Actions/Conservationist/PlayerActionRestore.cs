using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// An action for restoring fallow land.
/// </summary>
public class PlayerActionRestore : PlayerAction
{
	/// <summary>
	/// Initialize the tile modifications.
	/// </summary>
	void Awake()
	{
		m_tileModifications.resource_tile = new ModifiedFields();
	}
	
	/// <summary>
	/// Modified fields.
	/// </summary>
	internal class ModifiedFields
	{
		/// <summary>
		/// The land cover class code correspondong to grassland_herbaceous.
		/// </summary>
		public readonly int landcover_class_code = 71;
		public readonly int housing_capacity = 0;
		public readonly int housing_occupants = 0;
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
		return (
			(tile.baseCoverType == BaseCoverType.Barren) || 
			(tile.baseCoverType == BaseCoverType.Developed && tile.zoneType != ZoneType.Residential)
			);
	}
}