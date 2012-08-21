#define FOR_JONGEE

using UnityEngine;
using System;
using System.Collections.Generic;

public abstract class IGUIIcon {
	public abstract Texture	GetTexture();
}

[Serializable]
public class GUIIcon : IGUIIcon {
	[SerializeField]
	protected Texture	texture;
	
	public override Texture	GetTexture() {
		return texture;
	}
}

[Serializable]
public class GUIAnimatedIcon : IGUIIcon {
	[SerializeField]
	protected Texture[] frames;
	protected int	index = 0;
	
	protected float	frameStartTime = 0.0f;
	public float	frameRate;
	public float	frameDuration {
		get {
			return 1.0f / frameRate;
		}
	}
	
	public override Texture	GetTexture() {
		return frames[ index ];
	}
	
	public void		Start() {
		frameStartTime = Time.time;
		index = 0;
	}
	
	public void		Stop() {
		frameStartTime = -1.0f;
	}
	
	public void		Update() {
		if( Time.time < (frameStartTime + frameDuration) ) {
			return;
		}
		
		float overflowTime = Time.time - (frameStartTime + frameDuration);
		frameStartTime = overflowTime + Time.time;
		
		index = (index + 1) % frames.Length;
	}
}

[Serializable]
public class PlayerStatus {
	public Rect	DrawRect;
	
	[SerializeField]
	protected GUIContent		content = new GUIContent();
	
	[SerializeField]
	protected GUIStyle			style;
	
	[SerializeField]
	protected GUIAnimatedIcon	waitingIcon;
	
	[SerializeField]
	protected GUIIcon			errorIcon;
	
	[SerializeField]
	protected GUIIcon			okIcon;
	
	public	void	SetWaiting() {
		waitingIcon.Start();
		icon = waitingIcon;
	}
	
	public	void	SetError() {
		icon = errorIcon;
	}
	
	public	void	SetOK() {
		icon = okIcon;
	}
	
	public bool		IsOk() {
		return icon == okIcon;
	}
	
	protected IGUIIcon			icon;
	public Texture	Icon {
		get {
			return content.image;
		}
		
		set {
			content.image = value;
		}
	}
	
	public string	Name {
		get {
			return content.text;
		}
		
		set {
			content.text = value;
		}
	}

	public void	Draw() {
		waitingIcon.Update();
		
		Icon = icon.GetTexture();

		GUI.Label( DrawRect, content, style );
	}
}

[Serializable]
public class WaitingToEnterGameState : State<LoginGUI> {
	[SerializeField]
	protected GUILayoutAreaStyle	background;

	[SerializeField]
	protected GUIStyleEx			title;
	
	[SerializeField]
	protected PlayerStatus[]		status = new PlayerStatus[3];
	
	protected Dictionary<string, PlayerStatus>		playerStatus = new Dictionary<string, PlayerStatus>();
	
	protected WebCoroutine	co = new WebCoroutine( WebRequests.RefreshWorldData );
	
	#region FOR_GLS
	[SerializeField]
	protected GUIStyleEx			debugEnterGameButton;
	#endregion
	
	public override void Enter( LoginGUI self, State<LoginGUI> prevState ) {
		base.Enter( self, prevState );
		playerStatus.Add( "Conserver", status[0] );
		playerStatus.Add( "Lumberjack", status[1] );
		playerStatus.Add( "Developer", status[2] );
		
		foreach( PlayerStatus s in status ) {
			s.SetWaiting();
		}
	}
	
	public bool		ReadyToEnterGame() {
		foreach( PlayerStatus s in status ) {
			if( !s.IsOk() ) {
				return false;
			}
		}
		return true;
	}
	
	protected void	DrawStatus() {
		foreach( PlayerStatus s in status ) {
			if( s == null ) {
				continue;
			}
				
			s.Draw();
		}
	}
	
	public override void OnGUI( LoginGUI self ) {
		base.OnGUI( self );
		
		#region FOR_GLS
		if( Debug.isDebugBuild && GUI.Button(debugEnterGameButton.DrawRect, debugEnterGameButton.Name, debugEnterGameButton.Style) ) {
			ApplicationEx.Instance.LoadLevel( "Game Level" );
		}
		#endregion
		
		background.Begin();
			GUI.Label( title.DrawRect, title.Name, title.Style );
		
			if( co.IsDone && GameManager.worldData != null && GameManager.worldData.players != null ) {
				PlayerData	data = null;
				for( int ix = 0; ix < GameManager.worldData.players.Length; ++ix ) {
					data = GameManager.worldData.players[ ix ];
					if( data == null ) {
						continue;
					}
			
					if( data.type == null ) {
						continue;
					}
					try {
						playerStatus[data.type].SetOK();
					}
					catch( Exception e ) {
						Debug.LogError( e );
					}
				}
#if FOR_JONGEE
				ApplicationEx.Instance.LoadLevel( "Game Level" );		
#endif
			}

			DrawStatus();
		background.End();
		
		if( ReadyToEnterGame() ) {
			ApplicationEx.Instance.LoadLevel( "Game Level" );
		}
		
		if( co.IsDone ) {
			co.Start( self );
		}
	}
}