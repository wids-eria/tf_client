using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// An action to schedule a regular harvest.
/// </summary>
public class PlayerActionScheduleHarvest : PlayerActionHarvestTimber
{
	/// <summary>
	/// Modified fields.
	/// </summary>
	internal class ModifiedFields {
		/// <summary>
		/// Field specifying that the tile is a harvest area.
		/// </summary>
		public readonly bool harvest_area = true;
		/// <summary>
		/// Initializes a new instance of the <see cref="PlayerActionScheduleHarvest.ModifiedFields"/> class.
		/// </summary>
		/// <param name='isHarvestArea'>
		/// Is harvest area.
		/// </param>
		public ModifiedFields(bool isHarvestArea) {
			harvest_area = isHarvestArea;
		}
	}
	
	/// <summary>
	/// The harvests.
	/// </summary>
	private List<Harvest> m_harvests = new List<Harvest>();
	/// <summary>
	/// Gets the end time.
	/// </summary>
	/// <value>
	/// The end time.
	/// </value>
	private int m_duration = m_maxDuration/3;
	/// <summary>
	/// Constant max duration.
	/// </summary>
	private const int m_maxDuration = 15;
	/// <summary>
	/// The harvest execution progress.
	/// </summary>
	private float m_harvestExecutionProgress = 1f;
	
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
		return base.IsPermittedOnResourceTile(tile);
	}
	
	/// <summary>
	/// Configure modifications then call base implementation. 
	/// </summary>
	/// <param name='selection'>
	/// The selection to which the action should be applied.
	/// </param>
	public override void DoIt(object selection)
	{
		ResourceTileSelection sel = (ResourceTileSelection)selection;
		// initialize as a clearcut
		Harvest harvest = new Harvest(
			sel.ToArray(), m_duration
		);
		switch (m_cutType) {
		case CutType.DiameterLimitCut:
			harvest = new Harvest(
				sel.ToArray(), m_duration,
				m_diameterLimit, m_diameterLimitDirection
			);
			break;
		case CutType.QRatioCut:
			harvest = new Harvest(
				sel.ToArray(), m_duration,
				m_qRatio, m_basalArea
			);
			break;
		}
		StartCoroutine(harvest.ComputeCenterPoint()); // TODO: this is not pretty, but in the end this will be determined on the server anyway
		m_harvests.Add(harvest);
		m_tileModifications.resource_tile = new ModifiedFields(true);
		base.DoIt(selection);
	}
	
	/// <summary>
	/// Listen for messages.
	/// </summary>
	protected override void Awake()
	{
		base.Awake();
		MessengerAM.Listen(MessageHarvestExecuted.listenerType, this);
	}
	
	/// <summary>
	/// Debug
	/// </summary>
	protected override void Update()
	{
		base.Update();
		// execute each harvest as though the game were advancing
		// THIS IS JUST FOR DEBUGGING
		if (Input.GetKey(KeyCode.LeftApple) && Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.H)) {
			ExecuteHarvests();
		}
	}
	
	/// <summary>
	/// Schedules the harvest.
	/// </summary>
	/// <param name='harvest'>
	/// Harvest.
	/// </param>
	public void ScheduleHarvest(Harvest harvest)
	{
		m_harvests.Add(harvest);
	}
	
	/// <summary>
	/// Executes the harvests.
	/// </summary>
	public void ExecuteHarvests()
	{
		StartCoroutine(ExecuteHarvests(m_harvests.ToArray()));
	}
	
	/// <summary>
	/// Gets a value indicating whether this <see cref="PlayerActionScheduleHarvest"/> action is in progress.
	/// </summary>
	/// <value>
	/// <c>true</c> if action is in progress; otherwise, <c>false</c>.
	/// </value>
	public override bool isActionInProgress {
		get {
			return base.isActionInProgress || m_harvestExecutionProgress < 1f;
		}
	}
	
	/// <summary>
	/// Executes the harvests.
	/// </summary>
	/// <returns>
	/// The harvests to execute.
	/// </returns>
	private IEnumerator ExecuteHarvests(Harvest[] harvests)
	{
		m_actionProgress = 0f;
		float div = 1f/harvests.Length;
		foreach (Harvest harvest in harvests) {
			yield return StartCoroutine(harvest.DoIt());
			m_actionProgress += div;
		}
		m_actionProgress = 1f;
	}
	
	/// <summary>
	/// Concludes the harvest.
	/// </summary>
	/// <param name='harvest'>
	/// Harvest.
	/// </param>
	private IEnumerator ConcludeHarvest(Harvest harvest)
	{
		m_harvests.Remove(harvest);
		while (isActionInProgress) { // wait for action to finish
			yield return 0;
		}
		m_tileModifications.resource_tile = new ModifiedFields(false);
		base.DoIt(new ResourceTileSelection(harvest.ids));
		m_currentlySelectedHarvestIndex = m_currentlySelectedHarvestIndex;
		MessengerAM.Send(new MessageHarvestEnded(harvest));
	}
	
	/// <summary>
	/// Raises the harvest executed event.
	/// </summary>
	/// <remarks>
	/// Concludes the harvest if needed
	/// </remarks>
	/// <param name='msg'>
	/// Message.
	/// </param>
	public void _HarvestExecuted(MessageHarvestExecuted msg)
	{
		// early out if the harvest doesn't belong to this action
		if (!m_harvests.Contains(msg.harvest)) {
			return;
		}
		// conclude the harvest if needed
		if (msg.harvest.duration == msg.harvest.history.Length) {
			StartCoroutine(ConcludeHarvest(msg.harvest));
		}
		// throw an exception if the harvest has gone on too long for some reason
		else if (msg.harvest.duration < msg.harvest.history.Length) {
			throw new System.NotSupportedException("The harvest's history has exceeded its duration!");
		}
	}
	
	#region GUI Handling
	
	/// <summary>
	/// Tab.
	/// </summary>
	internal enum Tab { CreateNewHarvest, ReviewExistingHarvests }
	/// <summary>
	/// The current tab.
	/// </summary>
	private Tab m_tab {
		get { return m_tabBackingField; }
		set {
			m_tabBackingField = value;
			if (m_tabBackingField == Tab.CreateNewHarvest) {
				//TerrainProjections.use.HighlightResourceTileSelection(ResourceTileSelection.GetCurrent());
			}
			else {
				if (m_currentlySelectedHarvest == null) {
					return;
				}
				//TerrainProjections.use.HighlightResourceTileSelection(new ResourceTileSelection(m_currentlySelectedHarvest.ids));
				// jump to harvest center
				CameraRig.use.JumpTo(m_currentlySelectedHarvest.centerPoint);
			}
		}
	}
	/// <summary>
	/// The m_tab backing field.
	/// </summary>
	private Tab m_tabBackingField;
	/// <summary>
	/// The scroll position.
	/// </summary>
	private Vector2 m_createTabScrollPosition;
	/// <summary>
	/// The nested scroll position.
	/// </summary>
	private Vector2 m_reviewHarvestsScrollPosition;
	/// <summary>
	/// Gets the cost scalar.
	/// </summary>
	/// <value>
	/// The cost scalar.
	/// </value>
	protected override int m_costScalar {
		get {
			return base.m_costScalar * m_duration;
		}
	}
	/// <summary>
	/// Gets the currently selected harvest.
	/// </summary>
	/// <value>
	/// The currently selected harvest.
	/// </value>
	private Harvest m_currentlySelectedHarvest {
		get {
			if (m_harvests.Count == 0) {
				return null;
			}
			return m_harvests[m_currentlySelectedHarvestIndex];
		}
	}
	/// <summary>
	/// The index of the currently selected harvest.
	/// </summary>
	private int m_currentlySelectedHarvestIndex {
		get { return Mathf.Clamp(m_currentlySelectedHarvestIndexBackingField, 0, m_harvests.Count); }
		set {
			// set value
			bool isNewValue = value != m_currentlySelectedHarvestIndexBackingField;
			m_currentlySelectedHarvestIndexBackingField = Mathf.Clamp(value, 0, m_harvests.Count);
			// highlight harvest
			if (m_currentlySelectedHarvest != null) {
				//TerrainProjections.use.HighlightResourceTileSelection(new ResourceTileSelection(m_currentlySelectedHarvest.ids));
			}
			else {
				//TerrainProjections.use.ClearSelectionHighlight();
			}
			// early out if no change
			if (!isNewValue) {
				return;
			}
			// jump to harvest center
			CameraRig.use.JumpTo(m_currentlySelectedHarvest.centerPoint);
		}
	}
	/// <summary>
	/// The currently selected harvest index backing field.
	/// </summary>
	private int m_currentlySelectedHarvestIndexBackingField;
	/// <summary>
	/// The harvest details scroll position.
	/// </summary>
	private Vector2 m_harvestDetailsScrollPosition;
	
	/// <summary>
	/// Displays the controls contents.
	/// </summary>
	protected override void DisplayControlsContents()
	{
		// tab selector
		m_tab = (Tab)GUILayout.SelectionGrid(
			(int)m_tab,
			StringHelpers.GetEnumNamesAsCamelCase(typeof(Tab)), 2,
			m_button
		);
		// main view
		DisplayNewHarvestTab();
		DisplayReviewHarvestsTab();
	}
	
	/// <summary>
	/// Displays the new harvest tab.
	/// </summary>
	private void DisplayNewHarvestTab()
	{
		// early out if not in the right tab
		if (m_tab != Tab.CreateNewHarvest) {
			return;
		}
		// stuff everything inside scroll view
		m_createTabScrollPosition = GUILayout.BeginScrollView(m_createTabScrollPosition, false, true); {
			// stuff from base class
			DisplayCutSelector();
			GUILayout.BeginHorizontal(); {
				GUILayout.Label("Duration:", m_mainText);
				GUILayout.Label(string.Format("{0} years", m_duration), m_mainTextAlt);
				GUILayout.FlexibleSpace();
				m_duration = (int)GUILayout.HorizontalSlider(
					(float)m_duration,
					1f, (float)m_maxDuration,
					GUILayout.Width(m_sliderWidth)
				);
			} GUILayout.EndHorizontal();
			DisplayOperationCost();
		} GUILayout.EndScrollView();
		m_styles.DrawLine(
			GUIStyles.LineDirection.Horizontal,
			GUIStyles.LineColor.Medium
		);
		// stuff from PlayerAction
		DisplayPaintSelectionControlGroup();
		m_styles.DrawLine(GUIStyles.LineDirection.Horizontal, GUIStyles.LineColor.Highlighted);
		DoItButton();
	}
	
	/// <summary>
	/// Displays the review harvests tab.
	/// </summary>
	private void DisplayReviewHarvestsTab()
	{
		// early out if not in the right tab
		if (m_tab != Tab.ReviewExistingHarvests) {
			return;
		}
		if (m_harvests.Count == 0) {
			GUILayout.Label("No harvests active.", m_mainText);
		}
		else {
			// show harvests
			DisplayHarvestList();
			m_styles.DrawLine(GUIStyles.LineDirection.Horizontal, GUIStyles.LineColor.Medium);
			// show details
			DisplayCurrentHarvestDetails();
		}
		GUILayout.FlexibleSpace();
	}
	
	/// <summary>
	/// Displays the harvest list.
	/// </summary>
	private void DisplayHarvestList()
	{
		GUILayout.Label("Select a Harvest:", m_mainText);
		List<GUIContent> harvestSelectors = new List<GUIContent>();
		foreach (Harvest harvest in m_harvests) {
			harvestSelectors.Add(
				new GUIContent(
					string.Format(
						"{0} Harvest on {1} acre{2} (Year {3}/{4})",
						StringHelpers.CamelCaseToWords(harvest.cutType.ToString()),
						harvest.ids.Length, harvest.ids.Length==1?"":"s",
						harvest.history.Length, harvest.duration
					), "Review this harvest")
				);
		}
		m_reviewHarvestsScrollPosition = GUILayout.BeginScrollView(
			m_reviewHarvestsScrollPosition, 
			false, true, 
			GUILayout.Height(controlsAreaHeight*0.15f)
		); {
			m_currentlySelectedHarvestIndex = GUILayout.SelectionGrid(
				m_currentlySelectedHarvestIndex,
				harvestSelectors.ToArray(), 1,
				m_button
			);
		} GUILayout.EndScrollView();
	}
	
	/// <summary>
	/// Displays the current harvest details.
	/// </summary>
	private void DisplayCurrentHarvestDetails()
	{
		// early out if nothing is selected
		if (m_currentlySelectedHarvest == null) {
			GUILayout.Label("No harvest selected.", m_minorText);
			return;
		}
		// common details
		GUILayout.BeginHorizontal(); {
			GUILayout.Label("Acres:", m_mainText);
			GUILayout.FlexibleSpace();
			GUILayout.Label(m_currentlySelectedHarvest.ids.Length.ToString(), m_mainTextAlt);
		} GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(); {
			GUILayout.Label("Harvest Type:", m_mainText);
			GUILayout.FlexibleSpace();
			GUILayout.Label(StringHelpers.CamelCaseToWords(m_currentlySelectedHarvest.cutType.ToString()), m_mainTextAlt);
		} GUILayout.EndHorizontal();
		// cut-specific information
		DisplayDiameterLimitDetails();
		DisplayQRatioDetails();
		// duration
		GUILayout.BeginHorizontal(); {
			GUILayout.Label("Duration:", m_mainText);
			GUILayout.FlexibleSpace();
			GUILayout.Label(string.Format("{0} years", m_currentlySelectedHarvest.duration), m_mainTextAlt);
		} GUILayout.EndHorizontal();
		// history viewer
		DisplayCurrentHarvestHistoryViewer();
	}
	
	/// <summary>
	/// History tab.
	/// </summary>
	public enum HistoryTab { Volume, Value }
	
	/// <summary>
	/// The history tab.
	/// </summary>
	private HistoryTab m_historyTab;
	
	/// <summary>
	/// The pole timber point style.
	/// </summary>
	[SerializeField]
	private TimeSeries m_poleTimberPointStyle;
	/// <summary>
	/// The saw timber point style.
	/// </summary>
	[SerializeField]
	private TimeSeries m_sawTimberPointStyle;
	/// <summary>
	/// The line graph line material.
	/// </summary>
	[SerializeField]
	private Material m_lineGraphLineMaterial;
	
	/// <summary>
	/// Displays the current harvest history viewer.
	/// </summary>
	private void DisplayCurrentHarvestHistoryViewer()
	{
		// early out if no history to show
		if ((m_currentlySelectedHarvest.history == null ||
			m_currentlySelectedHarvest.history.Length == 0)
		) {
			GUILayout.Label("No history for this harvest yet.", m_minorText);
			return;
		}
		// otherwise display history of products
		else {
			GUILayout.BeginHorizontal(); {
				GUILayout.Label("Product Extraction History:", m_mainText);
				m_historyTab = (HistoryTab)GUILayout.SelectionGrid(
					(int)m_historyTab,
					System.Enum.GetNames(typeof(PlayerActionScheduleHarvest.HistoryTab)),
					2,
					m_button
				);
			} GUILayout.EndHorizontal();
			string tooltip = DisplayHarvestHistoryGraph(
				BottomTrayGUI.use.rectActionControls,
				m_currentlySelectedHarvest, m_historyTab,
				m_poleTimberPointStyle, m_sawTimberPointStyle,
				m_lineGraphLineMaterial
			);
			// display info about the current point, if any
			GUILayout.Label(tooltip, m_mainText);
		}
	}
	
	/// <summary>
	/// The rect for displaying a harvest history.
	/// </summary>
	private static Rect m_lineGraphRect;
	
	/// <summary>
	/// Displays the history of the supplied harvest as a line graph.
	/// </summary>
	/// <returns>
	/// The tooltip for the current point, if any.
	/// </returns>
	/// <param name='parent'>
	/// The parent rectangle
	/// </param>
	/// <param name='harvest'>
	/// Harvest.
	/// </param>
	/// <param name='currentTab'>
	/// Current tab.
	/// </param>
	/// <param name='poleTimberSeries'>
	/// Pole timber point style.
	/// </param>
	/// <param name='sawTimberSeries'>
	/// Saw timber point style.
	/// </param>
	/// <param name='lineGraphMaterial'>
	/// Line graph material.
	/// </param>
	public static string DisplayHarvestHistoryGraph(
		Rect parent,
		Harvest harvest, HistoryTab currentTab,
		TimeSeries poleTimberSeries, TimeSeries sawTimberSeries,
		Material lineGraphMaterial
	)
	{
		// create space in the layout and store it
		Rect placeholder = GUILayoutUtility.GetRect(
			//BottomTrayGUI.use.rectActionControls.width*0.5f, // min width
			//BottomTrayGUI.use.rectActionControls.width, // max width
			150f, 300f,
			m_histogramHeight, // min height
			m_histogramHeight // max height
		);
		if (Event.current.type == EventType.Repaint) {
			m_lineGraphRect = GUILayoutUtility.GetLastRect();
		}
		// determine max value for ordinate
		float maxOrdinate = 0f;
		if (currentTab == HistoryTab.Value) {
			foreach (Harvest.Products products in harvest.history) {
				maxOrdinate = Mathf.Max(maxOrdinate, products.poletimberValue);
				maxOrdinate = Mathf.Max(maxOrdinate, products.sawtimberValue);
			}
		} else {
			foreach (Harvest.Products products in harvest.history) {
				maxOrdinate = Mathf.Max(maxOrdinate, products.poletimberVolume);
				maxOrdinate = Mathf.Max(maxOrdinate, products.sawtimberVolume);
			}
		}
		// graph gui
		Color currentGUIColor = GUI.color;
		GUI.BeginGroup(m_lineGraphRect); {
			// allocations for background
			Color opaqueLine = GUI.color;
			Color fadeLine = new Color(opaqueLine.r, opaqueLine.g, opaqueLine.b, opaqueLine.a * 0.5f);
			Rect r = new Rect();
			int numLines = 5;
			float div = 1f/numLines;
			bool fill = true;
			GUIStyle labelStyle = GameGUIManager.use.styles.smallTextMedium;
			// draw background
			for (int i=0; i<numLines; ++i) {
				// ordinate number
				r.width = m_lineGraphRect.width; r.x = 0f;
				r.height = m_lineGraphRect.height*div; r.y = r.height*i;
				GUI.Label(r, string.Format("{0:0,0}", maxOrdinate*div*(numLines-i)), labelStyle);
				// line
				if (fill) {
					GUI.color = fadeLine;
					GUI.DrawTexture(r, GameGUIManager.use.styles.histogramBar.active.background, ScaleMode.StretchToFill);
					GUI.color = opaqueLine;
				}
				fill = !fill;
			}
			// get points
			Vector2[] poleTimberPoints = new Vector2[harvest.history.Length];
			Vector2[] sawTimberPoints = new Vector2[harvest.history.Length];
			float divOrdinate = 1f/maxOrdinate;
			float divAbcissa = m_lineGraphRect.width/(harvest.duration-1);
			if (currentTab == HistoryTab.Value) {
				for (int i=0; i<harvest.history.Length; ++i) {
					poleTimberPoints[i] = new Vector2(divAbcissa*i, divOrdinate*harvest.history[i].poletimberValue*m_histogramHeight);
					sawTimberPoints[i] = new Vector2(divAbcissa*i, divOrdinate*harvest.history[i].sawtimberValue*m_histogramHeight);
				}
			}
			else {
				for (int i=0; i<harvest.history.Length; ++i) {
					poleTimberPoints[i] = new Vector2(divAbcissa*i, divOrdinate*harvest.history[i].poletimberVolume*m_histogramHeight);
					sawTimberPoints[i] = new Vector2(divAbcissa*i, divOrdinate*harvest.history[i].sawtimberVolume*m_histogramHeight);
				}
			}
			// draw lines
			if (Event.current.type == EventType.Repaint) {
				Vector2 corner = new Vector2(
					parent.x+placeholder.x,
					Screen.height-parent.y-placeholder.y-placeholder.height
				);
				DrawLineInGraph(poleTimberSeries, poleTimberPoints, lineGraphMaterial, corner);
				DrawLineInGraph(sawTimberSeries, sawTimberPoints, lineGraphMaterial, corner);
			}
			// draw points
			for (int i=0; i<harvest.history.Length; ++i) {
				// draw poletimber point
				DrawPointInGraph(
					poleTimberSeries, poleTimberPoints[i],
					string.Format(
						"Pole Timber Year {0:0}: {1}", i+1,
						currentTab==HistoryTab.Value?
						string.Format("{0:c}", harvest.history[i].poletimberValue):
						string.Format("{0:0} cords", harvest.history[i].poletimberVolume)
					)
				);
				// draw sawtimber point
				DrawPointInGraph(
					sawTimberSeries, sawTimberPoints[i],
					string.Format(
						"Saw Timber Year {0:0}: {1}", i+1,
						currentTab==HistoryTab.Value?
						string.Format("{0:c}", harvest.history[i].sawtimberValue):
						string.Format("{0:0} board feed", harvest.history[i].sawtimberVolume)
					)
				);
			}
		} GUI.EndGroup();
		GUI.color = currentGUIColor;
		// axis labels
		GUILayout.BeginHorizontal(); {
			for (int i=0; i<harvest.duration; ++i) {
				GUILayout.Label(
					string.Format("{0}", i+1),
					GameGUIManager.use.styles.smallTextMedium
				);
				if (i<harvest.duration-1) {
					GUILayout.FlexibleSpace();
				}
			}
		} GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(); {
			GUILayout.FlexibleSpace();
			GUILayout.Label("Harvest Year", GameGUIManager.use.styles.smallTextLight);
			GUILayout.FlexibleSpace();
		} GUILayout.EndHorizontal();
		// return the tooltip if the mouse is in the graph
		return (placeholder.Contains(Event.current.mousePosition)?GUI.tooltip:"");
	}
	
	/// <summary>
	/// Draws the point in the line graph.
	/// </summary>
	/// <param name='timeSeries'>
	/// Time series.
	/// </param>
	/// <param name='centerPoint'>
	/// Center point in GUI space.
	/// </param>
	/// <param name='tooltip'>
	/// Tooltip.
	/// </param>
	private static void DrawPointInGraph(TimeSeries timeSeries, Vector2 centerPoint, string tooltip)
	{
		Color c = GUI.color;
		GUI.color = timeSeries.color;
		GUI.Box(
			new Rect(
				centerPoint.x - timeSeries.graphic.width*0.5f,
				m_histogramHeight - centerPoint.y - timeSeries.graphic.height*0.5f,
				timeSeries.graphic.width,
				timeSeries.graphic.height
			), new GUIContent(timeSeries.graphic, tooltip), GameGUIManager.use.styles.empty
		);
		GUI.color = c;
	}
	
	/// <summary>
	/// Draws the line in graph.
	/// </summary>
	/// <param name='timeSeries'>
	/// Time series.
	/// </param>
	/// <param name='points'>
	/// Points in graph space.
	/// </param>
	/// <param name='material'>
	/// Material.
	/// </param>
	/// <param name='upperLeftCorner'>
	/// The upper left corner of the drawing area in GUI coordinates
	/// </param>
	private static void DrawLineInGraph(TimeSeries timeSeries, Vector2[] points, Material material, Vector2 upperLeftCorner)
	{
		GL.PushMatrix();
		material.SetPass(0);
		GL.LoadPixelMatrix();
		GL.Color(timeSeries.color);
		GL.Begin(GL.LINES);
		for (int i=0; i<points.Length-1; ++i) {
			GL.Vertex(upperLeftCorner+points[i]);
			GL.Vertex(upperLeftCorner+points[i+1]);
		}
		GL.End();
		GL.PopMatrix();
	}
	
	/// <summary>
	/// Displays the diameter limit details.
	/// </summary>
	private void DisplayDiameterLimitDetails()
	{
		// early out if not the right cut type
		if (m_currentlySelectedHarvest.cutType != CutType.DiameterLimitCut) {
			return;
		}
		GUILayout.BeginHorizontal(); {
			GUILayout.Label("Rule:", m_mainText);
			GUILayout.FlexibleSpace();
			GUILayout.Label(
				string.Format(
					"All trees {0} than {1}\"",
					(m_currentlySelectedHarvest.diameterLimitCutDirection.ToString()).ToLower(),
					m_currentlySelectedHarvest.diameterLimit
				), m_mainTextAlt
			);
		} GUILayout.EndHorizontal();
	}
	
	/// <summary>
	/// Displays the Q ratio details.
	/// </summary>
	private void DisplayQRatioDetails()
	{
		// early out if not the right cut type
		if (m_currentlySelectedHarvest.cutType != CutType.QRatioCut) {
			return;
		}
		GUILayout.BeginHorizontal(); {
			GUILayout.Label("Target Basal Area:", m_mainText);
			GUILayout.FlexibleSpace();
			GUILayout.Label(string.Format("{0:0} sq ft/acre", m_currentlySelectedHarvest.basalArea), m_mainTextAlt);
		} GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(); {
			GUILayout.Label("Q-Ratio:", m_mainText);
			GUILayout.FlexibleSpace();
			GUILayout.Label(string.Format("{0:0.00}", m_currentlySelectedHarvest.qRatio), m_mainTextAlt);
		} GUILayout.EndHorizontal();
	}
	
	#endregion
}