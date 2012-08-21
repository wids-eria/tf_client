using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GUICircularData {
	public bool isRadioGroup;
	public bool openFromCenter;
	public float buttonSize;
	public float radius;
	public float startAngle;
	public float arcDegrees;
	public GUIObject.Direction direction;
}

public class GUICircularMenuOptions : System.Object {
	public Modifiable<bool>		isRadioGroup;
	public Modifiable<bool>		openFromCenter;
	public Modifiable<float> 	buttonSize;
	public Modifiable<float> 	radius;
	public Modifiable<float> 	startAngle;
	public Modifiable<float> 	arcDegrees;
	public Modifiable<GUIObject.Direction> direction;
	
	public GUICircularMenu owner;
	
	public GUICircularMenuOptions(GUICircularMenu owner, GUICircularData data) {
		isRadioGroup = 	 new Modifiable<bool>(data.isRadioGroup,ValueChanged);
		openFromCenter = new Modifiable<bool>(data.openFromCenter,ValueChanged);
		radius = 		 new Modifiable<float>(data.radius,ValueChanged);
		startAngle = 	 new Modifiable<float>(data.startAngle,ValueChanged);
		arcDegrees = 	 new Modifiable<float>(data.arcDegrees,ValueChanged);
		direction = 	 new Modifiable<GUIObject.Direction>(data.direction,ValueChanged);
		buttonSize =	 new Modifiable<float>(data.buttonSize,ValueChanged);
		
		this.owner = owner;
	}
	public GUICircularMenuOptions Copy() {
		return (GUICircularMenuOptions)this.MemberwiseClone();
	}
	protected void ValueChanged() {
		owner.Refresh();
	}
}

[System.Serializable]
public class GUICircularMenu : GUIObject
{
	public 		GUICircularMenuOptions 	options {get; protected set;}

	public 		List<GUISquareButton> 	buttons = new List<GUISquareButton>();
	public 		GUIRadioGroup			radioGroup;
	
	private float m_prevT;
	private float m_directionMod { get {return (options.direction.Get() == GUIObject.Direction.Clockwise) ? -1 : 1;}}
	
	public GUICircularMenu(IEnumerable<GUIButtonData> buttonSet, GUICircularData guiOptions) : 
		base(new Rect(0,0,guiOptions.radius*2,guiOptions.radius*2)) 
	{
		options = new GUICircularMenuOptions(this,guiOptions);
		
		position.AddBehavior(Refresh);
		foreach(GUIButtonData data in buttonSet) {
			buttons.Add(new GUISquareButton(data,options.buttonSize.Get()));
		}
		radioGroup = new GUIRadioGroup(buttons.ToArray());
		Refresh();
	}
	
	public static void SetButtonSize(GUICircularMenu menu, float buttonSize)
	{
		foreach(GUISquareButton button in menu.buttons) {
			button.size = buttonSize;
		}
	}
	public void SetButtonSize(GUISquareButton button, float size) {
		button.size = size;
	}
	public void SetButtonPositions(float t)
	{
		SetPositions(this,t);
	}
	public static void SetPositions(GUICircularMenu menu, float t) {
		t = Mathf.Clamp01(t);
		
		float angle, angleSpread, baseAngle;
		
		angleSpread = ( menu.options.arcDegrees.Get() / (menu.buttons.Count-1) ) * -menu.m_directionMod;
		baseAngle = -menu.options.startAngle.Get();
		
		for (int i = 0; i < menu.buttons.Count; i++) {
			angle = Mathf.LerpAngle(
				baseAngle, 
				baseAngle + (menu.m_directionMod * ((!menu.options.openFromCenter.Get()) ? 0 : (menu.options.arcDegrees.Get()/2))) + (i*angleSpread), 
				t
			);
			angle *= Mathf.Deg2Rad;
			menu.buttons[i].position.Value = menu.position.Get() + 
				new Vector2(menu.options.radius.Get() * Mathf.Cos(angle), menu.options.radius.Get() * Mathf.Sin(angle));
		}
		menu.m_prevT = t;
	}
	public static void InverseSetPostions(GUICircularMenu menu, float t) {
		t = 1 - Mathf.Clamp01(t);
		SetPositions(menu,t);
	}
	
	public void Refresh() {
		if (options.isRadioGroup.Get() && radioGroup == null) {
			radioGroup = new GUIRadioGroup(buttons.ToArray());
		} else if (!options.isRadioGroup.Get() && radioGroup != null) {
			radioGroup = null;
		}
		SetButtonPositions(m_prevT);
	}
	
	public override void Draw()
	{
		if (!visible) return;
		foreach(GUISquareButton button in buttons) {
			button.Draw();
		}
	}
	
	public override bool IsMouseOver ()
	{
		foreach(GUISquareButton button in buttons) {
			if (button.IsMouseOver()) {
				return true;
			}
		}
		return false;
	}
}

