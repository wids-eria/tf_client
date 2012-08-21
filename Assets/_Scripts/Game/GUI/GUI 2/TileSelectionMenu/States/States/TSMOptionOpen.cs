using UnityEngine;
using System.Collections;

public class TSMOptionOpen : State<TSMOptionMenu, TileSelectionMenu, GUICircularMenu>
{
	
	TileSelectionMenu 	mainMenu 	{ get { return Parm1; } }
	GUICircularMenu 	optionMenu 	{ get { return Parm2; } }
	
	
	public override void Enter(TSMOptionMenu self, State<TSMOptionMenu> prevState)
	{
		base.Enter (self, prevState);
		
		//Invert selection display
		DataMapController.GetProjector<ProjectorSelection>().SetActiveKey(ProjectorSelection.SelectionType.Inverted);
		
		//optionMenu.options.position.Set(GetParmPosition(self));
		optionMenu.visible = true;
		optionMenu.SetButtonPositions(0);
			
		//Animate
		optionMenu.animationQueue.Enqueue(
			new CommandList(
				new GUIEffects.CircularMenuOpen(optionMenu,mainMenu.animateTimeInSeconds),
				new GUIEffects.CircularMenuRotate(optionMenu,0.001f,self.baseAngle),
				new GUIEffects.CircularMenuRadius(optionMenu,mainMenu.animateTimeInSeconds*1.5f,150f),
				new GUIEffects.CircularMenuArc(optionMenu,mainMenu.animateTimeInSeconds,60f)
		));
	}
	
	public override void OnGUI (TSMOptionMenu self)
	{
		base.OnGUI (self);
	}
}

