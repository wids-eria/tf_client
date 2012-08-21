using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TSMBuyMenu : State<TSMOptionMenu, GUICircularMenu, TileSelectionMenu>
{
	protected TSMOptionMenu owner;
	GUICircularMenu optionMenu 	{ get { return Parm1; } }
	GUIContainer BuyMenuContainer;
	List<GUIObject> AnimatedScreenObjects;	
	
	float animationTime = 0.4f;
	
	Rect rect = new Rect(0,0,300,100);
	
	public override void Enter (TSMOptionMenu self, State<TSMOptionMenu> prevState)
	{
		base.Enter(self,prevState);
		
		owner = self;
		
		AnimatedScreenObjects = new List<GUIObject>();
		
		BuyMenuContainer = new GUIContainer(rect, true);
		BuyMenuContainer.visible = false;
		
		GUILabelCentered priceLabel = new GUILabelCentered(string.Format("Price : ${0}",TSMOptionMenu.kCostBuy), new Rect(0,-50,300,100));
		BuyMenuContainer.AddElement(priceLabel);
		
		GUIButtonData purchaseButtonData = new GUIButtonData();
		purchaseButtonData.usePersonalArea = true;
		purchaseButtonData.x = -50;
		purchaseButtonData.y = -40;		
		purchaseButtonData.width = 100;
		purchaseButtonData.height = 50;
		purchaseButtonData.text = "Purchase";
		GUIButtonBase purchaseButton = new GUIButtonBase(purchaseButtonData);
		purchaseButton.OnButtonPressed += AttemptPurchase;
		
		//Enable the button based on cost
		purchaseButton.enabled =  GameManager.economyController.IsBalanceAvailable(TSMOptionMenu.kCostBuy);
		
		BuyMenuContainer.AddElement(purchaseButton);
		//AnimatedScreenObjects.Add(purchaseButton);
		
		//Setup Action Button List
		/*actionMenu = new GUIButtonList(rect,true);
		GUIButtonData data = Parm2.actionButtonTemplate.Clone();
		GUISquareButton button;
		
		foreach(PlayerAction p in Player.current.actions) {
			data.icon = p.icon;
			button = new GUISquareButton(data,data.width);
			button.OnButtonPressed += p.DoAction;
			actionMenu.AddElement(button);
		}*/
				
		if(prevState.GetType() == typeof(TSMOptionOpen))
		{
			AnimateCircularMenu();
		}
		else
		{
			BuyMenuContainer.visible = true;
		}
		
		BuyMenuContainer.anchor.Set(GUIAnchor.Bottom | GUIAnchor.Center);
		BuyMenuContainer.position.Set(optionMenu.position.Get() + new Vector2(0,-20));
		Parm2.menuGroup.AddElement(BuyMenuContainer);
		
		//AnimateMenu();
	}
	
	protected void AttemptPurchase(GUIButtonBase button) 
	{
		PurchaseTileCoroutine co = new PurchaseTileCoroutine(Parm2, owner);
		co.Start(Parm2, owner);

		//OpenSurveyMenu(optionMenu,owner);
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
				new GUIEffects.CircularMenuRotate(optionMenu,animationTime*1.25f,315f,Parm2.curve),
				new GUIEffects.CircularMenuRadius(optionMenu,animationTime*1.5f,130f),
				new GUIEffects.CircularMenuArc(optionMenu,animationTime*1.5f,55f),
				new GUIEffects.CircularMenuButtonSize(optionMenu,0.5f,48f,Parm2.curve)
			),
			new GUIEffects.SetVisibility(BuyMenuContainer, true)
		);	
	}
	
	protected void AnimateMenu()
	{
		/* * * * * * * * * * * * * * * * * * * * * * *
		 * Animate Action Buttons
		 * * * * * * * * * * * * * * * * * * * * * * */ 
		/*BuyMenuContainer.anchor.Set(GUIAnchor.Bottom | GUIAnchor.Center);
		BuyMenuContainer.visible = true;
		BuyMenuContainer.position.Set(optionMenu.position.Get() + new Vector2(0,-50));*/
		//BuyMenuContainer.SetPosition(new Modifiable<Vector2>(optionMenu.position.Get() + new Vector2(0,-50)));
		
		foreach(GUIObject obj in AnimatedScreenObjects)
			Parm2.menuGroup.AddElement(obj);
			
		//Parm2.menuGroup.AddElement(BuyMenuContainer);
		
		/*BuyMenuContainer.animationQueue.Enqueue(
			new CommandList(
				new GUIEffects.CommandMove(BuyMenuContainer,1.0f,
					BuyMenuContainer.position.Get() + new Vector2(0,-600),
					BuyMenuContainer.position.Get(),Parm2.curve),
				new CommandQueue(
					new CommandWait(0.2f),
					new GUIEffects.SetVisibility(BuyMenuContainer,true)
			))
		);*/
	}
	
	public override void Exit (TSMOptionMenu self, State<TSMOptionMenu> nextState)
	{
		base.Exit (self, nextState);
		
		Parm2.menuGroup.RemoveElement(BuyMenuContainer);
		
		BuyMenuContainer = null;
	}
	
	public override void OnGUI (TSMOptionMenu self)
	{
		base.OnGUI (self);
	}
}

public class PurchaseTileCoroutine : WebCoroutine<TSMOptionMenu> {
		
	public PurchaseTileCoroutine( MonoBehaviour owner, TSMOptionMenu menuCallback ) {
		AddExecutionHandler( OnExecuteStage1 );
	}
		
	protected override void	OnStart() {
		base.OnStart();
		SignalSystemBusy();
	}
	
	protected override void	BroadcastOnComplete() {
		base.BroadcastOnComplete();
		SignalSystemIdle();
	}
		
	protected IEnumerator	OnExecuteStage1( TSMOptionMenu menuCallback, AWebCoroutine self ) {
		self.Request = new HTTP.Request( "Put", WebRequests.urlPurchaseMegatile );
		self.Request.AddParameters( WebRequests.authenticatedParameters );
		self.Request.SetHeader("Content-Length","0");
		self.Request.Send();
		
		while( !self.RequestIsDone ) {
			yield return 0;
		}
		
		if (self.Status != 200) {
			Debug.LogWarning (string.Format("Web message expected 200 status, got: {0}",self.Status));
			Debug.LogError (self.ResponseText);
		}
		
		//Refresh the selection
		/* HACK?!? We don't have a way to get a status from this... so it may not be
		 * refreshed quick enough to make me happy */
		TerrainManager.use.RefreshFromIds(InputManager.use.resourceTileSelection.resource_tile_ids.ToArray());
		
		//Refresh Player Data
		IEnumerator e = WebRequests.DownloadPlayerData(Player.current);
		while (e.MoveNext()) {
			yield return 0;
		}
		//Close the menu
		//Messenger.Broadcast(TileSelectionMenu.kClose);
		Messenger<bool>.Broadcast("LandBought", true);
		
		//Rather than clear selection, go back to init state
		menuCallback.BoughtLand();
		menuCallback.BuyToAction(menuCallback.optionMenu,menuCallback.owner);
		//Clear the selection
		//InputManager.use.ClearResourceTileSelection();
	}
	
}