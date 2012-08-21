using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GUIButtonData {
	public Texture2D icon;
	public string text;
	public GUIAnchor anchor;
	public bool isSelectable;
	public bool usePersonalArea;
	public float x;
	public float y;
	public float width;
	public float height;
	
	public Rect rect {get {return new Rect(x,y,width,height);}}
	
	public Vector2 position {get {return new Vector2(x,y); } set {x = value.x; y = value.y; } }
	
	public GUIButtonData() {
		width = 64;
		height = 64;
		text = "T";
	}
	public GUIButtonData(GUIButtonData dataToCopy) {
		icon = dataToCopy.icon;
		text = dataToCopy.text;
		anchor = dataToCopy.anchor;
	 	isSelectable = dataToCopy.isSelectable;
		usePersonalArea = dataToCopy.usePersonalArea;
		x = dataToCopy.x;
		y = dataToCopy.y;
		width = dataToCopy.width;
		height = dataToCopy.height;
	}
	public GUIButtonData Clone() {
		return (GUIButtonData)MemberwiseClone();
	}
}


public class GUIButtonBase : GUIObject, ISelectable
{
	public Action<GUIButtonBase> 	OnButtonPressed;
	public Action<ISelectable> 		OnSelected {get { return m_OnSelected; } set { m_OnSelected = value; }}
	
	protected Action<ISelectable>	m_OnSelected;
	
	public bool enabled { get {return m_enabled;} set {SetEnabled(value);}}
	public bool isSelected { get; protected set; }
	public bool usePersonalArea { get; set; }
	
	public string text;
	
	private bool m_enabled = true;
	private bool m_isSelectable = false;
	
	protected virtual GUIStyle style {get { return (isSelected) ? Styles.smallButtonFocused : (enabled) ? Styles.smallButton : Styles.smallButtonDisabled; }}
	
	
	
	public GUIButtonBase(GUIButtonData data) : this(data,null) {
	}
	public GUIButtonBase(GUIButtonData data, Action<GUIButtonBase> buttonAction) : base(data.rect) {
		text = data.text;
		usePersonalArea = data.usePersonalArea;
		anchor.Set(data.anchor);
		SetSelectable(data.isSelectable);
		OnButtonPressed += buttonAction;
	}

	public void SetEnabled(bool isEnabled) {
		m_enabled = isEnabled;
		if (isEnabled) {
			Enable();
		} else {
			Disable();
		}
	}
	public virtual void Enable(){
	}
	public virtual void Disable() {
	}
	
	public override void Draw() {
		if (!visible) return;
		if (usePersonalArea) {
			GUILayout.BeginArea(rect);
			DrawButton();
			GUILayout.EndArea();
		} else {
			DrawButton();
		}
	}
	protected virtual void DrawButton() {
		if ( GUILayout.Button(text,style,layoutOptions.ToArray()) && enabled ) {
			if (OnButtonPressed != null) {
				OnButtonPressed(this);
			}
		}
	}
	
	// * * * * * * * ISelectable Implementations * * * * * * * //
	
	public virtual void SetSelected (bool selected)  {
		isSelected = selected & (IsSelectable());
		if (isSelected) OnSelected(this);
	}
	public virtual bool GetSelected() {
		return isSelected;
	}
	public virtual bool IsSelectable() {
		return (enabled & m_isSelectable);
	}
	public virtual void SetSelectable(bool isSelectable) {
		if (m_isSelectable == isSelectable) return;
		
		m_isSelectable = isSelectable;
		if (m_isSelectable) {
			OnButtonPressed += AddSelectable;
		} else { 
			OnButtonPressed -= AddSelectable;
		}
	}
	private void AddSelectable(GUIButtonBase obj) {
		SetSelected(true);
	}
}

public abstract class GUIButtonIcon : GUIButtonBase
{
	public Texture2D	icon;
	public GUIButtonIcon(GUIButtonData data) : base(data) {
		icon = data.icon;
	}
	protected override void DrawButton ()
	{
		if ( GUILayout.Button(icon,style,layoutOptions.ToArray()) && enabled ) {
			if (OnButtonPressed != null) OnButtonPressed(this);
		}
	}
}


public class GUISquareButton : GUIButtonIcon
{
	public virtual float 		size 		{ get { return width.Get(); } set { width.Set(value); height.Set(value); } }
	
	public GUISquareButton(GUIButtonData data, float aSize) : base(data) {
		width.AddBehavior(ResetLayoutOptions);
		height.AddBehavior(ResetLayoutOptions);
		size = aSize;
		ResetLayoutOptions();
	}
	protected void ResetLayoutOptions() {
		layoutOptions.Clear();
		layoutOptions.Add(GUILayout.Height(size));
		layoutOptions.Add(GUILayout.Width(size));
	}
}


