using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

internal class BuildingCapacityComparer : IComparer {
	public static BuildingCapacityComparer	Instance = new BuildingCapacityComparer();

	int IComparer.Compare( object lhs, object rhs ) {
    	return Compare( (Building)lhs, (Building)rhs );
    }
	
	protected int Compare( Building lhs, Building rhs ) {
    	return Mathf.Clamp( lhs.capacity - rhs.capacity, -1, 1 );
    }
}


/// <summary>
/// An action for building housing on a section of land.
/// </summary>
public class PlayerActionBuildHousing : PlayerAction
{
	/// <summary>
	/// Temporary Container for sending tile modifications
	/// </summary>
	internal class ModifiedFields
	{
		/// <summary>
		/// The housing_capacity.
		/// </summary>
		public readonly int housing_capacity = 0;
		/// <summary>
		/// Initializes a new instance of the <see cref="PlayerActionBuildHousing.ModifiedFields"/> class.
		/// </summary>
		/// <param name='newCapacity'>
		/// New capacity.
		/// </param>
		public ModifiedFields(int newCapacity)
		{
			this.housing_capacity = newCapacity;
		}
	}

	/// <summary>
	/// The buildings.
	/// </summary>
	public Building[] buildings;
	
	/// <summary>
	/// Initialize field modifiers.
	/// </summary>
	void Awake()
	{
		this.m_tileModifications.resource_tile = new ModifiedFields(buildings[m_currentlySelectedBuildingIndex].capacity);
	}
	
	/// <summary>
	/// Gets the building with capacity up to that specified.
	/// </summary>
	/// <returns>
	/// The building with the smallest capacity greater than or equal to that specified.
	/// </returns>
	/// <param name='capacity'>
	/// Capacity.
	/// </param>
	public Building GetBuildingWithCapacity(int capacity)
	{
		if (capacity == 0) {
			return null;
		}
		
		// Sort putting smallest capacity first
		Array.Sort( buildings, BuildingCapacityComparer.Instance );
		
		// Find first building that has capacity that is closest to or greater than capacity passed in
		foreach( Building b in buildings ) {
			if( b.capacity >= capacity ) {
				return b;
			}
		}
		
		return null;
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
			tile.zoneType != ZoneType.Residential ||
			tile.baseCoverType != BaseCoverType.Developed ||
			tile.housingCapacity > 0 ||
			tile.housingOccupants > 0
		);
	}
	
	protected override bool DoResourceCalculations (ResourceTileSelection selection)
	{
		bool success = base.DoResourceCalculations (selection);
		if (success) {
			//Calculate Tree usage
			if (GameManager.worldData.timber_count >= selection.Count() * 15) {
				GameManager.economyController.UseTimber(selection.Count()*15);
				GameManager.economyController.IncreasePendingBalance(Player.CharacterClass.TimberCompany, 
					selection.Count() * EconomyController.timberRate * 15);
			} else {
				GameGUIManager.use.SetErrorMessage("There is not enough timber available to build.");
				GameGUIManager.use.PlayInvalidActionSound();
				return false;
			}
		}
		return success;
	}
	
	#region GUI Handling
	
	/// <summary>
	/// The buildings sorted in order of capacity.
	/// </summary>
	private Building[] m_buildingsInOrderOfCapacity;
	/// <summary>
	/// The selection icons.
	/// </summary>
	private GUIContent[] m_selectionIcons;
	/// <summary>
	/// Gets the currently selected building.
	/// </summary>
	/// <value>
	/// The currently selected building.
	/// </value>
	private Building m_currentlySelectedBuilding {
		get { return buildings[m_currentlySelectedBuildingIndex]; }
	}
	/// <summary>
	/// Gets or sets the index of the currently selected building.
	/// </summary>
	/// <value>
	/// The index of the currently selected building.
	/// </value>
	private int m_currentlySelectedBuildingIndex {
		get { return m_currentlySelectedBuildingIndexBackingField; }
		set {
			if (value != m_currentlySelectedBuildingIndexBackingField) {
				this.m_tileModifications.resource_tile = new ModifiedFields(buildings[value].capacity);
			}
			m_currentlySelectedBuildingIndexBackingField = value;
		}
	}
	/// <summary>
	/// The currently selected building index backing field.
	/// </summary>
	private int m_currentlySelectedBuildingIndexBackingField = 0;
	
	/// <summary>
	/// Initialize the gui styles as needed.
	/// </summary>
	void Start()
	{
		// a list of the buildings sorted by capacity
		/*List<Building> bldgList = new List<Building>(buildings);
		bldgList.Sort((x,y) => x.capacity.CompareTo(y.capacity));
		m_buildingsInOrderOfCapacity = bldgList.ToArray();*/
		
		m_selectionIcons = new GUIContent[buildings.Length];
		for (int i=0; i<m_selectionIcons.Length; ++i) {
			m_selectionIcons[i] = new GUIContent(
				buildings[i].icon,
				string.Format("Create {0}.", buildings[i].name)
			);
		}
	}
	
	/// <summary>
	/// Displays the controls contents.
	/// </summary>
	protected override void DisplayControlsContents()
	{
		// building selector
		m_currentlySelectedBuildingIndex = GUILayout.SelectionGrid(
			m_currentlySelectedBuildingIndex,
			m_selectionIcons,
			m_selectionIcons.Length,
			m_styles.smallButton,
			GUILayout.Height(96f)
		);
		GUILayout.BeginHorizontal(); {
			GUILayout.Label(m_currentlySelectedBuilding.name, m_mainText);
			GUILayout.FlexibleSpace();
			GUILayout.Label(string.Format("Capacity: {0}", m_currentlySelectedBuilding.capacity), m_mainTextAlt);
		} GUILayout.EndHorizontal();
		GUILayout.Label(m_currentlySelectedBuilding.description, m_footnoteText);
		m_styles.DrawLine(GUIStyles.LineDirection.Horizontal, GUIStyles.LineColor.Medium);
		GUILayout.FlexibleSpace();
		
		// base implementation
		base.DisplayControlsContents();
	}
	
	#endregion
}