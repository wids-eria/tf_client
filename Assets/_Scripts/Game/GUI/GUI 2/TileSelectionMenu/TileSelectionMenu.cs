using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileSelectionMenu : MonoBehaviour
{
	public const string kClose = "CloseTileSelectionMenu";
	
	
	public Camera controllerCamera;
	[HideInInspector]public Vector3 prevPosition;
	
	public AnimationCurve curve;
	
	public float animateTimeInSeconds 	= 0.5f;
	
	public GUIButtonData actionButtonTemplate;
	
	public List<GUIButtonData> optionButtons;
	
	public GUICircularData optionSelectionOptions;
	
	[HideInInspector]public GUIContainer menuGroup;
	
	//public static Vector3 baseMenuPos { get {return GUIHelpers.GUIToWorldPosition(new Vector2(Screen.width/2,Screen.height)); } }
	
	/*
	 * 
	 * State Machine
	 * 
	 */
	
	protected StateMachine<TileSelectionMenu> behaviour = new StateMachine<TileSelectionMenu>();
	
	protected Dictionary<string,State<TileSelectionMenu>> states = new Dictionary<string, State<TileSelectionMenu>>() {
		{ "Waiting", 	new TSMWaitForSelection() },
		{ "Options", 	new TSMOptionMenu() },
	};
	
	void Awake() {
		
		controllerCamera = Camera.mainCamera;
		
		//Setup Messenger
		Messenger.AddListener(InputManager.kMegatileCapMet, 			OnOpenMenu);
		Messenger<Vector3>.AddListener(InputManager.kRightClickedTile, 	OnCloseMenu);
		Messenger.AddListener(kClose,									OnCloseMenu);
		
		//Setup GUIGroup
		menuGroup = new GUIContainer(controllerCamera.pixelRect,false);
		menuGroup.anchor.Set(GUIAnchor.BottomCenter);  
		
		GameGUIManager.AddGUIObject(menuGroup);
		
		//Setup Transitions and State Machine
		Transition<TileSelectionMenu> goToOptions = new Transition<TileSelectionMenu>(this, states["Options"]);
		Transition<TileSelectionMenu> closeMenu = new Transition<TileSelectionMenu>(this, states["Waiting"]);
		
		states["Waiting"].AddTransition(goToOptions);
		states["Options"].AddTransition(closeMenu);
		
		goToOptions.InitFromExpression	( "{ TransitionBehavior_OnEvent openMenu( OpenMenu ); } openMenu");
		closeMenu.InitFromExpression	( "{ TransitionBehavior_OnEvent closeMenu( CloseMenu ); } closeMenu");
		
		behaviour.ChangeState( states["Waiting"], this );
	}
	void OnDestroy() {
		Messenger.RemoveListener(InputManager.kMegatileCapMet, 				OnOpenMenu);
		Messenger<Vector3>.RemoveListener(InputManager.kRightClickedTile, 	OnCloseMenu);
		Messenger.RemoveListener(kClose,									OnCloseMenu);
	}
	
	void OnGUI() {
		behaviour.OnGUI(this);
	}
	
	void OnOpenMenu() {		
		//Get the center of the megatile selection
		Region region = new Region(InputManager.use.resourceTileSelection.ToArray());
		Vector3 center = Vector3.zero;
		region.GetCenter(out center);
		
		//Open the menu
		OpenMenu(center,center);
	}
	void OnCloseMenu(Vector3 p) {
		CloseMenu();
	}
	void OnCloseMenu() {
		CloseMenu();
	}
	
	public TransitionEventDelegate<Vector3,Vector3>	OpenMenu;
	public TransitionEventDelegate	CloseMenu;
}

