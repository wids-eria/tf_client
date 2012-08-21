using UnityEditor;
using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Custom editor for the GUIStyles component
/// </summary>
[CustomEditor(typeof(GUIStyles))]
public class GUIStylesEditor : Editor
{
	GUIStyles styles;
	
	void OnEnable()
	{
		styles = (GUIStyles) target;
	}
	
	public override void OnInspectorGUI()
	{
		// mass color editors
		EditorGUI.indentLevel = 1;
		styles.darkTextColor = EditorGUILayout.ColorField("Dark Text", styles.darkTextColor);
		styles.mediumTextColor = EditorGUILayout.ColorField("Medium Text", styles.mediumTextColor);
		styles.lightTextColor = EditorGUILayout.ColorField("Light Text", styles.lightTextColor);
		styles.highlightTextColor = EditorGUILayout.ColorField("Highlight Text", styles.highlightTextColor);
		
		// mass font editors
		FontEditor("ELEGANT FONT:",
		           () => styles.elegantFont, value => styles.elegantFont = value,
		           () => styles.elegantFontSize, value => styles.elegantFontSize = value,
		           () => styles.elegantFontAlignment, value => styles.elegantFontAlignment = value,
		           () => styles.elegantFontMargin, value => styles.elegantFontMargin = value);
		FontEditor("GIANT FONT:",
		           () => styles.giantFont, value => styles.giantFont = value,
		           () => styles.giantFontSize, value => styles.giantFontSize = value,
		           () => styles.giantFontAlignment, value => styles.giantFontAlignment = value,
		           () => styles.giantFontMargin, value => styles.giantFontMargin = value);
		FontEditor("LARGE FONT:",
		           () => styles.largeFont, value => styles.largeFont = value,
		           () => styles.largeFontSize, value => styles.largeFontSize = value,
		           () => styles.largeFontAlignment, value => styles.largeFontAlignment = value,
		           () => styles.largeFontMargin, value => styles.largeFontMargin = value);
		FontEditor("MEDIUM FONT:",
		           () => styles.mediumFont, value => styles.mediumFont = value,
		           () => styles.mediumFontSize, value => styles.mediumFontSize = value,
		           () => styles.mediumFontAlignment, value => styles.mediumFontAlignment = value,
		           () => styles.mediumFontMargin, value => styles.mediumFontMargin = value);
		FontEditor("SMALL FONT:",
		           () => styles.smallFont, value => styles.smallFont = value,
		           () => styles.smallFontSize, value => styles.smallFontSize = value,
		           () => styles.smallFontAlignment, value => styles.smallFontAlignment = value,
		           () => styles.smallFontMargin, value => styles.smallFontMargin = value);
		
		// draw the rest of the inspector
		DrawDefaultInspector();
		
		// set dirty
		EditorUtility.SetDirty(this);
	}
	
	void FontEditor(string title,
	                Func<Font> getFont, Action<Font> setFont,
	                Func<int> getSize, Action<int> setSize,
	                Func<TextAnchor> getAlignment, Action<TextAnchor> setAlignment,
	                Func<RectOffset> getMargin, Action<RectOffset> setMargin)
	{
		EditorGUILayout.BeginVertical(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).box);
		{
			EditorGUILayout.PrefixLabel(title);
			GUILayout.BeginHorizontal();
			{
				setFont((Font)EditorGUILayout.ObjectField(getFont(), typeof(Font), false));
				setSize(EditorGUILayout.IntField(getSize(), GUILayout.Width(50f)));
				setAlignment((TextAnchor)EditorGUILayout.EnumPopup(getAlignment()));;
			}
			GUILayout.EndHorizontal();
			EditorGUILayout.BeginVertical();
			{
				RectOffset margin = getMargin();
				margin.left = EditorGUILayout.IntField("Left Margin", margin.left);
				margin.right = EditorGUILayout.IntField("Right Margin", margin.right);
				margin.top = EditorGUILayout.IntField("Top Margin", margin.top);
				margin.bottom = EditorGUILayout.IntField("Bottom Margin", margin.bottom);
				setMargin(margin);
			}
			EditorGUILayout.EndVertical();
		}
		EditorGUILayout.EndVertical();
	}
}