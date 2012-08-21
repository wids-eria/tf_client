using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


//IF ANYBODY CAN FIGURE OUT A BETTER WAY TO IMPLEMENT THE LERPING COMMANDS AND STUFF.... cookies for you

/*
 * = = = = = = = = = = = = USAGE NOTES = = = = = = = = = = = =
 * 
 * Anything that inherits from Move<T,U> needs to call 
 * the base() constructor or assign 'coroutine' themselves,
 * otherwise a NullReferenceException will be thrown at
 * run time.
 * 
 * Anthing in GUIEffects should be things that inherit from
 * 'CommandBase', and most likely 'CommandCoroutine' and added
 * to a GUIObject's 'animationQueue'.
 * 
 * = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =
 */ 

/// <summary>
/// This class contains commands for executing GUI Animations and Effects.
/// </summary>
public class GUIEffects
{
	public static AnimationCurve easingCurve = AnimationCurve.EaseInOut(0,0,1,1);
	public static AnimationCurve linearCurve = AnimationCurve.Linear(0,0,1,1);
	
	/// <summary>
	/// Use AnimationCurve.EaseInOut or whatever for easing movment (arg 4)
	/// </summary>
	public class CommandMove : Move<GUIObject, Vector2> {
		public CommandMove(GUIObject obj, float time, Vector2 destination) : base() {
			settings.time = time;
			settings.obj = obj;
			settings.valA = obj.position.Get();
			settings.valB = destination;
			Lerp = Vector2.Lerp;
			Set = Setter;
		}
		public CommandMove(GUIObject obj, float time, Vector2 destination, AnimationCurve curve) : this(obj,time,destination) {
			settings.curve = curve;
		}
		public CommandMove(GUIObject obj, float time, Vector2 start, Vector2 destination, AnimationCurve curve) : this(obj,time,destination,curve) {
			settings.valA = start;
		}
		protected void Setter(GUIObject obj, Vector2 val) {
			obj.position.Set(val);
		}
	}
	
	public class CircularMenuButtonSize : Move<GUICircularMenu,float> {
		public CircularMenuButtonSize(GUICircularMenu menu, float time, float size) : base() {
			settings.time = time;
			settings.obj = menu;
			settings.valA = menu.buttons[0].size;
			settings.valB = size;
			Lerp = Mathf.Lerp;
			Set = GUICircularMenu.SetButtonSize;
		}
		public CircularMenuButtonSize(GUICircularMenu menu, float time, float size, AnimationCurve curve) : this(menu,time,size) {
			settings.curve = curve;
		}
	}
	
	public class CircularMenuOpen : Move<GUICircularMenu,float> {
		public CircularMenuOpen(GUICircularMenu menu, float time) : base() {
			settings.time = time;
			settings.obj = menu;
			settings.valA = 0f;
			settings.valB = 1f;
			Lerp = Mathf.Lerp;
			Set = GUICircularMenu.SetPositions;
		}
	}
	public class CircularMenuClose : Move<GUICircularMenu,float> {
		public CircularMenuClose(GUICircularMenu menu, float time) : base() {
			settings.time = time;
			settings.obj = menu;
			settings.valA = 1f;
			settings.valB = 0f;
			Lerp = Mathf.Lerp;
			Set = GUICircularMenu.SetPositions;
		}
	}
	public class CircularMenuRotate : Move<GUICircularMenu,float> {
		public CircularMenuRotate(GUICircularMenu menu, float time, float newAngle) : base() {
			settings.time = time;
			settings.obj = menu;
			settings.valA = menu.options.startAngle.Get();
			settings.valB = newAngle;
			Lerp = Mathf.LerpAngle;
			Set = Setter;
		}
		public CircularMenuRotate(GUICircularMenu menu, float time, float newAngle, AnimationCurve curve) : this(menu,time,newAngle) {
			settings.curve = curve;
		}
		protected void Setter(GUICircularMenu menu, float val) {
			menu.options.startAngle.Set(val);
		}
	}
	public class CircularMenuArc : Move<GUICircularMenu,float> {
		public CircularMenuArc(GUICircularMenu menu, float time, float newArc) : base() {
			settings.time = time;
			settings.obj = menu;
			settings.valA = menu.options.arcDegrees.Get();
			settings.valB = newArc;
			Lerp = Mathf.LerpAngle;
			Set = Setter;
		}
		protected void Setter(GUICircularMenu menu, float val) {
			menu.options.arcDegrees.Set(val);
		}
	}
	public class CircularMenuRadius : Move<GUICircularMenu,float> {
		public CircularMenuRadius(GUICircularMenu menu, float time, float radius) : base() {
			settings.time = time;
			settings.obj = menu;
			settings.valA = menu.options.radius.Get();
			settings.valB = radius;
			Lerp = Mathf.LerpAngle;
			Set = Setter;
		}
		protected void Setter(GUICircularMenu menu, float val) {
			menu.options.radius.Set(val);
		}
	}
	
	public class SetVisibility : CommandBase {
		protected GUIObject obj;
		protected bool isVisible;
		
		public SetVisibility(GUIObject obj, bool isVisible) {
			this.obj = obj;
			this.isVisible = isVisible;
		}
		protected override IEnumerator Execute() {
			obj.visible = isVisible;
			return base.Execute ();
		}
	}
	
	
	

}

