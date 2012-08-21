#define FOR_JONGEE

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Base class for a player action
/// </summary>
public abstract class PlayerAction : MonoBehaviour
{
	public string notificationMessage = "MessageName";
	
	/// <summary>
	/// Action input mode.
	/// </summary>
	public enum InputMode { SelectMegatile, PaintResourceTiles, None }
	
	/// <summary>
	/// The input mode.
	/// </summary>
	public InputMode inputMode = InputMode.PaintResourceTiles;
	
#if FOR_JONGEE
	public bool		allowPerformance = false;
#endif
	
	/// <summary>
	/// Gets a value indicating whether this <see cref="PlayerAction"/> action is in progress.
	/// </summary>
	/// <value>
	/// <c>true</c> if action is in progress; otherwise, <c>false</c>.
	/// </value>
	public virtual bool isActionInProgress { get { return m_actionProgress < 1f; } }

	/// <summary>
	/// description of what the action does
	/// </summary>
	public string 		tooltip = "Do awesome stuff to things.";	
	/// <summary>
	/// the base cost to perform the action; modify in GatherData if it is variable
	/// </summary>
	public int 			cost = 25;	
	/// <summary>
	/// tooltip for the action
	/// </summary>
	public string 		currentToolTip { get { return string.Format("{0}: {1}", name, tooltip); } }
	/// <summary>
	/// Prefab for the special effect to spawn, if any
	/// </summary>
	public GameObject 	effectPrefab;
	/// <summary>
	/// the icon associated with this action
	/// </summary>
	public Texture2D 	icon;
	
	/// <summary>
	/// Resource tile modification wrapper.
	/// </summary>
	/// <remarks>
	/// Must specify modifications in a resource_tile field for Rails
	/// </remarks>
	public struct ResourceTileModification {
		public object resource_tile { get; set; }
	}
	
	/// <summary>
	/// The tile modifications wrapper.
	/// </summary>
	protected ResourceTileModification m_tileModifications;	
	/// <summary>
	/// Gets the styles.
	/// </summary>
	/// <value>
	/// The styles.
	/// </value>
	protected GUIStyles m_styles { get { return GameGUIManager.use.styles; } }
	/// <summary>
	/// The action progress.
	/// </summary>
	protected float m_actionProgress = 1f;
	
	void Start() {
		downloadTilesCoroutine =  new DownloadTilesInRegionCoroutine( this );
	}

	/// <summary>
	/// Raises the enable event.
	/// </summary>
	public virtual void OnEnable()
	{
		// early out if this is happening on the first frame
		if (Mathf.Approximately(Time.timeSinceLevelLoad, 0f)) {
			return;
		}
		// filter the selection to remove any inpermitted tiles
		//InputManager.use.FilterSelection();
		// update mask and selection highlighter	
		UpdateMask();
		UpdateSelectionHighlighter();
	}
	
	/// <summary>
	/// Display loading icon as needed
	/// </summary>
	public virtual void OnGUI()
	{
		/*
		if (isActionInProgress) {
			GameGUIManager.use.DisplayLoadingIcon(m_actionProgress);
		}*/
	}
	
	/// <summary>
	/// Updates the mask.
	/// </summary>
	public void UpdateMask()
	{		
		InputManager.use.ClearResourceTileSelection();
		Messenger.Broadcast(TerrainManager.kRefreshAll);
		DataMapController.GetProjector<ProjectorActions>("Action").SetActiveKey(this);
	}
	
	/// <summary>
	/// Updates the selection highlighter.
	/// </summary>
	public void UpdateSelectionHighlighter()
	{
		switch (inputMode) {
		case InputMode.None:
			//TerrainProjections.use.HideSelection();
			break;
		case InputMode.PaintResourceTiles:
			//TerrainProjections.use.ShowSelection();
			break;
		case InputMode.SelectMegatile:
			break;
		}
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
	public abstract bool IsPermittedOnResourceTile(ResourceTile tile);
	
	/// <summary>
	/// Perform the action.
	/// </summary>
	/// <param name='selection'>
	/// The selection to which the action should be applied.
	/// </param>
	public virtual void DoIt(object selection)
	{
		SubmitResourceTileSelection((ResourceTileSelection)selection);
	}
	
	public void DoAction(GUIButtonBase button) {
		ResourceTileSelection selection = InputManager.use.resourceTileSelection;
		if (selection.Count() <= 0) return;
		Messenger.Broadcast("Clear Current Tiles");
		DoIt(selection);
		InputManager.use.ClearResourceTileSelection();
	}
	
	/// <summary>
	/// Gets the do it WWW.
	/// </summary>
	/// <returns>
	/// The do it WWW.
	/// </returns>
	/// <param name='selection'>
	/// The selection.
	/// </param>
	virtual protected WWW GetDoItWWW(ResourceTileSelection selection)
	{
		return null;
	}
	
	/// <summary>
	/// Submits the resource tile selection.
	/// </summary>
	/// <remarks>
	/// NOTE: This may need a more sophisticated implementation if e.g., player action changes before server responds.
	/// </remarks>
	/// <param name='selection'>
	/// The resource tile selection.
	/// </param>
	protected void SubmitResourceTileSelection(ResourceTileSelection selection)
	{
		if (selection.Count() == 0) {
			GameGUIManager.use.SetErrorMessage("No tiles selected.");
			return;
		}
		if (isActionInProgress) {
			GameGUIManager.use.PlayInvalidActionSound();
			return;
		}
		
		//Check Resources
		/*
		if (!DoResourceCalculations(selection)) {
			return;
		}*/
		
		// if there is no API point, use the god mode implementation
		if (!hasServerAPIPoint) {
			StartCoroutine(Put(selection));
		}
		// otherwise use the api point on the server
		else {
			StartCoroutine(SubmitResourceTileSelectionToServerAPI(selection));
		}
	}
	
	/// <summary>
	/// Submits the resource tile selection to server API.
	/// </summary>
	/// <returns>
	/// The resource tile selection to server API.
	/// </returns>
	/// <param name='selection'>
	/// The resource tile selection.
	/// </param>
	protected virtual IEnumerator SubmitResourceTileSelectionToServerAPI(ResourceTileSelection selection)
	{
		WWW www = GetDoItWWW((ResourceTileSelection)selection);
		while (!www.isDone) {
			m_actionProgress = 0.5f*(www.uploadProgress+www.progress);
			yield return 0;
		}
		// see if there was an error
		if (!string.IsNullOrEmpty(www.error)) {
			Debug.LogError(www.error);
			yield return StartCoroutine(RefreshAfterActionFailed(www.error));
			m_actionProgress = 1f;
			yield break;
		}
		// deserialize the retrieved resource tile list
		ResourceTiles tiles;
		try {
			tiles = JSONDecoder.Decode<ResourceTiles>(www.text);
		}
		catch (JsonException) {
			Debug.LogError(www.text);
			StartCoroutine(RefreshAfterActionFailed("Error parsing server response."));
			m_actionProgress = 1f;
			yield break;
		}
		// conclude the action
		//ConcludeActionOnResourceTiles(tiles.ToArray());
		// refresh other data as needed
		yield return StartCoroutine(Refresh());
		//Update the world
		IEnumerator e = WebRequests.RefreshWorldData(new WebCoroutine());
		yield return e.Current;
		while(e.MoveNext()) yield return e.Current;
	}
	
	/// <summary>
	/// Flag specifying whether or not the action has a server API point.
	/// </summary>
	/// <remarks>
	/// When this is false, the action uses a god mode implementation. It will eventually be removed once every action has an API point.
	/// </remarks>
	public bool hasServerAPIPoint = false;
	
	/// <summary>
	/// Forces a put of resource tile modifications using god mode.
	/// </summary>
	/// <remarks>
	/// This is a temporary solution for implementing actions with no server API point, and can eventually be removed.
	/// </remarks>
	/// <param name='selection'>
	/// The selection.
	/// </param>
	protected virtual IEnumerator Put(ResourceTileSelection selection)
	{
		HTTP.Request request;
		m_actionProgress = 0f;
		string json = GetActionJson();
		Debug.Log(json);
		List<ResourceTile> tiles = new List<ResourceTile>();
		float div = 1f/selection.Count();
		for (int i=0; i<selection.Count(); ++i) {
			int id = selection[i];
			request = new HTTP.Request( "Put", WebRequests.GetURLToResourceTile(id) );
			request.SetText( json );
			request.AddParameters( WebRequests.authenticatedGodModeParameters );
			request.Send();
			
			while (!request.isDone) {
				//m_actionProgress = (i+www.uploadProgress)*div;
				yield return 0;
			}
			if( request.ProducedError ) {
				m_actionProgress = 1f;
				yield break;
			}
			try {
				tiles.Add(JSONDecoder.Decode<IndividualResourceTile>(request.response.Text).resource_tile);
			}
			catch (JsonException) {
				Debug.LogError(request.response.Text);
			}
		}
//		ConcludeActionOnResourceTiles(tiles.ToArray());
		//Update the world
		IEnumerator e = WebRequests.RefreshWorldData(new WebCoroutine());
		yield return e.Current;
		while(e.MoveNext()) yield return e.Current;
	}
	
	/// <summary>
	/// Sends out a message stating that an action was performed
	/// </summary>
	protected virtual void SendActionMessage(ResourceTile[] modifiedTiles)
	{
		Messenger<int>.Broadcast(string.Format("{0}_Count",notificationMessage,modifiedTiles.Length), modifiedTiles.Length);
	}
	
	protected virtual bool DoResourceCalculations(ResourceTileSelection selection)
	{
		//Make sure we have enough money to perform the action
		TerrainManager.Status status = TerrainManager.Status.Failed;
		ResourceTileLite[] tiles = TerrainManager.GetResourceTileCacheGroup(out status, selection.resource_tile_ids.ToArray());
		if (status == TerrainManager.Status.Failed) {
			Debug.LogError("Invalid tiles were part of the selection! o_o"); 
			return false;
		}
		int cost = CalculateCost(tiles);
		if (GameManager.economyController.IsBalanceAvailable(cost)) {
			//Spend the money
			GameManager.economyController.SpendMoney(cost);
		} else {
			GameGUIManager.use.SetErrorMessage("You do not have enough money to perform this action.");
			GameGUIManager.use.PlayInvalidActionSound();
			return false;
		}
		return true;
	}
	
	/// <summary>
	/// Calculates the cost of performing this action on the given tiles
	/// </summary>
	/// <returns>
	/// The cost.
	/// </returns>
	/// <param name='tiles'>
	/// Tiles.
	/// </param>
	protected virtual int CalculateCost(ResourceTileLite[] tiles)
	{
		//This is the default way. Override to do tweaked calculations
		return tiles.Length * cost;
	}
	
	
	/// <summary>
	/// Concludes the action on resource tiles.
	/// </summary>
	/// <param name='selection'>
	/// Tiles.
	/// </param>
	protected void ConcludeActionOnResourceTiles(ResourceTileSelection selection)
	{
		// set the action progress to complete
		m_actionProgress = 1f;
		// get the effect spawn locations
		Vector3[] spawnLocations = GetEffectSpawnLocationsFromResourceTiles(selection);
		// refresh the tiles using the new data
		//TerrainManager.use.SetTerrainFromResourceTiles(tiles, SetTerrainMode.Flush);
		
		
		
		// filter the current selection
		//InputManager.use.FilterSelection(); // TODO: this may need to be more sophisticated
		InputManager.use.ClearResourceTileSelection();
		// Spawn effects
		SpawnEffectsAtLocations(spawnLocations);
		
		//Send the Action done message
//		SendActionMessage(selection);
	}
	
	#region Utility Methods
	
		
	/// <summary>
	/// Gets JSON representation for the action.
	/// </summary>
	/// <remarks>
	/// Creates a JSON object for use with god mode.
	/// </remarks>
	/// <returns>
	/// The JSON representation for the action.
	/// </returns>
	protected string GetActionJson()
	{
		return JsonMapper.ToJson(m_tileModifications);
	}
	protected string GetActionJson(object mods)
	{
		return JsonMapper.ToJson(mods);
	}
	
	/// <summary>
	/// Gets the effect spawn locations from resource tiles.
	/// </summary>
	/// <returns>
	/// The effect spawn locations from resource tiles.
	/// </returns>
	/// <param name='selection'>
	/// Tiles.
	/// </param>
	protected Vector3[] GetEffectSpawnLocationsFromResourceTiles(ResourceTileSelection selection)
	{
		System.Collections.Generic.List<Vector3> spawnLocations = new System.Collections.Generic.List<Vector3>();
		TerrainManager.Status status = TerrainManager.Status.Failed;
		foreach (ResourceTileLite tile in TerrainManager.GetResourceTileCacheGroup(out status, selection.resource_tile_ids.ToArray())) {
			spawnLocations.Add(tile.GetCenterPoint());
		}
		return spawnLocations.ToArray();
	}
	
	/// <summary>
	/// Spawns the effects at locations.
	/// </summary>
	/// <param name='spawnLocations'>
	/// Spawn locations.
	/// </param>
	protected void SpawnEffectsAtLocations(Vector3[] spawnLocations)
	{
		if (effectPrefab == null) {
			return;
		}
		foreach (Vector3 p in spawnLocations) {
			Instantiate(effectPrefab, p, Quaternion.AngleAxis(Random.Range(-180, 180f), Vector3.up));
		}
	}
	
	/// <summary>
	/// Refreshs after the action failed.
	/// </summary>
	/// <param name='errorMessage'>
	/// Error message.
	/// </param>
	protected IEnumerator RefreshAfterActionFailed(string errorMessage)
	{
		// set the error message if needed
		if (!string.IsNullOrEmpty(errorMessage)) {
			GameGUIManager.use.SetErrorMessage(errorMessage);
		}
		m_actionProgress = 1f;
		yield return StartCoroutine(Refresh());
	}
	
	protected DownloadTilesInRegionCoroutine	downloadTilesCoroutine;
	protected virtual IEnumerator Refresh()
	{
		// update player data to reflect e.g., change in money
		yield return StartCoroutine(WebRequests.DownloadPlayerData(Player.current));
		// update selected tile if in Megatile selection mode
//		if (inputMode == InputMode.SelectMegatile) {
//			yield return StartCoroutine(InputManager.use.RefreshSelectedTileData());
//		}
//		// otherwise update visible region
//		else {
			downloadTilesCoroutine.Start( this, CameraRig.use.GetVisibleRegion(), true );
			while( !downloadTilesCoroutine.IsDone ) {
				yield return 0;
			}
//		}
	}
	
	#endregion
	
	#region GUI Handling
	
	/// <summary>
	/// The height of the controls area.
	/// </summary>
	public float controlsAreaHeight = 100f;
	/// <summary>
	/// The clear resource tile selection icon.
	/// </summary>
	private Texture2D m_clearResourceTileSelectionIcon {
		get { return GameGUIManager.use.clearResourceTileSelectionIcon; }
	}
	/// <summary>
	/// Gets the pan selection icon in the on state.
	/// </summary>
	/// <value>
	/// The pan selection icon in the on state.
	/// </value>
	private Texture2D m_panSelectionIconOn {
		get { return GameGUIManager.use.panSelectionIconOn; }
	}
	/// <summary>
	/// Gets the pan selection icon in the off state.
	/// </summary>
	/// <value>
	/// The pan selection icon in the off state.
	/// </value>
	private Texture2D m_panSelectionIconOff {
		get { return GameGUIManager.use.panSelectionIconOff; }
	}
	/// <summary>
	/// The select resource tile icon on.
	/// </summary>
	private Texture2D m_selectResourceTileIconOn {
		get { return GameGUIManager.use.selectResourceTileIconOn; }
	}
	/// <summary>
	/// The select resource tile icon off.
	/// </summary>
	private Texture2D m_selectResourceTileIconOff {
		get { return GameGUIManager.use.selectResourceTileIconOff; }
	}
	/// <summary>
	/// The deselect resource tile icon on.
	/// </summary>
	private Texture2D m_deselectResourceTileIconOn {
		get { return GameGUIManager.use.deselectResourceTileIconOn; }
	}
	/// <summary>
	/// The deselect resource tile icon off.
	/// </summary>
	private Texture2D m_deselectResourceTileIconOff {
		get { return GameGUIManager.use.deselectResourceTileIconOff; }
	}
	/// <summary>
	/// The submit resource tile selection icon.
	/// </summary>
	private Texture2D m_submitResourceTileSelectionIcon {
		get { return GameGUIManager.use.submitResourceTileSelectionIcon; }
	}
	
	/// <summary>
	/// Gets the size of the square button.
	/// </summary>
	/// <value>
	/// The size of the square button.
	/// </value>
	protected float m_squareButtonSize { get { return GameGUIManager.cursorSize; } }
	/// <summary>
	/// Gets the footnote text.
	/// </summary>
	/// <value>
	/// The footnote text.
	/// </value>
	protected GUIStyle m_footnoteText { get { return m_styles.smallTextMedium; } }
	/// <summary>
	/// Gets the main text.
	/// </summary>
	/// <value>
	/// The main text.
	/// </value>
	protected GUIStyle m_mainText { get { return m_styles.mediumTextLight; } }
	/// <summary>
	/// Gets the main text alternate.
	/// </summary>
	/// <value>
	/// The main text alternate.
	/// </value>
	protected GUIStyle m_mainTextAlt { get { return m_styles.mediumTextHighlighted; } }
	/// <summary>
	/// Gets the minor text.
	/// </summary>
	/// <value>
	/// The minor text.
	/// </value>
	protected GUIStyle m_minorText { get { return m_styles.smallTextLight; } }
	
	/// <summary>
	/// Gets the available button style.
	/// </summary>
	/// <value>
	/// The available button style.
	/// </value>
	protected GUIStyle m_buttonFocused { get { return m_styles.smallButtonFocused; } }
	/// <summary>
	/// Gets the button style.
	/// </summary>
	/// <value>
	/// The button style.
	/// </value>
	protected GUIStyle m_button { get { return m_styles.smallButton; } }
	/// <summary>
	/// Gets the disabled button.
	/// </summary>
	/// <value>
	/// The disabled button.
	/// </value>
	protected GUIStyle m_buttonDisabled { get { return m_styles.smallButtonDisabled; } }
	/// <summary>
	/// Gets the height of the button.
	/// </summary>
	/// <value>
	/// The height of the button.
	/// </value>
	protected float m_buttonHeight {
		get {
			return m_button.padding.top + GUIHelpers.GetFontSizeFromName(m_button.font) + m_button.padding.bottom;
		}
	}
	
	/// <summary>
	/// long description to appear under the question in the dialog
	/// </summary>
	public string dialogLongDescription = "Doing this thing will make totally awesome stuff happen all over.";
	/// <summary>
	/// Labels to appear on the submission button.
	/// </summary>
	public GUIContent dialogConfirmButtonDo = new GUIContent("DO IT", "Make it so.");
	
	/// <summary>
	/// Begins the GUI area.
	/// </summary>
	protected void BeginGUIArea()
	{
		BottomTrayGUI.use.SetActionControlsRectHeight(controlsAreaHeight);
		GUILayout.BeginArea(BottomTrayGUI.use.rectActionControls, m_styles.roundDarkBox);
		
		// header
		GUILayout.Label(name.ToUpper(), m_styles.largeTextLight);
		m_styles.DrawLine(GUIStyles.LineDirection.Horizontal, GUIStyles.LineColor.Highlighted);
		GUILayout.Label(dialogLongDescription, m_minorText);
	}
	
	/// <summary>
	/// Ends the GUI area.
	/// </summary>
	protected void EndGUIArea()
	{
		GUILayout.EndArea();
	}
	
	/// <summary>
	/// Configure the dialog
	/// </summary>
	public void DisplayControls()
	{
		BeginGUIArea(); {
			DisplayControlsContents();
		} EndGUIArea();
	}
	
	/// <summary>
	/// Displays the controls contents.
	/// </summary>
	protected virtual void DisplayControlsContents()
	{
		if (inputMode == PlayerAction.InputMode.PaintResourceTiles) {
			DisplayPaintSelectionControlGroup();
		}
		m_styles.DrawLine(GUIStyles.LineDirection.Horizontal, GUIStyles.LineColor.Highlighted);
		DoItButton();
	}
	
	/// <summary>
	/// Displays the paint selection control group.
	/// </summary>
	protected void DisplayPaintSelectionControlGroup()
	{
		GUILayout.BeginHorizontal();
		{
			GUILayout.BeginVertical(); {
				GUILayout.Label("SELECT TILES:", m_mainText);
				GUILayout.Label(string.Format("({0} acres selected)", ResourceTileSelection.GetCurrent().Count()), m_mainTextAlt);
			} GUILayout.EndVertical();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button(
					new GUIContent(
						InputManager.paintSelectionInputMode==InputManager.PaintSelectionInputMode.Pan?m_panSelectionIconOn:m_panSelectionIconOff,
						"Pan camera."
					), InputManager.paintSelectionInputMode == InputManager.PaintSelectionInputMode.Pan?m_buttonFocused:m_button,
					GUILayout.Height(m_squareButtonSize), GUILayout.Width(m_squareButtonSize)
				)
			){
				InputManager.paintSelectionInputMode = InputManager.PaintSelectionInputMode.Pan;
			}
			if (GUILayout.Button(
					new GUIContent(
						InputManager.paintSelectionInputMode==InputManager.PaintSelectionInputMode.Select?m_selectResourceTileIconOn:m_selectResourceTileIconOff,
						"Select resource tiles."
					), InputManager.paintSelectionInputMode==InputManager.PaintSelectionInputMode.Select?m_buttonFocused:m_button,
					GUILayout.Height(m_squareButtonSize), GUILayout.Width(m_squareButtonSize)
				)
			) {
				InputManager.paintSelectionInputMode = InputManager.PaintSelectionInputMode.Select;
			}
			if (GUILayout.Button(
					new GUIContent(
						InputManager.paintSelectionInputMode==InputManager.PaintSelectionInputMode.Deselect?m_deselectResourceTileIconOn:m_deselectResourceTileIconOff,
						"Deselect resource tiles."
					), InputManager.paintSelectionInputMode==InputManager.PaintSelectionInputMode.Deselect?m_buttonFocused:m_button,
					GUILayout.Height(m_squareButtonSize), GUILayout.Width(m_squareButtonSize)
				)
			) {
				InputManager.paintSelectionInputMode = InputManager.PaintSelectionInputMode.Deselect;
			}
			if (GUILayout.Button(
					new GUIContent(m_clearResourceTileSelectionIcon, "Clear resource tile selection."),
					m_button,
					GUILayout.Height(m_squareButtonSize), GUILayout.Width(m_squareButtonSize)
				)
			) {
				InputManager.use.ClearResourceTileSelection();
			}
		}
		GUILayout.EndHorizontal();
	}
	
	/// <summary>
	/// The button to perform the action.
	/// </summary>
	protected void DoItButton()
	{	
		bool isButtonDisabled = isActionInProgress;
		object selection = ResourceTileSelection.GetCurrent();
		switch (inputMode) {
		case InputMode.PaintResourceTiles:
			isButtonDisabled |= ((ResourceTileSelection)selection).Count() == 0;
			break;
		case InputMode.SelectMegatile:
			//isButtonDisabled |= InputManager.use.selectedMegatile == null;
			//selection = InputManager.use.selectedMegatile;
			break;
		}
		if (GUILayout.Button(dialogConfirmButtonDo, isButtonDisabled?m_buttonDisabled:m_button)) {
			DoIt(selection);
		}
	}
	
	#endregion
}