using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class GUILabel : GUIObject
{
	public string text;
	
	public GUILabel(string aString, Rect rect) : base(rect) {
		text = aString;
	}
	public GUILabel(string aString, float x, float y, float width, float height) : this (aString, new Rect(x,y,width,height)) {
	}
	public override void Draw ()
	{
		if (!visible) return;
		GUILayout.BeginArea(rect); //,Styles.roundDarkBox);
			GUILayout.Label(text,Styles.largeTextLight);
		GUILayout.EndArea();
	}
}

[System.Serializable]
public class GUILabelCentered : GUILabel
{
	protected static GUIStyle centeredStyle;
	
	public GUILabelCentered(string aString, Rect rect) : base(aString, rect) {
		if (centeredStyle == null) {
			centeredStyle = new GUIStyle(Styles.largeTextLight);
			centeredStyle.alignment = TextAnchor.MiddleCenter;
		}
		anchor.Set(GUIAnchor.Centered);
	}
	public GUILabelCentered(string aString, float x, float y, float width, float height) : this (aString, new Rect(x,y,width,height)) {
	}
	public override void Draw ()
	{
		if (!visible) return;
		GUILayout.BeginArea(rect,Styles.roundDarkBox);
			GUILayout.Label(text,centeredStyle);
		GUILayout.EndArea();
	}
}

