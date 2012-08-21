using UnityEngine;
using System.Collections;
using System.Collections.Generic;
         
[ExecuteInEditMode]
/// <summary>
/// Manager for in-game GUI
/// </summary>
public class GameGUIManager : MonoBehaviour
{
	/// <summary>
	/// Gets or sets the singleton.
	/// </summary>
	/// <value>
	/// The singleton.
	/// </value>
	public static GameGUIManager use { get; private set; }
	
	/// <summary>
	/// Initialize
	/// </summary>
	void Awake()
	{
		if (use != null) {
			Destroy(use.gameObject);
		}
		use = this;
		MessengerAM.Listen("game flow", this);
		MessengerAM.Listen(MessengerAM.listenTypeConfig, this);
		Messenger<int>.AddListener(BottomTrayGUI.kPaintSelectionInputMode, SetInputSelection);
		InitializeGUI(); // just add a call to this here too, so things that need public attributes will have them before Start() is called
	}
	
	void OnDestroy(){
		Messenger<int>.RemoveListener(BottomTrayGUI.kPaintSelectionInputMode, SetInputSelection);
	}
	
	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start()
	{
		StartCoroutine(DownloadMinimap());
	}
	
	/// <summary>
	/// Downloads the minimap.
	/// </summary>
	/// <returns>
	/// The minimap.
	/// </returns>
	private IEnumerator DownloadMinimap()
	{
		yield return StartCoroutine(Minimap.use.Download());
		InitializeGUI();
	}
	
	#region ErrorManagement
	/// <summary>
	/// sound to play when e.g. a dialog fails
	/// </summary>
	[SerializeField]
	private AudioClip m_invalidAction;
	
	/// <summary>
	/// Plays the invalid action sound.
	/// </summary>
	public void PlayInvalidActionSound()
	{
		AudioSource.PlayClipAtPoint(m_invalidAction, Camera.main.transform.position);
	}
	
	/// <summary>
	/// Sets the error message on the status line.
	/// </summary>
	/// <param name='msg'>
	/// Message.
	/// </param>
	public void SetErrorMessage(string msg)
	{
		GameGUIManager.use.PlayInvalidActionSound();
		m_errorMessageTimer = m_errorMessageDuration;
		m_errorMessage = msg;
	}
	/// <summary>
	/// The error message.
	/// </summary>
	private string m_errorMessage = "";
	/// <summary>
	/// How long an error message should stay up in the status line?
	/// </summary>
	[SerializeField]
	private float m_errorMessageDuration = 5f;
	/// <summary>
	/// How long has the error message been up?
	/// </summary>
	private float m_errorMessageTimer = 0f;
	/// <summary>
	/// Gets the error message.
	/// </summary>
	/// <value>
	/// The error message.
	/// </value>
	public string ErrorMessage{get{return m_errorMessage;}}
	
	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update(){		
		// manage error message
		if (string.IsNullOrEmpty(m_errorMessage)) {
			return;
		}
		m_errorMessageTimer -= Time.deltaTime;
		if (m_errorMessageTimer <= 0f) {
			m_errorMessage = "";
		}
	}
	#endregion
	
	/// <summary>
	/// the collection of styles
	/// </summary>
	public GUIStyles styles;
	
	/// <summary>
	/// The icon texture.
	/// </summary>
	[SerializeField]
	public GUITextures iconTexture;
	
	/// <summary>
	/// the style to use for boxes throughout the gui
	/// </summary>
	public GUIStyle styleBox { get; private set; }
	
	public bool		SystemIsBusy = false;
	
	/// <summary>
	/// Handle screen resolution change event.
	/// </summary>
	/// <param name='msg'>
	/// Message.
	/// </param>
	public void _ScreenResolutionChanged(MessageScreenResolutionChanged msg)
	{
		InitializeGUI();
	}
	
	#region GUIObject
	/// <summary>
	/// The m_gui objects.
	/// </summary>
	protected static List<GUIObject> m_guiObjects;
	/// <summary>
	/// Inits the GUI objects.
	/// </summary>
	protected void InitGUIObjects() {
		if (m_guiObjects == null) {
			m_guiObjects = new List<GUIObject>();
			//Add new GUI Objects here
			m_guiObjects.Add (new BottomTrayGUI());
			//m_guiObjects.Add(new NturnUI());
			m_guiObjects.Add(new GameDockGUI());
			m_guiObjects.Add (new ProjectionInterfaceGUI());
			m_guiObjects.Add (new QuestGUI());
			m_guiObjects.Add (new Minimap());
			//m_guiObjects.Add (new PlayerImportantInfoUI());
		}
		
	}
	/// <summary>
	/// Adds the GUI object.
	/// </summary>
	/// <param name='obj'>
	/// Object.
	/// </param>
	public static void AddGUIObject(GUIObject obj)
	{
		if (!m_guiObjects.Contains(obj)) {
			m_guiObjects.Add(obj);
		} else {
			Debug.LogWarning(string.Format("GUI Manager already contains GUIObject `{0}'.",obj));
		}
	}
	/// <summary>
	/// Removes the GUI object.
	/// </summary>
	/// <param name='obj'>
	/// Object.
	/// </param>
	public static void RemoveGUIObject(GUIObject obj)
	{
		if (m_guiObjects.Contains(obj)) {
			m_guiObjects.Remove(obj);
		} else {
			Debug.LogWarning(string.Format("GUI Manager does not contain GUIObject `{0}'. It may have already been removed or was not added.",obj));
		}
	}
	#endregion
	
	
	/// <summary>
	/// Initializes the GUI rects and styles.
	/// </summary>
	public void InitializeGUI()
	{	
		m_loadingIconRect.width = loadingIconSize;
		m_loadingIconRect.height = loadingIconSize;
		m_loadingIconRect.x = -0.5f*m_loadingIconRect.width; // its position is set via GUI.matrix
		m_loadingIconRect.y = -0.5f*m_loadingIconRect.height;
		
		// zoning box
//		m_rectZoningBox.x = m_minimapBackgroundRect.x;
//		m_rectZoningBox.width = m_minimapBackgroundRect.width;
//		m_rectZoningBox.height = 50f;
//		m_rectZoningBox.y = m_minimapBackgroundRect.y - m_rectZoningBox.height - 0.5f*(styleBox.margin.top+styleBox.margin.bottom);
		
		//Add all GUI Objects into list
		InitGUIObjects();
		
	}
	
	/// <summary>
	/// Is the mouse cursor in a GUI rect?
	/// </summary>
	/// <returns>
	/// <c>true</c> if the mouse is currently in a GUI rect; otherwise, <c>false</c>.
	/// </returns>
	public bool IsCursorInGUI()
	{
		bool check = false;
		foreach (GUIObject g in m_guiObjects) {
			check |= g.IsMouseOver();
		}
		return check;
	}
	

	
	/// <summary>
	/// The pan cursor when the mouse button is up.
	/// </summary>
	public Texture2D panCursorUp;
	/// <summary>
	/// The pan cursor when the mouse button is down.
	/// </summary>
	public Texture2D panCursorDn;
	/// <summary>
	/// The paint cursor for selecting.
	/// </summary>
	public Texture2D paintCursorSelect;
	/// <summary>
	/// The paint cursor for deselecting.
	/// </summary>
	public Texture2D paintCursorDeselect;
	
	/// <summary>
	/// The clear resource tile selection icon.
	/// </summary>
	public Texture2D clearResourceTileSelectionIcon;
	/// <summary>
	/// The select resource tile icon on.
	/// </summary>
	public Texture2D selectResourceTileIconOn;
	/// <summary>
	/// The select resource tile icon off.
	/// </summary>
	public Texture2D selectResourceTileIconOff;
	/// <summary>
	/// The deselect resource tile icon on.
	/// </summary>
	public Texture2D deselectResourceTileIconOn;
	/// <summary>
	/// The deselect resource tile icon off.
	/// </summary>
	public Texture2D deselectResourceTileIconOff;
	/// <summary>
	/// The pan selection icon on.
	/// </summary>
	public Texture2D panSelectionIconOn;
	/// <summary>
	/// The pan selection icon off.
	/// </summary>
	public Texture2D panSelectionIconOff;
	/// <summary>
	/// The submit resource tile selection icon.
	/// </summary>
	public Texture2D submitResourceTileSelectionIcon;
	
	/// <summary>
	/// Raises the GUI event.
	/// </summary>
	void OnGUI()
	{
		// draw loading icon if downloading
		if( SystemIsBusy ) {
			DisplayLoadingIcon( WebRequests.tileDownloadProgress );
		}
		
		//to prevent Player haven't loaded before painting
		if(Player.current == null)
			return;
			
		foreach(GUIObject obj in m_guiObjects) {
			if (obj.visible) obj.Draw();
		}

		// set cursor
		SetCursor();
	}
	
	#region Loading Icon
	/// <summary>
	/// The loading icon rect.
	/// </summary>
	private Rect m_loadingIconRect = new Rect();
	/// <summary>
	/// The loading icon.
	/// </summary>
	[SerializeField]
	private Texture2D m_loadingIcon;
	/// <summary>
	/// Gets the size of the loading icon.
	/// </summary>
	/// <remarks>
	/// Assumes square texture.
	/// </remarks>
	/// <value>
	/// The size of the loading icon.
	/// </value>
	public int loadingIconSize { get { return m_loadingIcon.width; } }
	/// <summary>
	/// The loading icon angular velocity.
	/// </summary>
	[SerializeField]
	private float m_loadingIconAngularVelocity = 10f;
	
	/// <summary>
	/// Displays the loading icon at the center of the screen.
	/// </summary>
	/// <param name='progress'>
	/// Normalized progress.
	/// </param>
	public void DisplayLoadingIcon(float progress) {
		DisplayLoadingIcon(progress, new Vector2(Screen.width, Screen.height)*0.5f, styles.darkTextColor);
	}
	
	/// <summary>
	/// Displays the loading icon at the specified position.
	/// </summary>
	/// <param name='progress'>
	/// Normalized progress.
	/// </param>
	/// <param name='position'>
	/// Position.
	/// </param>
	/// <param name='color'>
	/// Color
	/// </param>
	public void DisplayLoadingIcon(float progress, Vector2 position, Color color)
	{
		Color col = GUI.color;
		GUI.color = color;
		Matrix4x4 m = GUI.matrix;
		int numPoints = 12;
		float angle = (360f/numPoints)*(int)(Time.time*m_loadingIconAngularVelocity);
		GUIUtility.RotateAroundPivot(angle, position);
		m_loadingIconRect.x = position.x - 0.5f*m_loadingIconRect.width;
		m_loadingIconRect.y = position.y - 0.5f*m_loadingIconRect.height;
		GUI.DrawTexture(m_loadingIconRect, m_loadingIcon);
		GUI.matrix = m;
		if (progress < 1f) {
			GUI.color = styles.darkTextColor;
			Rect loadingBar = new Rect(
				position.x-0.5f*loadingIconSize,
				position.y+0.5f*loadingIconSize+2f,
				loadingIconSize,
				0.25f*loadingIconSize
			);
			GUI.BeginGroup(loadingBar, styles.histogramBar); {
				GUI.color = styles.highlightTextColor;
				GUI.Box(new Rect(2f, 2f, (loadingBar.width-4f)*progress, loadingBar.height-4f), "", styles.histogramBar);
			} GUI.EndGroup();
		}
		GUI.color = col;
	}
	#endregion 
	
	/// <summary>
	/// The input selection.
	/// </summary>
	private int InputSelection=1;
	
	private void SetInputSelection(int selection){
		InputSelection = selection;
	}
	
	/// <summary>
	/// Sets the cursor.
	/// </summary>
	private void SetCursor()
	{
		if (IsCursorInGUI()) { // use standard cursor in the GUI windows
			Screen.showCursor = true;
			return;
		}
		switch (InputSelection)
		{
		case 0:
			Screen.showCursor = false;
			GUI.DrawTexture(m_panCursorRect, Input.GetMouseButton(0)?panCursorDn:panCursorUp);
			break;
		case 2:
			Screen.showCursor = false;
//			if (InputManager.paintSelectionInputMode == InputManager.PaintSelectionInputMode.Deselect ||
//				InputManager.isHoldingAltButton
//			) {
//				cursor = paintCursorDeselect;
//			}
//			else if (InputManager.paintSelectionInputMode == InputManager.PaintSelectionInputMode.Pan ||
//				InputManager.isHoldingPanClickModifier
//			) {
//				cursor = Input.GetMouseButton(0)?panCursorDn:panCursorUp;
//			}
			GUI.DrawTexture(m_paintCursorRect, paintCursorDeselect);
			break;
		case 1:
			Screen.showCursor = false;
			GUI.DrawTexture(m_paintCursorRect, paintCursorSelect);
			break;
		}
	}
	
	/// <summary>
	/// The size of the cursor.
	/// </summary>
	public static readonly float cursorSize = 32f;
	
	/// <summary>
	/// Gets the pan cursor rect.
	/// </summary>
	/// <value>
	/// The pan cursor rect.
	/// </value>
	private Rect m_panCursorRect {
		get {
			return new Rect(
				Event.current.mousePosition.x-0.5f*cursorSize,
				Event.current.mousePosition.y-0.5f*cursorSize,
				cursorSize, cursorSize
				);
		}
	}
	
	/// <summary>
	/// Gets the paint cursor rect.
	/// </summary>
	/// <value>
	/// The paint cursor rect.
	/// </value>
	private Rect m_paintCursorRect {
		get {
			return new Rect(
				Event.current.mousePosition.x,
				Event.current.mousePosition.y,
				cursorSize, cursorSize
				);
		}
	}
	
	/// <summary>
	/// The fade amount for inactive GUI elements.
	/// </summary>
	public float inactiveGUIFade = 0.5f;
	
	
}
