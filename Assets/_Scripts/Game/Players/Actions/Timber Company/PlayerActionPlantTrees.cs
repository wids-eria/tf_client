using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// An action for planting trees on selected land.
/// </summary>
public class PlayerActionPlantTrees : PlayerAction
{
	/// <summary>
	/// Forces a put of resource tile modifications using god mode.
	/// </summary>
	/// <remarks>
	/// This is a temporary solution for implementing actions with no server API point, and can eventually be removed.
	/// </remarks>
	/// <param name='selection'>
	/// The selection.
	/// </param>
	protected override IEnumerator Put(ResourceTileSelection selection)
	{
		m_actionProgress = 0f;
		string url;
		List<ResourceTile> tiles = new List<ResourceTile>();
		float div = 1f/selection.Count();
		for (int i=0; i<selection.Count(); ++i) {
			int id = selection[i];
			// get url for current tile
			url = WebRequests.GetURLToResourceTile(id);
			// get current tile to determine supported saplings
			WWW www = WWWX.Get(url, WebRequests.authenticatedGodModeParameters);
			while (!www.isDone) {
				m_actionProgress = (i+www.progress*0.3f)*div; // about 30% done
				yield return 0;
			}
			ResourceTile tile = JSONDecoder.Decode<IndividualResourceTile>(www.text).resource_tile;
			m_tileModifications.resource_tile = new ModifiedFields(tile.supportedSaplings);
			// add new trees
			www = WWWX.Put(url, GetActionJson(), WebRequests.authenticatedGodModeParameters);
			while (!www.isDone) {
				m_actionProgress = (i+0.3f+www.progress*0.7f)*div; // about 70% done
				yield return 0;
			}
			// get the results back and append to array
			www = WWWX.Get(url, WebRequests.authenticatedGodModeParameters);
			while (!www.isDone) {
				m_actionProgress = (i+0.7f+www.progress+0.3f)*div; // all the way done
				yield return 0;
			}
			try {
				tiles.Add(JSONDecoder.Decode<IndividualResourceTile>(www.text).resource_tile);
			}
			catch (JsonException) {
				Debug.LogError(www.text);
			}
		}
//		ConcludeActionOnResourceTiles(tiles.ToArray());
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
			tile.baseCoverType == BaseCoverType.Barren ||
			tile.baseCoverType == BaseCoverType.CultivatedCrops ||
			tile.baseCoverType == BaseCoverType.Excluded ||
			tile.baseCoverType == BaseCoverType.Unknown ||
			tile.baseCoverType == BaseCoverType.Water ||
			tile.supportedSaplings == 0
		);
	}
	
	#region GUI Handling
	
	/// <summary>
	/// The tree type selectors.
	/// </summary>
	private GUIContent[] m_treeTypeSelectors = new GUIContent[System.Enum.GetValues(typeof(TreeType)).Length-1]; // no NONE
	/// <summary>
	/// The selected type of tree.
	/// </summary>
	private TreeType m_selectedTreeType = TreeType.Mixed;
	
	/// <summary>
	/// Initialize gui styles.
	/// </summary>
	void Start()
	{
		string[] names = System.Enum.GetNames(typeof(TreeType));
		for (int i=1; i<names.Length; ++i) {
			m_treeTypeSelectors[i-1] = new GUIContent(
				names[i],
				string.Format("Plant {0} trees.", (TreeType)i)
			);
		}
	}
	/// <summary>
	/// Displays the controls contents.
	/// </summary>
	protected override void DisplayControlsContents()
	{
		GUILayout.FlexibleSpace();
		GUILayout.BeginHorizontal(); {
			GUILayout.Label("Select Type:", m_mainText);
			m_selectedTreeType = (TreeType)(GUILayout.SelectionGrid(
				((int)m_selectedTreeType)-1,
				m_treeTypeSelectors,
				m_treeTypeSelectors.Length,
				m_styles.smallButton
			)+1);
		} GUILayout.EndHorizontal();
		GUILayout.FlexibleSpace();
		
		// call base implementation
		base.DisplayControlsContents();
	}
	
	#endregion
	
	#region Tile Modifier
	
	/// <summary>
	/// Modified fields.
	/// </summary>
	internal class ModifiedFields
	{
		/// <summary>
		/// Add to the number of smallest trees.
		/// </summary>
		public readonly int num_2_inch_diameter_trees;
		/// <summary>
		/// Initializes a new instance of the <see cref="PlayerActionPlantTrees.ModifiedFields"/> class.
		/// </summary>
		/// <param name='numTrees'>
		/// Number trees.
		/// </param>
		public ModifiedFields(int numTrees) : this() {
			this.num_2_inch_diameter_trees = numTrees;
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="PlayerActionPlantTrees.ModifiedFields"/> class.
		/// </summary>
		public ModifiedFields() {
			
		}
	}
	
	#endregion
}