using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// An action for requesting that a research outpost be constructed.
/// </summary>
public class PlayerActionRequestOutpost : PlayerActionBuildOutpost
{
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
		return base.IsPermittedOnResourceTile(tile);
	}
}
