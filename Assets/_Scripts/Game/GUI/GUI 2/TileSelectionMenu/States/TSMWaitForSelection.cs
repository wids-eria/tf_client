using UnityEngine;
using System.Collections;

public class TSMWaitForSelection : State<TileSelectionMenu>
{
	public override void Enter (TileSelectionMenu self, State<TileSelectionMenu> prevState)
	{
		base.Enter(self,prevState);
		//Invert selection display
		ProjectorSelection selection = DataMapController.GetProjector<ProjectorSelection>();
		if (selection != null) selection.SetActiveKey(ProjectorSelection.SelectionType.Normal);
		
		self.menuGroup.visible = false;
		//self.menuGroup.position.Set(TileSelectionMenu.baseMenuPos);
	}
}

