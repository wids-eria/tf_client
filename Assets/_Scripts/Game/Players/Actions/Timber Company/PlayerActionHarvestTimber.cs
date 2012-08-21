using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// An action for enacting a harvesting strategy on a section of land.
/// </summary>
public class PlayerActionHarvestTimber : PlayerAction
{
	/// <summary>
	/// The time to wait for refreshing data on the current select.
	/// </summary>
	public float timeToWaitForRefresh = 0.5f;
	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="PlayerActionHarvestTimber"/> is dirty.
	/// </summary>
	/// <value>
	/// <c>true</c> if is dirty; otherwise, <c>false</c>.
	/// </value>
	public bool isDirty { get; private set; }
	
	/// <summary>
	/// The q ratio.
	/// </summary>
	protected float m_qRatio {
		get { return m_qRatioBackingField; }
		set {
			if (value == m_qRatioBackingField) {
				return;
			}
			m_qRatioBackingField = value;
			m_qRatioCurve = Harvest.GetPartialSelectionCurve(m_qRatio, m_basalArea);
		}
	}
	/// <summary>
	/// The basal area.
	/// </summary>
	protected float m_basalArea {
		get { return m_basalAreaBackingField; }
		set {
			if (value == m_basalAreaBackingField) {
				return;
			}
			m_basalAreaBackingField = value;
			m_qRatioCurve = Harvest.GetPartialSelectionCurve(m_qRatio, m_basalArea);
		}
	}
	
	/// <summary>
	/// The type of the cut.
	/// </summary>
	protected CutType m_cutType;
	/// <summary>
	/// The diameter limit.
	/// </summary>
	protected int m_diameterLimit {
		get { return m_diameterLimitBackingField; }
		set {
			m_diameterLimitBackingField = (m_diameterLimitDirection==DiameterLimitCutDirection.Less)?
				Mathf.Max(value, ResourceTile.treeSizeClassInterval*2): // must be at least second size class
				Mathf.Min(value, ResourceTile.treeSizeClassInterval*(ResourceTile.treeSizeClassCount-1)); // cannot exceed penultimate size class
		}
	}
	/// <summary>
	/// The diameter limit backing field.
	/// </summary>
	private int m_diameterLimitBackingField = ResourceTile.treeSizeClassInterval;
	/// <summary>
	/// The diameter limit direction.
	/// </summary>
	protected DiameterLimitCutDirection m_diameterLimitDirection {
		get { return m_diameterLimitDirectionBackingField; }
		set {
			// ensure diameter limit value is updated
			if (value != m_diameterLimitDirectionBackingField) {
				m_diameterLimit = m_diameterLimit;
			}
			m_diameterLimitDirectionBackingField = value;
		}
	}
	/// <summary>
	/// The diameter limit direction backing field.
	/// </summary>
	private DiameterLimitCutDirection m_diameterLimitDirectionBackingField;
	
	protected int[] m_actualFrequencyDistributionOfSelectedTiles = new int[ResourceTile.treeSizeClassCount];
	/// <summary>
	/// The known frequency distribution of selected tiles.
	/// </summary>
	protected int[] m_knownFrequencyDistributionOfSelectedTiles = new int[ResourceTile.treeSizeClassCount];
	/// <summary>
	/// The desired tree values.
	/// </summary>
	/// <remarks>
	/// This is only used in god mode, and can be discarded when it is no longer used.
	/// </remarks>
	protected int[] m_desiredTreeValues = new int[ResourceTile.treeSizeClassCount];
	
	/// <summary>
	/// The q ratio curve.
	/// </summary>
	private float[] m_qRatioCurve = new float[ResourceTile.treeSizeClassCount];
	/// <summary>
	/// The download timer.
	/// </summary>
	private float m_downloadTimer;
	/// <summary>
	/// The refresh download progress.
	/// </summary>
	private float m_refreshDownloadProgress;
	/// <summary>
	/// The q ratio backing field.
	/// </summary>
	private float m_qRatioBackingField = 1.5f;
	/// <summary>
	/// The basal area backing field.
	/// </summary>
	private float m_basalAreaBackingField = 100f;
	
	
	/// <summary>
	/// Configure modifications then call base implementation.
	/// </summary>
	/// <param name='selection'>
	/// The selection to which the action should be applied.
	/// </param>
	public override void DoIt(object selection)
	{
		// don't want descended schedule class to configure these properties
		if (this.GetType() == typeof(PlayerActionHarvestTimber)) {
			int[] treesToCut;
			ComputeTreesToBeCut(out treesToCut, out m_desiredTreeValues);
		}
		base.DoIt(selection);
	}
	
	protected override void SendActionMessage(ResourceTile[] modifiedTiles)
	{
		Messenger<CutType>.Broadcast(notificationMessage, m_cutType);
		base.SendActionMessage(modifiedTiles);
	}
	
	
	/// <summary>
	/// Listen for input messages.
	/// </summary>
	protected virtual void Awake()
	{
		MessengerAM.Listen(MessengerAM.listenTypeInput, this);
	}
	
	
	/// <summary>
	/// Initialize q ratio curve.
	/// </summary>
	private void Start()
	{
		InitializeQRatioCurve();
		ClearFrequencyDistribution();
	}
	
	/// <summary>
	/// Raises the enable event.
	/// </summary>
	public override void OnEnable()
	{
		base.OnEnable();
		InitializeQRatioCurve();
		ClearFrequencyDistribution();
	}
	
	/// <summary>
	/// Clears the frequency distribution.
	/// </summary>
	private void ClearFrequencyDistribution()
	{
		m_actualFrequencyDistributionOfSelectedTiles = new int[ResourceTile.treeSizeClassCount];
	}
	
	/// <summary>
	/// Initializes the Q ratio curve.
	/// </summary>
	private void InitializeQRatioCurve()
	{
		m_qRatioCurve = Harvest.GetPartialSelectionCurve(m_qRatio, m_basalArea);
	}
	
	/*
	/// <summary>
	/// Reset tile download timer when selection changes.
	/// </summary>
	/// <param name='msg'>
	/// Message.
	/// </param>
	public void _ResourceTileSelectionChanged(MessageResourceTileSelectionChanged msg)
	{
		m_downloadTimer = timeToWaitForRefresh;
		isDirty = true;
		m_refreshDownloadProgress = 0f;
		if (ResourceTileSelection.GetCurrent().Count() == 0) {
			ClearFrequencyDistribution();
		}
	}*/
	
	/// <summary>
	/// Refresh the info for the selection as needed.
	/// </summary>
	protected virtual void Update()
	{
		m_downloadTimer = Mathf.Max(0f, m_downloadTimer-Time.deltaTime);
		if (InputManager.use.isSelectionChanging) {
			//_ResourceTileSelectionChanged(null);
		}
		if (!isDirty) {
			return;
		}
		if (m_downloadTimer <= 0f) {
			StartCoroutine(RefreshDataForSelection(ResourceTileSelection.GetCurrent()));
		}
	}
	
	/// <summary>
	/// Refreshs the data for selection.
	/// </summary>
	/// <param name='selection'>
	/// The selection.
	/// </param>
	private IEnumerator RefreshDataForSelection(ResourceTileSelection selection)
	{
		// TODO: this is temporary. it should actually get back survey data.
		isDirty = false; // set this now so multiple downloads dont happen
		WWW www;
		int[] frequencyDistribution = new int[ResourceTile.treeSizeClassCount];
		int[] knownFreqDistribution = new int[ResourceTile.treeSizeClassCount];
		m_refreshDownloadProgress = 0f;
		float div = 1f/selection.Count();
		for (int i=0; i<selection.Count(); ++i) {
			int id = selection[i];
			www = WWWX.Get(WebRequests.GetURLToResourceTile(id), WebRequests.authenticatedGodModeParameters);
			while (!www.isDone) {
				m_refreshDownloadProgress = (i+www.progress)*div;
				yield return 0;
			}
			if (!string.IsNullOrEmpty(www.error)) {
				Debug.LogError(www.error);
				continue;
			}
			ResourceTile tile;
			try {
				tile = JSONDecoder.Decode<IndividualResourceTile>(www.text).resource_tile;
			}
			catch (JsonException e) {
				Debug.LogError(e);
				continue;
			}
			for (int j=0; j<ResourceTile.treeSizeClassCount; ++j) {
				if (tile.bought_by_timber_company) {
					knownFreqDistribution[j] += tile.treeDistribution[j];
				}
				frequencyDistribution[j] += tile.treeDistribution[j];
			}
		}
		m_knownFrequencyDistributionOfSelectedTiles = knownFreqDistribution;
		m_actualFrequencyDistributionOfSelectedTiles = frequencyDistribution;
		m_refreshDownloadProgress = 1f;
	}
	
	
	/// <summary>
	/// HACK
	/// </summary>
	/// <returns>
	/// The selection.
	/// </returns>
	/// <param name='selection'>
	/// Selection.
	/// </param>
	protected ResourceTileSelection FilterSelection(ResourceTileSelection selection) {
		//Filter out tiles we cannot use
		TerrainManager.Status status = TerrainManager.Status.Succeeded;
		ResourceTileLite[] tiles = TerrainManager.GetResourceTileCacheGroup(out status,selection.resource_tile_ids.ToArray());
		ResourceTileSelection newSelection = new ResourceTileSelection(new List<int>());
		foreach(ResourceTileLite tile in tiles) {
			if(tile.IsActionPermitted(this)) {
				newSelection.Add(tile.id);	
			}
		}
		return newSelection;
	}
	
	
	/// <summary>
	/// Submits the resource tile selection to server API.
	/// </summary>
	/// <remarks>
	/// This differs from the base implementation because the API returns timber values, not modified resource tiles
	/// </remarks>
	/// <returns>
	/// The resource tile selection to server API.
	/// </returns>
	/// <param name='selection'>
	/// The resource tile selection.
	/// </param>
	protected override IEnumerator SubmitResourceTileSelectionToServerAPI(ResourceTileSelection selection)
	{
		//HACK -- HACK -- HACK -- HACK
		/*selection = FilterSelection(selection);
		if (selection.Count() <= 0) {
			Debug.LogError("No tiles in the selection can have an action performed on them.");
			yield break;
		}*/
		//END HACK
		
		
		// initialize a clearcut
		Harvest harvest = new Harvest(selection.ToArray(), 1);
		// configure harvest to perform
		if (m_cutType == CutType.DiameterLimitCut) {
			harvest = new Harvest(
				selection.ToArray(), 1,
				m_diameterLimit, m_diameterLimitDirection
			);
		} else if (m_cutType == CutType.QRatioCut) {
			harvest = new Harvest(
				selection.ToArray(), 1,
				m_qRatio, m_basalArea
			);
		}
		// perform harvest (50% of action progress)
		StartCoroutine(harvest.DoIt());
		while (harvest.progress < 1f) {
			m_actionProgress = 0.5f*harvest.progress;
			if (!string.IsNullOrEmpty(harvest.doItError)) {
				Debug.LogError(harvest.doItError);
				StartCoroutine(RefreshAfterActionFailed(harvest.doItError));
				yield break;
			}
			yield return 0;
		}
		// get updated tiles (100% of action progress)
		/*List<ResourceTile> tiles = new List<ResourceTile>();
		float div = 1f/selection.Count();
		HTTP.Request request;
		for (int i=0; i<selection.Count(); ++i) {
			int id = selection[i];
			request = new HTTP.Request( "Get", WebRequests.GetURLToResourceTile(id) );
			request.AddParameters( WebRequests.authenticatedParameters );
			request.Send();
			
			while (!request.isDone) {
				//m_actionProgress = 0.5f*(1f+(i+www.progress)*div);
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
				//Debug.LogError(www.text);
			}
		}*/
		// conclude the action
		ConcludeActionOnResourceTiles(selection);
		GameManager.use.worldDataIsValid = false;
		TerrainManager.use.RefreshFromIds(selection.resource_tile_ids.ToArray());
		IEnumerator e = WebRequests.DownloadPlayerData(Player.current);
		while (e.MoveNext()) {
			yield return 0;
		}
	}
	
	/// <summary>
	/// Computes the trees to be cut.
	/// </summary>
	/// </param>
	/// <param name='treesToCut'>
	/// Trees to cut.
	/// </param>
	/// <param name='treesToKeep'>
	/// Trees to keep.
	/// </param>
	private void ComputeTreesToBeCut(out int[] treesToCut, out int[] treesToKeep)
	{
		switch (m_cutType) {
		case CutType.DiameterLimitCut:
			Harvest.ComputeTreesToBeCut(m_actualFrequencyDistributionOfSelectedTiles, m_diameterLimit, m_diameterLimitDirection, out treesToCut, out treesToKeep);
			break;
		case CutType.QRatioCut:
			Harvest.ComputeTreesToBeCut(m_actualFrequencyDistributionOfSelectedTiles, m_qRatioCurve, out treesToCut, out treesToKeep);
			break;
		default: // clearcut
			treesToCut = m_actualFrequencyDistributionOfSelectedTiles;
			treesToKeep = new int[m_actualFrequencyDistributionOfSelectedTiles.Length];
			break;
		}
	}
	/// <summary>
	/// Computes the trees to be cut (Overload function).
	/// </summary>
	/// <param name='frequencyDistribution'>
	/// Frequency distribution.
	/// </param>
	/// <param name='treesToCut'>
	/// Trees to cut.
	/// </param>
	/// <param name='treesToKeep'>
	/// Trees to keep.
	/// </param>
	private void ComputeTreesToBeCut(int[] frequencyDistribution, out int[] treesToCut, out int[] treesToKeep)
	{
		switch (m_cutType) {
		case CutType.DiameterLimitCut:
			Harvest.ComputeTreesToBeCut(frequencyDistribution, m_diameterLimit, m_diameterLimitDirection, out treesToCut, out treesToKeep);
			break;
		case CutType.QRatioCut:
			Harvest.ComputeTreesToBeCut(frequencyDistribution, m_qRatioCurve, out treesToCut, out treesToKeep);
			break;
		default: // clearcut
			treesToCut = frequencyDistribution;
			treesToKeep = new int[frequencyDistribution.Length];
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
	public override bool IsPermittedOnResourceTile(ResourceTile tile)
	{
		return !(
			tile.isHarvestArea ||
			tile.zoneType == ZoneType.Protected ||
			tile.baseCoverType == BaseCoverType.Excluded ||
			tile.baseCoverType == BaseCoverType.Unknown ||
			tile.baseCoverType == BaseCoverType.Water ||
			// TODO: server throws a 500 if land cover class code is not one of the following
			(
				tile.landCoverType != LandCoverType.Coniferous &&
				tile.landCoverType != LandCoverType.Deciduous &&
				tile.landCoverType != LandCoverType.Mixed &&
				tile.landCoverType != LandCoverType.ForestedWetland
			)
		);
	}
	
	#region GUI Handling
	
	/// <summary>
	/// Gets the width of the slider.
	/// </summary>
	/// <value>
	/// The width of the slider.
	/// </value>
	protected float m_sliderWidth {
		//get { return m_sliderWidthFactor*BottomTrayGUI.use.rectActionControls.width; }
		get { return m_sliderWidthFactor*300f; }
	}
	/// <summary>
	/// Gets the cost scalar.
	/// </summary>
	/// <value>
	/// The cost scalar.
	/// </value>
	protected virtual int m_costScalar {
		get { return ResourceTileSelection.GetCurrent().Count(); }
	}
	
	/// <summary>
	/// The basal area slider snap interval.
	/// </summary>
	[SerializeField]
	private float m_basalAreaSliderSnap = 5f;
	/// <summary>
	/// The q ratio slider snap.
	/// </summary>
	[SerializeField]
	private float m_qRatioSliderSnap = 0.01f;
	/// <summary>
	/// The slider width factor.
	/// </summary>
	[SerializeField]
	private float m_sliderWidthFactor = 0.35f;
	/// <summary>
	/// The height of the histogram.
	/// </summary>
	protected static readonly float m_histogramHeight = 100f;
	/// <summary>
	/// The histogram rect.
	/// </summary>
	protected static Rect m_histogramRect;
	
	protected static Rect m_needSurveyMessageRect;
	
	/// <summary>
	/// Displays the controls contents.
	/// </summary>
	protected override void DisplayControlsContents()
	{
		DisplayCutSelector();
		DisplayOperationCost();
		m_styles.DrawLine(
			GUIStyles.LineDirection.Horizontal,
			GUIStyles.LineColor.Medium
		);
		
		
		
//		m_needSurveyMessageRect = m_histogramRect;
//		m_needSurveyMessageRect.y = m_histogramRect.yMax/2.0f;
//		m_needSurveyMessageRect.height = m_histogramRect.height/2.0f;
//		bool needMoreSurvey = false;
//		for(int num=0; num < ResourceTile.treeSizeClassCount; num++){
//			if(m_actualFrequencyDistributionOfSelectedTiles[num]!= m_knownFrequencyDistributionOfSelectedTiles[num]){
//					needMoreSurvey = true;
//					break;
//			}
//		}
//		if(needMoreSurvey){
//			GUILayout.BeginArea(m_needSurveyMessageRect, GameGUIManager.use.styleBox);
//			{
//				GUILayout.TextArea("Your selected land are not fully surveyed!", m_styles.largeTextHighlighted);	
//			}
//			GUILayout.EndArea();
//		}
		
		// base implementation
		base.DisplayControlsContents();
	}
	
	private TerrainManager.Status stat;
	
	/// <summary>
	/// Displays the cut selector.
	/// </summary>
	protected void DisplayCutSelector()
	{
		// determine trees to be included in cut
		int[] treesToCut = new int[m_actualFrequencyDistributionOfSelectedTiles.Length];
		int[] treesToKeep = new int[m_actualFrequencyDistributionOfSelectedTiles.Length];
		ComputeTreesToBeCut(m_knownFrequencyDistributionOfSelectedTiles, out treesToCut, out treesToKeep);
		// histogram
		GUILayout.BeginHorizontal(); {
			GUILayout.Label("Forest Profile:", m_mainText);
			GUILayout.FlexibleSpace();
			GUILayout.Label(string.Format("({0} Acres)", ResourceTileSelection.GetCurrent().Count()), m_mainTextAlt);
		} GUILayout.EndHorizontal();

		//display the graph based on surveyed area
		if(ResourceTileSelection.GetCurrent().Count() <= 0) 
			{
			GUILayout.TextArea("\n\n", m_styles.largeTextDark);
			GUILayout.BeginHorizontal();
			{
				GUILayout.FlexibleSpace();
				GUILayout.TextArea("Please select tile(s) to get your graph. ", m_styles.largeTextHighlighted);
				GUILayout.FlexibleSpace();
			}
			GUILayout.EndHorizontal();
			GUILayout.TextArea("\n\n", m_styles.largeTextDark);
		}
		else{
			DisplayHistogram(treesToKeep, treesToCut, m_refreshDownloadProgress);
		}
		
		//check for surveyed land status
		bool needMoreSurvey = false;
		for(int num=0; num < ResourceTile.treeSizeClassCount; num++){
			if(m_actualFrequencyDistributionOfSelectedTiles[num]!= m_knownFrequencyDistributionOfSelectedTiles[num]){
					needMoreSurvey = true;
					break;
			}
		}
		if(needMoreSurvey){
			GUILayout.TextArea("Your selected land are not fully surveyed!",m_styles.mediumTextHighlighted);
		}
		
		m_styles.DrawLine(
			GUIStyles.LineDirection.Horizontal,
			GUIStyles.LineColor.Medium
		);
		
		// cut selector
		GUILayout.BeginHorizontal(); {
			GUILayout.Label("Configure Cut:", m_mainText);
			m_cutType = (CutType)GUILayout.SelectionGrid(
				(int)m_cutType,
				StringHelpers.GetEnumNamesAsCamelCase(typeof(CutType)),
				1,
				m_button
			);
		} GUILayout.EndHorizontal();
		// parameters to tune
		switch (m_cutType) {
		case CutType.DiameterLimitCut:
			GUILayout.BeginHorizontal(); {
				GUILayout.Label("Target Diameter:", m_mainText);
				GUILayout.Label(string.Format("{0}\"", m_diameterLimit), m_mainTextAlt);
				GUILayout.FlexibleSpace();
				m_diameterLimit = ResourceTile.treeSizeClassInterval*(int)GUILayout.HorizontalSlider(
					(1f/ResourceTile.treeSizeClassInterval)*m_diameterLimit,
					1f, ResourceTile.treeSizeClassCount,
					GUILayout.Width(m_sliderWidth)
				);
			} GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(); {
				GUILayout.Label("Direction:", m_mainText);
				GUILayout.Label(m_diameterLimitDirection.ToString(), m_mainTextAlt);
				GUILayout.FlexibleSpace();
				m_diameterLimitDirection = (DiameterLimitCutDirection)Mathf.Sign(GUILayout.HorizontalSlider((float)m_diameterLimitDirection, -1f, 1f, GUILayout.Width(m_sliderWidth)));
			} GUILayout.EndHorizontal();
			break;
		case CutType.QRatioCut:
			GUILayout.BeginHorizontal(); {
				GUILayout.Label("Target Basal Area:", m_mainText);
				GUILayout.Label(string.Format("{0:0} sq ft/acre", m_basalArea), m_mainTextAlt);
				GUILayout.FlexibleSpace();
				m_basalArea = GUILayout.HorizontalSlider(m_basalArea, 0f, 200f, GUILayout.Width(m_sliderWidth));
				m_basalArea -= m_basalArea%m_basalAreaSliderSnap;
			} GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(); {
				GUILayout.Label("Q-Ratio:", m_mainText);
				GUILayout.Label(string.Format("{0:0.00}", m_qRatio), m_mainTextAlt);
				GUILayout.FlexibleSpace();
				m_qRatio = GUILayout.HorizontalSlider(m_qRatio, 1f, 2f, GUILayout.Width(m_sliderWidth));
				m_qRatio -= m_qRatio%m_qRatioSliderSnap;
			} GUILayout.EndHorizontal();
			break;
		}
		GUILayout.FlexibleSpace();
		
	}
	
	/// <summary>
	/// Displays the operation cost.
	/// </summary>
	protected void DisplayOperationCost()
	{
		GUILayout.BeginHorizontal(); {
			GUILayout.Label("Total Operation Cost:", m_mainText);
			GUILayout.FlexibleSpace();
			GUILayout.Label(string.Format("{0:c}", cost*m_costScalar), m_mainTextAlt); // TODO: this shit needs to be fo' real, yo
		} GUILayout.EndHorizontal();
	}
	
	/// <summary>
	/// Displays the histogram.
	/// </summary>
	/// <remarks>
	/// Presently, implicitly assumes that both arrays have length of ResourceTile.treeSizeClassCount and that all values are >= 0. KNOW YOUR DATA BRO.
	/// </remarks>
	/// <param name='treesToKeep'>
	/// Trees to keep.
	/// </param>
	/// <param name='treesToCut'>
	/// Trees to cut.
	/// </param>
	/// <param name='progress'>
	/// Normalized progress.
	/// </param>
	public static void DisplayHistogram(int[] treesToKeep, int[] treesToCut, float progress)
	{
		// create space in the layout and store it
		GUILayoutUtility.GetRect(
			//BottomTrayGUI.use.rectActionControls.width*0.5f, // min width
			//BottomTrayGUI.use.rectActionControls.width, // max width
			150f, 300f,
			m_histogramHeight, // min height
			m_histogramHeight // max height
		);
		
		if (Event.current.type == EventType.Repaint) {
			m_histogramRect = GUILayoutUtility.GetLastRect();
		}
		
		// determine max value for histogram
		int maxTreeCountPerClass = 0;
		for (int i=0; i<ResourceTile.treeSizeClassCount; ++i) {
			maxTreeCountPerClass = Mathf.Max(maxTreeCountPerClass, treesToKeep[i]+treesToCut[i]);
		}
		// determine colors
		Color opaque = GUI.color; // for trees to be cut
		Color fade = new Color(opaque.r, opaque.g, opaque.b, opaque.a * 0.5f); // for trees to keep
		// an allocation
		Rect r = new Rect();
		float barWidth = m_histogramRect.width/ResourceTile.treeSizeClassCount;
		GUI.BeginGroup(m_histogramRect); {
			// display values
			int numLines = 5;
			float div = 1f/numLines;
			bool fill = true;
			GUIStyle labelStyle = GameGUIManager.use.styles.smallTextMedium;
			for (int i=0; i<numLines; ++i) {
				// number
				r.width = m_histogramRect.width; r.x = 0f;
				r.height = m_histogramRect.height*div; r.y = r.height*i;
				GUI.Label(r, string.Format("{0:0,0}", maxTreeCountPerClass*div*(numLines-i)), labelStyle);
				// line
				if (fill) {
					GUI.color = fade;
					GUI.DrawTexture(r, GameGUIManager.use.styles.histogramBar.active.background, ScaleMode.StretchToFill);
					GUI.color = opaque;
				}
				fill = !fill;
			}
			// draw each size class bar
			r.width = barWidth;
			for (int i=0; i<ResourceTile.treeSizeClassCount; ++i) {
				// trees to keep
				r.x = i*barWidth;
				r.height = (treesToKeep[i]/(float)maxTreeCountPerClass)*m_histogramRect.height;
				r.y = m_histogramRect.height - r.height;
				GUI.color = fade;
				GUI.Box(
					r, new GUIContent("", string.Format("{0} trees with {1}\" diameter remain.", treesToKeep[i], (i+1)*2)),
					GameGUIManager.use.styles.histogramBar
				);
				// trees to cut
				r.height = (treesToCut[i]/(float)maxTreeCountPerClass)*m_histogramRect.height;
				r.y = r.y - r.height;
				GUI.color = opaque;
				GUI.Box(
					r, new GUIContent("", string.Format("{0} trees with {1}\" diameter cut.", treesToCut[i], (i+1)*2)),
					GameGUIManager.use.styles.histogramBar
				);
			}
			// draw loading icon as needed
			if (progress<1f && Event.current.type == EventType.Repaint) {
				Vector2 position = new Vector2(
					m_histogramRect.width*0.5f,
					m_histogramRect.height*0.5f
				);
				GameGUIManager.use.DisplayLoadingIcon(progress, position, GameGUIManager.use.styles.lightTextColor);
			}
		} GUI.EndGroup();
		GUI.color = opaque;
		// axis labels
		GUILayout.BeginHorizontal(); {
			GUILayout.FlexibleSpace();
			for (int i=1; i<ResourceTile.treeSizeClassCount+1; ++i) {
				GUILayout.Label(
					string.Format("{0}\"", i*ResourceTile.treeSizeClassInterval),
					GameGUIManager.use.styles.smallTextMedium,
					GUILayout.Width(barWidth*0.8f)
				);
			}
		} GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(); {
			GUILayout.FlexibleSpace();
			GUILayout.Label("Tree Diameter", GameGUIManager.use.styles.smallTextLight);
			GUILayout.FlexibleSpace();
		} GUILayout.EndHorizontal();
	}
	
	#endregion
}