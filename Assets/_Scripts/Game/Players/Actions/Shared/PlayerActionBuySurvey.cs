using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// An action for buying survey data on selected land.
/// </summary>
public class PlayerActionBuySurvey : PlayerAction
{
	internal class ModifiedFieldsDeveloper
	{
		public readonly bool bought_by_developer = true;
	}
	internal class ModifiedFieldsTimber
	{
		public readonly bool bought_by_timber_company = true;
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
		return tile.is_surveyed;
	}
	
	void Start()
	{
		switch(GameManager.use.currentPlayer.characterClass) {
		case Player.CharacterClass.Developer:
			m_tileModifications.resource_tile = new ModifiedFieldsDeveloper();
			break;
		case Player.CharacterClass.TimberCompany:
			m_tileModifications.resource_tile = new ModifiedFieldsTimber();
			break;
		case Player.CharacterClass.Conservationist:
			break;
		}
	}
	protected override bool DoResourceCalculations (ResourceTileSelection selection)
	{
		bool success = base.DoResourceCalculations (selection);
		if (success) {
			GameManager.economyController.IncreasePendingBalance(Player.CharacterClass.Conservationist, cost*selection.Count());
		}
		return success;
	}
}