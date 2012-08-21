using UnityEngine;
using System;

[Serializable]
public class GUISliderStyle {
	[SerializeField]
	protected GUIStyleEx	sliderStyle;
	
	[SerializeField]
	protected GUIStyle		thumbStyle;
	
	public Rect			DrawRect {
		get {
			return sliderStyle.DrawRect;
		}
		
		set {
			sliderStyle.DrawRect = value;
		}
	}
	
	public string 		Name {
		get {
			return sliderStyle.Name;
		}
	}
	
	public GUIStyle		SliderStyle {
		get {
			return sliderStyle.Style;
		}
	}
	
	public GUIStyle		ThumbStyle {
		get {
			return thumbStyle;
		}
	}
	
	[HideInInspector]
	public float	Value;
	public float	Min;
	public float	Max;
}

[Serializable]
public abstract class GUILayoutSlider {
	public float	Min;
	public float	Max;

	[HideInInspector]
	public float	Value;

	[SerializeField]
	protected Size		size;
	
	[SerializeField]
	protected GUIStyle	sliderStyle;
	
	[SerializeField]
	protected GUIStyle	thumbStyle;
	
	public string 		Name {
		get {
			return sliderStyle.name;
		}
	}
	
	public GUIStyle		SliderStyle {
		get {
			return sliderStyle;
		}
	}
	
	public GUIStyle		ThumbStyle {
		get {
			return thumbStyle;
		}
	}
	
	public abstract void		Draw();
}

[Serializable]
public class GUILayoutHorizontalSlider : GUILayoutSlider {
	public override void		Draw() {
		Value = GUILayout.HorizontalSlider( Value, Min, Max, SliderStyle, ThumbStyle, GUILayout.Width(size.width), GUILayout.Height(size.height) );
	}
}

[Serializable]
public class GUILayoutVerticalSlider : GUILayoutSlider {
	public override void		Draw() {
		Value = GUILayout.VerticalSlider( Value, Min, Max, SliderStyle, ThumbStyle, GUILayout.Width(size.width), GUILayout.Height(size.height) );
	}
}