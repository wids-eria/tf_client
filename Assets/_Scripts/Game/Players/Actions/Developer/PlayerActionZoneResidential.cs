using UnityEngine;
using System.Collections;

public class PlayerActionZoneResidential : PlayerAction 
{
	/// <summary>
	/// Modified fields.
	/// </summary>
	internal class ModifiedFields {
			public readonly int zoning_code = 12;
		}
	/// <summary>
	/// Determines whether this instance is permitted on resource tile the specified tile.
	/// </summary>
	/// <returns>
	/// <c>true</c> if this instance is permitted on resource tile the specified tile; otherwise, <c>false</c>.
	/// </returns>
	/// <param name='tile'>
	/// If set to <c>true</c> tile.
	/// </param>
	public override bool IsPermittedOnResourceTile(ResourceTile tile)
	{
		return (
			tile.zoneType == ZoneType.None 
		);
	}
	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake()
	{
		m_tileModifications.resource_tile = new ModifiedFields();
	}
}
