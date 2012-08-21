using UnityEngine;
using System.Collections;

/// <summary>
/// A player action for manually setting tile properties as requested.
/// </summary>
public class PlayerActionDivineIntervention : PlayerAction
{
	/// <summary>
	/// The width of the selection grid.
	/// </summary>
	private readonly int m_selectionGridCount = 3;
	
	#region zoneType
	/// <summary>
	/// The type of the zone.
	/// </summary>
	private ZoneType m_zoneType;
	/// <summary>
	/// Modify zone type.
	/// </summary>
	internal struct ModifyZoneType {
		/// <summary>
		/// Gets or sets the zoning_code.
		/// </summary>
		/// <value>
		/// The zoning_code.
		/// </value>
		public int zoning_code { get; private set; }
		/// <summary>
		/// Initializes a new instance of the <see cref="PlayerActionDivineIntervention.ModifyZoneType"/> struct.
		/// </summary>
		/// <param name='zoneType'>
		/// Zone type.
		/// </param>
		public ModifyZoneType(ZoneType zoneType) : this() {
			this.zoning_code = 2; // AP (None);
			switch (zoneType) {
			case ZoneType.Protected:
				this.zoning_code = 16; // REC
				break;
			case ZoneType.Residential:
				this.zoning_code = 12; // R-1
				break;
			}
		}
	}
	/// <summary>
	/// Gets the zoning modification.
	/// </summary>
	/// <value>
	/// The zoning modification.
	/// </value>
	private ModifyZoneType m_zoningModification {
		get {
			return new ModifyZoneType(m_zoneType);
		}
	}
	#endregion
	#region baseCoverType
	/// <summary>
	/// The type of the base cover.
	/// </summary>
	private BaseCoverType m_baseCoverType;
	/// <summary>
	/// Modify base cover type.
	/// </summary>
	internal struct ModifyBaseCoverType {
		/// <summary>
		/// Gets or sets the landcover_class_code.
		/// </summary>
		/// <value>
		/// The landcover_class_code.
		/// </value>
		public int landcover_class_code { get; private set; }
		/// <summary>
		/// Initializes a new instance of the <see cref="PlayerActionDivineIntervention.ModifyBaseCoverType"/> struct.
		/// </summary>
		/// <param name='baseCoverType'>
		/// Base cover type.
		/// </param>
		public ModifyBaseCoverType(BaseCoverType baseCoverType) : this() {
			this.landcover_class_code = 255; // excluded
			switch (baseCoverType) {
			case BaseCoverType.Barren:
				this.landcover_class_code = 31;
				break;
			case BaseCoverType.CultivatedCrops:
				this.landcover_class_code = 82;
				break;
			case BaseCoverType.Developed:
				this.landcover_class_code = 21; // developed_open_space
				break;
			case BaseCoverType.Forest: // NOTE: this will have the side effect of setting treeType as well, until it is specified separately on the server
				this.landcover_class_code = 43; // mixed
				break;
			case BaseCoverType.Herbaceous:
				this.landcover_class_code = 71; // grassland_herbaceous
				break;
			case BaseCoverType.Water:
				this.landcover_class_code = 11;
				break;
			case BaseCoverType.Wetland: // NOTE: this will have the side effect of setting treeType as well, until it is specified separately on the server
				this.landcover_class_code = 90; // forested_wetland
				break;
			}
		}
	}
	/// <summary>
	/// Gets the base cover modification.
	/// </summary>
	/// <value>
	/// The base cover modification.
	/// </value>
	private ModifyBaseCoverType m_baseCoverModification {
		get {
			return new ModifyBaseCoverType(m_baseCoverType);
		}
	}
	#endregion
	#region trees
	/// <summary>
	/// The type of the tree.
	/// </summary>
//	private TreeType m_treeType {
//		get { return m_treeTypeBackingField; }
//		set {
//			this.m_treeTypeBackingField = value;
//			if (this.m_treeTypeBackingField == TreeType.None) {
//				for (int i=0; i<m_treeDistribution.Length; ++i) {
//					m_treeDistribution[i] = 0;
//				}
//			}
//		}
//	}
	/// <summary>
	/// The tree type backing field.
	/// </summary>
//	private TreeType m_treeTypeBackingField;
	/// <summary>
	/// The tree distribution.
	/// </summary>
	private int[] m_treeDistribution = new int[ResourceTile.treeSizeClassCount];
	/// <summary>
	/// Modify trees.
	/// </summary>
	internal struct ModifyTrees {
		public int num_2_inch_diameter_trees { get; private set; }
		public int num_4_inch_diameter_trees { get; private set; }
		public int num_6_inch_diameter_trees { get; private set; }
		public int num_8_inch_diameter_trees { get; private set; }
		public int num_10_inch_diameter_trees { get; private set; }
		public int num_12_inch_diameter_trees { get; private set; }
		public int num_14_inch_diameter_trees { get; private set; }
		public int num_16_inch_diameter_trees { get; private set; }
		public int num_18_inch_diameter_trees { get; private set; }
		public int num_20_inch_diameter_trees { get; private set; }
		public int num_22_inch_diameter_trees { get; private set; }
		public int num_24_inch_diameter_trees { get; private set; }
		public ModifyTrees(int[] treeDistribution) : this() {
			this.num_2_inch_diameter_trees = treeDistribution[0];
			this.num_4_inch_diameter_trees = treeDistribution[1];
			this.num_6_inch_diameter_trees = treeDistribution[2];
			this.num_8_inch_diameter_trees = treeDistribution[3];
			this.num_10_inch_diameter_trees = treeDistribution[4];
			this.num_12_inch_diameter_trees = treeDistribution[5];
			this.num_14_inch_diameter_trees = treeDistribution[6];
			this.num_16_inch_diameter_trees = treeDistribution[7];
			this.num_18_inch_diameter_trees = treeDistribution[8];
			this.num_20_inch_diameter_trees = treeDistribution[9];
			this.num_22_inch_diameter_trees = treeDistribution[10];
			this.num_24_inch_diameter_trees = treeDistribution[11];
		}
	}
	/// <summary>
	/// Gets the tree modifications.
	/// </summary>
	/// <value>
	/// The tree modifications.
	/// </value>
	private ModifyTrees m_treeModifications {
		get {
			return new ModifyTrees(m_treeDistribution);
		}
	}
	#endregion
	#region harvestArea
	/// <summary>
	/// Value indicating whether a tile is harvest area or not.
	/// </summary>
	private bool m_isHarvestArea = false;
	/// <summary>
	/// Modify harvest area.
	/// </summary>
	internal struct ModifyHarvestArea {
		public readonly bool harvest_area;
		public ModifyHarvestArea(bool isHarvestArea) : this() {
			harvest_area = isHarvestArea;
		}
	}
	/// <summary>
	/// Gets the harvest area modifications.
	/// </summary>
	/// <value>
	/// The harvest area modifications.
	/// </value>
	private ModifyHarvestArea m_harvestAreaModifications {
		get { return new ModifyHarvestArea(m_isHarvestArea); }
	}
	#endregion
	#region animals
	// TODO
	#endregion
	#region housing
	/// <summary>
	/// Modify housing.
	/// </summary>
	internal struct ModifyHousing {
		public readonly int housing_occupants;
		public readonly int housing_capacity;
		public ModifyHousing(int occupants, int capacity) : this() {
			this.housing_capacity = Mathf.Max(0, capacity);
			this.housing_occupants = Mathf.Clamp(occupants, 0, this.housing_capacity);
		}
	}
	/// <summary>
	/// The housing capacity.
	/// </summary>
	private int m_housingCapacity;
	/// <summary>
	/// The housing occupants.
	/// </summary>
	private int m_housingOccupants;
	/// <summary>
	/// The possible housing capacity values.
	/// </summary>
	private System.Collections.Generic.List<int> m_possibleHousingCapacityValues = new System.Collections.Generic.List<int>();
	/// <summary>
	/// Gets the housing modifications.
	/// </summary>
	/// <value>
	/// The housing modifications.
	/// </value>
	private ModifyHousing m_housingModifications {
		get {
			return new ModifyHousing(m_housingOccupants, m_housingCapacity);
		}
	}
	#endregion
	
	void Start()
	{
		m_possibleHousingCapacityValues.Add(0);
		foreach (Building building in TerrainManager.use.buildHousingAction.buildings) {
			m_possibleHousingCapacityValues.Add(building.capacity);
		}
		m_possibleHousingCapacityValues.Sort();
	}
	
	/// <summary>
	/// The scroll position.
	/// </summary>
	private Vector2 m_scrollPosition;
	
	/// <summary>
	/// Displays the controls contents.
	/// </summary>
	protected override void DisplayControlsContents()
	{
		ResourceTileSelection selection = ResourceTileSelection.GetCurrent();
		bool areButtonsDisabled = isActionInProgress || selection.Count() == 0;
		m_scrollPosition = GUILayout.BeginScrollView(m_scrollPosition); {
			GUILayout.Label("Zone Type:", m_mainText);
			m_zoneType = (ZoneType)GUILayout.SelectionGrid(
				(int)m_zoneType,
				System.Enum.GetNames(typeof(ZoneType)),
				m_selectionGridCount,
				m_styles.smallButton
			);
			if (GUILayout.Button(new GUIContent("Set Zone Type", dialogConfirmButtonDo.tooltip), areButtonsDisabled?m_buttonDisabled:m_button)) {
				m_tileModifications.resource_tile = m_zoningModification;
				StartCoroutine(Put(selection));
			}
			m_styles.DrawLine(GUIStyles.LineDirection.Horizontal, GUIStyles.LineColor.Medium);
			GUILayout.Label("Housing:", m_mainText);
			GUILayout.BeginHorizontal(); {
				GUILayout.Label(string.Format("Capacity: {0}", m_housingCapacity), m_minorText);
				int capacity = (int)GUILayout.HorizontalSlider(
					(float)m_housingCapacity,
					(float)m_possibleHousingCapacityValues[0],
					(float)m_possibleHousingCapacityValues[m_possibleHousingCapacityValues.Count-1]
				);
				for (int i=1; i<m_possibleHousingCapacityValues.Count; ++i) {
					if (capacity >= m_possibleHousingCapacityValues[i-1] &&
						capacity < m_possibleHousingCapacityValues[i]
					) {
						capacity = m_possibleHousingCapacityValues[i-1];
					}
				}
				m_housingCapacity = capacity;
			} GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(); {
				GUILayout.Label(string.Format("Occupants: {0}", m_housingOccupants), m_minorText);
				m_housingOccupants = (int)GUILayout.HorizontalSlider(
					m_housingOccupants, 0, m_housingCapacity
				);
				m_housingOccupants = Mathf.Clamp(m_housingOccupants, 0, m_housingCapacity);
			} GUILayout.EndHorizontal();
			if (GUILayout.Button(new GUIContent("Set Housing", dialogConfirmButtonDo.tooltip), areButtonsDisabled?m_buttonDisabled:m_button)) {
				m_tileModifications.resource_tile = m_housingModifications;
				StartCoroutine(Put(selection));
			}
			m_styles.DrawLine(GUIStyles.LineDirection.Horizontal, GUIStyles.LineColor.Medium);
			GUILayout.Label("Base Cover Type:", m_mainText);
			m_baseCoverType = (BaseCoverType)GUILayout.SelectionGrid(
				(int)m_baseCoverType,
				System.Enum.GetNames(typeof(BaseCoverType)),
				m_selectionGridCount,
				m_styles.smallButton
			);
			if (GUILayout.Button(new GUIContent("Set Base Cover Type", dialogConfirmButtonDo.tooltip), areButtonsDisabled?m_buttonDisabled:m_button)) {
				m_tileModifications.resource_tile = m_baseCoverModification;
				StartCoroutine(Put(selection));
			}
			m_styles.DrawLine(GUIStyles.LineDirection.Horizontal, GUIStyles.LineColor.Medium);
			GUILayout.Label("Trees:", m_mainText);
			float max = 100f;
			GUILayout.BeginHorizontal(); {
				for (int i=0; i<m_treeDistribution.Length; ++i) {
					GUILayout.BeginVertical(); {
						GUILayout.Label(m_treeDistribution[i].ToString(), m_minorText);
						m_treeDistribution[i] = (int)(max-GUILayout.VerticalSlider(max-(float)m_treeDistribution[i], 0f, max, GUILayout.Height(40f)));
						GUILayout.Label(string.Format("{0}\"", (i+1)*2), m_minorText);
					} GUILayout.EndVertical();
				}
			} GUILayout.EndHorizontal();
			if (GUILayout.Button(new GUIContent("Set Trees", dialogConfirmButtonDo.tooltip), areButtonsDisabled?m_buttonDisabled:m_button)) {
				m_tileModifications.resource_tile = m_treeModifications;
				StartCoroutine(Put(selection));
			}
			GUILayout.BeginHorizontal(); {
				m_isHarvestArea = GUILayout.Toggle(m_isHarvestArea, string.Format("Harvest Area: {0}", m_isHarvestArea), m_button);
				if (GUILayout.Button(new GUIContent("Set Harvest Area", dialogConfirmButtonDo.tooltip), areButtonsDisabled?m_buttonDisabled:m_button)) {
					m_tileModifications.resource_tile = m_harvestAreaModifications;
					StartCoroutine(Put(selection));
				}
			} GUILayout.EndHorizontal();
			// TODO: supported saplings
	//		GUILayout.Label("Aminals:", m_mainText);
		} GUILayout.EndScrollView();
		m_styles.DrawLine(GUIStyles.LineDirection.Horizontal, GUIStyles.LineColor.Medium);
		DisplayPaintSelectionControlGroup();
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
		return true;
	}
}