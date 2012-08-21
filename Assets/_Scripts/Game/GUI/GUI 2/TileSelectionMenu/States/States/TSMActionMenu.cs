#define FOR_JONGEE


using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TSMActionMenu : State<TSMOptionMenu, GUICircularMenu, TileSelectionMenu>
{
	GUICircularMenu optionMenu 	{ get { return Parm1; } }
	
	GUIButtonList	listAction;
	GUIRadioGroup	listRadioGroup;
	Rect 			listRect = new Rect(0,0,300,100);
	
	GUIContainer	groupAction;
	Rect 			groupRect = new Rect(0,0,300,200);
	GUIButtonBase	buttonDoAction;
	GUILabel		labelCost;
	
	float animationTime = 0.4f;
	
	/*
	public TSMActionMenu() {
		Messenger.AddListener(TileSelectionMenu.kClose, Clear);
	}
	~TSMActionMenu() {
		Messenger.RemoveListener(TileSelectionMenu.kClose, Clear);
	}*/
	
	
	public override void Enter (TSMOptionMenu self, State<TSMOptionMenu> prevState)
	{
		base.Enter(self,prevState);
		
		//Setup Action Button List
		listAction = new GUIButtonList(listRect,true);
		listAction.visible = false;
		listRadioGroup = new GUIRadioGroup();
		GUIButtonData data = Parm2.actionButtonTemplate.Clone();
		GUISquareButton button;
		
		//Assign OnButtonPressed delegate
		foreach(PlayerAction p in Player.current.actions) {
			data.icon = p.icon;
			button = new GUISquareButton(data,data.width);
			button.OnSelected += OnIndexChanged; // OnButtonPressed += //p.DoAction;
			listAction.AddElement(button);
			listRadioGroup.AddItem(button);
		}
		
		//Setup Grouper
		SetupActionGroup();
						
		if(prevState.GetType() == typeof(TSMOptionOpen))
		{
			AnimateCircularMenu();
		}
		else
		{
			listAction.visible = true;
		}
		
		listAction.anchor.Set(GUIAnchor.Bottom | GUIAnchor.Center);
		listAction.position.Set(optionMenu.position.Get() + new Vector2(0,-20));
		Parm2.menuGroup.AddElement(listAction);
		
		//AnimateMenu();
	}

	protected void OnIndexChanged( ISelectable item ) {
		//Set the group visible based on selected index
		int index = listRadioGroup.IndexOf(item);
		groupAction.visible = (index != -1);
		labelCost.text = string.Format("{0}\nCost: ${1}",Player.current.actions[index].name,TSMOptionMenu.kCostAction);
		buttonDoAction.text = Player.current.actions[index].dialogConfirmButtonDo.text;
		buttonDoAction.OnButtonPressed = Player.current.actions[index].DoAction;
		buttonDoAction.enabled = (GameManager.economyController.IsBalanceAvailable(TSMOptionMenu.kCostAction));
		
#if FOR_JONGEE
		buttonDoAction.enabled &= Player.current.actions[index].allowPerformance;
#endif
	}
	
	protected void SetupActionGroup() {
		GUIButtonData doActionData = new GUIButtonData();
		doActionData.anchor = GUIAnchor.Bottom | GUIAnchor.Right;
		doActionData.usePersonalArea = true;
		doActionData.position = new Vector2(140,0);//optionMenu.position.Get() + new Vector2(150,-105);
		doActionData.width = 100;
		doActionData.height = 50;
		doActionData.text = "TEXT HERE";
		
		buttonDoAction	= new GUIButtonBase(doActionData);
		labelCost 		= new GUILabel("ClearCut\nCost: $",-50,-40,220,50);
		labelCost.anchor.Set(GUIAnchor.Bottom | GUIAnchor.Left);
		labelCost.position.Set( new Vector2(-140,0));//optionMenu.position.Get() + new Vector2(-150,-105));
		
		
		groupAction = new GUIContainer(groupRect,true,buttonDoAction,labelCost);
		groupAction.anchor.Set( GUIAnchor.BottomCenter );
		groupAction.position.Set( optionMenu.position.Get() + new Vector2(0,-130) );
		groupAction.visible = false;
		//groupAction.visible = false;
		Parm2.menuGroup.AddElement(groupAction);
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
			new GUIEffects.SetVisibility(listAction, true)
		);	
	}
	
	protected void AnimateMenu()
	{
		/* * * * * * * * * * * * * * * * * * * * * * *
		 * Animate Radio List
		 * * * * * * * * * * * * * * * * * * * * * * */ 
		listAction.anchor.Set(GUIAnchor.Bottom | GUIAnchor.Center);
		listAction.visible = false;
		listAction.position.Set(optionMenu.position.Get() + new Vector2(0,-20));
		
		listAction.animationQueue.Enqueue(
			new CommandList(
				new GUIEffects.CommandMove(listAction,1.0f,
					listAction.position.Get() + new Vector2(0,-900),
					listAction.position.Get(),Parm2.curve),
				new CommandQueue(
					new CommandWait(0.2f),
					new GUIEffects.SetVisibility(listAction,true)
			))
		);
	}
	
	public override void Exit (TSMOptionMenu self, State<TSMOptionMenu> nextState)
	{
		base.Exit (self, nextState);
		Parm2.menuGroup.RemoveElement(listAction);
		Parm2.menuGroup.RemoveElement(groupAction);
		listAction 		= null;
		listRadioGroup 	= null;
		groupAction 	= null;
		buttonDoAction 	= null;
		labelCost 		= null;
	}
	
	public override void OnGUI (TSMOptionMenu self)
	{
		base.OnGUI (self);
	}
}

