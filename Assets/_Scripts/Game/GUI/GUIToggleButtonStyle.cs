using UnityEngine;
using System;

[Serializable]
public class GUIToggleButtonStyle {
	[SerializeField]
	protected GUIStyleEx	style;
	
	[HideInInspector]
	public bool			Selected = false;
	
	public string		Name {
		get {
			return style.Name;
		}
	}
	
	public Rect			DrawRect {
		get {
			return style.DrawRect;
		}
	}
	
	public GUIStyle		Style {
		get {
			return style.Style;
		}
	}
}