using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Input manager.
/// </summary>
public class InputManager : MonoBehaviour
{
	public const string kTileSelectionChanged = "TileSelectionChanged";
	
	public const string kRightClickedTile 	= "RightClickedTile";
	public const string kLeftClickedTile 	= "LeftClickedTile";
	public const string kMegatileCapMet		= "MegatileCapMet";
	
	protected int TilesSelectedCount =0;
	
	protected List<int> m_selectedMegatiles = new List<int>();
	protected Dictionary<int, List<Survey>> m_cachedSurveys;
	
	[SerializeField]
	protected int CapMegatilesCount =5;
	
	
	/// <summary>
	/// camera movement sensitivity
	/// </summary>
	[SerializeField]
	private float m_cameraInputSensitivity = 0.5f;
	/// <summary>
	/// Speed scalar when holding shift key 
	/// </summary>
	[SerializeField]
	private float m_fullThrottleScalar = 5f;
	
	/// <summary>
	/// is the manager currently processing input?
	/// </summary>
	public bool isProcessingInput = false;
	
	/// <summary>
	/// the prefab for the cursor object
	/// </summary>
	[SerializeField]
	private Transform m_cursorPrefab;
	/// <summary>
	/// the instance of the cursor object
	/// </summary>
	private Transform m_cursor;
	/// <summary>
	/// the projector associated with the cursor
	/// </summary>
	private Projector m_cursorProjector;
	
	/// <summary>
	/// the prefab for the selection highlighter
	/// </summary>
	[SerializeField]
	private Transform m_selectionHighlightPrefab;
	/// <summary>
	/// the instance of the selection highlighter
	/// </summary>
	private Transform m_selectionHighlight;
	/// <summary>
	/// the projector associated with the selection highlighter
	/// </summary>
	private Projector m_selectionHighlightProjector;
	
	/// <summary>
	/// singleton
	/// </summary>
	public static InputManager use { get; private set; }
	
	/// <summary>
	/// Initialize singleton
	/// </summary>
	void Awake()
	{
		if (use != null) Destroy(use.gameObject);
		use = this;
		MessengerAM.Listen("www", this);
//		Messenger.AddListener("Clear Current Tiles", ClearTileSelectionCount);  //broadcast location: PlayerAction
	}
	
	void OnDestroy(){
//		Messenger.RemoveListener("Clear Current Tiles", ClearTileSelectionCount);
	}
	
	/// <summary>
	/// Spawn projectors when the world has finished loading
	/// </summary>
	/// <param name="msg">
	/// A <see cref="MessageWorldLoaded"/>
	/// </param>
	public void _WorldLoaded(MessageWorldLoaded msg)
	{
		int ignoreLayers = ~(
			(1<<TerrainManager.use.terrain.gameObject.layer)|
			(1<<TerrainManager.use.paddingMeshes[0].gameObject.layer)
		);
		m_cursor = Instantiate(m_cursorPrefab) as Transform;
		m_cursorProjector = m_cursor.GetComponentInChildren<Projector>();
		m_cursorProjector.orthographicSize = Megatile.size;
		m_cursorProjector.ignoreLayers = ignoreLayers;
		m_selectionHighlight = Instantiate(m_selectionHighlightPrefab) as Transform;
		m_selectionHighlightProjector = m_selectionHighlight.GetComponentInChildren<Projector>();
		m_selectionHighlightProjector.orthographicSize = Megatile.size;
		m_selectionHighlightProjector.ignoreLayers = ignoreLayers;
		m_selectionHighlight.position = new Vector3(-1f, 0f, -1f)*Megatile.size;
	}
	
	/// <summary>
	/// The drag scale factor.
	/// </summary>
	[SerializeField]
	private float m_dragScaleFactor = 0.05f;
	
	/// <summary>
	/// A mouse cursor drag.
	/// </summary>
	internal class CursorDrag
	{
		/// <summary>
		/// Gets or sets the start position.
		/// </summary>
		/// <value>
		/// The start position.
		/// </value>
		public Vector3 start { get; private set; }
		/// <summary>
		/// Gets the current position.
		/// </summary>
		/// <value>
		/// The current position.
		/// </value>
		public Vector3 current { get { return Input.mousePosition; } }
		/// <summary>
		/// The previous position.
		/// </summary>
		public Vector3 previous;
		/// <summary>
		/// Gets the delta position since the previous frame.
		/// </summary>
		/// <value>
		/// The delta.
		/// </value>
		public Vector3 delta { get { return current-previous; } }
		/// <summary>
		/// Initializes a new instance of the <see cref="InputManager.CursorDrag"/> class.
		/// </summary>
		public CursorDrag()
		{
			start = Input.mousePosition;
			previous = start;
		}
	}
	
	/// <summary>
	/// Gets a value indicating whether this <see cref="InputManager"/> is holding alternate button.
	/// </summary>
	/// <value>
	/// <c>true</c> if is holding alternate button; otherwise, <c>false</c>.
	/// </value>
	public static bool isHoldingAltButton {
		get {
			return Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
		}
	}
	
	/// <summary>
	/// Gets a value indicating whether this <see cref="InputManager"/> is holding alternate click.
	/// </summary>
	/// <value>
	/// <c>true</c> if is holding alternate click; otherwise, <c>false</c>.
	/// </value>
	public static bool isHoldingAltClick {
		get {
			return Input.GetMouseButton(1) || (Input.GetMouseButton(0)&&isHoldingAltButton);
		}
	}
	
	/// <summary>
	/// Gets a value indicating whether this <see cref="InputManager"/> is alternate click down.
	/// </summary>
	/// <value>
	/// <c>true</c> if is alternate click down; otherwise, <c>false</c>.
	/// </value>
	public static bool isAltClickDown {
		get {
			return Input.GetMouseButtonDown(1) || (Input.GetMouseButtonDown(0)&&isHoldingAltButton);
		}
	}
	
	/// <summary>
	/// The pan key modifier.
	/// </summary>
	[SerializeField]
	private KeyCode m_panKeyModifier = KeyCode.Space;
	/// <summary>
	/// Gets or sets the pan key modifier.
	/// </summary>
	/// <value>
	/// The pan key modifier.
	/// </value>
	public KeyCode panKeyModifier {
		get { return m_panKeyModifier; }
	}
	
	/// <summary>
	/// Gets a value indicating whether this <see cref="InputManager"/> is holding pan click modifier.
	/// </summary>
	/// <value>
	/// <c>true</c> if is holding pan click modifier; otherwise, <c>false</c>.
	/// </value>
	public static bool isHoldingPanClickModifier {
		get { return Input.GetKey(use.panKeyModifier); }
	}
	
	/// <summary>
	/// Gets a value indicating whether this <see cref="InputManager"/> is holding pan click.
	/// </summary>
	/// <value>
	/// <c>true</c> if is holding pan click; otherwise, <c>false</c>.
	/// </value>
	public static bool isHoldingPanClick {
		get {
			return Input.GetMouseButton(0) && isHoldingPanClickModifier;
		}
	}
	
	/// <summary>
	/// Gets a value indicating whether this <see cref="InputManager"/> is holding primary click.
	/// </summary>
	/// <value>
	/// <c>true</c> if is holding primary click; otherwise, <c>false</c>.
	/// </value>
	public static bool isHoldingPrimaryClick {
		get {
			return Input.GetMouseButton(0) && !isHoldingAltButton;
		}
	}
	
	/// <summary>
	/// Gets a value indicating whether this <see cref="InputManager"/> is primary click down.
	/// </summary>
	/// <value>
	/// <c>true</c> if is primary click down; otherwise, <c>false</c>.
	/// </value>
	public bool isPrimaryClickDown {
		get {
			return Input.GetMouseButtonDown(0) && !isHoldingAltButton;
		}
	}
	
	/// <summary>
	/// The drag.
	/// </summary>
	private CursorDrag m_drag = null;
	
	public bool isDragging
	{
		get{
			if(m_drag != null)
				return (m_drag.current != m_drag.previous);	
			else
				return false;
		}
	}
	
	#region CurrentAction
	private PlayerAction[] m_availableActions{get{return BottomTrayGUI.use.AvailableActions;}}
	/// <summary>
	/// The currently selected action.
	/// </summary>
	private int m_currentlySelectedAction {
		get {
			return m_currentlySelectedActionBackingField;
		}
		set {
			if (value == m_currentlySelectedActionBackingField) {
				return;
			}
			m_availableActions[m_currentlySelectedActionBackingField].enabled = false;
			m_currentlySelectedActionBackingField = value;
			m_availableActions[m_currentlySelectedActionBackingField].enabled = true;
		}
	}
	/// <summary>
	/// The currently selected action backing field.
	/// </summary>
	private int m_currentlySelectedActionBackingField = 0;
	
	
	/// <summary>
	/// Gets the current action.
	/// </summary>
	/// <value>
	/// The current action.
	/// </value>
	public PlayerAction currentAction {
		get {
			return (m_availableActions==null || m_availableActions.Length==0)?
				null:
				m_availableActions[m_currentlySelectedAction];
		}
	}
	
	/// <summary>
	/// Sets the current action.
	/// </summary>
	/// <param name='action'>
	/// Action.
	/// </param>
	public void SetCurrentAction(PlayerAction action)
	{
		m_currentlySelectedAction = Player.current.permissionBitmaskByActionType[action.GetType()]; // corresponds to index
	}
	
	/// <summary>
	/// Gets the current input mode.
	/// </summary>
	/// <value>
	/// The current input mode.
	/// </value>
	public PlayerAction.InputMode currentInputMode { get { return (currentAction==null)?PlayerAction.InputMode.None:currentAction.inputMode; } }
	#endregion
	
	/// <summary>
	/// Gets the resource tile selection.
	/// </summary>
	/// <value>
	/// The resource tile selection.
	/// </value>
	public ResourceTileSelection resourceTileSelection {
		get {
			return new ResourceTileSelection(m_resourceTileSelection.resource_tile_ids);
		}
	}
	
	/// <summary>
	/// The resource tile selection backing field.
	/// </summary>
	private ResourceTileSelection m_resourceTileSelection = new ResourceTileSelection(new int[0]);
	
	/// <summary>
	/// Gets a value indicating whether the selection is currently changing.
	/// </summary>
	/// <value>
	/// <c>true</c> if is selection changing; otherwise, <c>false</c>.
	/// </value>
	public bool isSelectionChanging {
		get {
			return currentInputMode == PlayerAction.InputMode.PaintResourceTiles && 
				(!GameGUIManager.use.IsCursorInGUI() && Input.GetMouseButton(0));
		}
	}
	
	/// <summary>
	/// Returns a list of selected Megatile id numbers
	/// </summary>
	/// <returns>
	/// The selected megatile identifiers.
	/// </returns>
	public List<int> GetSelectedMegatileIds() {
		return m_selectedMegatiles;
	}
	
	public ResourceTileLite[] GetSelectionAsResourceTiles() {
		TerrainManager.Status status = TerrainManager.Status.Failed;
		return TerrainManager.GetResourceTileCacheGroup(out status, m_resourceTileSelection.ToArray());
	}
	
	public List<Survey> GetCachedSurveys(int MegaTileID)
	{
		if(m_cachedSurveys.ContainsKey(MegaTileID))
			return m_cachedSurveys[MegaTileID];	
		else
			return new List<Survey>();
	}
	
	public void InitCachedSurveyForMegaTile(int MegaTileID)
	{
		CheckSurveyCacheInit();
		m_cachedSurveys[MegaTileID] = new List<Survey>();	
	}
	
	public void AddCachedSurvey(int MegaTileID, Survey newSurvey)
	{
		CheckSurveyCacheInit();
		if(!m_cachedSurveys.ContainsKey(MegaTileID))
			m_cachedSurveys[MegaTileID] = new List<Survey>();
		
		m_cachedSurveys[MegaTileID].Add(newSurvey);
	}
	
	public bool HasCachedSurvey(int MegaTileID)
	{
		CheckSurveyCacheInit();
		return m_cachedSurveys.ContainsKey(MegaTileID);	
	}
	
	private void InitSurveyCache()
	{
		Debug.Log ("cache survey init");
		m_cachedSurveys = new Dictionary<int, List<Survey>>();	
	}
	
	private void CheckSurveyCacheInit()
	{
		if(m_cachedSurveys == null)
			InitSurveyCache();
	}
	
	/// <summary>
	/// Sets the selection/highlight cursors on or off.
	/// </summary>
	void SetSelectionCursors()
	{
		// set cursor enable status
		m_cursorProjector.enabled = Camera.main.pixelRect.Contains(Input.mousePosition) &&
			!GameGUIManager.use.IsCursorInGUI() &&
			currentInputMode == PlayerAction.InputMode.SelectMegatile;
	}
	
	/// <summary>
	/// Gets a value indicating whether this <see cref="InputManager"/> is full throttle.
	/// </summary>
	/// <value>
	/// <c>true</c> if is full throttle; otherwise, <c>false</c>.
	/// </value>
	private bool m_isFullThrottle { get { return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift); } }
	
	/// <summary>
	/// The current throttle.
	/// </summary>
	private float m_currentThrottle { get { return (m_isFullThrottle)?m_fullThrottleScalar:1f; } }
	
	/// <summary>
	/// Moves the camera with keyboard.
	/// </summary>
	void MoveCameraWithKeyboard()
	{
//		if (m_currentInputMode != PlayerAction.InputMode.None) {
//			return;
//		}
		Vector3 desired = CameraRig.use.desiredPosition;
		desired.x += Input.GetAxis("Horizontal")*m_cameraInputSensitivity*m_currentThrottle;
		desired.x = Mathf.Clamp(desired.x, TerrainManager.worldRegion.left, TerrainManager.worldRegion.right);
		desired.z += Input.GetAxis("Vertical")*m_cameraInputSensitivity*m_currentThrottle;
		desired.z = Mathf.Clamp(desired.z, TerrainManager.worldRegion.bottom, TerrainManager.worldRegion.top);
		CameraRig.use.desiredPosition = desired;
	}
	
	/// <summary>
	/// Zooms the camera.
	/// </summary>
	void ZoomCamera()
	{
		if (!Camera.main.pixelRect.Contains(GUIHelpers.MouseToGUIPosition(Input.mousePosition))) return;
		float zoom = -Input.GetAxis("Mouse ScrollWheel");
		if (!Mathf.Approximately(zoom, 0f)) {
			CameraRig.use.Zoom(zoom);
		}
	}
	
	/// <summary>
	/// Pans the camera.
	/// </summary>
	void PanCamera()
	{
		
		if (isHoldingPrimaryClick) {
			// possible that mouse event got missed
			if (m_drag == null) {
				m_drag = new CursorDrag();
			}
			Vector3 p = new Vector3(m_drag.delta.x, 0f, m_drag.delta.y);
			CameraRig.use.desiredPosition -= p*m_dragScaleFactor*m_currentThrottle*Mathf.Lerp(1f, 0.5f, CameraRig.use.zoomScaleFactor);
			m_drag.previous = m_drag.current;
		}
		else
		{
			m_drag = null;	
		}
		/*if (isPrimaryClickDown) {
			m_drag = new CursorDrag();
		}
		if (isHoldingPrimaryClick) {
			// possible that mouse event got missed
			if (m_drag == null) {
				m_drag = new CursorDrag();
			}
			Vector3 p = new Vector3(m_drag.delta.x, 0f, m_drag.delta.y);
			CameraRig.use.desiredPosition -= p*m_dragScaleFactor*m_currentThrottle*Mathf.Lerp(1f, 0.5f, CameraRig.use.zoomScaleFactor);
			m_drag.previous = m_drag.current;
		}*/
	}
	/*
	//clear current tiles selection and switch to select mode
	void ClearTileSelectionCount(){
		TilesSelectedCount = 0;
		paintSelectionInputMode = PaintSelectionInputMode.Pan;
		BottomTrayGUI.use.inputModeSelection = 0;
		Messenger<int>.Broadcast(BottomTrayGUI.kPaintSelectionInputMode, 0);
	}*/
	
	/// <summary>
	/// Sets the resource tile selection.
	/// </summary>
	void SetResourceTileSelection()
	{
		int count, idMegatile;
		
		Vector3 coordinate = TerrainManager.GetWorldCoordinateAtMousePosition();
		coordinate = WorldPositionToMegatileCoord(coordinate);
		
		count = m_resourceTileSelection.Count();
		
		//Make sure the area is selectable
		ResourceTileLite tile;
		if (!TerrainManager.GetResourceTileCacheAtWorldPosition(coordinate,out tile)) {
			return;
		}
		//Retrieve the id of the megatile
		idMegatile = tile.idMegatile;
		
		/***********************************
		 * 
		 *     ADD TILES TO SELECTION
		 * 
		 ***********************************/
		if (paintSelectionInputMode==PaintSelectionInputMode.Select && isHoldingPrimaryClick) {
			/* --- Old Selection Code --- 
			foreach(ResourceTileLite tile in currentResourceTiles){
			if (tile.IsActionPermitted(currentAction)) {
				bool isInSelectionList = false;
				foreach (int tileId in m_resourceTileSelection) {
					if (tileId == tile.id) {
						isInSelectionList = true;
						break;
					}
				}
				if (!isInSelectionList) {
					m_resourceTileSelection.Add(tile.id);
					TilesSelectedCount++;
				}
				//TerrainProjections.use.SetSelectionHighlight((int)localPoint.x, (int)localPoint.z, true);
			}
			}*/
			//Avoid selecting an already selected tile
			if (m_selectedMegatiles.Contains(idMegatile)) return;
			
			//Add the selected Megatile
			if (m_selectedMegatiles.Count < CapMegatilesCount) {
				m_selectedMegatiles.Add( idMegatile );
				foreach (int id in PosToMegatileSelection(coordinate)) {
					if (!m_resourceTileSelection.Contains(id)) {
						m_resourceTileSelection.Add(id);
					}
				}
				if (m_selectedMegatiles.Count == CapMegatilesCount) {
					Messenger.Broadcast(kMegatileCapMet);
					paintSelectionInputMode = PaintSelectionInputMode.Pan;
					Messenger<int>.Broadcast(BottomTrayGUI.kPaintSelectionInputMode, 0);
					BottomTrayGUI.use.inputModeSelection = 0;
				} 
			}
		}
		
		/***********************************
		 * 
		 *   REMOVE TILES FROM SELECTION
		 * 
		 ***********************************/
		else if ( paintSelectionInputMode==PaintSelectionInputMode.Deselect && isHoldingPrimaryClick )
		{
			/* --- Old Deselection code --- 
			foreach(ResourceTileLite tile in currentResourceTiles)
			{
				int j = -1;
				for (int i=0; i<m_resourceTileSelection.Count(); ++i) {
					if (m_resourceTileSelection[i] == tile.id) {
						j = i;
					}
				}
				if (j >= 0) 
				{
					m_resourceTileSelection.RemoveAt(j);
 					TilesSelectedCount--;
				}
			}*/
			//We haven't selected this tile, so we can leave
			if (!m_selectedMegatiles.Contains(idMegatile)) return;
			m_selectedMegatiles.Remove(idMegatile);
			
			//Close the menu if it's open
			Messenger.Broadcast(TileSelectionMenu.kClose);
			
			//Deselect the selected megatile
			foreach (int id in PosToMegatileSelection( coordinate ) ) {
				m_resourceTileSelection.Remove( id );
			}
		}
		
		/***********************************
		 * 
		 *   REPORT CHANGES IN SELECTION
		 * 
		 ***********************************/
		
		//Tile count is different
		if (count != m_resourceTileSelection.Count()) {
			Messenger<ResourceTileSelection>.Broadcast(kTileSelectionChanged,resourceTileSelection);
			Messenger<Vector3>.Broadcast(kLeftClickedTile,coordinate);
		}
		
	}
	
	public List<int> PosToMegatileSelection( Vector3 position ) {
		List<int> ids = new List<int>();
		//if(TilesSelectedCount> (CapMegatilesCount*9)){
			//paintSelectionInputMode = PaintSelectionInputMode.Deselect;
			//Messenger<int>.Broadcast(BottomTrayGUI.kPaintSelectionInputMode, 2);
			//BottomTrayGUI.use.inputModeSelection = 2;
		//}
		position = WorldPositionToMegatileCoord(position);
		position.x -= 1;
		position.z -=1;
		
		ResourceTileLite tile;
		for(int i=0; i<=2;i++){
			for(int j=0; j<=2;j++){
				TerrainManager.GetResourceTileCacheAtWorldPosition(new Vector3(position.x+i,position.y+j,position.z+j),out tile);
				ids.Add(tile.id);
			}
		}
		return ids;
	}
	
	public Vector3 WorldPositionToMegatileCoord( Vector3 worldPosition ) {
		return Megatile.ConvertPointToMegatileCenter(worldPosition);
	}
	
	/// <summary>
	/// Clears the resource tile selection.
	/// </summary>
	public void ClearResourceTileSelection() {
		m_resourceTileSelection.Clear();
		m_selectedMegatiles.Clear();
		Messenger<ResourceTileSelection>.Broadcast(kTileSelectionChanged,resourceTileSelection);
		Messenger.Broadcast(TileSelectionMenu.kClose);
	}
	
	/// <summary>
	/// Paint selection input mode.
	/// </summary>
	public enum PaintSelectionInputMode {  Select, Deselect, Pan }
	
	
	/// <summary>
	/// The paint selection input mode.
	/// </summary>
	public static PaintSelectionInputMode paintSelectionInputMode {
		get {
			if (m_paintSelectionInputMode == PaintSelectionInputMode.Deselect //||
				//isHoldingAltClick || isHoldingAltButton
			) {
				return PaintSelectionInputMode.Deselect;
			}
			else if (m_paintSelectionInputMode == PaintSelectionInputMode.Pan ||
				isHoldingPanClick || isHoldingPanClickModifier
			) {
				return PaintSelectionInputMode.Pan;
			}
			else {
				return PaintSelectionInputMode.Select;
			}
		}
		set {
			m_paintSelectionInputMode = value;
		}
	}
	
	/// <summary>
	/// The paint selection input mode backing field.
	/// </summary>
	private static PaintSelectionInputMode m_paintSelectionInputMode;
	
	/// <summary>
	/// Flag specifying whether the current mouse click started in the GUI.
	/// </summary>
	private bool m_didClickStartInGUI = false;
	
	
	/// <summary>
	/// Handles the mouse input.
	/// </summary>
	void HandleMouseInput()
	{
		// early out if the click is outside the screen
		if (!Camera.main.pixelRect.Contains(Input.mousePosition)) {
			return;
		}
		
		// early out if the cursor is in the GUI rather than open space
		if(GameGUIManager.use.IsCursorInGUI()) {
			if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) {
				m_didClickStartInGUI = true;
			}
			m_selectionHighlightProjector.enabled = false;
			return;
		}
		
		// unset flag if mouse up
		if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1)) {
			m_didClickStartInGUI = false;
		}
		
		// zoom
		ZoomCamera();
		
		// early out and force off cursor if mouse is out of bounds
		Vector3 p = TerrainManager.GetWorldCoordinateAtMousePosition();
		if (!TerrainManager.worldRegion.Contains(p)) {
			m_cursorProjector.enabled = false;
			return;
		}
		
		// move cursor/highlighter
		p = Megatile.ConvertPointToMegatileCenter(p);
		m_cursor.transform.position = p;
		
		// early out if point is outside world region
		if (!TerrainManager.worldRegion.Contains(p)) {
			return;
		}
		
		// early out if the click started in the gui
		if (m_didClickStartInGUI) {
			return;
		}
		
		//See if it was a right click
		if (isAltClickDown) {
			Messenger<Vector3>.Broadcast(kRightClickedTile,p);
			return;
		}
		
		
		if(paintSelectionInputMode != PaintSelectionInputMode.Pan){
			SelectMegatileAt(m_cursor.position);
			SetResourceTileSelection();
		}
		else{
			PanCamera();
		}
		
//		switch (paintSelectionInputMode) {
//		case PaintSelectionInputMode.Select:
//			SelectMegatileAt(m_cursor.position);
//			SetResourceTileSelection();
//			break;
//		case PaintSelectionInputMode.Deselect:
//			SelectMegatileAt(m_cursor.position);
//			SetResourceTileSelection();
//			break;
//		default:
//			PanCamera();
//			break;
//		}
		
		/*
		// process mouse based on current input mode
		switch (m_currentInputMode)
		{
		case PlayerAction.InputMode.None:
			PanCamera();
			break;
		case PlayerAction.InputMode.PaintResourceTiles:
			if (paintSelectionInputMode == PaintSelectionInputMode.Pan) {
				PanCamera();
			} else {
				SelectMegatileAt(m_cursor.position);
				//SetResourceTileSelection();
			}
			break;
		case PlayerAction.InputMode.SelectMegatile:
			if (isPrimaryClickDown) {
				SelectMegatileAt(m_cursor.position);
			}
			break;
		}*/
	}
	
	public void SelectMegatileAt(Vector3 position) {
		position = WorldPositionToMegatileCoord(position);
		m_selectionHighlight.transform.position = position + new Vector3(0f,1f,0f);
		m_selectionHighlightProjector.enabled = true;
	}
	
	/*
	/// <summary>
	/// Refreshes the selected tile data and notifies the current action when complete.
	/// </summary>
	public IEnumerator RefreshSelectedTileData() {
//		if (selectedMegatile == null) {
//			yield break;
//		}
		WWW www = WWWX.Get(
			WebRequests.urlGetSelectedMegatile,
			WebRequests.authenticatedParameters
		);
		yield return www;
		if (string.IsNullOrEmpty(www.error)) {
			try {
				selectedMegatile = JSONDecoder.Decode<SingleMegatile>(www.text).megatile;
			}
			catch (JsonException e) {
				Debug.Log(e);
				Debug.LogError(www.text);
			}
		}
		else {
			Debug.LogError(www.error);
		}
		//m_currentAction.OnSetMegatileSelection();
	}*/
	
	/// <summary>
	/// Process any input
	/// </summary>
	void Update ()
	{
		// early out if not processing input
		if (!isProcessingInput || !TerrainManager.isWorldLoaded) {
			return;
		}
		
		// move the camera as needed
		MoveCameraWithKeyboard();
		
		// enable/disable selection cursors as needed
		SetSelectionCursors();
		
		// handle zooming, hovering, and clicking
		HandleMouseInput();
	}
}
