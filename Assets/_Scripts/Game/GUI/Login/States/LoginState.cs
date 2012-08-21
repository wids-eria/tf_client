using UnityEngine;
using System;

[Serializable]
public class LoginState : State<LoginGUI> {
	[SerializeField]
	protected GUIStyle		serverLabelStyle;
	
	[SerializeField]
	protected GUIStyle		emailLabelStyle;
	
	[SerializeField]
	protected GUIStyle		passwordLabelStyle;
	
	[SerializeField]
	protected Rect			classSelectionButtonRect;
	
	[SerializeField]
	protected Rect			transitionButtonRect;
	
	[SerializeField]
	protected GUILayoutStyleEx	quitButtonStyle;
	
	[SerializeField]
	protected GUILayoutStyleEx	registerButtonStyle;
	
	[SerializeField]
	protected GUILayoutStyleEx	loginButtonStyle;
	
	[SerializeField]
	protected Rect			statusLabelRect;
	
	public override void OnGUI( LoginGUI self ) {
		float lblWid = 100f;
		float inpHgt = GUIHelpers.GetFontSizeFromName(serverLabelStyle.font)*1.5f+self.styles.inputField.padding.top+self.styles.inputField.padding.bottom;
		
		// server url
		GUILayout.BeginHorizontal(); {
			GUILayout.Label(serverLabelStyle.name, serverLabelStyle, GUILayout.Width(lblWid), GUILayout.Height(inpHgt));
			WebRequests.urlServer = GUILayout.TextField(WebRequests.urlServer, self.styles.inputField, GUILayout.Height(inpHgt));
		} GUILayout.EndHorizontal();
		
		// username
		GUILayout.BeginHorizontal(); {
			GUILayout.Label(emailLabelStyle.name, emailLabelStyle, GUILayout.Width(lblWid), GUILayout.Height(inpHgt));
			self.m_loginName = GUILayout.TextField(self.m_loginName, self.styles.inputField, GUILayout.Height(inpHgt));
		} GUILayout.EndHorizontal();
		
		// password
		GUILayout.BeginHorizontal(); {
			GUI.SetNextControlName(self.m_passwordFieldCtrlName);
			GUILayout.Label(passwordLabelStyle.name, passwordLabelStyle, GUILayout.Width(lblWid), GUILayout.Height(inpHgt));
			self.m_loginPassword = GUILayout.PasswordField(self.m_loginPassword, '*', self.styles.inputField, GUILayout.Height(inpHgt));
		} GUILayout.EndHorizontal();
		
		// give password field focus
		if( !self.m_hasFocus ) {
			GUI.FocusControl(self.m_passwordFieldCtrlName);
			self.m_hasFocus = true;
		}
		
		GUILayout.FlexibleSpace();
		
		//GUILayout.BeginArea( classSelectionButtonRect );
		//GUILayout.BeginHorizontal(); {
		//	GUILayout.Label("Select Character Class:", self.styles.mediumTextLight);
		//	Player.current.characterClass = (Player.CharacterClass)GUILayout.SelectionGrid(
		//		(int)Player.current.characterClass,
		//		System.Enum.GetNames(typeof(Player.CharacterClass)),
		//		System.Enum.GetValues(typeof(Player.CharacterClass)).Length,
		//		self.styles.smallButton
		//	);
		//} GUILayout.EndHorizontal();
		//GUILayout.EndArea();
		
		GUILayout.BeginArea( transitionButtonRect );
		GUILayout.BeginHorizontal(); {
			if( GUILayout.Button(loginButtonStyle.Name, loginButtonStyle.Style, GUILayout.Width(loginButtonStyle.Size.width), GUILayout.Height(loginButtonStyle.Size.height)) ) {
				//self.StartCoroutine(WebRequests.Login(self.m_loginName, self.m_loginPassword));
				login.Start( self, self.m_loginName, self.m_loginPassword );
			}

			if( GUILayout.Button(registerButtonStyle.Name, registerButtonStyle.Style, GUILayout.Width(registerButtonStyle.Size.width), GUILayout.Height(registerButtonStyle.Size.height)) ) {
				self.Register();
			}
			
			if( GUILayout.Button(quitButtonStyle.Name, quitButtonStyle.Style, GUILayout.Width(quitButtonStyle.Size.width), GUILayout.Height(quitButtonStyle.Size.height)) ) {
				Application.Quit();
			}
		} GUILayout.EndHorizontal();
		GUILayout.EndArea();

		GUILayout.BeginArea( statusLabelRect );
			GUILayout.Label( WebRequests.loginStatus, self.styles.mediumTextHighlighted );
		GUILayout.EndArea();
		
		// process return key
		if( GUI.GetNameOfFocusedControl() == self.m_passwordFieldCtrlName && Event.current.isKey && Event.current.keyCode == KeyCode.Return ) {
			//self.StartCoroutine( WebRequests.Login(self.m_loginName, self.m_loginPassword) );
			login.Start( self, self.m_loginName, self.m_loginPassword );
		}
	}
	
	protected LoginCoroutine login = new LoginCoroutine();
}