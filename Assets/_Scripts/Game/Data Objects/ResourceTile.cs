using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Land cover type (using USGS desginations).
/// </summary>
public enum LandCoverType {
	Unknown=256,					// Unknown surface.
	Excluded=255,					// Data outside the world (e.g., periphery of map).
	Barren=31,						// Bedrock, desert pavement, scarps, talus, slides, volcanic material, glacial debris, sand dunes, strip mines, gravel pits and other accumulations of earthen material.
	Coniferous=42,					// More than 75% of the tree species maintain their leaves all year. Canopy is never without green foliage.
	CultivatedCrops=82,				// Used for the production of annual crops, such as corn, soybeans, vegetables, tobacco, and cotton, and also perennial woody crops such as orchards and vineyards.
	Deciduous=41,					// More than 75% of the tree species shed foliage simultaneously in response to seasonal change.
	DevelopedHigh=24,				// Highly developed areas where people reside or work in high numbers. Examples include apartment complexes, row houses and commercial/industrial.
	DevelopedLow=22,				// These areas most commonly include single-family housing units.
	DevelopedMed=23,				// These areas most commonly include single-family housing units.
	DevelopedOpen=21,				// These areas most commonly include large-lot single-family housing units, parks, golf courses, and vegetation planted in developed settings for recreation, erosion control, or aesthetic purposes.
	DwarfScrub=51,					// I don't want no scrubs. A scurb is a guy who can't get no love from me--hanging' out the passenger's side of his best friend's ride, tryin' to holler at me.
	EmergentHerbaceousWetland=95,	// Areas where perennial herbaceous vegetation accounts for greater than 80% of vegetative cover and the soil or substrate is periodically saturated with or covered with water.
	ForestedWetland=90,				// Areas where forest or shrubland vegetation accounts for greater than 20% of vegetative cover and the soil or substrate is periodically saturated with or covered with water.
	GrasslandHerbaceous=71,			// Areas dominated by gramanoid or herbaceous vegetation, generally greater than 80% of total vegetation. These areas are not subject to intensive management such as tilling, but can be utilized for grazing.
	Mixed=43,						// Neither deciduous nor evergreen species are greater than 75% of total tree cover.
	OpenWater=11,					// Areas of open water.
	PastureHay=81,					// Areas of grasses, legumes, or grass-legume mixtures planted for livestock grazing or the production of seed or hay crops, typically on a perennial cycle.
	ShrubScrub=52					// Areas dominated by shrubs; less than 5 meters tall with shrub canopy typically greater than 20% of total vegetation. This class includes true shrubs, young trees in an early successional stage or trees stunted from environmental conditions.
}

/// <summary>
/// Base land cover type.
/// </summary>
public enum BaseCoverType {
	Unknown,
	Excluded,
	Barren,
	CultivatedCrops,
	Developed,
	Forest,
	Herbaceous,
	Water,
	Wetland,
}
/*
/// <summary>
/// Zone type.
/// </summary>
public enum ZoneType {
	Agricultural,					// AG
	AllPurpose,						// AP
	CommercialLow,					// C-1
	CommercialHigh,					// C-2
	CommunityBusiness,				// CB
	Forestry,						// F
	GeneralBusiness,				// GB
	Instrustrial,					// I
	Municipal,						// OR
	Parks,							// PR
	PlannedUnitDevelopment,			// PUD
	SingleFamilyResidential,		// R-1
	SingleMultiFamilyResidential,	// R-1-2
	MultiFamilyResidential,			// R-2
	MultiUnitResidential,			// R-3
	RecreationalDistrict,			// REC
	WoodedCommercial,				// WC
	None							// No Data
}*/

public enum ZoneType {
	None,
	Residential,
	Protected
}

/// <summary>
/// Tree type.
/// </summary>
public enum TreeType {
	None,
	Coniferous,
	Deciduous,
	Mixed
}

/// <summary>
/// A struct for doing a GET on an individual resource tile
/// </summary>
public struct IndividualResourceTile
{
	/// <summary>
	/// The resource tile. That's right.
	/// </summary>
	public ResourceTile resource_tile;
}

/// <summary>
/// Object for receiving a list of resource tiles e.g., after a successful action submission.
/// </summary>
public struct ResourceTiles
{
	/// <summary>
	/// The resource tiles list.
	/// </summary>
	public ResourceTile[] resource_tiles;
	
	/// <summary>
	/// Array accessor.
	/// </summary>
	/// <returns>
	/// The array.
	/// </returns>
	public ResourceTile[] ToArray() {
		return resource_tiles;
	}
	
	/// <summary>
	/// Gets the region of the tiles.
	/// </summary>
	/// <value>
	/// The region of the tiles.
	/// </value>
	public Region region { get { return new Region(this.resource_tiles); } }
	
	/// <summary>
	/// Gets or sets the <see cref="ResourceTiles"/> with the specified index.
	/// </summary>
	/// <param name='i'>
	/// Index.
	/// </param>
	public ResourceTile this[int i] {
		get { return resource_tiles[i]; }
		set { resource_tiles[i] = value; }
	}
	
	/// <summary>
	/// Gets the length of the resource tiles array.
	/// </summary>
	/// <value>
	/// The length of the resource tiles array.
	/// </value>
	public int Length { get { return resource_tiles.Length; } }
	
	/// <summary>
	/// Gets the enumerator.
	/// </summary>
	/// <returns>
	/// The enumerator.
	/// </returns>
	public Enumerator GetEnumerator()
	{
		return new Enumerator(this);
	}
	/// <summary>
	/// Enumerator.
	/// </summary>
	public class Enumerator 
	{
		/// <summary>
		/// The current index.
		/// </summary>
		private int i;
		/// <summary>
		/// The resource tiles.
		/// </summary>
		ResourceTiles resourceTiles;
		/// <summary>
		/// Initializes a new instance of the <see cref="ResourceTiles.Enumerator"/> class.
		/// </summary>
		/// <param name='tiles'>
		/// Tiles.
		/// </param>
		public Enumerator(ResourceTiles tiles) {
			resourceTiles = tiles;
			i = -1;
		}
		/// <summary>
		/// Moves the next.
		/// </summary>
		/// <returns>
		/// The next.
		/// </returns>
		public bool MoveNext() {
			++i;
			return i < resourceTiles.resource_tiles.Length;
		}
		/// <summary>
		/// Gets the current.
		/// </summary>
		/// <value>
		/// The current.
		/// </value>
		public ResourceTile Current {
			get {
				return resourceTiles.resource_tiles[i];
			}
		}
	}
}

/// <summary>
/// A reduced resource tile model to manually cache only needed data.
/// </summary>
public struct ResourceTileLite
{
	/// <summary>
	/// Create an empty ResourceTileLite.
	/// </summary>
	public static ResourceTileLite Empty()
	{
		ResourceTileLite ret = new ResourceTileLite();
		ret.id = -1;
		ret.zone = ZoneType.None;
		ret.permittedActions = 0;
		return ret;
	}
	
	/// <summary>
	/// Initializes a new instance of the <see cref="ResourceTileLite"/> struct.
	/// </summary>
	/// <param name='rawTileData'>
	/// Raw tile data.
	/// </param>
	public ResourceTileLite (ResourceTile rawTileData) : this()
	{
		this.id = rawTileData.id;
		this.idOwner = rawTileData.idOwner;
		this.idMegatile = rawTileData.idMegatile;
		
		this.x = rawTileData.x;
		this.y = rawTileData.z;
		this.zone = rawTileData.zoneType;
		this.permittedActions = rawTileData.permittedActions;
		this.baseCoverType = rawTileData.baseCoverType;
		this.treeCount = rawTileData.totalTreeCount;
		this.boughtByDeveloper = rawTileData.bought_by_developer;
		this.boughtByTimberCompany = rawTileData.bought_by_timber_company;
		this.desirability = (float)rawTileData.total_desirability_score;
		this.surveyRequested = rawTileData.survey_requested;
		this.canBeSurveyed = rawTileData.can_be_surveyed;
		this.isSurveyed = rawTileData.is_surveyed;
		this.outpostRequested = rawTileData.outpost_requested;
		this.animalCount = 0;
	}
	
	public override bool Equals (object obj) {
		ResourceTileLite tile = (ResourceTileLite)obj;
		return (tile.id == this.id);
	}
	public override int GetHashCode () {
		return base.GetHashCode ();
	}
	
	/// <summary>
	/// The identifier.
	/// </summary>
	public int id;
	
	//HACK: owner from megatile
	public int idOwner;
	//HACK: megatile id
	public int idMegatile;
	
	public int x {get;private set;}
	public int y {get;private set;}
	/// <summary>
	/// The zone.
	/// </summary>
	public ZoneType zone;
	/// <summary>
	/// The permitted actions as a bit field.
	/// </summary>
	/// <remarks>
	/// Each bit corresponds to an index in the player's list of actions.
	/// </remarks>
	public int permittedActions;
	/// <summary>
	/// Determines whether the specified action is permitted on this instance.
	/// </summary>
	/// <returns>
	/// <c>true</c> if the specified action is permitted on this instance; otherwise, <c>false</c>.
	/// </returns>
	/// <param name='action'>
	/// The action being requested.
	/// </param>
	public bool IsActionPermitted(PlayerAction action)
	{
		//GameManager.use.currentPlayer.currentAction.IsPermittedOnResourceTile(
		return (permittedActions & (1<<Player.current.permissionBitmaskByActionType[action.GetType()])) != 0;
	}
	/// <summary>
	/// The type of the base cover.
	/// </summary>
	/// <remarks>
	/// This is here for now since desirability is simulated on the client. It is otherwise unused for now.
	/// </remarks>
	public BaseCoverType baseCoverType;
	
	public bool boughtByTimberCompany;
	public bool boughtByDeveloper;
	public bool canBeSurveyed;
	public bool surveyRequested;
	public bool isSurveyed;
	public float desirability;
	public bool outpostRequested;
	
	/// <summary>
	/// The total number of trees on the tile
	/// </summary>
	public int treeCount;
	/// <summary>
	/// The total number of animals on the tile
	/// </summary>
	public int animalCount;
	
	/// <summary>
	/// Returns a <see cref="System.String"/> that represents the current <see cref="ResourceTileLite"/>.
	/// </summary>
	/// <returns>
	/// A <see cref="System.String"/> that represents the current <see cref="ResourceTileLite"/>.
	/// </returns>
	public override string ToString()
	{
		return string.Format(
			"ResourceTileLite {0} (zone: {1}, permittedActions: {2})",
			this.id, this.zone, this.permittedActions
		);
	}
	public Vector3 GetCenterPoint() {
		return new Vector3(x+0.5f,0.0f,y+0.5f);
	}
}

/// <summary>
/// Resource tile data about an individual resource tile
/// </summary>
public class ResourceTile
{
	/// <summary>
	/// Gets or sets the identifier.
	/// </summary>
	/// <value>
	/// The identifier.
	/// </value>
	public int id { get; private set; }
	
	//HACK: This is very likely a bad place to be storing this information
	public int idOwner = -1;
	
	public int idMegatile = -1;
	
	
	/// <summary>
	/// Gets or sets the x coordinate.
	/// </summary>
	/// <value>
	/// The x coordinate.
	/// </value>
	public int x { get; private set; }
	/// <summary>
	/// Gets or sets the y coordinate.
	/// </summary>
	/// <remarks>
	/// On the server, this coordinate represents latitude and begins in the north. It should not be used on the client.
	/// </remarks>
	/// <value>
	/// The y coordinate.
	/// </value>
	public int y { get; private set; }
	/// <summary>
	/// Gets or sets the z .
	/// </summary>
	/// <remarks>
	/// This field does not exist on the server. It exists for the purpose of reconciling differences in coordinates between the client and server.
	/// </remarks>
	/// <value>
	/// The z.
	/// </value>
	public int z {
		get { return ToClientZ(y); }
		set { this.y = ToServerY(value); }
	}
	/// <summary>
	/// Gets or sets the zoning_code.
	/// </summary>
	/// <value>
	/// The zoning_code.
	/// </value>
	public int zoning_code { get; private set; }
	/// <summary>
	/// Gets or sets the zone type.
	/// </summary>
	/// <value>
	/// The zone type.
	/// </value>
	public string zone_type { get; private set; }
	/// <summary>
	/// Gets the type of the zone.
	/// </summary>
	/// <remarks>
	/// This field does not exist on the server. It is simply a convenience accessor for getting the zone type as an enum.
	/// It presently maps from zoning_code, which uses old data. It can be refactored when the server has implemented the new zoning scheme.
	/// </remarks>
	/// <value>
	/// The type of the zone.
	/// </value>
	public ZoneType zoneType {
		get {
			switch (this.zoning_code) {
			case 1: // AG
				return ZoneType.None;
			case 2: // AP
				return ZoneType.None;
			case 3: // C-1
				return ZoneType.Residential;
			case 4: // C-2
				return ZoneType.Residential;
			case 5: // CB
				return ZoneType.Residential;
			case 6: // F
				return ZoneType.Protected;
			case 7: // GB
				return ZoneType.Residential;
			case 8: // I
				return ZoneType.Residential;
			case 9: // OR
				return ZoneType.Residential;
			case 10: // PR
				return ZoneType.Protected;
			case 11: // PUD
				return ZoneType.Residential;
			case 12: // R-1
				return ZoneType.Residential;
			case 13: // R-1-2
				return ZoneType.Residential;
			case 14: // R-2
				return ZoneType.Residential;
			case 15: // R-3
				return ZoneType.Residential;
			case 16: // REC
				return ZoneType.Protected;
			case 17: // WC
				return ZoneType.Residential;
			case 255: // No Data
				return ZoneType.None;
			default:
				Debug.LogWarning(string.Format("Unknown zoning code {0} encountered on tile {1}", zoning_code, id));
				return ZoneType.None;
			}
		}
	}
	
	/// <summary>
	/// Gets or sets the land cover type as a string.
	/// </summary>
	/// <value>
	/// The land cover type as a string.
	/// </value>
	public string base_cover_type { get; set; }
	/// <summary>
	/// Gets the type of the base cover.
	/// </summary>
	/// <remarks>
	/// This field does not exist on the server. It is simply a convenience accessor for getting the base cover type as an enum.
	/// </remarks>
	/// <value>
	/// The type of the base cover.
	/// </value>
	public BaseCoverType baseCoverType {
		get {
			switch (this.base_cover_type) {
			case "barren":
				return BaseCoverType.Barren;
			case "cultivated_crops":
				return BaseCoverType.CultivatedCrops;
			case "developed":
				return BaseCoverType.Developed;
			case "excluded":
				return BaseCoverType.Excluded;
			case "forest":
				return BaseCoverType.Forest;
			case "herbaceous":
				return BaseCoverType.Herbaceous;
			case "unknown":
				return BaseCoverType.Unknown;
			case "water":
				return BaseCoverType.Water;
			case "wetland":
				return BaseCoverType.Wetland;
			default:
				Debug.LogError(string.Format("Unknown base cover type encountered: {0}", this.base_cover_type));
				return BaseCoverType.Unknown;
			}
		}
	}
	
	/// <summary>
	/// Gets or sets the tree_type.
	/// </summary>
	/// <value>
	/// The tree_type.
	/// </value>
	public string tree_type { get; set; }
	/// <summary>
	/// Gets the type of the tree.
	/// </summary>
	/// <remarks>
	/// This is a temporary property until it is actually implemented on the server.
	/// </remarks>
	/// <value>
	/// The type of the tree.
	/// </value>
	public TreeType treeType {
		get {
			//TODO: interpret a hash from tree_type instead of landCoverType
			if (totalTreeCount == 0) { // TODO: Not sure if this is ideal
				return TreeType.None;
			}
			switch (landCoverType) {
			case LandCoverType.Coniferous:
				return TreeType.Coniferous;
			case LandCoverType.Deciduous:
				return TreeType.Deciduous;
			case LandCoverType.ForestedWetland:
			case LandCoverType.Mixed:
			case LandCoverType.ShrubScrub: // TODO: Not sure if this is a correct interpretation
				return TreeType.Mixed;
			default:
				return TreeType.None;
			}
		}
	}
	
	/// <summary>
	/// The desirabilty of this tile, not considering any neighboring tiles
	/// </summary>
	public float localDesirability {get {return (float)local_desirability_score;}}
	public double local_desirability_score {get; set;}
	/// <summary>
	/// The total desirability of this tile, taking into account neighboring tile scores
	/// </summary>
	public float totalDesirability {get {return (float)total_desirability_score;}}
	public double total_desirability_score {get; set;}
	
	public bool is_marten_suitable {get; set;} 
	
	public bool can_be_surveyed {get; set;}
	public bool is_surveyed {get; set;}
	public bool survey_requested {get; set;}
	public bool outpost_requested {get; set;}
	public bool bought_by_developer {get; set;}
	public bool bought_by_timber_company {get; set;}
	
	/// <summary>
	/// Gets or sets the number of 2-inch diameter trees.
	/// </summary>
	/// <value>
	/// The number of 2-inch diameter trees.
	/// </value>
	public double num_2_inch_diameter_trees {
		get { return this.treeDistribution[0]; }
		set { SetTreeDistributionFrequency(0, (int)value); }
	}
	/// <summary>
	/// Gets or sets the number of 4-inch diameter trees.
	/// </summary>
	/// <value>
	/// The number of 4-inch diameter trees.
	/// </value>
	public double num_4_inch_diameter_trees {
		get { return this.treeDistribution[1]; }
		set { SetTreeDistributionFrequency(1, (int)value); }
	}
	/// <summary>
	/// Gets or sets the number of 6-inch diameter trees.
	/// </summary>
	/// <value>
	/// The number of 6-inch diameter trees.
	/// </value>
	public double num_6_inch_diameter_trees {
		get { return this.treeDistribution[2]; }
		set { SetTreeDistributionFrequency(2, (int)value); }
	}
	/// <summary>
	/// Gets or sets the number of 8-inch diameter trees.
	/// </summary>
	/// <value>
	/// The number of 8-inch diameter trees.
	/// </value>
	public double num_8_inch_diameter_trees {
		get { return this.treeDistribution[3]; }
		set { SetTreeDistributionFrequency(3, (int)value); }
	}
	/// <summary>
	/// Gets or sets the number of 10-inch diameter trees.
	/// </summary>
	/// <value>
	/// The number of 10-inch diameter trees.
	/// </value>
	public double num_10_inch_diameter_trees {
		get { return this.treeDistribution[4]; }
		set { SetTreeDistributionFrequency(4, (int)value); }
	}
	/// <summary>
	/// Gets or sets the number of 12-inch diameter trees.
	/// </summary>
	/// <value>
	/// The number of 12-inch diameter trees.
	/// </value>
	public double num_12_inch_diameter_trees {
		get { return this.treeDistribution[5]; }
		set { SetTreeDistributionFrequency(5, (int)value); }
	}
	/// <summary>
	/// Gets or sets the number of 14-inch diameter trees.
	/// </summary>
	/// <value>
	/// The number of 14-inch diameter trees.
	/// </value>
	public double num_14_inch_diameter_trees {
		get { return this.treeDistribution[6]; }
		set { SetTreeDistributionFrequency(6, (int)value); }
	}
	/// <summary>
	/// Gets or sets the number of 16-inch diameter trees.
	/// </summary>
	/// <value>
	/// The number of 16-inch diameter trees.
	/// </value>
	public double num_16_inch_diameter_trees {
		get { return this.treeDistribution[7]; }
		set { SetTreeDistributionFrequency(7, (int)value); }
	}
	/// <summary>
	/// Gets or sets the number of 18-inch diameter trees.
	/// </summary>
	/// <value>
	/// The number of 18-inch diameter trees.
	/// </value>
	public double num_18_inch_diameter_trees {
		get { return this.treeDistribution[8]; }
		set { SetTreeDistributionFrequency(8, (int)value); }
	}
	/// <summary>
	/// Gets or sets the number of 20-inch diameter trees.
	/// </summary>
	/// <value>
	/// The number of 20-inch diameter trees.
	/// </value>
	public double num_20_inch_diameter_trees {
		get { return this.treeDistribution[9]; }
		set { SetTreeDistributionFrequency(9, (int)value); }
	}
	/// <summary>
	/// Gets or sets the number of 22-inch diameter trees.
	/// </summary>
	/// <value>
	/// The number of 22-inch diameter trees.
	/// </value>
	public double num_22_inch_diameter_trees {
		get { return this.treeDistribution[10]; }
		set { SetTreeDistributionFrequency(10, (int)value); }
	}
	/// <summary>
	/// Gets or sets the number of 24-inch diameter trees.
	/// </summary>
	/// <value>
	/// The number of 24-inch diameter trees.
	/// </value>
	public double num_24_inch_diameter_trees {
		get { return this.treeDistribution[11]; }
		set { SetTreeDistributionFrequency(11, (int)value); }
	}
	/// <summary>
	/// Gets the tree distribution.
	/// </summary>
	/// <value>
	/// The tree distribution.
	/// </value>
	public int[] treeDistribution { get; private set; }
	/// <summary>
	/// Gets the total tree count.
	/// </summary>
	/// <value>
	/// The total tree count.
	/// </value>
	public int totalTreeCount {
		get {
			int ret = 0;
			foreach (int i in treeDistribution) {
				ret += i;
			}
			return ret;
		}
	}
	/// <summary>
	/// The tree size class interval.
	/// </summary>
	public const int treeSizeClassInterval = 2;
	/// <summary>
	/// The tree size class count.
	/// </summary>
	public const int treeSizeClassCount = 12;
	/// <summary>
	/// Sets the tree distribution frequency at index.
	/// </summary>
	/// <param name='index'>
	/// Index.
	/// </param>
	/// <param name='frequency'>
	/// Frequency.
	/// </param>
	private void SetTreeDistributionFrequency(int index, int frequency)
	{
		if (this.treeDistribution == null) {
			this.treeDistribution = new int[treeSizeClassCount];
		}
		this.treeDistribution[index] = Mathf.Max(frequency, 0);
	}
	/// <summary>
	/// One half the tree size class count.
	/// </summary>
	/// <remarks>
	/// This is used to avoid having to divide by two.
	/// </remarks>
	public static readonly int oneHalfTreeSizeClassCount = treeSizeClassCount/2;
	/// <summary>
	/// Gets the number of trees represented by each type of TreeGraphic.
	/// </summary>
	/// <returns>
	/// The number of trees for each graphical representation, where each index corresponds to a TreeGraphicType.
	/// </returns>
	public int[] GetTreeCountsByGraphicType()
	{
		// initialize return value
		int[] treeCounts = new int[System.Enum.GetValues(typeof(TreeGraphicType)).Length];
		// determine total count of small and large trees in the distribution
		int largeTreeCount = 0;
		int smallTreeCount = 0;
		for (int i=0; i<oneHalfTreeSizeClassCount; ++i) {
			smallTreeCount += treeDistribution[i];
		}
		for (int i=oneHalfTreeSizeClassCount; i<treeSizeClassCount; ++i) {
			largeTreeCount += treeDistribution[i];
		}
		// determine the proportion of each type of tree in the distribution
		float coniferScalar = 0f, deciduousScalar = 0f;
		switch (this.treeType) {
		case TreeType.Coniferous:
			coniferScalar = 1f;
			break;
		case TreeType.Deciduous:
			deciduousScalar = 1f;
			break;
		case TreeType.Mixed:
			coniferScalar = 0.5f;
			deciduousScalar = 0.5f;
			break;
		}
		// set the number for each type of tree
		treeCounts[(int)TreeGraphicType.LargeConifer] = (int)(coniferScalar*largeTreeCount);
		treeCounts[(int)TreeGraphicType.SmallConifer] = (int)(coniferScalar*smallTreeCount);
		treeCounts[(int)TreeGraphicType.LargeDeciduous] = (int)(deciduousScalar*largeTreeCount);
		treeCounts[(int)TreeGraphicType.SmallDeciduous] = (int)(deciduousScalar*smallTreeCount);
		return treeCounts;
	}
	
	/// <summary>
	/// The ground texture type, based on the land cover type.
	/// </summary>
	/// <remarks>
	/// This field does not exist on the server. It is simply a convenience accessor for mapping base cover type to a ground texture type.
	/// </remarks>
	public GroundTextureType groundTextureType {
		get {
			switch (baseCoverType) {
			case BaseCoverType.Barren:
				return GroundTextureType.Barren;
			case BaseCoverType.CultivatedCrops:
				return GroundTextureType.Crops;
			case BaseCoverType.Developed:
				return GroundTextureType.Developed;
			case BaseCoverType.Excluded:
				return GroundTextureType.None;
			case BaseCoverType.Forest:
				return GroundTextureType.Forest;
			case BaseCoverType.Herbaceous:
				return GroundTextureType.Grass;
			case BaseCoverType.Water:
				return GroundTextureType.Water;
			case BaseCoverType.Wetland:
				return GroundTextureType.Wetland;
			}
			return GroundTextureType.None;
		}
	}
	
	public int housingOccupants { get {return housing_occupants;}}
	/// <summary>
	/// Gets or sets the housing occupants.
	/// </summary>
	/// <value>
	/// The housing occupants.
	/// </value>
	public int housing_occupants { get; set; }
	
	public int housingCapacity { get {return housing_capacity;}}
	/// <summary>
	/// Gets or sets the housing capacity.
	/// </summary>
	/// <remarks>
	/// Currently inferred from land cover type. Should be property existing in database on server.
	/// </remarks>
	/// <value>
	/// The housing capacity.
	/// </value>
	public int housing_capacity { get; set; }
	
	/// <summary>
	/// Max number of trees that can occupy a single resource tile.
	/// </summary>
	/// <remarks>
	/// This number was made up for use in determining the visual representation.
	/// </remarks>
	public const int maxNumberOfTreesPerAcre = 100;
	
	/// <summary>
	/// One over max number of trees per acre (for use as an alternative to division where computationally intensive).
	/// </summary>
	public const float oneOverMaxNumberOfTreesPerAcre = 1f/maxNumberOfTreesPerAcre;
	
	public int supportedSaplings { get {return supported_saplings;}}
	/// <summary>
	/// Gets or sets the supported saplings.
	/// </summary>
	/// <remarks>
	/// This temporarily is just based off the max number of trees used for visual representations. Should eventually be determined wholly by server.
	/// </remarks>
	/// <value>
	/// The supported saplings.
	/// </value>
	public int supported_saplings {
		get;
		set;
		//get {
		//	return Mathf.Max(0, maxNumberOfTreesPerAcre-this.totalTreeCount);
		//}
	}
	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="ResourceTile"/> is a harvest area.
	/// </summary>
	/// <value>
	/// <c>true</c> if is a harvest area; otherwise, <c>false</c>.
	/// </value>
	public bool harvest_area { get; set; }
	/// <summary>
	/// Gets a value indicating whether this <see cref="ResourceTile"/> is a harvest area.
	/// </summary>
	/// <value>
	/// <c>true</c> if is a harvest area; otherwise, <c>false</c>.
	/// </value>
	public bool isHarvestArea {
		get { return harvest_area; }
	}
	
	/// <summary>
	/// Gets or sets the permitted_actions.
	/// </summary>
	/// <remarks>
	/// This is currently being ignored altogether.
	/// </remarks>
	/// <value>
	/// The permitted_actions.
	/// </value>
	public string[] permitted_actions { get; private set; }
	
	/// <summary>
	/// Gets the permissions bit field.
	/// </summary>
	/// <remarks>
	/// This will be refactored when we get actual permitted_actions from the server.
	/// NOTE: Ignores resource requirements like money and timber for now.
	/// </remarks>
	/// <value>
	/// The permissions bit field.
	/// </value>
	public int permittedActions {
		get {
			int ret = 0;
			for (int i=0; i<Player.current.actions.Count; ++i) {
				if (!Player.current.actions[i].IsPermittedOnResourceTile(this)) {
					continue;
				}
				ret |= 1 << i;
			}
			return ret;
		}
	}
	
	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="ResourceTile"/> has an outpost.
	/// </summary>
	/// <value>
	/// <c>true</c> if there is an outpost; otherwise, <c>false</c>.
	/// </value>
	public bool outpost { get; private set; }
	
	/// <summary>
	/// Gets a value indicating whether this <see cref="ResourceTile"/> has an outpost.
	/// </summary>
	/// <value>
	/// <c>true</c> if there is an outpost; otherwise, <c>false</c>.
	/// </value>
	public bool hasOutpost { get { return this.outpost; } }
	
	public float marten_population { get; set; }
	public float vole_population   { get; set; }
	
	
	/// <summary>
	/// Convert z coordinate to a y coordinate for the server, which starts from upper left
	/// </summary>
	/// <param name="z">
	/// A <see cref="System.Int32"/>
	/// </param>
	/// <returns>
	/// A <see cref="System.Int32"/>
	/// </returns>
	public static int ToServerY(int z)
	{
		return TerrainManager.worldRegion.height-z-1;
	}
	/// <summary>
	/// Convert y coordinate to a z coordinate for the client, which starts from lower left
	/// </summary>
	/// <param name="y">
	/// A <see cref="System.Int32"/>
	/// </param>
	/// <returns>
	/// A <see cref="System.Int32"/>
	/// </returns>
	public static int ToClientZ(int y)
	{
		return TerrainManager.worldRegion.height-y-1;
	}
	
	/// <summary>
	/// Get the center point in world space 
	/// </summary>
	/// <returns>
	/// A <see cref="Vector3"/>
	/// </returns>
	public Vector3 GetCenterPoint()
	{
		Vector3 v = GetSimpleCenterPoint();
		v.y = TerrainManager.use.terrain.SampleHeight(v);
		return v;
	}
	/// <summary>
	/// Gets the simple center point.
	/// </summary>
	/// <remarks>
	/// Ignores height
	/// </remarks>
	/// <returns>
	/// The simple center point.
	/// </returns>
	public Vector3 GetSimpleCenterPoint()
	{
		return new Vector3(x+0.5f, 0f, z+0.5f);
	}
	
	/// <summary>
	/// Determine whether the microtile contains the specified point 
	/// </summary>
	/// <param name="p">
	/// A <see cref="Vector3"/>
	/// </param>
	/// <returns>
	/// A <see cref="System.Boolean"/> true if p is on the tile; otherwise false
	/// </returns>
	public bool Contains(Vector3 p)
	{
		return (p.x>=x && p.x<=x+1) || (p.z>=z && p.z<=z+1);
	}
	
	/// <summary>
	/// Returns a <see cref="System.String"/> that represents the current <see cref="ResourceTile"/>.
	/// </summary>
	/// <returns>
	/// A <see cref="System.String"/> that represents the current <see cref="ResourceTile"/>.
	/// </returns>
	public override string ToString()
	{
		return string.Format(
			"ResourceTile {0} at ({1}, {2})\n(zone: {3}, permittedActions: {4}, coverType: {5})",
			this.id, this.x, this.y, this.zoneType, this.permittedActions, this.baseCoverType
		);
	}
	
	/* 
	 * / / / / / / / / / / / / / / / / / / / / / / / / / / / / / / / / / / / / /
	 * OLD TILE MODEL STUFF
	 * / / / / / / / / / / / / / / / / / / / / / / / / / / / / / / / / / / / / /
	 */
	
	#region Old Tile Model Stuff
	public string type { get ; set; } // class of tile, not used on client
	public string primary_use { get; private set; }
	public object people_density { get; set; } // TODO: these are coming back null
	public double housing_density { get; set; }
	public double tree_density { get; set; }
	/// <summary>
	/// Gets or sets the underlying cover type in the data set as a string.
	/// </summary>
	/// <value>
	/// The underlying cover type in the data set as a string (from USGS designations).
	/// </value>
	public string land_cover_type { get; set; }
	/// <summary>
	/// Dictionary to convert incoming land cover strings to enum.
	/// </summary>
	private static Dictionary<string, LandCoverType> m_strToLandCoverType = new Dictionary<string, LandCoverType>
	{
		{ "barren", LandCoverType.Barren },
		{ "coniferous", LandCoverType.Coniferous },
		{ "cultivated_crops", LandCoverType.CultivatedCrops },
		{ "deciduous", LandCoverType.Deciduous },
		{ "developed_high_intensity", LandCoverType.DevelopedHigh },
		{ "developed_low_intensity", LandCoverType.DevelopedLow },
		{ "developed_medium_intensity", LandCoverType.DevelopedMed },
		{ "developed_open_space", LandCoverType.DevelopedOpen },
		{ "excluded", LandCoverType.Excluded },
		{ "emergent_herbaceous_wetland", LandCoverType.EmergentHerbaceousWetland },
		{ "forested_wetland", LandCoverType.ForestedWetland },
		{ "grassland_herbaceous", LandCoverType.GrasslandHerbaceous },
		{ "mixed", LandCoverType.Mixed },
		{ "open_water", LandCoverType.OpenWater },
		{ "pasture_hay", LandCoverType.PastureHay },
		{ "shrub_scrub", LandCoverType.ShrubScrub },
		{ "unknown", LandCoverType.Unknown }
	};
	/// <summary>
	/// Gets the type of the land cover.
	/// </summary>
	/// <value>
	/// The type of the land cover.
	/// </value>
	public LandCoverType landCoverType {
		get {
			try {
				return m_strToLandCoverType[this.land_cover_type];
			}
			catch (System.Collections.Generic.KeyNotFoundException) {
				Debug.LogError(string.Format("Unknown land cover type encountered: {0}", this.land_cover_type));
				return LandCoverType.Unknown;
			}
		}
	}
	public double tree_size { get; set; }
	public object development_intensity { get; set; } // TODO: these are coming back null
	public double imperviousness { get; set; }
	#endregion
}