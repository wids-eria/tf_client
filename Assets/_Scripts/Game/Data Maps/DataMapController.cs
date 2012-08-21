using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum MappingColor {
	Empty,     // NOTE: do NOT, WHATSOEVER, add values at the top of this enum, or rearrange them
	Disabled,
	Residential,
	Protected,
	NoZoning,
	Highlight,
	Desirable,
	Undesirable,
	UserOwned,
	OtherOwned,
	Unowned,
}

public enum ProjectionStyle {
	Basic, // NOTE: do NOT, WHATSOEVER, add values at the top of this enum, or rearrange them
	Grid,
	Cloud,
}

public struct ProjectorPack {
	public ProjectionStyle 	name 		{get; private set;}
	public Material 		material 	{get; private set;}
	public Texture2D 		baseTexture {get; private set;}
	
	public ProjectorPack(ProjectionStyle name, Material material, Texture2D texture) : this() {
		this.name = name;
		this.material = material;
		this.baseTexture = texture;
	}
}

/// <summary>
/// Used to sort Projectors by depth
/// </summary>
internal class ProjecterBaseDepthComparer : IComparer<ProjectorBase> {
	public static ProjecterBaseDepthComparer	Instance = new ProjecterBaseDepthComparer();
	
	public int Compare( ProjectorBase lhs, ProjectorBase rhs ) {
    	return Mathf.Clamp( lhs.renderOrder - rhs.renderOrder, -1, 1 );
    }
}

/// <summary>
/// A class for configuring and managing the projections on the terrain
/// </summary>
public class DataMapController : MonoBehaviour
{
	public const string 	kProjectorsModified = "ProjectorModified";
	
	[HideInInspector]
	public List<Color> 		colors;
	[HideInInspector]
	public List<Material> 	projectionTypes;
	
	public LayerMask		ignoredLayers;
	public Projector		projectionPrefab;
	
	public static List<string> dataMapNames = new List<string>();
	
	protected static LayerMask 									m_ignoredLayers;
	protected static Dictionary<MappingColor,Color> 			m_colorMap;
	protected static Dictionary<ProjectionStyle,ProjectorPack> 	m_projectionTypes;
	protected static Projector 									m_projectorPrefab;
	
	protected static List<ProjectorBase> 						m_dataMapProjectors;
	protected static GameObject									m_projectionGroup;
	
	void Awake() {
		//Listeners
		MessengerAM.Listen("world", this);
		Messenger.AddListener(kProjectorsModified,RefreshRenderOrder);
		Messenger<int>.AddListener(ProjectionInterfaceGUI.kProjectionOptionChange, ChangeVisibleDataView);
		
		
		if (m_colorMap == null) {
			m_colorMap = new Dictionary<MappingColor, Color>();
			MappingColor[] colorKeys = (MappingColor[])System.Enum.GetValues(typeof(MappingColor));
			for(int i=0;i<colorKeys.Length;i++) {
				m_colorMap.Add(colorKeys[i],colors[i]);
			}
		}
		if (m_projectionTypes == null) {
			m_projectionTypes = new Dictionary<ProjectionStyle,ProjectorPack>();
			ProjectionStyle[] projectionKeys = (ProjectionStyle[])System.Enum.GetValues(typeof(ProjectionStyle));
			for(int i=0;i<projectionKeys.Length;i++) {
				m_projectionTypes.Add(projectionKeys[i],new ProjectorPack(projectionKeys[i],projectionTypes[i],projectionTypes[i].GetTexture("_MainTex")as Texture2D));
			}
		}
		m_ignoredLayers = ignoredLayers;
		m_projectorPrefab = projectionPrefab;
		
		//Projection Group
		m_projectionGroup = new GameObject("Projectors");
		m_projectionGroup.transform.parent = TerrainManager.use.transform;
		m_projectionGroup.transform.localPosition = Vector3.zero;
	}
	void OnDestroy()
	{
		m_colorMap.Clear();
		m_projectionTypes.Clear();
		foreach(ProjectorBase proj in m_dataMapProjectors) {
			proj.Destroy();
		}
		m_dataMapProjectors.Clear();
		
		
		MessengerAM.StopListen("world",this.gameObject);
		Messenger.RemoveListener(kProjectorsModified,RefreshRenderOrder);
		Messenger<int>.RemoveListener(ProjectionInterfaceGUI.kProjectionOptionChange, ChangeVisibleDataView);
	}
	
	void Update()
	{
		//CLOUD HACK
		ProjectorCloud cloud = GetProjector<ProjectorCloud>("Clouds");
		if (cloud != null) {
			Vector2 offset = cloud.activeProjector.material.mainTextureOffset;
			cloud.activeProjector.material.mainTextureOffset = offset + (new Vector2(-0.001f,0.002f) * Time.deltaTime);
		}
	}
	
	void ChangeVisibleDataView(int index) {
		GetProjector<ProjectorMapList>("DataMaps").SetActiveKey(index);
	}
	
	/// <summary>
	/// Adds the projection textures to the terrain
	/// NOTE: requires world to be initialized (e.g. megatileSize)
	/// NOTE: requires that ground splats already be added
	/// </summary>
	public void Init()
	{
		m_dataMapProjectors = new List<ProjectorBase>();
		
		List<DataMapBase> m_dataViewerMaps = new List<DataMapBase>() {
			new DataMapEmpty(),
			new DataMapDesirability(),
			new DataMapZoning(),
			new DataMapOwnership(),
		};
		foreach(DataMapBase dmb in m_dataViewerMaps) {
			dataMapNames.Add(dmb.name);
		}
		
		AddProjection(new ProjectorGrid		("Grid",GameManager.worldData.megatile_width));
		//AddProjection(new ProjectorActions	("Action"));
		AddProjection(new ProjectorSelection("Selection"));
		AddProjection(new ProjectorCloud	("Clouds"));
		
		AddProjection(new ProjectorMapList	("DataMaps", m_dataViewerMaps));
	}
	
	public static List<T> GetProjectors<T>() where T: ProjectorBase {
		List<T> projectors = new List<T>();
		if (m_dataMapProjectors == null) return null;
		foreach(ProjectorBase dmp in m_dataMapProjectors) {
			if (dmp.GetType() == typeof(T)) {
				projectors.Add((T)dmp);
			}
		}
		return (projectors.Count > 0) ? projectors : null;
	}
	
	public static T GetProjector<T>() where T: ProjectorBase {
		if (m_dataMapProjectors == null) return null;
		foreach(ProjectorBase dmp in m_dataMapProjectors) {
			if (dmp.GetType() == typeof(T)) {
				return (T)dmp;
			}
		}
		return null;
	}
	public static T GetProjector<T>(string name) where T: ProjectorBase {
		List<T> projectors = GetProjectors<T>();
		if (projectors == null) return null;
		foreach(T dmp in projectors) {
			if (dmp.name == name) {
				return dmp;
			}
		}
		return null;
	}
	
	protected void AddProjection(ProjectorBase projection)
	{
		foreach(ProjectorBase p in m_dataMapProjectors) {
			if (p.GetType() == projection.GetType()) {
				Debug.LogError(string.Format("DataMapController already contains projector of type `{0}'.",p.GetType().ToString()));
				break;
			}
		}
		m_dataMapProjectors.Add(projection);
		RefreshRenderOrder();
	}
	
	protected void RefreshRenderOrder()
	{
		//Deactivate projectors
		foreach(ProjectorBase projector in m_dataMapProjectors) {
			projector.activeProjector.enabled = false;
		}
		//Enable/sort Projectors in order of depth
		m_dataMapProjectors.Sort(ProjecterBaseDepthComparer.Instance);
		foreach(ProjectorBase projector in m_dataMapProjectors) {
			projector.activeProjector.enabled = true;
		}
	}
	
	public static Color GetColor(MappingColor color) {
		return m_colorMap[color];
	}
	public static ProjectorPack GetProjectorPack(ProjectionStyle kind) {
		return m_projectionTypes[kind];
	}
	public static Material GetProjectionMaterial(ProjectionStyle kind) {
		return GetProjectorPack(kind).material;
	}
	public static LayerMask GetIgnoredLayers() {
		return m_ignoredLayers;
	}
	public static Projector GetProjectorPrefab() {
		return m_projectorPrefab;
	}
	public static void ConnectToProjectorGroup(GameObject go) {
		go.transform.parent = m_projectionGroup.transform;
	}
	
	/// <summary>
	/// Update the grid projector as needed once the world has loaded.
	/// </summary>
	/// <param name='msg'>
	/// Message.
	/// </param>
	public void _WorldLoaded(MessageWorldLoaded msg)
	{
		Debug.Log("World Loaded");
		//Init projectors and things
		if (m_dataMapProjectors == null) Init();
	}

}