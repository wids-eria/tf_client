using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Grid display mode. <----- Move this to somewhere more appropriate
/// </summary>
[System.Flags]
public enum GridDisplayFlags {
	Megatile,
	Tile,
}

/// <summary>
/// Base class for displaying map projections. (Base holds only one map. Inherit to do more with it)
/// </summary>
public class ProjectorBase
{
	protected const string 	kMainTexture = "_MainTex";
	
	public string 			name;
	public DataMapBase 		activeMap 		{ get; protected set; }
	public Projector		activeProjector { get; protected set; }
	
	public bool				isMapVisible 	{ get; protected set; }
	public bool				isProjecting	{ get; protected set; }
	public ProjectorPack	projectionPack  { get { return DataMapController.GetProjectorPack(projectionStyle); } }
	
	public int				renderOrder	 	{ get {return m_renderOrder;} set { m_renderOrder = value;}}
	private int				m_renderOrder = 0;
	
	public ProjectionStyle 	projectionStyle { get { return m_projectionStyle; } set { SetProjectionStyle(value); } }
	private ProjectionStyle	m_projectionStyle = ProjectionStyle.Basic;
	
	protected static DataMapBase	emptyMap = new DataMapEmpty();
	
	public ProjectorBase(string aName) {
		activeMap = emptyMap;
		Init(aName);
		SetProjectionStyle(projectionStyle);
		SetProjecting(true);
	}
	public ProjectorBase(string aName, DataMapBase defaultMap) : this (aName) {
		activeMap = defaultMap;
		SetProjectionMap(activeMap);
	}
	
	protected void Init(string aName)
	{
		//Setup the projector object
		this.name = aName;
		activeProjector = GameObject.Instantiate(DataMapController.GetProjectorPrefab()) as Projector;
		activeProjector.ignoreLayers = DataMapController.GetIgnoredLayers();
		activeProjector.name = aName;
		
		DataMapController.ConnectToProjectorGroup(activeProjector.gameObject);
		activeProjector.transform.localPosition = new Vector3(0.5f*TerrainManager.terrainData.size.x,
		                                                           2,
		                                                           0.5f*TerrainManager.terrainData.size.z);
		activeProjector.orthographic = true;
		activeProjector.orthographicSize = Mathf.Max(TerrainManager.terrainData.size.x, TerrainManager.terrainData.size.z)*0.5f;
	}
	public void Destroy()
	{
		if (activeProjector != null) {
			GameObject.Destroy(activeProjector.gameObject);
		}
	}
	
	protected void SetProjectionMap(DataMapBase map)
	{
		activeMap = map;
		if (isProjecting) {
			activeProjector.material.SetTexture(kMainTexture,map.texture);
		} else {
			activeProjector.material.SetTexture(kMainTexture,null);
		}
		if (map != null) {
			activeProjector.material.SetTextureScale(kMainTexture,map.displayScale);
			activeProjector.material.SetTextureOffset(kMainTexture,map.offset);
		}
	}
	protected void SetProjector(ProjectorBase projector)
	{
		activeProjector = projector.activeProjector;
		activeMap = projector.activeMap;
		SetProjectionStyle(projector.projectionStyle);
	}
	protected void SetProjectionStyle(ProjectionStyle style)
	{
		m_projectionStyle = style;
		
		Material material = new Material(projectionPack.material);
		material.mainTextureScale = activeMap.displayScale;
		material.mainTextureOffset = activeMap.offset;
		
		activeProjector.material = material;
		SetProjectionMap(activeMap);
		//Let the Data Map controller know that we need a refresh
		Messenger.Broadcast(DataMapController.kProjectorsModified);
	}
	
	public void SetProjecting(bool active)
	{
		isProjecting = active;
		SetProjectionMap(activeMap);
	}
	public void ToggleProjecting()
	{
		SetProjecting(!isProjecting);
	}
	
	public void HideMap()
	{
		isMapVisible = false;
		SetProjectionMap(emptyMap);
		
	}
	public void ShowMap()
	{
		isMapVisible = true;
		SetProjectionMap(activeMap);
	}
	public void ToggleMap()
	{
		SetMapVisibility(!isMapVisible);
	}
	public void SetMapVisibility(bool visible)
	{
		if (visible) {
			ShowMap();
		} else {
			HideMap();
		}
	}
}

public abstract class ProjectorDictionary<TKey,TValue> : ProjectorBase
{
	protected TKey 		m_activeKey;
	protected TKey 		m_defaultKey = default(TKey);
	protected TValue 	m_defaultValue = default(TValue);
	
	private Dictionary<TKey,TValue> m_dictionary = new Dictionary<TKey, TValue>();
	
	protected int 		Count {get {return m_dictionary.Count;}}
	
	public ProjectorDictionary(string name) : base(name) {
		m_activeKey = m_defaultKey;
	}
	
	protected void Add(TKey key, TValue value)
	{
		try {
			m_dictionary.Add(key,value);
		}
		catch ( ArgumentNullException ) {
			Debug.LogWarning("Trying to add 'null' key to dictionary");
			return;
		}
	}
	protected void Remove(TKey key)
	{
		if (!m_dictionary.ContainsKey(key)) {
			Debug.LogError(string.Format("Key `{0}' does not exist in the dictionary.",key.ToString()));
		} else {
			m_dictionary.Remove(key);
		}
	}
	public virtual void SetActiveKey(TKey key)
	{
		//if (key.Equals(m_activeKey)) return;
		if (m_dictionary.ContainsKey(key)) {
			m_activeKey = key;
		} else {
			m_activeKey = m_defaultKey;
		}
	}
	
	public TValue GetValue(TKey index) {
		try {
			return m_dictionary[index];
		}
		catch( KeyNotFoundException ) {
			return m_defaultValue;
		}
		catch( ArgumentNullException ) {
			return m_defaultValue;
		}
	}
	public bool IsCurrentIndex(TKey index) {
		return index.Equals(m_activeKey);
	}

	protected bool ChangeValue(TKey key, TValue newValue) {
		try {
			m_dictionary[key] = newValue;
			return true;
		}
		catch ( KeyNotFoundException ) {
			Add(key,newValue);
			return true;
		}
		catch( ArgumentNullException ) {
			return false;
		}
	}
}

public class ProjectorMapSet<TKey> : ProjectorDictionary<TKey,DataMapBase> 
{
	public ProjectorMapSet(string name) : base(name) {
		m_defaultValue = emptyMap;
	}
	public override void SetActiveKey(TKey key) {
		base.SetActiveKey(key);
		SetProjectionMap(GetValue(key));
	}
}

/*
public class ProjectorSet<TKey> : ProjectorDictionary<TKey,ProjectorBase>
{
	public ProjectorSet(string name) : base(name) {
		m_defaultValue = this;
	}
	public override void SetActiveKey(TKey key) {
		base.SetActiveKey(key);
		SetProjector(GetValue(key));
	}
}*/

/// <summary>
/// A projector that is indexed by an int
/// </summary>
public class ProjectorMapList : ProjectorMapSet<int>
{
	public ProjectorMapList(string name, IEnumerable<DataMapBase> maps) : base(name) {
		int i = Count-1;
		foreach (DataMapBase map in maps) {
			Add(i,map);
			i++;
		}
	}
	public override void SetActiveKey (int key) {
		key = key % Count;
		base.SetActiveKey(key);
	}
}
	
	
	
	
	
	
	
	

