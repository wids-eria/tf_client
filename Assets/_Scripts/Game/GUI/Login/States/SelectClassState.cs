#define FOR_JONGEE

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class ClassDescription {
	[SerializeField]
	protected GUILayoutAreaStyle	background;
	
	[SerializeField]
	protected GUIStyleEx			title;
	
	[SerializeField]
	protected GUITextArea			desc;
	
	[SerializeField]
	protected GUIStyleEx			selectButton;
	
	protected GUIStyle				disabledTitle;
	protected GUIStyle				disabledSelectButton;
	
	public Action					OnSelect;
	
	public bool						Disabled = false;
	
	public void						SetBackgroundActive() {
		background.SetActive();
	}
	
	public void						SetBackgroundNormal() {
		background.SetNormal();
	}
	
	public void	OnInit() {
		disabledTitle = new GUIStyle( title.Style );
		disabledTitle.normal.textColor = new Color( 0.5f, 0.5f, 0.5f, 1.0f );
		
		disabledSelectButton = new GUIStyle( selectButton.Style );
		disabledSelectButton.normal.textColor = new Color( 0.5f, 0.5f, 0.5f, 1.0f );
		
		disabledSelectButton.hover.textColor = new Color( 0.5f, 0.5f, 0.5f, 1.0f );
		disabledSelectButton.hover.background = disabledSelectButton.normal.background;
		
		disabledSelectButton.active.textColor = new Color( 0.5f, 0.5f, 0.5f, 1.0f );
		disabledSelectButton.active.background = disabledSelectButton.normal.background;
	}
	
	public void	Draw() {
		background.Begin();
			GUI.Label( title.DrawRect, title.Name, Disabled ? disabledTitle : title.Style );
			desc.Draw();
			if( GUI.Button(selectButton.DrawRect, selectButton.Name, Disabled ? disabledSelectButton : selectButton.Style) ) {
				if( OnSelect != null && !Disabled ) {
					OnSelect();
				}
			}
		background.End();
	}
}

[Serializable]
public class SelectClassState : State<LoginGUI> {
	[SerializeField]
	protected ClassDescription[]	classDescs;
	
	protected Dictionary<Player.CharacterClass, int>	classToDescIndex = new Dictionary<Player.CharacterClass, int>();
	
	[SerializeField]
	protected GUIStyleEx			confirmButtonStyle;
	
	[SerializeField]
	protected GUIStyleEx			cancelButtonStyle;
	
#region FOR_GLS
	protected Player.CharacterClass	characterClass;
	
	protected IEnumerator	RefreshWorldData( LoginGUI self, AWebCoroutine co ) {
		IEnumerator e = WebRequests.RefreshWorldData( co );
		while( e.MoveNext() ) {
			yield return e.Current;
		}
		
		int id = UserData.current.id;
		WorldData wd = GameManager.worldData;
		if( wd.GetClassForUser(id, ref characterClass) ) {
#if FOR_JONGEE
			GamePrefs.SetCharacterClass( Player.CharacterClass.TimberCompany );
#else
			GamePrefs.SetCharacterClass( characterClass );
#endif
			JoinGame(self);
			yield break;
		}
#if FOR_JONGEE
		JoinGame(self);	
#endif
	}
#endregion
	
	public override void	Enter( LoginGUI self, State<LoginGUI> prevState ) {
		base.Enter( self, prevState );
		
		foreach( ClassDescription cd in classDescs ) {
			cd.OnInit();
		}
		
		classToDescIndex.Clear();
		classToDescIndex.Add( Player.CharacterClass.Conservationist, 0 );
		classToDescIndex.Add( Player.CharacterClass.TimberCompany, 1 );
		classToDescIndex.Add( Player.CharacterClass.Developer, 2 );
		
		coRefreshWorld.Start( self );
		
#region FOR_GLS
		classDescs[0].OnSelect = delegate() { characterClass = Player.CharacterClass.Conservationist; };
		classDescs[0].OnSelect += classDescs[0].SetBackgroundActive;
		classDescs[0].OnSelect += classDescs[1].SetBackgroundNormal;
		classDescs[0].OnSelect += classDescs[2].SetBackgroundNormal;
		
		classDescs[1].OnSelect = delegate() { characterClass = Player.CharacterClass.TimberCompany; };
		classDescs[1].OnSelect += classDescs[0].SetBackgroundNormal;
		classDescs[1].OnSelect += classDescs[1].SetBackgroundActive;
		classDescs[1].OnSelect += classDescs[2].SetBackgroundNormal;
		
		classDescs[2].OnSelect = delegate() { characterClass = Player.CharacterClass.Developer; };
		classDescs[2].OnSelect += classDescs[0].SetBackgroundNormal;
		classDescs[2].OnSelect += classDescs[1].SetBackgroundNormal;
		classDescs[2].OnSelect += classDescs[2].SetBackgroundActive;

		WebCoroutine<LoginGUI> co = new WebCoroutine<LoginGUI>( RefreshWorldData );
		co.Start( self, self );
#endregion
	}
	
	protected WebCoroutine	coRefreshWorld = new WebCoroutine( WebRequests.RefreshWorldData );
	
	public override void OnGUI( LoginGUI self ) {
		if( GameManager.worldData != null && GameManager.worldData.players != null ) {
			PlayerData	data = null;
			for( int ix = 0; ix < GameManager.worldData.players.Length; ++ix ) {
				data = GameManager.worldData.players[ ix ];
				if( data == null ) {
					continue;
				}
			
				Player.CharacterClass cc = Player.GetCharacterClass( data.type );
				classDescs[ classToDescIndex[cc] ].Disabled = true;
			}
		}
		
		foreach( ClassDescription desc in classDescs ) {
			desc.Draw();
		}
		
		if( GUI.Button(confirmButtonStyle.DrawRect, confirmButtonStyle.Name, confirmButtonStyle.Style) ) {
			JoinGame(self);
		}
		
		if( GUI.Button(cancelButtonStyle.DrawRect, cancelButtonStyle.Name, cancelButtonStyle.Style) ) {
			self.OnCancelSelectClass();
		}
	}
	
	protected void JoinGame(LoginGUI self) {
#region FOR_GLS
		
#if !FOR_JONGEE
		GamePrefs.SetCharacterClass( characterClass );// Need to map these to the world
#endif
		PlayerData pd = LoginGUI.playerData.GetPlayerWithWorldId( UserData.worldNumber );
		if( pd == null ) {
#if !FOR_JONGEE
			WebRequests.JoinWorld( UserData.worldNumber, Player.GetCharacterClassName(characterClass) );// This call is blocking
#else
			WebRequests.JoinWorld( UserData.worldNumber, "Lumberjack" );
#endif
		} else if( pd.type == null ) {
			
		}
		
		WebCoroutine co = new WebCoroutine( WebRequests.GetPlayerData );
		co.Start( self );
		while( !co.RequestIsDone ) {}
#endregion
		self.OnJoinGame();
	}
}