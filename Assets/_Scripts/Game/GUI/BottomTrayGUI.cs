using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BottomTrayGUI : GUIObject
{
	/// <summary>
	/// The turn status.
	/// </summary>
	protected bool turnStatus = false;
	/// <summary>
	/// The rect for the bottom tray.
	/// </summary>
	private Rect m_rectBottomTray = new Rect();
	/// <summary>
	/// The style to use for the bottom tray.
	/// </summary>
	private GUIStyle m_styleBottomTray;
	/// <summary>
	/// Gets or sets the style for the status line background.
	/// </summary>
	/// <value>
	/// The style for the status line background.
	/// </value>
	public GUIStyle styleStatusLineBG { get; private set; }
	/// <summary>
	/// Gets or sets the style for the status line text.
	/// </summary>
	/// <value>
	/// The style for the status line text.
	/// </value>
	public GUIStyle styleStatusLineText { get; private set; }
	/// <summary>
	/// Gets or sets the style for the status line error text.
	/// </summary>
	/// <value>
	/// The style for the status line error text.
	/// </value>
	public GUIStyle styleStatusLineError { get; private set; }
	
	/// <summary>
	/// Enable other script to use static variables of this script
	/// </summary>
	/// <value>
	/// The use.
	/// </value>
	public static BottomTrayGUI use{get; private set;}
	
	/// <summary>
	/// Initializes a new instance of the <see cref="BottomTrayGUI"/> class.
	/// </summary>
	public BottomTrayGUI(){
		Messenger<bool>.AddListener("TurnChanged",changeTurn);
		use=this;
		m_rectBottomTray.height = 85f;
		m_rectBottomTray.width = Screen.width;
		m_rectBottomTray.y = Screen.height - m_rectBottomTray.height;
		m_styleBottomTray = Styles.sqLightGreyBox;
		styleStatusLineBG = Styles.roundDarkBox;
		styleStatusLineText = Styles.mediumTextLight;
		styleStatusLineError = Styles.mediumTextHighlighted;
		m_rectActionControls.width = 300f;
		m_rectActionControls.x = Screen.width - m_rectActionControls.width - Styles.roundDarkBox.margin.right;
	}
	
	/// <summary>
	/// Releases unmanaged resources and performs other cleanup operations before the <see cref="BottomTrayGUI"/> is
	/// reclaimed by garbage collection.
	/// </summary>
	~BottomTrayGUI(){
		Messenger<bool>.RemoveListener("TurnChanged",changeTurn);
	}
	
	/// <summary>
	/// Changes the turn.
	/// </summary>
	/// <param name='status'>
	/// Status.
	/// </param>
	private void changeTurn(bool status){
		turnStatus = status;
	}
	
	/// <summary>
	/// Gets the rect for the action controls.
	/// </summary>
	/// <value>
	/// The rect for the action controls.
	/// </value>
	public Rect rectActionControls { get { return m_rectActionControls; } }
	/// <summary>
	/// The rect for the action controls.
	/// </summary>
	private Rect m_rectActionControls = new Rect();
	
	/// <summary>
	/// Sets the height of the action controls rect.
	/// </summary>
	/// <param name='height'>
	/// Height.
	/// </param>
	public void SetActionControlsRectHeight(float height)
	{
		m_rectActionControls.height = height;
		m_rectActionControls.y = m_rectBottomTray.y - m_rectActionControls.height - Styles.roundDarkBox.margin.bottom - m_styleBottomTray.margin.top;
	}
	
	
	/// <summary>
	/// Allocation for text input in a numeric field.
	/// </summary>
	static string m_numberInputTxt;
	/// <summary>
	/// Draws a numeric input field.
	/// </summary>
	/// <returns>
	/// The value in the numeric input field.
	/// </returns>
	/// <param name='currentValue'>
	/// Current value.
	/// </param>
	public static int DrawNumericInputField(int currentValue)
	{
		GUI.skin.settings.cursorColor = Styles.inputField.normal.textColor;
		m_numberInputTxt = GUILayout.TextField(currentValue.ToString(), Styles.inputField);
		int i;
		if (int.TryParse(m_numberInputTxt, out i)) return i;
		else return currentValue;
	}
	
	/// <summary>
	/// Gets the height of the status line.
	/// </summary>
	/// <value>
	/// The height of the status line.
	/// </value>
	public static float statusLineHeight { get { return Styles.roundDarkBox.padding.bottom+Styles.roundDarkBox.padding.top+GUIHelpers.GetFontSizeFromName(Styles.mediumTextLight.font)/*use.styles.mediumTextLight.fontSize*/; } }
	
	/// <summary>
	/// Displays a status line at the bottom with the specified message
	/// </summary>
	/// <param name="message">
	/// A <see cref="System.String"/>
	/// </param>
	public static void DisplayStatusLine(string message)
	{
		DisplayStatusLine(message, BottomTrayGUI.use.styleStatusLineText);
	}
	/// <summary>
	/// Displays a status line at the bottom with the specified message
	/// </summary>
	/// <param name="message">
	/// A <see cref="System.String"/>
	/// </param>
	/// <param name="textStyle">
	/// A <see cref="GUIStyle"/>
	/// </param>
	public static void DisplayStatusLine(string message, GUIStyle textStyle)
	{
		GUILayout.BeginHorizontal(BottomTrayGUI.use.styleStatusLineBG, GUILayout.Height(statusLineHeight));
		{
			GUILayout.Label(message, textStyle);
		}
		GUILayout.EndHorizontal();
	}
	

	/// <summary>
	/// The cash readout icon.
	/// </summary>
	public Texture2D cashReadoutIcon;
	
	#region ActionButtons 
	/// <summary>
	/// The action buttons.
	/// </summary>
	private GUIContent[] m_actionButtons = null;
	/// <summary>
	/// Flag specifying whether the action buttons are initialized.
	/// </summary>
	private bool m_areActionButtonsInitialized = false;
	/// <summary>
	/// The available actions.
	/// </summary>
	public PlayerAction[] AvailableActions{get; private set;}
	
	
	/// <summary>
	/// Initializes the action buttons.
	/// </summary>
	private void InitializeActionButtons()
	{
		List<GUIContent> actionButtons = new List<GUIContent>();
		List<PlayerAction> actions = new List<PlayerAction>();
		foreach (PlayerAction a in Player.current.actions) {
			actionButtons.Add(new GUIContent(a.icon, a.tooltip));
			actions.Add(a);
		}
		m_actionButtons = actionButtons.ToArray();
		AvailableActions = actions.ToArray();
		m_areActionButtonsInitialized = true;
	}
	
	
	/// <summary>
	/// Update this instance.
	/// </summary>
	private void UpdateActionButtons()
	{
		// initialize the action buttons as needed
		if (!m_areActionButtonsInitialized && Player.current != null) {
			InitializeActionButtons();
		}
		
		//
		if(isActionDisabled || turnStatus){
			InputManager.use.SetCurrentAction(AvailableActions[0]);
		}
		
	}
	
	
	/// <summary>
	/// disable Action Panel
	/// </summary>
	[HideInInspector]
	public bool isActionDisabled {get{
			if(Player.current == null){
				return true;
			}
			
			if(Player.current.isActionRunning || turnStatus){
				return true;}
			return false;
		}}
	#endregion
	
	/// <summary>
	/// Displays the tray at the bottom of the screen with action buttons, etc.
	/// </summary>
	public override void Draw()
	{	
		UpdateActionButtons();
		// display current action controls
		/*
		if (InputManager.use.currentAction != null) {
			InputManager.use.currentAction.DisplayControls();
		}*/
		// create bottom tray
		GUILayout.BeginArea(m_rectBottomTray, m_styleBottomTray);
		{			
			GUILayout.BeginVertical();
			{
				// cash readout and buttons
				GUILayout.BeginHorizontal();
				{
					float hgt = Player.current.actions[0].icon.height;
					// cash readout
					GUILayout.BeginVertical();
					{
						GUILayout.FlexibleSpace();
						GUILayout.BeginHorizontal();
						{
							GUILayout.Box(cashReadoutIcon, Styles.empty4, GUILayout.Height(hgt));
							GUILayout.FlexibleSpace();
							GUILayout.Label(
								new GUIContent(string.Format("Current {0:c} | Pending {1:c}", Player.current.balance, Player.current.pendingBalance), "Your current funds."),
								Styles.elegantTextDark,
								GUILayout.Height(hgt)
							);
						}
						GUILayout.EndHorizontal();
						GUILayout.FlexibleSpace();
					}
					GUILayout.EndVertical();
					// dividing line
					Styles.DrawLine(GUIStyles.LineDirection.Vertical, GUIStyles.LineColor.Dark, hgt);
					DrawButtons();
				}
				GUILayout.EndHorizontal();
				GUILayout.FlexibleSpace();
				
				// status line
				DisplayStatusLine(
					!string.IsNullOrEmpty(GameGUIManager.use.ErrorMessage)?
					GameGUIManager.use.ErrorMessage:
					GUI.tooltip, (GameGUIManager.use.ErrorMessage.Length>0)?styleStatusLineError:styleStatusLineText
				);
			}
			GUILayout.EndVertical();
		}
		GUILayout.EndArea();
	}
	
	/// <summary>
	/// The k paint selection input mode.
	/// </summary>
	public static string kPaintSelectionInputMode = "SelectionActionChanged";
	/// <summary>
	/// The current input mode , set this up to prevent flickering
	/// </summary>
	private int currentInputMode=1;
	public int inputModeSelection {get{return currentInputMode;} set{currentInputMode=value;}}
	float m_squareButtonSize { get { return 32f; } }
	/// <summary>
	/// Draws the buttons.
	/// </summary>
	void DrawButtons()
	{
		if (GUILayout.Button(
				new GUIContent(
					InputManager.paintSelectionInputMode==InputManager.PaintSelectionInputMode.Pan? 
			GameGUIManager.use.panSelectionIconOn :GameGUIManager.use.panSelectionIconOff,
					"Pan camera."
				), InputManager.paintSelectionInputMode == InputManager.PaintSelectionInputMode.Pan? Styles.smallButtonDisabled: Styles.smallButton,
				GUILayout.Height(m_squareButtonSize), GUILayout.Width(m_squareButtonSize)
			)
		){
			if(currentInputMode != 0){
				Messenger<int>.Broadcast(kPaintSelectionInputMode, 0);
				currentInputMode = 0;
			}
			InputManager.paintSelectionInputMode = InputManager.PaintSelectionInputMode.Pan;
		}
		if (GUILayout.Button(
				new GUIContent(
					InputManager.paintSelectionInputMode==InputManager.PaintSelectionInputMode.Select?
			GameGUIManager.use.selectResourceTileIconOn:GameGUIManager.use.selectResourceTileIconOff,
					"Select resource tiles."
				), InputManager.paintSelectionInputMode==InputManager.PaintSelectionInputMode.Select? Styles.smallButtonDisabled: Styles.smallButton,
				GUILayout.Height(m_squareButtonSize), GUILayout.Width(m_squareButtonSize)
			)
		) {
			if(currentInputMode != 1){
				Messenger<int>.Broadcast(kPaintSelectionInputMode, 1);
				currentInputMode = 1;
			}
			InputManager.paintSelectionInputMode = InputManager.PaintSelectionInputMode.Select;
		}
		if (GUILayout.Button(
				new GUIContent(
					InputManager.paintSelectionInputMode==InputManager.PaintSelectionInputMode.Deselect?
			GameGUIManager.use.deselectResourceTileIconOn:GameGUIManager.use.deselectResourceTileIconOff,
					"Deselect resource tiles."
				), InputManager.paintSelectionInputMode==InputManager.PaintSelectionInputMode.Deselect?Styles.smallButtonDisabled: Styles.smallButton,
				GUILayout.Height(m_squareButtonSize), GUILayout.Width(m_squareButtonSize)
			)
		) {
			if(currentInputMode != 2){
				Messenger<int>.Broadcast(kPaintSelectionInputMode, 2);
				currentInputMode = 2;
			}
			InputManager.paintSelectionInputMode = InputManager.PaintSelectionInputMode.Deselect;
		}
		if (GUILayout.Button(
				new GUIContent(GameGUIManager.use.clearResourceTileSelectionIcon, "Clear resource tile selection."),
				Styles.smallButton,
				GUILayout.Height(m_squareButtonSize), GUILayout.Width(m_squareButtonSize)
			)
		) {
			InputManager.use.ClearResourceTileSelection();
			Messenger.Broadcast("Clear Current Tiles");
		}
	}
	
	/// <summary>
	/// Determines whether this instance is mouse over.
	/// </summary>
	/// <returns>
	/// <c>true</c> if this instance is mouse over; otherwise, <c>false</c>.
	/// </returns>
	public override bool IsMouseOver(){
		if (m_rectBottomTray.Contains(GUIHelpers.MouseToGUIPosition( Input.mousePosition)) ||
			m_rectActionControls.Contains(GUIHelpers.MouseToGUIPosition(Input.mousePosition))){
			return true;
		}
		else
			return false;
	}
}

