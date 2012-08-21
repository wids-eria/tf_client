using UnityEngine;
using System.Collections;

public class ProjectionInterfaceGUI : GUIObject {
	
	/// <summary>
	/// The rect for projection interface GUI.
	/// </summary>
	[HideInInspector]
	public Rect projectionInterfaceGUIRect = new Rect();
	/// <summary>
	/// Flag to decide whether to display this GUI or not
	/// </summary>
	public bool isProjectionGUIVisible;
	/// <summary>
	/// This activates game dock and display it.
	/// </summary>
	protected bool ActivateGameDock = false;
	/// <summary>
	/// The current view section of the scroll view.
	/// </summary>
	[SerializeField]
	private Vector2 m_ProjectionSelectionScrollView = Vector2.zero;
	/// <summary>
	/// The m_current projection selection.
	/// </summary>
	private int m_currentProjectionSelection;
	/// <summary>
	/// The m_prev projection selection.
	/// </summary>
	private int m_prevProjectionSelection;
	/// <summary>
	/// The message for broadcast when option being changed on the selection grid.
	/// </summary>
	public const string kProjectionOptionChange = "Projection Option Changed";


	/// <summary>
	/// Gets or sets the projection selection options.
	/// </summary>
	/// <value>
	/// The projection selection options.
	/// </value>
	protected string[] ProjectionSelectionOptions{get{return DataMapController.dataMapNames.ToArray();}	}
	
	/// <summary>
	/// Initializes a new instance of the <see cref="ProjectionInterfaceGUI"/> class.
	/// </summary>
	public ProjectionInterfaceGUI(){
		Messenger.AddListener(GameDockGUI.kOnProjectionInterface, VisibleEnabler);
		projectionInterfaceGUIRect.x = Styles.roundDarkBox.margin.left;
		projectionInterfaceGUIRect.y = Styles.roundDarkBox.margin.top;
		projectionInterfaceGUIRect.width = 180f;
		projectionInterfaceGUIRect.height = Screen.height/2 - projectionInterfaceGUIRect.y 
			- 0.5f*(Styles.roundDarkBox.margin.top+Styles.roundDarkBox.margin.bottom);
		this.visible= false;
	}
	
	/// <summary>
	/// Releases unmanaged resources and performs other cleanup operations before the <see cref="ProjectionInterfaceGUI"/>
	/// is reclaimed by garbage collection.
	/// </summary>
	~ProjectionInterfaceGUI(){
		Messenger.RemoveListener(GameDockGUI.kOnProjectionInterface, VisibleEnabler);
	}
	
	/// <summary>
	/// Displays the projection interface GU.
	/// </summary>
	public override void Draw(){
		
		//sync both options before letting user to choose
		m_prevProjectionSelection = m_currentProjectionSelection;
		GUILayout.BeginArea(projectionInterfaceGUIRect,Styles.roundDarkBox);
		{
			GUILayout.Label("Projection Type", Styles.mediumTextHighlighted);
			Styles.DrawLine(GUIStyles.LineDirection.Horizontal,GUIStyles.LineColor.Light);
			
			m_ProjectionSelectionScrollView = GUILayout.BeginScrollView(m_ProjectionSelectionScrollView, false, true);
			{
				m_currentProjectionSelection = GUILayout.SelectionGrid(m_prevProjectionSelection,ProjectionSelectionOptions,1);
			}
			GUILayout.EndScrollView();
			GUILayout.FlexibleSpace();
			ActivateGameDock = GUILayout.Toggle (ActivateGameDock, string.Format("{0}",ActivateGameDock? "Bye Bye": "Go Back"), Styles.smallButton);
		}
		GUILayout.EndArea();
		//display map layout type if the user choose to
		if(m_currentProjectionSelection != m_prevProjectionSelection){
			Messenger<int>.Broadcast(kProjectionOptionChange, m_currentProjectionSelection - 1);
		}
		//hide this GUi and load game dock.
		if(ActivateGameDock){
			Messenger.Broadcast(GameDockGUI.kOnGameDock);
			this.visible = false;
			ActivateGameDock = false;
		}
	}
	
	/// <summary>
	/// Determines whether this instance is mouse over.
	/// </summary>
	/// <returns>
	/// <c>true</c> if this instance is mouse over; otherwise, <c>false</c>.
	/// </returns>
	public override bool IsMouseOver(){
		if (visible && projectionInterfaceGUIRect.Contains(GUIHelpers.MouseToGUIPosition( Input.mousePosition))){
			return true;
		}
		else
			return false;
	}
}
