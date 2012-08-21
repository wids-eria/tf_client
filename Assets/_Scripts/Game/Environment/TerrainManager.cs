using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// A class for managing the terrain
/// NOTE: all region parameters are in world-space, unless otherwise specified
/// </summary>
public class TerrainManager : MonoBehaviour
{
	public const string kLoadedNewTiles = "LoadedNewTiles";
	public const string kRefreshAll = "RefreshAllTerrain";
	
	private GameObject m_objectGrouper;
		
	/// <summary>
	/// Gets or sets the singleton.
	/// </summary>
	/// <value>
	/// The singleton.
	/// </value>
	public static TerrainManager use { get; private set; }
	
	/// <summary>
	/// Initialize singleton
	/// </summary>
	void Awake()
	{
		if (use != null) {
			Destroy(use.gameObject);
		}
		use = this;
		
		MessengerAM.Listen(MessengerAM.listenTypeInput, this);
		MessengerAM.Listen("www", this);
		MessengerAM.Listen("world", this);
		
		m_objectGrouper = new GameObject("TerrainObjects");
		
		// initialize the terrain
		InitializeTerrain();
	}
	
	/// <summary>
	/// Initialize terrain etc
	/// </summary>
	void Start()
	{		
		downloadTilesCoroutine =  new DownloadTilesInRegionCoroutine( this );
			
		// instantiate build housing action
		buildHousingAction = Instantiate(buildHousingAction) as PlayerActionBuildHousing;
		
		// download world
		StartCoroutine(WebRequests.DownloadWorld());
	}
	
	/// <summary>
	/// The terrain piece in the world
	/// </summary>
	public Terrain terrain { get; private set; }
	/// <summary>
	/// The TerrainData associated with the terrain
	/// </summary>
	public static TerrainData terrainData { get { return use.terrain.terrainData; } }
	
	/// <summary>
	/// get the detail resolution to use for the terrain
	/// </summary>
	public int detailResolutionPerPatch { get { return m_detailResolutionPerPatch; } }
	/// <summary>
	/// the detail resolution per patch
	/// </summary>
	[SerializeField]
	private int m_detailResolutionPerPatch = 8;
	
	/// <summary>
	/// The ground textures to use on the terrain
	/// </summary>
	[SerializeField]
	private GroundTexture[] m_groundTextures = new GroundTexture[System.Enum.GetNames(typeof(GroundTextureType)).Length];
	/// <summary>
	/// Trees to use on the terrain
	/// </summary>
	[SerializeField]
	private TreeProperties[] m_trees = new TreeProperties[0];
	
	/// <summary>
	/// Gets or sets the tile cache.
	/// </summary>
	/// <value>
	/// The tile cache.
	/// </value>
	public ResourceTileLite[,] resourceTileCache { get; private set; }
	
	/// <summary>
	/// Initializes the resource tile cache.
	/// </summary>
	private void InitializeResourceTileCache()
	{
		resourceTileCache = new ResourceTileLite[(int)terrainData.size.x, (int)terrainData.size.z];
		for (int r=0; r<=resourceTileCache.GetUpperBound(0); ++r) {
			for (int c=0; c<=resourceTileCache.GetUpperBound(1); ++c) {
				resourceTileCache[r,c] = ResourceTileLite.Empty();
			}
		}
	}
	
	/// <summary>
	/// Gets the resource tile cache at world position.
	/// </summary>
	/// <returns>
	/// true if able to retrieve a cache at the specified position, otherwise false.
	/// </returns>
	/// <param name='worldPosition'>
	/// World position.
	/// </param>
	/// <param name='tile'>
	/// The cache at the specified position.
	/// </param>
	public static bool GetResourceTileCacheAtWorldPosition(Vector3 position, out ResourceTileLite tile)
	{
		position = use.terrain.transform.InverseTransformPoint(position);
		try {
			tile = use.resourceTileCache[(int)position.z, (int)position.x];
			return (!tile.Equals(ResourceTileLite.Empty()));
		}
		catch (System.IndexOutOfRangeException) {
			tile = ResourceTileLite.Empty();
			return false;
		}
	}
	
	/// <summary>
	/// Gets the resource tile cache under mouse cursor.
	/// </summary>
	/// <returns>
	/// true if able to retrieve a cache at the specified position, otherwise false.
	/// </returns>
	/// <param name='tile'>
	/// The cache under the mouse cursor.
	/// </param>
	public static bool GetResourceTileCacheUnderMouseCursor(out ResourceTileLite tile)
	{
		return GetResourceTileCacheAtWorldPosition(GetWorldCoordinateAtMousePosition(), out tile);
	}
	
	public static ResourceTileLite[,] GetResourceTileCacheRegion(Region region)
	{
		ResourceTileLite[,] tiles = new ResourceTileLite[region.height,region.width];
		for( int r=region.bottom; r<region.height; r++) {
			for (int c=region.left; c<region.width; c++) {
				try {
					tiles[r,c] = use.resourceTileCache[r,c];
				}
				catch (System.IndexOutOfRangeException) {
					Debug.LogWarning("Trying to get tile that doesn't exist in the cache");
					tiles[r,c] = new ResourceTileLite();
				}
			}
		}
		return tiles;
	}
	
	/// <summary>
	/// Status.
	/// </summary>
	public enum Status { Failed, Succeeded };
	
	/// <summary>
	/// Gets the resource tile cache with identifier.
	/// </summary>
	/// <returns>
	/// The resource tile cache with identifier.
	/// </returns>
	/// <param name='id'>
	/// Identifier.
	/// </param>
	/// <param name='status'>
	/// Status indicating whether the returned value is valid or not.
	/// </param>
	public static ResourceTileLite GetResourceTileCacheWithId(int id, out Status status)
	{
		status = Status.Failed;
		for (int r=0; r<use.resourceTileCache.GetUpperBound(0); ++r) {
			for (int c=0; c<use.resourceTileCache.GetUpperBound(1); ++c) {
				if (use.resourceTileCache[r,c].id == id) {
					status = Status.Succeeded;
					return use.resourceTileCache[r,c];
				}
			}
		}
		return new ResourceTileLite();
	}
	
	public static ResourceTileLite[] GetResourceTileCacheGroup(out Status status, params int[] ids)
	{
		ResourceTileLite[] tiles = new ResourceTileLite[ids.Length];
		status = Status.Failed;
		for(int i=0;i<ids.Length;i++) {
			tiles[i] = GetResourceTileCacheWithId(ids[i],out status);
			if (status == Status.Failed) break;
		}
		return tiles;
	}
	
	/// <summary>
	/// Initialize the terrain and collider
	/// </summary>
	private void InitializeTerrain()
	{
		terrain = GetComponentInChildren<Terrain>();
		m_detailResolutionPerPatch = Mathf.Max(8, Mathf.ClosestPowerOfTwo(m_detailResolutionPerPatch)); // NOTE: needs to be a power of two or alignment will get screwy
		TerrainData data = TerrainHelpers.Clone(terrainData, m_detailResolutionPerPatch);
		SplatPrototype[] splats = new SplatPrototype[m_groundTextures.Length];
		foreach (GroundTexture groundTex in m_groundTextures) {
			splats[(int)groundTex.groundTextureType] = groundTex.ToSplat();
		}
		data.splatPrototypes = splats;
		DetailPrototype[] details = new DetailPrototype[m_trees.Length];
		foreach (TreeProperties tree in m_trees) {
			details[(int)tree.treeType] = tree.ToDetailPrototype();
		}
		data.detailPrototypes = details;
		terrain.terrainData = data;
		float[,] heights = terrainData.GetHeights(0,0,terrainData.heightmapResolution, terrainData.heightmapResolution);
		for (int r=0; r<=heights.GetUpperBound(0); ++r) {
			for (int c=0; c<=heights.GetUpperBound(1); ++c) {
				heights[r,c] = terrainData.size.y; // set to ground level
			}
		}
		terrainData.SetHeights(0, 0, heights);
		ClearTerrain(SetTerrainMode.Flush); // ensure proper textures etc. are applied by default
		(terrain.collider as TerrainCollider).terrainData = terrainData;
		InitializeResourceTileCache();
		
		// create padding surrounding terrain
		CreatePaddingMeshes();
		
		// nullify arrays so garbage collector can toss unneeded assets
		m_groundTextures = null;
		m_trees = null;
	}
	
	/// <summary>
	/// The padding meshes
	/// </summary>
	[HideInInspector]
	public GameObject[] paddingMeshes = new GameObject[8];
	
	/// <summary>
	/// The material to apply to padding meshes
	/// </summary>
	[SerializeField]
	private Material m_paddingMeshMaterial;
	
	/// <summary>
	/// Create the padding meshes to surround the terrain
	/// </summary>
	private void CreatePaddingMeshes()
	{
		// generate vertices
		Vector3[] vertices = new Vector3[4];
		vertices[1] = Vector3.right*terrainData.size.x;
		vertices[2] = Vector3.forward*terrainData.size.z;
		vertices[3] = vertices[1]+vertices[2];
		// generate uvs
		Vector2[] uv = new Vector2[vertices.Length];
		uv[1] = Vector2.right;
		uv[2] = Vector2.up;
		uv[3] = Vector2.one;
		// generate triangles
		int[] triangles = new int[6];
		triangles[1] = 2;
		triangles[2] = 1;
		triangles[3] = 1;
		triangles[4] = 2;
		triangles[5] = 3;
		// apply to new mesh
		Mesh mesh = new Mesh();
		mesh.name = "Generated Padding Mesh";
		mesh.vertices = vertices;
		mesh.normals = new Vector3[4];
		mesh.uv = uv;
		mesh.triangles = triangles;
		mesh.RecalculateNormals();
		for (int r=0; r<3; r++) {
			for (int c=0; c<3; c++) {
				int i = 3*r+c;
				if (r==1 && c==1) {
					continue;
				}
				if (i>3) {
					--i;
				}
				paddingMeshes[i] = new GameObject("Terrain Padding", typeof(MeshRenderer));
				paddingMeshes[i].layer = LayerMask.NameToLayer("Terrain Padding");
				MeshFilter mf = paddingMeshes[i].AddComponent<MeshFilter>();
				mf.mesh = mesh;
				paddingMeshes[i].renderer.material = m_paddingMeshMaterial;
				// NOTE: default material values were tuned for 128 terrain, so this is a lazy hack
				paddingMeshes[i].renderer.material.mainTextureScale = new Vector2(
					terrainData.size.x/(float)128 * m_paddingMeshMaterial.mainTextureScale.x,
					terrainData.size.z/(float)128 * m_paddingMeshMaterial.mainTextureScale.y
				);
				paddingMeshes[i].transform.parent = terrain.transform;
				paddingMeshes[i].transform.localPosition = new Vector3(
					(1-c)*terrainData.size.x,
					terrainData.size.y*0.5f,
					(1-r)*terrainData.size.z
				);
			}
		}
	}
	
	/// <summary>
	/// Updates the padding mesh grid offset.
	/// </summary>
	private void UpdatePaddingMeshGridOffset()
	{
		foreach (GameObject go in paddingMeshes) {
			Vector3 p = go.transform.position;
			go.renderer.material.SetTextureOffset(
				"_Grid",
				new Vector2(
					(p.x%Megatile.size)/Megatile.size,
					(p.z%Megatile.size)/Megatile.size
				)
			);
		}
	}
	
	/// <summary>
	/// The build housing action.
	/// </summary>
	public PlayerActionBuildHousing buildHousingAction;
	/// <summary>
	/// The building instances on the terrain.
	/// </summary>
	private List<GameObject> m_buildingInstances = new List<GameObject>();
	/// <summary>
	/// The outpost prefab.
	/// </summary>
	[SerializeField]
	private ResearchOutpost m_outpostPrefab;
	/// <summary>
	/// The outpost instances.
	/// </summary>
	/// <remarks>
	/// Each key is the id of the tile to which the outpost belongs
	/// </remarks>
	private Dictionary<int, ResearchOutpost> m_outpostInstances = new Dictionary<int, ResearchOutpost>();
	/// <summary>
	/// Determines whether world position is in the radius of any outpost.
	/// </summary>
	/// <remarks>
	/// Requires us to know about all outposts on the map to get an accurate result.
	/// </remarks>
	/// <returns>
	/// <c>true</c> if world position is in the radius of any outpost; otherwise, <c>false</c>.
	/// </returns>
	/// <param name='worldPosition'>
	/// A point in world space.
	/// </param>
	/// 
	/*public bool IsWorldPositionInRadiusOfAnyOutpost(Vector3 worldPosition)
	{
		foreach (ResearchOutpost outpost in m_outpostInstances.Values) {
			if (outpost.IsWorldPositionInRadius(worldPosition)) {
				return true;
			}
		}
		return false;
	}*/
	/// <summary>
	/// Gets the current terrain region.
	/// </summary>
	/// <value>
	/// The current terrain region.
	/// </value>
	public Region currentTerrainRegion
	{
		get {
			return new Region(
				(int)terrain.transform.position.x,
				(int)(terrain.transform.position.x+terrainData.size.x),
				(int)(terrain.transform.position.z+terrainData.size.z),
				(int)terrain.transform.position.z
			);
		}
	}
	
	public Region mapRegion
	{
		get {
			return new Region(
				(int)terrain.transform.position.x+1,
				(int)(terrain.transform.position.x+terrainData.size.x)-1,
				(int)(terrain.transform.position.z+terrainData.size.z)-1,
				(int)terrain.transform.position.z+1
			);
		}
	}
	
	/// <summary>
	/// Clears the trees in region.
	/// </summary>
	/// <param name='region'>
	/// Region in world space specifying resource tiles to clear.
	/// </param>
	/// <param name='mode'>
	/// Mode.
	/// </param>
	private void ClearTreesInRegion(Region region, SetTerrainMode mode)
	{
		// set all details to 0 in the region
		region.ToTerrainRegion( out region );
		int[,] details = new int[region.right-region.left+1,
		                         region.top-region.bottom+1];
		for (int i=0; i<terrainData.detailPrototypes.Length; i++)
			terrainData.SetDetailLayer(region.left, region.bottom, i, details);
		
		// flush if requested
		if (mode == SetTerrainMode.Flush) {
			terrain.Flush();
		}
	}
	
	/// <summary>
	/// Clears the buildings in region.
	/// </summary>
	/// <param name='region'>
	/// Region in world space.
	/// </param>
	private void ClearBuildingsInRegion(Region region)
	{
		Vector3 p;
		GameObject[] instances = m_buildingInstances.ToArray();
		foreach (GameObject go in instances) {
			p = go.transform.position;
			if (!region.Contains(p)) {
				continue;
			}
			m_buildingInstances.Remove(go);
			Destroy(go);
		}
	}
	
	/// <summary>
	/// Clears all terrain objects (buildings and trees).
	/// </summary>
	/// <param name='mode'>
	/// Mode.
	/// </param>
	private void ClearAllTerrainObjects(SetTerrainMode mode)
	{
		// remove structures
		foreach (GameObject go in m_buildingInstances) {
			Destroy(go);
		}
		m_buildingInstances = new List<GameObject>();
		foreach (ResearchOutpost outpost in m_outpostInstances.Values) {
			Destroy(outpost.gameObject);
		}
		m_outpostInstances = new Dictionary<int, ResearchOutpost>();
		
		// remove trees
		ClearTreesInRegion(currentTerrainRegion, mode);
	}
	
	/// <summary>
	/// Clears the terrain of all data.
	/// </summary>
	/// <param name='mode'>
	/// Mode.
	/// </param>
	private void ClearTerrain(SetTerrainMode mode)
	{
		// early out if already cleared
		if (m_isTerrainDataCleared) {
			return;
		}
		
		// remove buildings and trees
		ClearAllTerrainObjects(SetTerrainMode.NoFlush);
		
		// flatten and set to loading texture
		InitializeResourceTileCache();
		float[,,] splats = new float[(int)terrainData.size.x,
		                             (int)terrainData.size.z,
		                             terrainData.splatPrototypes.Length];
		float[,] heights = new float[(int)terrainData.size.x,
		                             (int)terrainData.size.z];
		for (int r=0; r<=splats.GetUpperBound(0); r++) {
			for (int c=0; c<=splats.GetUpperBound(1); c++) {
				heights[r,c] = terrainData.size.y;
				splats[r,c,(int)GroundTextureType.Loading] = 1f;
			}
		}
		terrainData.SetAlphamaps(0, 0, splats);
		terrainData.SetHeights(0, 0, heights);
		
		// set the clear flag
		m_isTerrainDataCleared = true;
		
		// flush if requested
		if (mode == SetTerrainMode.Flush) {
			terrain.Flush();
		}
	}
	
	/// <summary>
	/// Flag specifying whether the terrain's data have been cleared.
	/// </summary>
	private bool m_isTerrainDataCleared = true;
	
	/// <summary>
	/// Sets the permissions.
	/// </summary>
	/// <param name='tiles'>
	/// Tiles from which to read permissions.
	/// </param>
	public void SetPermissions(ResourceTile[] tiles)
	{
		int xOffset = (int)terrain.transform.position.x;
		int zOffset = (int)terrain.transform.position.z;
		foreach (ResourceTile tile in tiles) {
			// get coordinates in the space of the chunk
			int x = tile.x - xOffset;
			int z = tile.z - zOffset;
			resourceTileCache[z,x].permittedActions = tile.permittedActions;
		}
		// update mask if displayed
		InputManager.use.currentAction.UpdateMask();
		// update selection
		//InputManager.use.FilterSelection();
		InputManager.use.currentAction.UpdateSelectionHighlighter();
	}
		
	/// <summary>
	/// Sets the terrain from resourceTiles.
	/// </summary>
	/// <param name='resourceTiles'>
	/// Microtiles.
	/// </param>
	/// <param name='mode'>
	/// Mode.
	/// </param>
	public void SetTerrainFromResourceTiles(ResourceTile[] resourceTiles, SetTerrainMode mode)
	{
		if (resourceTiles.Length <= 0) {
//			return;
			// if this case actually gets hit I want to know what is causing it.
			throw new System.ArgumentException("Please find out why the resourceTile array was empty and tell Adam.");
		}
		
		// if the view is not entirely terrain, clear it and move it to the center of the view
		if (!m_isTerrainFixedInPlace &&
			!CameraRig.use.isViewFrustumEntirelyTerrain
		) {
			ClearTerrain(SetTerrainMode.NoFlush);
			MoveTerrainToCenterOfView();
		}
		// determine region of update to minimize calls to Terrain APIs
		Region region = new Region(resourceTiles);
		// clear structures in the region
		ClearBuildingsInRegion(region);
		// convert to terrain-space region
		region.ToTerrainRegion( out region );
		// get current properties for the region
		float[,] heights = terrainData.GetHeights(region.left, region.bottom, region.width, region.height);
		float[,,] splats = terrainData.GetAlphamaps(region.left, region.bottom, region.width, region.height);
		int[][,] details = new int[terrainData.detailPrototypes.Length][,];
		for (int i=0; i<details.Length; i++) {
			details[i] = terrainData.GetDetailLayer(region.left, region.bottom, region.width, region.height, i);
		}
		// update values as needed
		int xOffset = (int)terrain.transform.position.x + region.left;
		int zOffset = (int)terrain.transform.position.z + region.bottom;
		Building b = null;
		foreach (ResourceTile tile in resourceTiles) {
			// get coordinates in the space of the chunk
			int x = tile.x - xOffset;
			int z = tile.z - zOffset;
			// skip any resourceTiles that are out of bounds, as when e.g., server returns a bigger cached chunk than we need
			if (x<0 || x>heights.GetUpperBound(1) ||
			    z<0 || z>heights.GetUpperBound(0)
			) {
				continue;
			}
//			// cache stuff that is not collapsed into other representations
//			resourceTileCache[region.bottom+z,region.left+x] = new ResourceTileLite(tile);
			// set heights
			GroundTextureType ground = tile.groundTextureType;
			heights[z,x] = (ground==GroundTextureType.Water)?0f:terrainData.size.y;
			// set ground texture
			for (int i=0; i<=splats.GetUpperBound(2); ++i) {
				splats[z,x,i] = 0f;
			}
			splats[z,x,(int)ground] = 1f;
			// get the raw number of trees of each type
			int[] treeCounts = tile.GetTreeCountsByGraphicType();
			// remap tree counts to the range for detail values
			for (int i=0; i<treeCounts.Length; ++i) {
				treeCounts[i] = (int)(detailResolutionPerPatch*Mathf.Min(treeCounts[i]*ResourceTile.oneOverMaxNumberOfTreesPerAcre, 1f));
			}
			// set details
			for (int i=0; i<treeCounts.Length; ++i) {
				details[i][z,x] = treeCounts[i];
			}
			// add structures
			if (tile.hasOutpost && !m_outpostInstances.ContainsKey(tile.id)) {
				m_outpostInstances.Add(
					tile.id,
					Instantiate(m_outpostPrefab, tile.GetCenterPoint(), Quaternion.identity) as ResearchOutpost
				);
			}
			else if (!tile.hasOutpost && m_outpostInstances.ContainsKey(tile.id)) {
				Destroy(m_outpostInstances[tile.id].gameObject);
				m_outpostInstances.Remove(tile.id);
			}
			b = buildHousingAction.GetBuildingWithCapacity(tile.housingCapacity);
			if (b==null) {
				continue;
			}
			Building building = Instantiate(b, tile.GetCenterPoint(), Quaternion.identity) as Building;
			building.gameObject.transform.parent = m_objectGrouper.transform;
			m_buildingInstances.Add(building.gameObject);
		}
		
		// cache stuff that is not collapsed into other representations
		// NOTE: must do afterward for now, since it relies on e.g. research outpost locations; can go back once server sets permissions
		foreach (ResourceTile tile in resourceTiles) {
			// get coordinates in the space of the chunk
			int x = tile.x - xOffset;
			int z = tile.z - zOffset;
			// skip any resourceTiles that are out of bounds, as when e.g., server returns a bigger cached chunk than we need
			if (x<0 || x>heights.GetUpperBound(1) ||
			    z<0 || z>heights.GetUpperBound(0)
			) {
				continue;
			}
			resourceTileCache[region.bottom+z,region.left+x] = new ResourceTileLite(tile);
		}
		// apply results
		terrainData.SetHeights(region.left, region.bottom, heights);
		terrainData.SetAlphamaps(region.left, region.bottom, splats);
		for (int i=0; i<details.Length; i++) {
			terrainData.SetDetailLayer(region.left, region.bottom, i, details[i]);
		}
		// set the cleared flag
		if (region.width>0 && region.height>0) {
			m_isTerrainDataCleared = false;
		}
		// flush if requested
		if (mode == SetTerrainMode.Flush) {
			terrain.Flush();
		}
		// broadcast that tiles have finished loading
		MessengerAM.Send(new MessageLoadedNewTiles(region));
		Messenger<Region>.Broadcast(kLoadedNewTiles,region);
	}
	
	/// <summary>
	/// The world region.
	/// </summary>
	public static Region worldRegion;
	
	/// <summary>
	/// Gets a value indicating whether this <see cref="TerrainManager"/>'s terrain is fixed in place.
	/// </summary>
	/// <remarks>
	/// Indicates whether or not the terrain needs to be moved around.
	/// </remarks>
	/// <value>
	/// <c>true</c> if terrain is fixed in place; otherwise, <c>false</c>.
	/// </value>
	private bool m_isTerrainFixedInPlace {
		get {
			return terrainData.size.z >= worldRegion.height &&
				terrainData.size.x >= worldRegion.width;
		}
	}
	
	/// <summary>
	/// Moves the terrain to center of view.
	/// </summary>
	public void MoveTerrainToCenterOfView()
	{
		Vector3 center = CameraRig.use.coordCenter;
		Vector3 newPosition;
		// if the terrain is fixed in place, then pin it to the lower corner with some buffer
		if (m_isTerrainFixedInPlace) {
			newPosition = new Vector3(-1f, 0f, -1f);
		}
		// otherwise move it!
		else {
			// pad it by one megatile
			newPosition = new Vector3(
				Mathf.Clamp((int)(center.x-terrainData.size.x*0.5f), -Megatile.size, worldRegion.width-terrainData.size.x+Megatile.size),
				0f,
				Mathf.Clamp((int)(center.z-terrainData.size.z*0.5f), -Megatile.size, worldRegion.width-terrainData.size.z+Megatile.size)
			);
		}
		// if the new position is substantially different, move the terrain
		if (!Mathf.Approximately((terrain.transform.position-newPosition).sqrMagnitude, 0f)) {
			terrain.transform.position = newPosition;
			MessengerAM.Send(new MessageTerrainMoved(newPosition));
		}
	}
	
	/// <summary>
	/// The intersection ray for performing raycasts.
	/// </summary>
	private static Ray m_intersectionRay;
	
	/// <summary>
	/// The intersection plane for performing raycasts.
	/// </summary>
	private static Plane m_intersectionPlane = new Plane(Vector3.up, Vector3.zero);
	
	/// <summary>
	/// Gets the world coordinate at mouse position.
	/// </summary>
	/// <returns>
	/// The world coordinate at mouse position.
	/// </returns>
	public static Vector3 GetWorldCoordinateAtMousePosition()
	{
		float dist;
		m_intersectionRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		m_intersectionPlane.Raycast(m_intersectionRay, out dist);
		return m_intersectionRay.GetPoint(dist);
	}
	
	/// <summary>
	/// Update the grid offsets when the terrain moves.
	/// </summary>
	/// <param name='msg'>
	/// Message.
	/// </param>
	//public void _TerrainMoved(MessageTerrainMoved msg)
	//{
	//	WebRequests.CancelTileDownload();
	//	UpdatePaddingMeshGridOffset();
	//}
	
	/// <summary>
	/// flag to mark when the world has finished loading
	/// </summary>
	public static bool isWorldLoaded { get; private set; }
	
	/// <summary>
	/// Mark the world loaded
	/// </summary>
	/// <param name="msg">
	/// A <see cref="MessageWorldLoaded"/>
	/// </param>
	public void _WorldLoaded(MessageWorldLoaded msg)
	{
		isWorldLoaded = true;
		// grid scale relies on megatile size being initialized
		UpdatePaddingMeshGridOffset();
		foreach (GameObject mesh in paddingMeshes) {
			mesh.renderer.material.SetTextureScale(
				"_Grid",
				(Mathf.Max(terrainData.size.x, terrainData.size.z)/(float)Megatile.size) * Vector2.one
			);
		}
	}
	
	/// <summary>
	/// Respond to camera moving
	/// </summary>
	/// <param name="msg">
	/// A <see cref="MessageCameraMoved"/>
	/// </param>
	public void _CameraMoved(MessageCameraMoved msg)
	{
		// if the terrain is not in the view, clear it and move it to the center of the view
		if (!CameraRig.use.isTerrainInViewFrustum) {
			ClearTerrain(SetTerrainMode.NoFlush);
			MoveTerrainToCenterOfView();
		}
	}
	
	/// <summary>
	/// Load the terrain when the camera stops
	/// </summary>
	/// <param name="msg">
	/// A <see cref="MessageCameraStopped"/>
	/// </param>
	public void _CameraStopped(MessageCameraStopped msg)
	{
		// load the terrain in the view frustum
		//WebRequests.CancelTileDownload();
		//StartCoroutine(WebRequests.DownloadTilesInRegion(CameraRig.use.GetVisibleRegion()));
	}
	
	/// <summary>
	/// The time between automatic refreshes.
	/// </summary>
	[SerializeField]
	private float m_timeBetweenRefresh = 5f;
	
	/// <summary>
	/// The refresh timer.
	/// </summary>
	private float m_refreshTimer;
	
	/// <summary>
	/// Raises the loaded new tiles event and resets the refresh timer
	/// </summary>
	/// <param name='msg'>
	/// Message.
	/// </param>
	
	
	public System.DateTime lastUpdateTime;
	public bool forceTileUpdate;
		
	public void _LoadedNewTiles(MessageLoadedNewTiles msg)
	{
		m_refreshTimer = m_timeBetweenRefresh;
	}
	
	public void RefreshFromIds(params int[] resourceTileIds) 
	{
		downloadTilesCoroutine.Start( this, new Region(resourceTileIds), true );
	}
	
	protected DownloadTilesInRegionCoroutine	downloadTilesCoroutine;
	protected bool updateRegion = false;
	private float curCameraWaitTime = 0f;
	protected float cameraWaitTime = 3f;
	
	void Update()
	{
		// early out if world is not loaded
		if (!isWorldLoaded) {
			return;
		}
		
		if(InputManager.use.isDragging)
		{
			updateRegion = true;
			curCameraWaitTime = cameraWaitTime;
		}
		
		if(curCameraWaitTime <= 0f)
		{
			if (m_refreshTimer <= 0f) {
				// start a new download if one is not already in progress
				if( downloadTilesCoroutine.IsDone ) {
					downloadTilesCoroutine.Start( this, CameraRig.use.GetVisibleRegion(), updateRegion );
					updateRegion = false;
				}
				m_refreshTimer = m_timeBetweenRefresh;
			}
		}
		else
		{
			curCameraWaitTime = Mathf.Clamp(curCameraWaitTime-Time.deltaTime, 0f, cameraWaitTime);
			// update timer
			m_refreshTimer = Mathf.Clamp(m_refreshTimer-Time.deltaTime, 0f, m_timeBetweenRefresh);	
		}
		// secret debugging keys
		/*
		if (Input.GetKey(KeyCode.LeftApple) && Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.T)) {
			StartCoroutine(ComputeDesirability());
		}*/
		if (Input.GetKeyDown(KeyCode.F5) || (Input.GetKey(KeyCode.LeftApple) && Input.GetKeyDown(KeyCode.R))) {
			downloadTilesCoroutine.Start( this, CameraRig.use.GetVisibleRegion(), true);  //Forced update
		}
		if (!DebugManager.use.isDebug) {
			return;
		}
		InputManager.use.isProcessingInput = true;
	}
	
	/// <summary>
	/// Debug GUI
	/// </summary>
	void OnGUI()
	{
		if (!DebugManager.use.isDebug) {
			return;
		}
		if (!Camera.main.pixelRect.Contains(Event.current.mousePosition)) {
			return;
		}
		Vector3 coord = GetWorldCoordinateAtMousePosition();
		coord = terrain.transform.InverseTransformPoint(coord);
		GUILayout.BeginArea(new Rect(Screen.width*0.25f, 20f, Screen.width*0.5f, Screen.height-20f)); {
			GUILayout.Label(string.Format("Cache Index: ({0}, {1})", (int)coord.z, (int)coord.x));
			ResourceTileLite tile;
			if (GetResourceTileCacheUnderMouseCursor(out tile)) {
				GUILayout.Label(tile.ToString());
			}
		} GUILayout.EndArea();
	}
}