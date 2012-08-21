using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class GUIStyleEx {
	public Rect		DrawRect;
	public GUIStyle	Style;
	
	public string	Name {
		get {
			return Style.name;
		}
	}
}

[Serializable]
public class Size {
	public float	width;
	public float	height;

	public Size( float w, float h ) {
		width = w;
		height = h;
	}
}

[Serializable]
public class GUILayoutStyleEx {
	public Size		Size;
	public GUIStyle	style;
	
	public string	Name {
		get {
			return Style.name;
		}
	}
	
	public GUIStyle	Style {
		get {
			//if( style == null ) {
			//	style = new GUIStyle();
			//}
			return style;
		}
	}
}

[Serializable]
public class SelectWorldState : State<LoginGUI> {
	[SerializeField]
	protected Rect				transitionButtonRect;
	
	[SerializeField]
	protected GUILayoutStyleEx	createGameButtonStyle;
	
	[SerializeField]
	protected GUILayoutStyleEx	joinGameButtonStyle;
	
	[SerializeField]
	protected GUILayoutStyleEx	logoutButtonStyle;
	
	[SerializeField]
	protected Rect				selectWorldDisplayRect;
	
	[SerializeField]
	protected GUILayoutScrollView	scrollViewInfo;
	
	[SerializeField]
	protected GUILayoutSelectionGridStyle	selectWorldGridStyle;
	
	[SerializeField]
	protected Rect				statusDisplayRect;
	
#region FOR_GLS
	protected string			DeterminePlayerClasses( WorldData data ) {
		string formattedStr;
		
		if( data.players.Length <= 0 ) {
			return "";
		}
		
		if( data.players[0].type == null ) {
			formattedStr = "invalid type";
		} else {
			formattedStr = data.players[0].type.ToString();
		}
		
		for( int ix = 1; ix < data.players.Length; ++ix ) {
			try {
				if( data.players[ix].type == null ) {
					formattedStr += "\ninvalid type";
				} else {
					formattedStr += string.Format( "\n{0}", data.players[ix].type.ToString() );
				}
			}
			catch( NullReferenceException e ) {
				Debug.LogError( e );
			}
		}
		return formattedStr;
	}
#endregion
	
	protected List<WorldData>	joinableWorlds = new List<WorldData>();
	protected List<WorldData>	emptyWorlds = new List<WorldData>();
	
	public override void Enter( LoginGUI self, State<LoginGUI> prevState ) {
		base.Enter( self, prevState );
		
#region FOR_GLS
		joinableWorlds.Clear();
		LoginGUI.worlds.FilterWorldsWith( WorldsData.JoinableWorldsPassFilter, UserData.current.id, joinableWorlds );
		
		emptyWorlds.Clear();
		LoginGUI.worlds.FilterWorldsWith( WorldsData.EmptyWorldsPassFilter, UserData.current.id, emptyWorlds );
#endregion
		
		selectWorldGridStyle.ClearRows();
		foreach( WorldData world in joinableWorlds ) {
			selectWorldGridStyle.AddRow( world.name, DeterminePlayerClasses(world), world.id.ToString() );
		}
	}

	public override void OnGUI( LoginGUI self ) {
		if( Debug.isDebugBuild ) {
			if( GUI.Button(new Rect(0, 50, 200, 25), "Clear World 1") ) {
				WorldsData.ClearWorldOfPlayers( 1 );
			}
			
			if( GUI.Button(new Rect(0, 75, 200, 25), "Clear World 2") ) {
				WorldsData.ClearWorldOfPlayers( 2 );
			}
			
			if( GUI.Button(new Rect(0, 100, 200, 25), "Clear World 3") ) {
				WorldsData.ClearWorldOfPlayers( 3 );
			}
		}
		
		GUILayout.BeginArea( transitionButtonRect );
			if( emptyWorlds.Count > 0 && GUILayout.Button(createGameButtonStyle.Name, createGameButtonStyle.Style, GUILayout.Width(createGameButtonStyle.Size.width), GUILayout.Height(createGameButtonStyle.Size.height)) ) {
				UserData.worldNumber = emptyWorlds.Count > 0 ? emptyWorlds[0].id : -1;
				self.OnCreateGame();
			}
		
			if( joinableWorlds.Count > 0 && GUILayout.Button(joinGameButtonStyle.Name, joinGameButtonStyle.Style, GUILayout.Width(joinGameButtonStyle.Size.width), GUILayout.Height(joinGameButtonStyle.Size.height)) || (Event.current.isKey && (Event.current.keyCode==KeyCode.KeypadEnter || Event.current.keyCode==KeyCode.Return)) ) {
				UserData.worldNumber = joinableWorlds[selectWorldGridStyle.SelectedIndex].id;
				self.OnJoinGame();
			}
		
			if( GUILayout.Button(logoutButtonStyle.Name, logoutButtonStyle.Style, GUILayout.Width(logoutButtonStyle.Size.width), GUILayout.Height(logoutButtonStyle.Size.height)) ) {
				UserData.current = null;
			}
		GUILayout.EndArea();
		
		// Auto-adjust scrollView as we add more elements
		float max = Math.Max( 0.0f, (float)(selectWorldGridStyle.Height - scrollViewInfo.Height) );
		scrollViewInfo.SetMax( 1, max );

		// list the worlds
		if( LoginGUI.playerData != null ) {
			GUILayout.BeginArea( selectWorldDisplayRect );
				scrollViewInfo.Begin();
						selectWorldGridStyle.Draw();
				scrollViewInfo.End();
			GUILayout.EndArea();
		}
		else {
			GUILayout.Label( "No world data found!", self.styles.smallTextLight );
			GUILayout.FlexibleSpace();
		}

		// status line
		GUILayout.BeginHorizontal(); {
			GUI.Label( statusDisplayRect, string.Format("Currently logged in as {0}", self.m_loginName), self.styles.mediumTextHighlighted );
		} GUILayout.EndHorizontal();
	}
}