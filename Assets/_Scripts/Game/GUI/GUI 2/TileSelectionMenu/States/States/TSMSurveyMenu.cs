using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TSMSurveyMenu : State<TSMOptionMenu, GUICircularMenu, TileSelectionMenu>
{
	protected TSMOptionMenu owner;
	GUICircularMenu optionMenu 	{ get { return owner.optionMenu; } }
	GUIContainer RequestSurveyMenuContainer;
	
	GUIButtonList pastSurveys;
	GUIRadioGroup pastSurveysGroup;
	List<Survey> listPastSurveys;
	
	float animationTime = 0.4f;
	
	Rect rect = new Rect(0,0,300,100);
	
	public override void Enter (TSMOptionMenu self, State<TSMOptionMenu> prevState)
	{
		base.Enter(self,prevState);
		owner = self;
				
		RequestSurveyMenuContainer = new GUIContainer(rect, true);
		RequestSurveyMenuContainer.visible = false;
		
		GUILabel pastLabel = new GUILabel("Previous Surveys", new Rect(-150,-100,150,100));
		RequestSurveyMenuContainer.AddElement(pastLabel);
		
		pastSurveys = new GUIButtonList(new Rect(-150,-85,150, 85),false);
		pastSurveysGroup = new GUIRadioGroup();
		listPastSurveys = InputManager.use.GetCachedSurveys(InputManager.use.GetSelectedMegatileIds()[0]);
		foreach(Survey pastSurvey in listPastSurveys)
		{
			GUIButtonBase button = new GUIButtonBase(new GUIButtonData());
			button.text = System.DateTime.Parse(pastSurvey.created_at).ToString("g");
			button.OnSelected += OnIndexChanged;
			pastSurveys.AddElement(button);
			pastSurveysGroup.AddItem(button);
		}	
		RequestSurveyMenuContainer.AddElement(pastSurveys);
		
		GUILabel newLabel = new GUILabel("New Survey", new Rect(0,-100,150,100));
		RequestSurveyMenuContainer.AddElement(newLabel);
		
		GUILabelCentered priceLabel = new GUILabelCentered(string.Format("Price : ${0}",TSMOptionMenu.kCostSurvey), new Rect(75,-42,150,85));
		RequestSurveyMenuContainer.AddElement(priceLabel);
				
		GUIButtonData purchaseButtonData = new GUIButtonData();
		purchaseButtonData.usePersonalArea = true;
		purchaseButtonData.x = 10;
		purchaseButtonData.y = -25;		
		purchaseButtonData.width = 130;
		purchaseButtonData.height = 100;
		purchaseButtonData.text = "Purchase";
		GUIButtonBase purchaseButton = new GUIButtonBase(purchaseButtonData);
		purchaseButton.OnButtonPressed += RequestSurvey;
		purchaseButton.enabled = GameManager.economyController.IsBalanceAvailable(TSMOptionMenu.kCostSurvey);
		RequestSurveyMenuContainer.AddElement(purchaseButton);
				
		if(prevState.GetType() == typeof(TSMOptionOpen))
		{
			AnimateCircularMenu();
		}
		else
		{
			RequestSurveyMenuContainer.visible = true;	
		}		
		
		RequestSurveyMenuContainer.anchor.Set(GUIAnchor.Bottom | GUIAnchor.Center);
		RequestSurveyMenuContainer.position.Set(optionMenu.position.Get() + new Vector2(0,-20));
		owner.owner.menuGroup.AddElement(RequestSurveyMenuContainer);	
	}
	
	protected void OnIndexChanged( ISelectable item ) {
		//Set the group visible based on selected index
		int index = pastSurveysGroup.IndexOf(item);
		Survey.current = listPastSurveys[index];
		Survey.current.loaded = true;
		owner.SurveyToLoadSurvey(optionMenu,owner.owner);
	}
	
	protected void RequestSurvey(GUIButtonBase button) 
	{
		button.enabled = false;
		if(Survey.current == null)
			Survey.current = new Survey();
		Survey.current.loaded = false;
		SurveyTileCoroutine co = new SurveyTileCoroutine(owner.owner);
		co.Start(owner.owner);	
		
		owner.SurveyToLoadSurvey(optionMenu,owner.owner);
	}
	
	protected void AnimateCircularMenu()
	{
		/* * * * * * * * * * * * * * * * * * * * * * *
		 * Animate Options Menu
		 * * * * * * * * * * * * * * * * * * * * * * */ 
		optionMenu.animationQueue.Enqueue(
			new GUIEffects.CircularMenuClose(optionMenu,animationTime/8f),
			new GUIEffects.CircularMenuRotate(optionMenu,0.1f,100f),
			new CommandList(	
				new GUIEffects.CircularMenuOpen(optionMenu,animationTime/1.25f),
				new GUIEffects.CircularMenuRotate(optionMenu,animationTime*1.25f,315f,owner.owner.curve),
				new GUIEffects.CircularMenuRadius(optionMenu,animationTime*1.5f,130f),
				new GUIEffects.CircularMenuArc(optionMenu,animationTime*1.5f,55f),
				new GUIEffects.CircularMenuButtonSize(optionMenu,0.5f,48f,owner.owner.curve)
			),
			new GUIEffects.SetVisibility(RequestSurveyMenuContainer, true)
		);	
	}
		
	public override void Exit (TSMOptionMenu self, State<TSMOptionMenu> nextState)
	{
		base.Exit (self, nextState);
		
		owner.owner.menuGroup.RemoveElement(RequestSurveyMenuContainer);
		
		RequestSurveyMenuContainer = null;
	}
	
	public override void OnGUI (TSMOptionMenu self)
	{
		base.OnGUI (self);
	}
}

public class PastSurveyCoroutine : WebCoroutine {
	Survey[] pastSurveys;
	
	public PastSurveyCoroutine( MonoBehaviour owner ) {
		AddExecutionHandler( OnExecuteStage1 );
		AddExecutionHandler( OnExecuteStage2 );
		AddExecutionHandler( OnExecuteStage3 );
	}
		
	protected override void	OnStart() {
		base.OnStart();
		SignalSystemBusy();
	}
	
	protected override void	BroadcastOnComplete() {
		base.BroadcastOnComplete();
		SignalSystemIdle();
	}
	
	protected IEnumerator	OnExecuteStage1( AWebCoroutine self ) {
		self.Request = new HTTP.Request( "Get", WebRequests.urlSurveyMegatile );
		self.Request.AddParameters( WebRequests.authenticatedParameters );
		self.Request.Send();
		
		while( !self.RequestIsDone ) {
			yield return 0;
		}
	}
	
	protected IEnumerator	OnExecuteStage2( AWebCoroutine self ) {
		Debug.Log (self.ResponseText);
		Func<string, Surveys> asyncDelegate = JSONDecoder.Decode<Surveys>;
		IAsyncResult ar = asyncDelegate.BeginInvoke( self.ResponseText, null, null );
		while( !ar.IsCompleted ) {
			yield return 0;
		}
		
		pastSurveys = asyncDelegate.EndInvoke( ar ).surveys;
		yield break;
	}
	
	protected IEnumerator	OnExecuteStage3( AWebCoroutine self ) {
		if(pastSurveys == null)
		{
			InputManager.use.InitCachedSurveyForMegaTile(InputManager.use.GetSelectedMegatileIds()[0]);	
		}
		else
		{
			if(pastSurveys.Length == 0)
			{
				InputManager.use.InitCachedSurveyForMegaTile(InputManager.use.GetSelectedMegatileIds()[0]);	
			}
			else
			{
				for(int i = 0; i < pastSurveys.Length; i++)
				{
					InputManager.use.AddCachedSurvey(InputManager.use.GetSelectedMegatileIds()[0], pastSurveys[i]);
				}
			}
		}	
			
		yield break;;
	}
}

public class SurveyTileCoroutine : WebCoroutine {
	Survey returnData;
	
	public SurveyTileCoroutine( MonoBehaviour owner ) {
		AddExecutionHandler( OnExecuteStage1 );
		AddExecutionHandler( OnExecuteStage2 );
		AddExecutionHandler( OnExecuteStage3 );
	}
		
	protected override void	OnStart() {
		base.OnStart();
		SignalSystemBusy();
	}
	
	protected override void	BroadcastOnComplete() {
		base.BroadcastOnComplete();
		SignalSystemIdle();
	}
	
	protected IEnumerator	OnExecuteStage1( AWebCoroutine self ) {
		self.Request = new HTTP.Request( "Post", WebRequests.urlSurveyMegatile );
		self.Request.AddParameters( WebRequests.authenticatedParameters );
		self.Request.Send();
		
		while( !self.RequestIsDone ) {
			yield return 0;
		}
	}
	
	protected IEnumerator	OnExecuteStage2( AWebCoroutine self ) {
		Func<string, SurveyWrapper> asyncDelegate = JSONDecoder.Decode<SurveyWrapper>;
		IAsyncResult ar = asyncDelegate.BeginInvoke( self.ResponseText, null, null );
		while( !ar.IsCompleted ) {
			yield return 0;
		}
		Debug.Log (self.ResponseText);		
		returnData = asyncDelegate.EndInvoke( ar ).survey;
		yield break;
	}
	
	protected IEnumerator	OnExecuteStage3( AWebCoroutine self ) {
		if(returnData != null)
		{
			InputManager.use.AddCachedSurvey(InputManager.use.GetSelectedMegatileIds()[0], returnData);
			Survey.current = returnData;
			Survey.current.loaded = true;
			Messenger<bool>.Broadcast("LandSurveyed", true);
		}
		else
		{
			Debug.LogError("AHHHHHHH");	
		}
			
		yield break;
	}
}