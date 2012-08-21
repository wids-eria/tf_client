using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIButtonList : GUIContainer
{
	Vector2 scrollPosition;
	bool horizontal;
	
	public GUIButtonList(Rect area) : base(area,true) {
	}
	public GUIButtonList(Rect area, bool horizontal) : this(area) {
		this.horizontal = horizontal;
	}
	public GUIButtonList(Rect area, params GUIButtonData[] data) : this(area) {
		GUIButtonBase button;
		foreach(GUIButtonData item in data) {
			button = new GUIButtonBase(item);
			elements.Add(button, new GUIElementSettings(this,button));
		}
	}
	public GUIButtonList(Rect area, float buttonSize, params GUIButtonData[] data) : base(area,true) {
		GUISquareButton button;
		foreach(GUIButtonData item in data) {
			button = new GUISquareButton(item,buttonSize);
			elements.Add(button, new GUIElementSettings(this,button));
		}
	}
	public GUIButtonList(Rect area, float buttonSize, bool horizontal, params GUIButtonData[] data) : this(area,buttonSize,data) {
		this.horizontal = horizontal;
	}
	
	public override void Draw()
	{
		if (!visible) return;
		GUILayout.BeginArea(rect,Styles.roundDarkBox);
		
		scrollPosition = GUILayout.BeginScrollView(scrollPosition);
		if (horizontal) {
			Horizontal();
		} else {
			Vertical();
		}
		GUILayout.EndScrollView();
		GUILayout.EndArea();
	}
	
	protected void Vertical() {
		GUILayout.BeginVertical(layoutOptions.ToArray());
			foreach(GUIObject obj in elements.Keys) {
				obj.Draw();
			}
		GUILayout.EndVertical();
	}
	protected void Horizontal() {
		GUILayout.BeginHorizontal(layoutOptions.ToArray());
			foreach(GUIObject obj in elements.Keys) {
				obj.Draw();
			}
		GUILayout.EndHorizontal();
	}
}

