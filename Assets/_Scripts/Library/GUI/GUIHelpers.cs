using UnityEngine;
using System.Collections;

/// <file>
/// 
/// <author>
/// Adam Mechtley
/// http://adammechtley.com/donations
/// </author>
/// 
/// <copyright>
/// Copyright (c) 2011,  Adam Mechtley.
/// All rights reserved.
/// 
/// Redistribution and use in source and binary forms, with or without
/// modification, are permitted provided that the following conditions are met:
/// 
/// 1. Redistributions of source code must retain the above copyright notice,
/// this list of conditions and the following disclaimer.
/// 
/// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
/// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
/// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
/// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE
/// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
/// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
/// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
/// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
/// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
/// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
/// POSSIBILITY OF SUCH DAMAGE.
/// </copyright>
/// 
/// <summary>
/// This file contains a class with static methods for working with GUI.
/// </summary>
/// 
/// </file>

/// <summary>
/// describes the corner of the screen to which a rect is anchored
/// </summary>
/// 
[System.Flags]
public enum GUIAnchor 
{ 
	None	= 0,
	Top 	= 1 << 0, 
	Bottom 	= 1 << 1,
	Middle 	= 1 << 2,
	Left 	= 1 << 3,
	Right 	= 1 << 4,
	Center 	= 1 << 5,
	TopLeft = Top | Left,
	Centered = Middle | Center,
	BottomCenter = Bottom | Center,
}

/// <summary>
/// A class containing various helper functionsfor working with GUI
/// </summary>
public static class GUIHelpers : System.Object
{
	public static bool CompareAnchor(GUIAnchor a, GUIAnchor b) {
		return ( (a & b) == b );
	}
	
	private static Ray m_intersectionRay;
	private static Plane m_intersectionPlane = new Plane(Vector3.up, Vector3.zero);

	/// <summary>
	/// the default height of a horizontal gui element when using GUILayout
	/// </summary>
	public static float defaultLineHeight { get { return 22f; } }
	
	/// <summary>
	/// Remaps GUI to mouse position and vice versa
	/// </summary>
	/// <param name="v">
	/// A <see cref="Vector3"/>
	/// </param>
	/// <returns>
	/// A <see cref="Vector3"/>
	/// </returns>
	public static Vector3 MouseToGUIPosition(Vector3 v)
	{
		return new Vector3(v.x, Screen.height-v.y, v.z);
	}
	
	public static Vector3 MouseToWorldCoord()
	{
		float dist;
		m_intersectionRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		m_intersectionPlane.Raycast(m_intersectionRay, out dist);
		return m_intersectionRay.GetPoint(dist);
	}
	public static Vector2 WorldToGUIPosition(Vector3 v)
	{
		Vector3 cam = Camera.mainCamera.WorldToViewportPoint(v);
		cam.x *= Screen.width;
		cam.y = 1-cam.y;
		cam.y *= Screen.height;
		return cam;
	}
	public static Vector3 GUIToWorldPosition(Vector2 v) {
		float dist;
		v.y = Screen.height - v.y;
        m_intersectionRay = Camera.mainCamera.ScreenPointToRay(v);
		m_intersectionPlane.Raycast(m_intersectionRay, out dist);
        return m_intersectionRay.GetPoint(dist);
	}
	
	/// <summary>
	/// Returns the height of one pixel at depth in frustum as world space distance
	/// </summary>
	/// <param name="camera">
	/// A <see cref="Camera"/>
	/// </param>
	/// <param name="depth">
	/// A <see cref="System.Single"/>
	/// </param>
	/// <returns>
	/// A <see cref="System.Single"/>
	/// </returns>
	public static float OnePixelInWorld(Camera camera, float depth)
	{
		return (camera.ScreenToWorldPoint(Vector3.up + Vector3.forward*depth) - camera.ScreenToWorldPoint(Vector3.zero + Vector3.forward*depth)).y;
	}
	
	/// <summary>
	/// Return a copy of source skin
	/// </summary>
	/// <param name="source">
	/// A <see cref="GUISkin"/>
	/// </param>
	/// <returns>
	/// A <see cref="GUISkin"/>
	/// </returns>
	public static GUISkin CopySkin(GUISkin source)
	{
		GUISkin newSkin = ScriptableObject.CreateInstance(typeof(GUISkin)) as GUISkin;
		newSkin.box = new GUIStyle(source.box);
		newSkin.button = new GUIStyle(source.button);
		newSkin.customStyles = new GUIStyle[source.customStyles.Length];
		for (int i=0; i<newSkin.customStyles.Length; i++)
			newSkin.customStyles[i] = new GUIStyle(source.customStyles[i]);
		newSkin.font = source.font;
		newSkin.horizontalScrollbar = new GUIStyle(source.horizontalScrollbar);
		newSkin.horizontalScrollbarLeftButton = new GUIStyle(source.horizontalScrollbarLeftButton);
		newSkin.horizontalScrollbarRightButton = new GUIStyle(source.horizontalScrollbarRightButton);
		newSkin.horizontalScrollbarThumb = new GUIStyle(source.horizontalScrollbarThumb);
		newSkin.horizontalSlider = new GUIStyle(source.horizontalSlider);
		newSkin.horizontalSliderThumb = new GUIStyle(source.horizontalSliderThumb);
		newSkin.label = new GUIStyle(source.label);
		newSkin.scrollView = new GUIStyle(source.scrollView);
		newSkin.textArea = new GUIStyle(source.textArea);
		newSkin.textField = new GUIStyle(source.textField);
		newSkin.toggle = new GUIStyle(source.toggle);
		newSkin.verticalScrollbar = new GUIStyle(source.verticalScrollbar);
		newSkin.verticalScrollbarDownButton = new GUIStyle(source.verticalScrollbarDownButton);
		newSkin.verticalScrollbarThumb = new GUIStyle(source.verticalScrollbarThumb);
		newSkin.verticalScrollbarUpButton = new GUIStyle(source.verticalScrollbarUpButton);
		newSkin.verticalSlider = new GUIStyle(source.verticalSlider);
		newSkin.verticalSliderThumb = new GUIStyle(source.verticalSliderThumb);
		newSkin.window = new GUIStyle(source.window);
		newSkin.name = string.Format("{0} (Duplicate)", source);
		return newSkin;
	}
	
	/// <summary>
	/// A regular expression to get a number at the end of a string
	/// </summary>
	private static System.Text.RegularExpressions.Regex m_getFontSize = new System.Text.RegularExpressions.Regex("[0-9]+$");
	
	/// <summary>
	/// Get the size of a font, assuming it is indicated at the end of the font's name (e.g., Font - 32)
	/// </summary>
	/// <param name="f">
	/// A <see cref="Font"/>
	/// </param>
	/// <returns>
	/// A <see cref="System.Int32"/>
	/// </returns>
	public static int GetFontSizeFromName(Font f)
	{
		int ret;
		int.TryParse(m_getFontSize.Match(f.name).Value, out ret);
		return ret;
	}
}