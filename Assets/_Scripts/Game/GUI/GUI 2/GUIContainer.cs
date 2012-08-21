using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * 
 * Add any element settings that seem relevent. Perhaps scale?
 * --Container will then need implementation of those settings
 * 
 */

public struct GUIElementSettings {
	public Vector2 offset;
	
	public GUIElementSettings(GUIContainer zone, GUIObject obj) {
		offset = obj.position.Get() - zone.position.Get();
		//offset = obj.position.Get();// - zone.bottomLeft;
		Debug.Log(string.Format("`{0}' Offset: {1}",obj.ToString(),offset));
	}
}

[System.Serializable]     
public class GUIContainer : GUIObject
{
	protected bool drawBackground = false;
	
	protected Dictionary<GUIObject,GUIElementSettings> elements = new Dictionary<GUIObject, GUIElementSettings>();

	public GUIContainer(Rect zoneSize, bool drawBackground) : base(zoneSize) {
		position.AddBehavior(AdjustElements);
		this.drawBackground = drawBackground;
	}
	public GUIContainer(Rect zoneSize, bool drawBackground, params GUIObject[] baseElements) : this(zoneSize,drawBackground) {
		AddElement(baseElements);
	}
	public void AddElement(params GUIObject[] objects) {
		foreach(GUIObject obj in objects) {
			elements.Add(obj,GetSettings(obj));
			obj.AddBehavior(RefreshElement);
		}
	}
	public void AddElement(GUIObject obj, Vector2 relativePosition) {
		obj.position.Set(position.Get()+relativePosition);
		AddElement(obj);
	}
	
	public void RemoveElement(params GUIObject[]  objects) {
		foreach(GUIObject obj in objects) {
			elements.Remove(obj);
			obj.RemoveBehavior(RefreshElement);
		}
	}
	
	protected void ClearElements() {
		elements.Clear();
	}
	
	protected GUIElementSettings GetSettings(GUIObject obj) {
		return new GUIElementSettings(this,obj);
	}
	
	protected void RefreshElement(ref GUIObject obj) {
		elements[obj] = GetSettings(obj);
	}
	
	public void AdjustElements() {
		foreach (GUIObject obj in elements.Keys) {
			//obj.position.Set(elements[obj].offset);
			obj.position.Set(position.Get() + elements[obj].offset);
			//obj.position.Set(bottomLeft + elements[obj].offset);
			
		}
	}
	
	public override void SetPosition (Modifiable<Vector2> newPosition)
	{
		base.SetPosition (newPosition);
		AdjustElements();
	}
	
	public override bool IsMouseOver ()
	{
		bool check = base.IsMouseOver() && drawBackground;
		foreach(GUIObject obj in elements.Keys) {
			check |= obj.IsMouseOver();
		}
		return check;
	}
	
	public override void Draw()
	{
		if (!visible) return;
		if (drawBackground) {
			GUILayout.BeginArea(rect,Styles.roundDarkBox);
		} else {
			GUILayout.BeginArea(rect);
		}
		GUILayout.EndArea();
		foreach(GUIObject obj in elements.Keys) {
			obj.Draw();
		}		
	}
}

