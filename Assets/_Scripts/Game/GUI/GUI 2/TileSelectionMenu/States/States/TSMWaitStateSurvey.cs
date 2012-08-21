using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TSMWaitStateSurvey : State<TSMOptionMenu, GUICircularMenu, TileSelectionMenu>
{
	protected TSMOptionMenu owner;
	GUICircularMenu optionMenu 	{ get { return owner.optionMenu; } }
	GUIContainer RequestSurveyMenuContainer;
	
	GUIButtonList pastSurveys;
	GUIRadioGroup pastSurveysGroup;
	
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
				
		GUILabel newLabel = new GUILabel("New Survey", new Rect(0,-100,150,100));
		RequestSurveyMenuContainer.AddElement(newLabel);
		
		GUILabelCentered priceLabel = new GUILabelCentered("Cost: $10", new Rect(75,-42,150,85));
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
		Debug.Log (index);
	}
	
	protected void RequestSurvey(GUIButtonBase button) 
	{
		SurveyTileCoroutine co = new SurveyTileCoroutine(owner.owner);
		co.Start(owner.owner);	
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