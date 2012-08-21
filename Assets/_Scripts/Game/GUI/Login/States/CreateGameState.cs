using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class CreateGameState : State<LoginGUI> {
	[SerializeField]
	protected Rect				turnTimeLimitRect;
	
	[SerializeField]
	protected GUILayoutStyleEx	turnLimitLabelStyle;
	
	[SerializeField]
	protected GUILayoutHorizontalSlider turnLimitSliderStyle;

	[SerializeField]
	protected GUIBoxStyle	background;

	[SerializeField]
	protected GUIStyleEx	selectClassButtonStyle;
	
	[SerializeField]
	protected GUIStyleEx	cancelButtonStyle;
	
	[SerializeField]
	protected Rect			mapSelectGroupRect;
	
	[SerializeField]
	protected GUILayoutScrollView	scrollViewInfo;
	
	[SerializeField]
	protected GUILayoutSelectionGridStyle	mapSelectStyle;

	public override void	Enter( LoginGUI self, State<LoginGUI> prevState ) {
		base.Enter( self, prevState );

		List<WorldData>	filteredWorlds = new List<WorldData>();
		Func<int, WorldData, bool> pred = delegate(int userId, WorldData arg) { 
			return arg.players == null || arg.players.Length <= 0;
		};
		LoginGUI.worlds.FilterWorldsWith( pred, UserData.current.id, filteredWorlds );
		mapSelectStyle.ClearRows();
	
		foreach( WorldData wd in filteredWorlds ) {
			mapSelectStyle.AddRow( wd.name, wd.name, wd.name );
		}
	}
	
	public override void OnGUI( LoginGUI self ) {
		base.OnGUI( self );

		GUI.Box( background.DrawRect, background.Background );
		
		GUI.BeginGroup( turnTimeLimitRect );
			GUILayout.BeginHorizontal();
				GUILayout.Label( turnLimitLabelStyle.Name, turnLimitLabelStyle.Style, GUILayout.Width(turnLimitLabelStyle.Size.width), GUILayout.Height(turnLimitLabelStyle.Size.height) );
				turnLimitSliderStyle.Draw();
			GUILayout.EndHorizontal();
		GUI.EndGroup();
		
		// Auto-adjust scrollView as we add more elements
		float max = Math.Max( 0.0f, (float)(mapSelectStyle.Height - scrollViewInfo.Height) );
		scrollViewInfo.SetMax( 1, max );

		GUI.BeginGroup( mapSelectGroupRect );
			scrollViewInfo.Begin();
				mapSelectStyle.Draw();
				UserData.worldNumber = LoginGUI.worlds.worlds[mapSelectStyle.SelectedIndex].id;
			scrollViewInfo.End();
		GUI.EndGroup();
		
		if( GUI.Button(selectClassButtonStyle.DrawRect, selectClassButtonStyle.Name, selectClassButtonStyle.Style) && !mapSelectStyle.Empty ) {
			self.OnSelectClass();
		}
		
		if( GUI.Button(cancelButtonStyle.DrawRect, cancelButtonStyle.Name, cancelButtonStyle.Style) ) {
			self.OnCancelCreateGame();
		}
	}
}