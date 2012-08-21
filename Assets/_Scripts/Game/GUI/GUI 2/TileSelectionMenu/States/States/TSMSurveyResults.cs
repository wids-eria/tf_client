using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TSMSurveyResults : State<TSMOptionMenu, GUICircularMenu, TileSelectionMenu>
{
	protected TSMOptionMenu owner;
	GUICircularMenu optionMenu 	{ get { return owner.optionMenu; } }
	GUIContainer ResultsContainer;
	
	float animationTime = 0.4f;
	
	Rect rect = new Rect(0,0,300,500);
	
	public override void Enter (TSMOptionMenu self, State<TSMOptionMenu> prevState)
	{
		base.Enter(self,prevState);
		owner = self;
		
		ResultsContainer = new GUIContainer(rect, true);
		
		GUILabelCentered ResultsLabel = new GUILabelCentered("Survey Results", new Rect(0,-250,300,500));
		ResultsContainer.AddElement(ResultsLabel);		
		
		int currentY = -500;
		int increment = 25;
		
		currentY += 35;
		GUILabel DateLabel = new GUILabel("Date:", new Rect(-140,currentY,140,25));
		GUILabel DateOut = new GUILabel(System.DateTime.Parse (Survey.current.created_at).ToString("g"), new Rect(-10,currentY,140,25));
		ResultsContainer.AddElement(DateLabel);	
		ResultsContainer.AddElement(DateOut);	
		
		currentY += increment;
		GUILabel TwoInchLabel = new GUILabel("2in Tree Count:", new Rect(-140,currentY,140,25));
		GUILabel TwoInchOut = new GUILabel(Survey.current.num_2in_trees.ToString(), new Rect(-10,currentY,140,25));
		ResultsContainer.AddElement(TwoInchLabel);	
		ResultsContainer.AddElement(TwoInchOut);
		
		currentY += increment;
		GUILabel FourInchLabel = new GUILabel("4in Tree Count:", new Rect(-140,currentY,140,25));
		GUILabel FourInchOut = new GUILabel(Survey.current.num_4in_trees.ToString(), new Rect(-10,currentY,140,25));
		ResultsContainer.AddElement(FourInchLabel);	
		ResultsContainer.AddElement(FourInchOut);
		
		currentY += increment;
		GUILabel SixInchLabel = new GUILabel("6in Tree Count:", new Rect(-140,currentY,140,25));
		GUILabel SixInchOut = new GUILabel(Survey.current.num_6in_trees.ToString(), new Rect(-10,currentY,140,25));
		ResultsContainer.AddElement(SixInchLabel);	
		ResultsContainer.AddElement(SixInchOut);
		
		currentY += increment;
		GUILabel EightInchLabel = new GUILabel("8in Tree Count:", new Rect(-140,currentY,140,25));
		GUILabel EightInchOut = new GUILabel(Survey.current.num_8in_trees.ToString(), new Rect(-10,currentY,140,25));
		ResultsContainer.AddElement(EightInchLabel);	
		ResultsContainer.AddElement(EightInchOut);
		
		currentY += increment;
		GUILabel TenInchLabel = new GUILabel("10in Tree Count:", new Rect(-140,currentY,140,25));
		GUILabel TenInchOut = new GUILabel(Survey.current.num_10in_trees.ToString(), new Rect(-10,currentY,140,25));
		ResultsContainer.AddElement(TenInchLabel);	
		ResultsContainer.AddElement(TenInchOut);
		
		currentY += increment;
		GUILabel TwelveInchLabel = new GUILabel("12in Tree Count:", new Rect(-140,currentY,140,25));
		GUILabel TwelveInchOut = new GUILabel(Survey.current.num_12in_trees.ToString(), new Rect(-10,currentY,140,25));
		ResultsContainer.AddElement(TwelveInchLabel);	
		ResultsContainer.AddElement(TwelveInchOut);
		
		currentY += increment;
		GUILabel FourteenInchLabel = new GUILabel("14in Tree Count:", new Rect(-140,currentY,140,25));
		GUILabel FourteenInchOut = new GUILabel(Survey.current.num_14in_trees.ToString(), new Rect(-10,currentY,140,25));
		ResultsContainer.AddElement(FourteenInchLabel);	
		ResultsContainer.AddElement(FourteenInchOut);
		
		currentY += increment;
		GUILabel SixteenInchLabel = new GUILabel("16in Tree Count:", new Rect(-140,currentY,140,25));
		GUILabel SixteenInchOut = new GUILabel(Survey.current.num_16in_trees.ToString(), new Rect(-10,currentY,140,25));
		ResultsContainer.AddElement(SixteenInchLabel);	
		ResultsContainer.AddElement(SixteenInchOut);
		
		currentY += increment;
		GUILabel EighteenInchLabel = new GUILabel("18in Tree Count:", new Rect(-140,currentY,140,25));
		GUILabel EighteenInchOut = new GUILabel(Survey.current.num_18in_trees.ToString(), new Rect(-10,currentY,140,25));
		ResultsContainer.AddElement(EighteenInchLabel);	
		ResultsContainer.AddElement(EighteenInchOut);
		
		currentY += increment;
		GUILabel TwentyInchLabel = new GUILabel("20in Tree Count:", new Rect(-140,currentY,140,25));
		GUILabel TwentyInchOut = new GUILabel(Survey.current.num_20in_trees.ToString(), new Rect(-10,currentY,140,25));
		ResultsContainer.AddElement(TwentyInchLabel);	
		ResultsContainer.AddElement(TwentyInchOut);
		
		currentY += increment;
		GUILabel TwentyTwoInchLabel = new GUILabel("22in Tree Count:", new Rect(-140,currentY,140,25));
		GUILabel TwentyTwoInchOut = new GUILabel(Survey.current.num_22in_trees.ToString(), new Rect(-10,currentY,140,25));
		ResultsContainer.AddElement(TwentyTwoInchLabel);	
		ResultsContainer.AddElement(TwentyTwoInchOut);
		
		currentY += increment;
		GUILabel TwentyFourLabel = new GUILabel("24in Tree Count:", new Rect(-140,currentY,140,25));
		GUILabel TwentyFourInchOut = new GUILabel(Survey.current.num_24in_trees.ToString(), new Rect(-10,currentY,140,25));
		ResultsContainer.AddElement(TwentyFourLabel);	
		ResultsContainer.AddElement(TwentyFourInchOut);
		
		GUIButtonData backButtonData = new GUIButtonData();
		backButtonData.usePersonalArea = true;
		backButtonData.x = 10;
		backButtonData.y = -25;		
		backButtonData.width = 130;
		backButtonData.height = 100;
		backButtonData.text = "Go Back";
		GUIButtonBase backButton = new GUIButtonBase(backButtonData);
		backButton.OnButtonPressed += GoBackToSurveyList;
		ResultsContainer.AddElement(backButton);
		
		ResultsContainer.anchor.Set(GUIAnchor.Bottom | GUIAnchor.Center);
		ResultsContainer.position.Set(optionMenu.position.Get() + new Vector2(0,-20));
		owner.owner.menuGroup.AddElement(ResultsContainer);	
	}
	
	protected void GoBackToSurveyList(GUIButtonBase button) 
	{
		owner.ResultsToSurvey(optionMenu,Parm2);
	}
	
	public override void Exit (TSMOptionMenu self, State<TSMOptionMenu> nextState)
	{
		base.Exit (self, nextState);
		
		owner.owner.menuGroup.RemoveElement(ResultsContainer);
		
		ResultsContainer = null;
	}
	
	public override void OnGUI (TSMOptionMenu self)
	{
		base.OnGUI (self);
	}	
}

