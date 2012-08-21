using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// An action for requesting survey data on specified land.
/// </summary>
public class PlayerActionRequestSurvey : PlayerAction
{
	internal class ModifiedFields {
		public readonly bool survey_requested = true;
	}
	
	void Awake() {
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
		return (
			tile.can_be_surveyed && 
			!tile.survey_requested &&
			!tile.is_surveyed
			);
	}
}