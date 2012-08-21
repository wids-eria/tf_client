using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// An action for zoning protection on a section of land.
/// </summary>
public class PlayerActionProtect : PlayerAction
{
	/// <summary>
	/// Modified fields.
	/// </summary>
	internal class ModifiedFields
	{
		/// <summary>
		/// The zoning code corresponding to REC.
		/// </summary>
		public readonly int zoning_code = 16;
	}
	
	
	/// <summary>
	/// Initialize the tile modifications.
	/// </summary>
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
		return !(
			tile.zoneType != ZoneType.None ||
			tile.housingCapacity > 0 ||
			tile.housingOccupants > 0
		);
	}
}