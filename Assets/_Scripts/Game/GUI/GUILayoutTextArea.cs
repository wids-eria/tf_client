using UnityEngine;
using System;

[Serializable]
public class GUILayoutTextArea {
	public string		Text;
	public int			MaxLength;
	
	[SerializeField]
	protected GUILayoutStyleEx	style;
	
	public GUIStyle		Style {	
		get {
			return style.Style;
		}
	}
	
	public	void	Draw() {
		if( MaxLength > 0 ) {
			GUILayout.TextArea( Text, MaxLength, Style, GUILayout.Width(style.Size.width), GUILayout.Height(style.Size.height) );
		} else {
			GUILayout.TextArea( Text, Style, GUILayout.Width(style.Size.width), GUILayout.Height(style.Size.height) );
		}
	}
}