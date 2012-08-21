using UnityEngine;
using System.Collections;

public class TSMOptionMenu : State<TileSelectionMenu, Vector3, Vector3>  //< xxx, MOUSE type, WORLD type >
{
	
	public const string kOptionButton = "OptionButtonPressed";
	public const string kSurveyButton = "SurveyButtonPressed";
	public const string kPurchaseButton = "PurchaseButtonPressed";
	
	public const int kCostBuy = 100;
	public const int kCostSurvey = 25;
	public const int kCostAction = 45;
	
	public GUICircularMenu  optionMenu;
	public GUILabelCentered optionLabel;
	
	public float baseAngle = 90;
	
	public Vector2 labelOffset;
	
	public TileSelectionMenu owner;
	
	
	//HACK -- These should not be tied directly to the option menu
	protected GUISquareButton buttonSurvey 	{ get { return optionMenu.buttons[0]; } }
	protected GUISquareButton buttonBuy 	{ get { return optionMenu.buttons[1]; } }
	protected GUISquareButton buttonAction 	{ get { return optionMenu.buttons[2]; } }
	
	
	//* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 
	//
	//               State Machine Code
	//
	//* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 
	
	
	protected StateMachine<TSMOptionMenu> stateMachine = new StateMachine<TSMOptionMenu>();
	
	protected TSMOptionOpen stateOpen = new TSMOptionOpen();
	protected TSMActionMenu stateAction = new TSMActionMenu();
	protected TSMBuyMenu stateBuy = new TSMBuyMenu();
	protected TSMSurveyMenu stateSurvey = new TSMSurveyMenu();
	protected TSMSurveyResults stateSurveyResults = new TSMSurveyResults();
	protected TSMWaitStateSurvey waitState = new TSMWaitStateSurvey();
	protected State<TSMOptionMenu, GUICircularMenu, TileSelectionMenu> loadSurveyWaitState = new State<TSMOptionMenu, GUICircularMenu, TileSelectionMenu>();
	
	public TransitionEventDelegate<TileSelectionMenu, GUICircularMenu>  OpenOptionsMenu;
	public TransitionEventDelegate<GUICircularMenu, TileSelectionMenu>	OpenActionMenu;
	public TransitionEventDelegate<GUICircularMenu, TileSelectionMenu>	OpenBuyMenu;
	public TransitionEventDelegate<GUICircularMenu, TileSelectionMenu>	OpenWaitState;
	//public TransitionEventDelegate<GUICircularMenu, TileSelectionMenu>	WaitToSurvey;
	public TransitionEventDelegate<GUICircularMenu, TileSelectionMenu>	BuyToWait;
	public TransitionEventDelegate<GUICircularMenu, TileSelectionMenu>	BuyToAction;
	public TransitionEventDelegate<GUICircularMenu, TileSelectionMenu>	ActionToWait;
	public TransitionEventDelegate<GUICircularMenu, TileSelectionMenu>	ActionToBuy;
	public TransitionEventDelegate<GUICircularMenu, TileSelectionMenu>	SurveyToAction;
	public TransitionEventDelegate<GUICircularMenu, TileSelectionMenu>	SurveyToBuy;
	public TransitionEventDelegate<GUICircularMenu, TileSelectionMenu>	SurveyToLoadSurvey;
	public TransitionEventDelegate<GUICircularMenu, TileSelectionMenu>	ResultsToAction;
	public TransitionEventDelegate<GUICircularMenu, TileSelectionMenu>	ResultsToBuy;
	public TransitionEventDelegate<GUICircularMenu, TileSelectionMenu>	ResultsToSurvey;
		
	public TransitionEventDelegate	CloseAction;
	public TransitionEventDelegate	CloseAll;
	
	
	//* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 
	//
	//               Management Etc.
	//
	//* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 
	
	
	public override void Enter (TileSelectionMenu self, State<TileSelectionMenu> prevState) {
		base.Enter(self, prevState);
		
		owner = self;
		self.menuGroup.visible = true;
		self.menuGroup.position.Set(GUIHelpers.WorldToGUIPosition(Parm2));
		self.prevPosition = self.controllerCamera.transform.position;
		
		InitMenu(self);
		InitButtonDelegates(self);
		InitStateMachine(self);		
	}
	
	
	protected void InitStateMachine(TileSelectionMenu self) {
		//TRANSITIONS
		Transition<TSMOptionMenu> openOptions =  new Transition<TSMOptionMenu>(this, stateOpen);
		
		Transition<TSMOptionMenu> openToAction = new Transition<TSMOptionMenu>(this, stateAction);
		Transition<TSMOptionMenu> openToBuy =    new Transition<TSMOptionMenu>(this, stateBuy);
		Transition<TSMOptionMenu> openToWaitSurvey = new Transition<TSMOptionMenu>(this, waitState);
		
		Transition<TSMOptionMenu> waitToSurvey = new Transition<TSMOptionMenu>(this, stateSurvey);
		
		Transition<TSMOptionMenu> buyToWait =  new Transition<TSMOptionMenu>(this, waitState);
		Transition<TSMOptionMenu> buyToAction =  new Transition<TSMOptionMenu>(this, stateAction);
		
		Transition<TSMOptionMenu> actionToWait =  new Transition<TSMOptionMenu>(this, waitState);
		Transition<TSMOptionMenu> actionToBuy =  new Transition<TSMOptionMenu>(this, stateBuy);
		
		Transition<TSMOptionMenu> surveyToAction =  new Transition<TSMOptionMenu>(this, stateAction);
		Transition<TSMOptionMenu> surveyToBuy =  new Transition<TSMOptionMenu>(this, stateBuy);
		Transition<TSMOptionMenu> surveyToLoadSurvey = new Transition<TSMOptionMenu>(this, loadSurveyWaitState);
		
		Transition<TSMOptionMenu> loadSurveyToResults =  new Transition<TSMOptionMenu>(this, stateSurveyResults);
		
		Transition<TSMOptionMenu> resultsToAction =  new Transition<TSMOptionMenu>(this, stateAction);
		Transition<TSMOptionMenu> resultsToBuy =  new Transition<TSMOptionMenu>(this, stateBuy);
		Transition<TSMOptionMenu> resultsToSurvey =  new Transition<TSMOptionMenu>(this, stateSurvey);
							
		//ADD TRANSITIONS TO STATES
		stateOpen.AddTransition(openToAction);
		stateOpen.AddTransition(openToBuy);
		stateOpen.AddTransition(openToWaitSurvey);
		
		waitState.AddTransition(waitToSurvey);
		
		stateBuy.AddTransition(buyToWait);
		stateBuy.AddTransition(buyToAction);
		
		stateAction.AddTransition(actionToWait);
		stateAction.AddTransition(actionToBuy);
		
		stateSurvey.AddTransition(surveyToAction);
		stateSurvey.AddTransition(surveyToBuy);
		stateSurvey.AddTransition(surveyToLoadSurvey);
		
		loadSurveyWaitState.AddTransition(loadSurveyToResults);
		
		stateSurveyResults.AddTransition(resultsToAction);
		stateSurveyResults.AddTransition(resultsToBuy);
		stateSurveyResults.AddTransition(resultsToSurvey);
				
		//INIT TRANSITIONS
		openOptions.InitFromExpression		( "{ TransitionBehavior_OnEvent openOptions			( OpenOptionsMenu ); } openOptions");
		
		openToAction.InitFromExpression		( "{ TransitionBehavior_OnEvent openActionMenu		( OpenActionMenu  ); } openActionMenu");
		openToBuy.InitFromExpression		( "{ TransitionBehavior_OnEvent openBuyMenu   		( OpenBuyMenu     ); } openBuyMenu");
		openToWaitSurvey.InitFromExpression	( "{ TransitionBehavior_OnEvent openToWaitSurvey	( OpenWaitState   ); } openToWaitSurvey");
		
		waitToSurvey.InitFromExpression		( "{ TransitionBehavior_OnState waitToSurveyMenu	( WaitToSurvey    ); } waitToSurveyMenu");
		
		buyToWait.InitFromExpression		( "{ TransitionBehavior_OnEvent buyToWaitSurveyMenu ( BuyToWait       ); } buyToWaitSurveyMenu");
		buyToAction.InitFromExpression		( "{ TransitionBehavior_OnEvent buyToActionMenu		( BuyToAction     ); } buyToActionMenu");
		
		actionToWait.InitFromExpression		( "{ TransitionBehavior_OnEvent actionToWaitSurveyMenu	( ActionToWait ); } actionToWaitSurveyMenu");
		actionToBuy.InitFromExpression		( "{ TransitionBehavior_OnEvent actionToBuyMenu		( ActionToBuy     ); } actionToBuyMenu");
		
		surveyToAction.InitFromExpression	( "{ TransitionBehavior_OnEvent surveyToActionMenu	( SurveyToAction  ); } surveyToActionMenu");
		surveyToBuy.InitFromExpression		( "{ TransitionBehavior_OnEvent surveyToBuyMenu		( SurveyToBuy     ); } surveyToBuyMenu");
		surveyToLoadSurvey.InitFromExpression	( "{ TransitionBehavior_OnEvent surveyToLoadSurvey	( SurveyToLoadSurvey ); } surveyToLoadSurvey");
		
		loadSurveyToResults.InitFromExpression	( "{ TransitionBehavior_OnState loadSurveyToResults	( LoadSurveyToResults ); } loadSurveyToResults");
		
		resultsToAction.InitFromExpression	( "{ TransitionBehavior_OnEvent resultsToActionMenu	( ResultsToAction ); } resultsToActionMenu");
		resultsToBuy.InitFromExpression		( "{ TransitionBehavior_OnEvent resultsToBuyMenu	( ResultsToBuy    ); } resultsToBuyMenu");
		resultsToSurvey.InitFromExpression	( "{ TransitionBehavior_OnEvent resultsToSurveyMenu	( ResultsToSurvey ); } resultsToSurveyMenu");
		
		//DEFAULT STATE MACHINE STATE
		stateOpen.Parm1 = self;
		stateOpen.Parm2 = optionMenu;
		stateMachine.ChangeState(stateOpen,this);
	}
	
	protected bool 	WaitToSurvey() {		
		return InputManager.use.HasCachedSurvey(InputManager.use.GetSelectedMegatileIds()[0]);
	}
	
	protected bool 	LoadSurveyToResults() 
	{
		if(Survey.current != null)
			return Survey.current.loaded;
		else
			return false;
	}
	
	protected void InitButtonDelegates(TileSelectionMenu self)
	{		
		optionMenu.buttons[0].OnButtonPressed += SurveyButtonPressed;
		optionMenu.buttons[1].OnButtonPressed += PurchaseButtonPressed;
		optionMenu.buttons[2].OnButtonPressed += ActionButtonPressed;
		
		Messenger<GUIButtonBase>.AddListener(kSurveyButton,OnSurveyButtonPressed);
		Messenger<GUIButtonBase>.AddListener(kPurchaseButton,OnPurchaseButtonPressed);
		Messenger<GUIButtonBase>.AddListener(kOptionButton,OnActionButtonPressed);
		
		buttonAction.enabled = IsSelectionOwnedByPlayer(InputManager.use.GetSelectionAsResourceTiles());
		buttonBuy.enabled = !IsSelectionOwned(InputManager.use.GetSelectionAsResourceTiles());//!buttonAction.enabled;
	}
	
	public void BoughtLand()
	{
		optionMenu.radioGroup.SetSelected(buttonAction);
		buttonAction.enabled = true;
		buttonAction.SetSelected(true);
		buttonBuy.enabled = false;
		buttonBuy.SetSelected(true);
	}
	
	//HACK?
	protected bool IsSelectionOwned(ResourceTileLite[] selection) {
		bool isOwned = true;
		foreach(ResourceTileLite tile in selection) {
			if (tile.idOwner == -1) {
				isOwned = false; break;
			}
		}
		return isOwned;
	}
	protected bool IsSelectionOwnedByPlayer(ResourceTileLite[] selection) {
		bool isOwned = true;
		foreach(ResourceTileLite tile in selection) {
			if (tile.idOwner != Player.current.id) {
				isOwned = false; break;
			}
		}
		return isOwned;
	}
	
	//HACK
	protected bool IsSelectionSurveyed(ResourceTileLite[] selection) {
		bool isSurveyed = true;
		foreach(ResourceTileLite tile in selection) {
			//HACK
			if (tile.isSurveyed != true) {
				isSurveyed = false; break;
			}
		}
		return isSurveyed;
	}
	
	protected void SurveyButtonPressed(GUIButtonBase button) {
		Messenger<GUIButtonBase>.Broadcast(kSurveyButton,button);
	}
	
	protected void PurchaseButtonPressed(GUIButtonBase button) {
		Messenger<GUIButtonBase>.Broadcast(kPurchaseButton,button);
	}
	
	protected void ActionButtonPressed(GUIButtonBase button) {
		Messenger<GUIButtonBase>.Broadcast(kOptionButton,button);
	}
		
	protected void OnSurveyButtonPressed(GUIButtonBase button) {
		if(!InputManager.use.HasCachedSurvey(InputManager.use.GetSelectedMegatileIds()[0]))
		{
			PastSurveyCoroutine co = new PastSurveyCoroutine(owner);
			co.Start(owner);
		}
		if(stateMachine.CurrentState == stateOpen)
		{
			OpenWaitState(optionMenu,owner);
		}
		else if(stateMachine.CurrentState == stateAction)
		{
			ActionToWait(optionMenu,owner);	
		}
		else if(stateMachine.CurrentState == stateBuy)
		{
			BuyToWait(optionMenu,owner);
		}
		else if(stateMachine.CurrentState == stateSurveyResults)
		{
			ResultsToSurvey(optionMenu,owner);
		}
	}
	
	protected void OnPurchaseButtonPressed(GUIButtonBase button) {
		if(stateMachine.CurrentState == stateOpen)
		{
			OpenBuyMenu(optionMenu,owner);
		}
		else if(stateMachine.CurrentState == stateAction)
		{
			ActionToBuy(optionMenu,owner);	
		}
		else if(stateMachine.CurrentState == stateSurvey)
		{
			SurveyToBuy(optionMenu,owner);
		}
		else if(stateMachine.CurrentState == stateSurveyResults)
		{
			ResultsToBuy(optionMenu,owner);
		}
	}
	
	protected void OnActionButtonPressed(GUIButtonBase button) {
		if(stateMachine.CurrentState == stateOpen)
		{
			OpenActionMenu(optionMenu,owner);
		}
		else if(stateMachine.CurrentState == stateBuy)
		{
			BuyToAction(optionMenu,owner);	
		}
		else if(stateMachine.CurrentState == stateSurvey)
		{
			SurveyToAction(optionMenu,owner);
		}
		else if(stateMachine.CurrentState == stateSurveyResults)
		{
			ResultsToAction(optionMenu,owner);
		}
	}
		
	protected void InitMenu(TileSelectionMenu self)
	{
		labelOffset = new Vector2(0,40);
		
		optionMenu = new GUICircularMenu(self.optionButtons,self.optionSelectionOptions);
		optionMenu.position.Set((Vector2)GUIHelpers.WorldToGUIPosition(Parm1));  
		
		//Add these objects to the group
		self.menuGroup.AddElement(optionMenu);
		//self.menuGroup.AddElement(optionMenu,optionLabel);
	}
	
	
	public override void OnGUI (TileSelectionMenu self)
	{
		Vector3 pos = self.controllerCamera.transform.position;
		if (pos != self.prevPosition) {
			self.menuGroup.position.Set(GUIHelpers.WorldToGUIPosition(Parm2));
		}
		self.prevPosition = pos;       
		
		stateMachine.OnGUI(this);
	}
	
	public override void Exit (TileSelectionMenu self, State<TileSelectionMenu> nextState)
	{
		base.Exit (self, nextState);
		
		self.menuGroup.RemoveElement(optionMenu);
		stateMachine.ChangeState(null,this);
		
		//Get rid of the listeners
		Messenger<GUIButtonBase>.RemoveListener(kSurveyButton,		OnSurveyButtonPressed);
		Messenger<GUIButtonBase>.RemoveListener(kPurchaseButton,	OnPurchaseButtonPressed);
		Messenger<GUIButtonBase>.RemoveListener(kOptionButton,		OnActionButtonPressed);
	}
}

