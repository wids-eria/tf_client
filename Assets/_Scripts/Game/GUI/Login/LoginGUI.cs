#define FOR_GLS
#define FOR_JONGEE

using UnityEngine;
using System.Collections.Generic;

public class LoginGUI : MonoBehaviour
{
	[SerializeField]
	protected GUILayoutAreaStyle	backgroundStyle;
	
	[SerializeField]
	protected GUILayoutAreaStyle	clientBackgroundStyle;
	
	[SerializeField]
	protected GUIBoxStyle	bannerStyle;
	
	internal string m_loginName = "";
	
	internal string m_loginPassword = "";
	
	internal Vector2 m_scrollWorldSelector = Vector2.zero;
	/// <summary>
	/// The index of the selected world.
	/// </summary>
	internal int m_selectedWorldIndex {
		get { return m_selectedWorldIndexBackingField; }
		set {
			UserData.worldNumber = playerData[value].world_id;
			m_selectedWorldIndexBackingField = value;
		}
	}
	private int m_selectedWorldIndexBackingField = 0;
	/// <summary>
	/// The player data.
	/// </summary>
	public static PlayersData 	playerData = null;
	public static WorldsData	worlds = null;
	
	public GUIStyles styles = null;
	
	[SerializeField]
	protected LoginState loginState;// = new LoginState();
	
	[SerializeField]
	protected SelectWorldState selectWorldState;// = new SelectWorldState();
	
	[SerializeField]
	protected CreateGameState	createGameState;// = new CreateGameState();
	
	[SerializeField]
	protected SelectClassState	selectClassState;// = new SelectClassState();
	
	[SerializeField]
	protected WaitingToEnterGameState	waitingToEnterGame;// = new WaitingToEnterGameState();
	
	protected StateMachine<LoginGUI>	behavior = new StateMachine<LoginGUI>();
	
	protected virtual void	InitBehaviors() {		
		Transition<LoginGUI> loginToSelectWorld = new Transition<LoginGUI>( this, selectWorldState );
		Transition<LoginGUI> selectWorldToLogin = new Transition<LoginGUI>( this, loginState );
		Transition<LoginGUI> createWorldToSelectWorld = new Transition<LoginGUI>( this, selectWorldState );
		Transition<LoginGUI> selectClassToWaitingToEnterGame = new Transition<LoginGUI>( this, waitingToEnterGame );
		Transition<LoginGUI> createWorldToSelectClass = new Transition<LoginGUI>( this, selectClassState );
		
		loginState.AddTransition( loginToSelectWorld );
		selectWorldState.AddTransition( selectWorldToLogin );
		selectClassState.AddTransition( selectClassToWaitingToEnterGame );
		
		loginToSelectWorld.InitFromExpression( "{ TransitionBehavior_OnState isLoggedIn( LoggedIn ); } isLoggedIn" );
		selectWorldToLogin.InitFromExpression( "{ TransitionBehavior_OnState isLoggedIn( LoggedIn ); } !isLoggedIn" );
		createWorldToSelectWorld.InitFromExpression( "{ TransitionBehavior_OnEvent onCancel( OnCancelCreateGame ); } onCancel" );
		createWorldToSelectClass.InitFromExpression( "{ TransitionBehavior_OnEvent onSelectClass( OnSelectClass ); } onSelectClass" );
		selectClassToWaitingToEnterGame.InitFromExpression( "{ TransitionBehavior_OnEvent onJoinGame( OnJoinGame ); } onJoinGame" );
		
#if FOR_GLS
		Transition<LoginGUI> selectWorldToSelectClass = new Transition<LoginGUI>( this, selectClassState );
		Transition<LoginGUI> selectClassToSelectWorld = new Transition<LoginGUI>( this, selectWorldState );

		selectWorldState.AddTransition( selectWorldToSelectClass );
		selectClassState.AddTransition( selectClassToSelectWorld );
		
		selectWorldToSelectClass.InitFromExpression( "{ TransitionBehavior_OnEvent onCreateGame( OnCreateGame ); TransitionBehavior_OnEvent onJoinGame( OnJoinGame ); } onCreateGame | onJoinGame" );
		selectClassToSelectWorld.InitFromExpression( "{ TransitionBehavior_OnEvent onCancel( OnCancelSelectClass ); } onCancel" );
#else
		Transition<LoginGUI> selectWorldToCreateWorld = new Transition<LoginGUI>( this, createGameState );
		Transition<LoginGUI> selectClassToCreateWorld = new Transition<LoginGUI>( this, createGameState );
		selectWorldState.AddTransition( selectWorldToCreateWorld );
		createGameState.AddTransition( createWorldToSelectWorld );
		createGameState.AddTransition( createWorldToSelectClass );
		selectClassState.AddTransition( selectClassToCreateWorld );
		selectWorldToCreateWorld.InitFromExpression( "{ TransitionBehavior_OnEvent onCreateGame( OnCreateGame ); } onCreateGame" );
		selectClassToCreateWorld.InitFromExpression( "{ TransitionBehavior_OnEvent onCancel( OnCancelSelectClass ); } onCancel" );
#endif

		behavior.ChangeState( loginState, this );
	}
	
	void Awake()
	{
		MessengerAM.Listen( MessengerAM.listenTypeConfig, this );
	}

	void Start()
	{		
		// initialize from prefs
		m_loginName = GamePrefs.GetUserName();
		InitBehaviors();
		
#if FOR_JONGEE
		GamePrefs.SetCharacterClass( Player.CharacterClass.TimberCompany );
#endif
		
	}
	
	internal bool m_hasFocus = false;
	
	/// <summary>
	/// Handle screen resolution changed event.
	/// </summary>
	/// <param name='msg'>
	/// Message.
	/// </param>
	public void _ScreenResolutionChanged(MessageScreenResolutionChanged msg)
	{
		//InitializeGUIRects();
	}
	
	void OnGUI()
	{
		// set cursor color
		GUI.skin.settings.cursorColor = styles.inputField.normal.textColor;
		
		// draw contents
		backgroundStyle.Begin();
			clientBackgroundStyle.Begin();
				behavior.OnGUI( this );
			clientBackgroundStyle.End();
		backgroundStyle.End();
		
		GUI.Box( bannerStyle.DrawRect, bannerStyle.Background );
	}
	
	/// <summary>
	/// the control name of the password field
	/// </summary>
	internal readonly string m_passwordFieldCtrlName = "password_field";
	
	internal bool 	LoggedIn() {
		return UserData.current != null && LoginGUI.playerData != null && LoginGUI.worlds != null;
	}

	public TransitionEventDelegate	OnCancelCreateGame;
	public TransitionEventDelegate	OnCreateGame;
	public TransitionEventDelegate	OnSelectClass;
	public TransitionEventDelegate	OnCancelSelectClass;
	public TransitionEventDelegate	OnJoinGame;
	
	internal void Register()
	{
		Application.OpenURL(WebRequests.urlRegisterNewPlayer);
	}
}