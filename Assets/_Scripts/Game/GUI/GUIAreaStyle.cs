using UnityEngine;
using System;

[Serializable]
public class GUILayoutAreaStyle {
	public Rect		DrawRect;
	public GUIStyle	Style;

	public void	Begin() {
		GUILayout.BeginArea( DrawRect, Style );
	}
	
	public void	End() {
		GUILayout.EndArea();
	}
	
	public void	SetNormal() {
		if( Style.onNormal.background != null && Style.onActive.background == null ) {
			Style.onActive.background = Style.normal.background;
			Style.normal.background = Style.onNormal.background;
			Style.onNormal.background = null;
		}
	}
	
	public void	SetActive() {
		if( Style.onActive.background != null && Style.onNormal.background == null ) {
			Style.onNormal.background = Style.normal.background;
			Style.normal.background = Style.onActive.background;
			Style.onActive.background = null;
		}
	}
}