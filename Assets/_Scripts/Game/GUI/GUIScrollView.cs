using UnityEngine;
using System;

[Serializable]
public class GUIScrollView {
	public Rect			DrawRect;
	
	[SerializeField]
	protected GUIStyle	sliderStyle;
	
	[SerializeField]
	protected GUIStyle	thumbStyle;
	
	[HideInInspector]
	public Vector2		Position = Vector2.zero;
	
	public GUIStyle	SliderStyle {
		get {
			return sliderStyle;
		}
	}
	
	public GUIStyle	ThumbStyle {
		get {
			return thumbStyle;
		}
	}
}

[Serializable]
public class GUILayoutScrollView {
	[SerializeField]
	protected Size		size;
	
	[SerializeField]
	protected GUILayoutHorizontalSlider	horizintalSliderStyle;
	
	[SerializeField]
	protected GUILayoutVerticalSlider	verticalSliderStyle;
	
	public	float		Width {
		get {
			return size.width;
		}
	}
	
	public	float		Height {
		get {
			return size.height;
		}
	}
	
	[HideInInspector]
	protected Vector2	position = new Vector2();
	
	[HideInInspector]
	public Vector2		Position {
		get {
			position.Set( horizintalSliderStyle.Value, verticalSliderStyle.Value );
			return position;
		}
		
		set {
			horizintalSliderStyle.Value = value.x;
			verticalSliderStyle.Value = value.y;
		}
	}

	public void		SetMin( int index, float m ) {
		switch( index ) {
			case 0:
				horizintalSliderStyle.Min = m;
				break;
			
			case 1:
				verticalSliderStyle.Min = m;
				break;
		}
	}
	
	public void		SetMax( int index, float m ) {
		switch( index ) {
			case 0:
				horizintalSliderStyle.Max = m;
				break;
			
			case 1:
				verticalSliderStyle.Max = m;
				break;
		}
	}
	
	public void	Begin() {
		GUILayout.BeginVertical();
		GUILayout.BeginHorizontal();
	    GUILayout.BeginScrollView( Position, GUIStyle.none, GUIStyle.none, GUILayout.Width(Width), GUILayout.Height(Height) );
	}

	public void	End() {
		GUILayout.EndScrollView();
	
		verticalSliderStyle.Draw();
		GUILayout.EndHorizontal();
		
		horizintalSliderStyle.Draw();
		GUILayout.EndVertical();
	}
}