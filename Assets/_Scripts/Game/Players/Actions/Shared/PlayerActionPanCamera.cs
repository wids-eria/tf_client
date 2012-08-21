using UnityEngine;
using System.Collections;

/// <summary>
/// Player action pan camera.
/// </summary>
public class PlayerActionPanCamera : PlayerAction
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
		return true;
	}
}