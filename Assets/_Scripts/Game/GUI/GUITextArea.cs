using UnityEngine;
using System;

[Serializable]
public class GUITextArea {
	public string		Text;
	public int			MaxLength;
	
	[SerializeField]
	protected GUIStyleEx	style;
	
	public GUIStyle		Style {	
		get {
			return style.Style;
		}
	}
	
	public Rect			DrawRect {
		get {
			return style.DrawRect;
		}
	}
	
	public	void	Draw() {
		if( MaxLength > 0 ) {
			GUI.TextArea( DrawRect, Text, MaxLength, Style );
		} else {
			GUI.TextArea( DrawRect, Text, Style );
		}
	}
}