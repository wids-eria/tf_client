using UnityEngine;
using System.Collections;

/// <summary>
/// A component to collect GUIStyles on a prefab
/// </summary>
public class GUIStyles : MonoBehaviour
{
	// mass color properties
	[HideInInspector][SerializeField]
	private Color _darkTextColor;
	public Color darkTextColor {
		get { return _darkTextColor; }
		set {
			elegantTextDark.normal.textColor = value;
			giantTextDark.normal.textColor = value;
			largeTextDark.normal.textColor = value;
			mediumTextDark.normal.textColor = value;
			smallTextDark.normal.textColor = value;
			_darkTextColor = value;
		}
	}
	[HideInInspector][SerializeField]
	private Color _mediumTextColor;
	public Color mediumTextColor {
		get { return _mediumTextColor; }
		set {
			elegantTextMedium.normal.textColor = value;
			giantTextMedium.normal.textColor = value;
			largeTextMedium.normal.textColor = value;
			mediumTextMedium.normal.textColor = value;
			smallTextMedium.normal.textColor = value;
			_mediumTextColor = value;
		}
	}
	[HideInInspector][SerializeField]
	private Color _lightTextColor;
	public Color lightTextColor {
		get { return _lightTextColor; }
		set {
			elegantTextLight.normal.textColor = value;
			giantTextLight.normal.textColor = value;
			largeTextLight.normal.textColor = value;
			mediumTextLight.normal.textColor = value;
			mediumTextLightRight.normal.textColor = value;
			smallTextLight.normal.textColor = value;
			_lightTextColor = value;
		}
	}
	[HideInInspector][SerializeField]
	private Color _highlightTextColor;
	public Color highlightTextColor {
		get { return _highlightTextColor; }
		set {
			elegantTextHighlighted.normal.textColor = value;
			giantTextHighlighted.normal.textColor = value;
			largeTextHighlighted.normal.textColor = value;
			mediumTextHighlighted.normal.textColor = value;
			smallTextHighlighted.normal.textColor = value;
			_highlightTextColor = value;
		}
	}
	
	// mass font face properties
	[HideInInspector][SerializeField]
	private Font _elegantFont;
	public Font elegantFont {
		get { return _elegantFont; }
		set {
			elegantTextDark.font = value;
			elegantTextMedium.font = value;
			elegantTextLight.font = value;
			elegantTextHighlighted.font = value;
			_elegantFont = value;
		}
	}
	[HideInInspector][SerializeField]
	private Font _giantFont;
	public Font giantFont {
		get { return _giantFont; }
		set {
			giantTextDark.font = value;
			giantTextMedium.font = value;
			giantTextLight.font = value;
			giantTextHighlighted.font = value;
			_giantFont = value;
		}
	}
	[HideInInspector][SerializeField]
	private Font _largeFont;
	public Font largeFont {
		get { return _largeFont; }
		set {
			largeTextDark.font = value;
			largeTextMedium.font = value;
			largeTextLight.font = value;
			largeTextHighlighted.font = value;
			_largeFont = value;
		}
	}
	[HideInInspector][SerializeField]
	private Font _mediumFont;
	public Font mediumFont {
		get { return _mediumFont; }
		set {
			mediumTextDark.font = value;
			mediumTextMedium.font = value;
			mediumTextLight.font = value;
			mediumTextLightRight.font = value;
			mediumTextHighlighted.font = value;
			_mediumFont = value;
		}
	}
	[HideInInspector][SerializeField]
	private Font _smallFont;
	public Font smallFont {
		get { return _smallFont; }
		set {
			smallTextDark.font = value;
			smallTextMedium.font = value;
			smallTextLight.font = value;
			smallTextHighlighted.font = value;
			_smallFont = value;
		}
	}
	
	// mass font size properties
	[HideInInspector][SerializeField]
	private int _elegantFontSize;
	public int elegantFontSize {
		get { return _elegantFontSize; }
		set {
			elegantTextDark.fontSize = value;
			elegantTextMedium.fontSize = value;
			elegantTextHighlighted.fontSize = value;
			elegantTextLight.fontSize = value;
			_elegantFontSize = value;
		}
	}
	[HideInInspector][SerializeField]
	private int _giantFontSize;
	public int giantFontSize {
		get { return _giantFontSize; }
		set {
			giantTextDark.fontSize = value;
			giantTextMedium.fontSize = value;
			giantTextHighlighted.fontSize = value;
			giantTextLight.fontSize = value;
			_giantFontSize = value;
		}
	}
	[HideInInspector][SerializeField]
	private int _largeFontSize;
	public int largeFontSize {
		get { return _largeFontSize; }
		set {
			largeTextDark.fontSize = value;
			largeTextMedium.fontSize = value;
			largeTextHighlighted.fontSize = value;
			largeTextLight.fontSize = value;
			_largeFontSize = value;
		}
	}
	[HideInInspector][SerializeField]
	private int _mediumFontSize;
	public int mediumFontSize {
		get { return _mediumFontSize; }
		set {
			mediumTextDark.fontSize = value;
			mediumTextMedium.fontSize = value;
			mediumTextHighlighted.fontSize = value;
			mediumTextLight.fontSize = value;
			mediumTextLightRight.fontSize = value;
			_mediumFontSize = value;
		}
	}
	[HideInInspector][SerializeField]
	private int _smallFontSize;
	public int smallFontSize {
		get { return _smallFontSize; }
		set {
			smallTextDark.fontSize = value;
			smallTextMedium.fontSize = value;
			smallTextHighlighted.fontSize = value;
			smallTextLight.fontSize = value;
			_smallFontSize = value;
		}
	}
	
	// mass font alignment properties
	[HideInInspector][SerializeField]
	private TextAnchor _elegantFontAlignment;
	public TextAnchor elegantFontAlignment {
		get { return _elegantFontAlignment; }
		set {
			elegantTextDark.alignment = value;
			elegantTextMedium.alignment = value;
			elegantTextHighlighted.alignment = value;
			elegantTextLight.alignment = value;
			_elegantFontAlignment = value;
		}
	}
	[HideInInspector][SerializeField]
	private TextAnchor _giantFontAlignment;
	public TextAnchor giantFontAlignment {
		get { return _giantFontAlignment; }
		set {
			giantTextDark.alignment = value;
			giantTextMedium.alignment = value;
			giantTextHighlighted.alignment = value;
			giantTextLight.alignment = value;
			_giantFontAlignment = value;
		}
	}
	[HideInInspector][SerializeField]
	private TextAnchor _largeFontAlignment;
	public TextAnchor largeFontAlignment {
		get { return _largeFontAlignment; }
		set {
			largeTextDark.alignment = value;
			largeTextMedium.alignment = value;
			largeTextHighlighted.alignment = value;
			largeTextLight.alignment = value;
			_largeFontAlignment = value;
		}
	}
	[HideInInspector][SerializeField]
	private TextAnchor _mediumFontAlignment;
	public TextAnchor mediumFontAlignment {
		get { return _mediumFontAlignment; }
		set {
			mediumTextDark.alignment = value;
			mediumTextMedium.alignment = value;
			mediumTextHighlighted.alignment = value;
			mediumTextLight.alignment = value;
			mediumTextLightRight.alignment = TextAnchor.MiddleRight;
			_mediumFontAlignment = value;
		}
	}
	[HideInInspector][SerializeField]
	private TextAnchor _smallFontAlignment;
	public TextAnchor smallFontAlignment {
		get { return _smallFontAlignment; }
		set {
			smallTextDark.alignment = value;
			smallTextMedium.alignment = value;
			smallTextHighlighted.alignment = value;
			smallTextLight.alignment = value;
			_smallFontAlignment = value;
		}
	}
	
	// mass font margin properties
	[HideInInspector][SerializeField]
	private RectOffset _elegantFontMargin;
	public RectOffset elegantFontMargin {
		get { return _elegantFontMargin; }
		set {
			elegantTextDark.margin = value;
			elegantTextMedium.margin = value;
			elegantTextHighlighted.margin = value;
			elegantTextLight.margin = value;
			_elegantFontMargin = value;
		}
	}
	[HideInInspector][SerializeField]
	private RectOffset _giantFontMargin;
	public RectOffset giantFontMargin {
		get { return _giantFontMargin; }
		set {
			giantTextDark.margin = value;
			giantTextMedium.margin = value;
			giantTextHighlighted.margin = value;
			giantTextLight.margin = value;
			_giantFontMargin = value;
		}
	}
	[HideInInspector][SerializeField]
	private RectOffset _largeFontMargin;
	public RectOffset largeFontMargin {
		get { return _largeFontMargin; }
		set {
			largeTextDark.margin = value;
			largeTextMedium.margin = value;
			largeTextHighlighted.margin = value;
			largeTextLight.margin = value;
			_largeFontMargin = value;
		}
	}
	[HideInInspector][SerializeField]
	private RectOffset _mediumFontMargin;
	public RectOffset mediumFontMargin {
		get { return _mediumFontMargin; }
		set {
			mediumTextDark.margin = value;
			mediumTextMedium.margin = value;
			mediumTextHighlighted.margin = value;
			mediumTextLight.margin = value;
			mediumTextLightRight.margin = value;
			_mediumFontMargin = value;
		}
	}
	[HideInInspector][SerializeField]
	private RectOffset _smallFontMargin;
	public RectOffset smallFontMargin {
		get { return _smallFontMargin; }
		set {
			smallTextDark.margin = value;
			smallTextMedium.margin = value;
			smallTextHighlighted.margin = value;
			smallTextLight.margin = value;
			_smallFontMargin = value;
		}
	}
	
	/// <summary>
	/// Line direction.
	/// </summary>
	public enum LineDirection { Horizontal, Vertical }
	/// <summary>
	/// Line color.
	/// </summary>
	public enum LineColor { Dark, Medium, Light, Highlighted }
	
	/// <summary>
	/// The color of the GUI.
	/// </summary>
	private Color m_GUIColor;
	
	/// <summary>
	/// Draws the line.
	/// </summary>
	/// <param name='direction'>
	/// Direction.
	/// </param>
	/// <param name='color'>
	/// Color.
	/// </param>
	public void DrawLine(LineDirection direction, LineColor color)
	{
		DrawLine(direction, color, -1f);
	}
	
	/// <summary>
	/// Draws the line.
	/// </summary>
	/// <param name='direction'>
	/// Direction.
	/// </param>
	/// <param name='color'>
	/// Color.
	/// </param>
	/// <param name='length'>
	/// Length.
	/// </param>
	public void DrawLine(LineDirection direction, LineColor color, float length)
	{
		m_GUIColor = GUI.color;
		switch (color) {
		case LineColor.Dark:
			GUI.color = darkTextColor;
			break;
		case LineColor.Medium:
			GUI.color = mediumTextColor;
			break;
		case LineColor.Light:
			GUI.color = lightTextColor;
			break;
		case LineColor.Highlighted:
			GUI.color = highlightTextColor;
			break;
		}
		switch (direction) {
		case LineDirection.Horizontal:
			if (length > 0f) {
				GUILayout.Box("", line, GUILayout.Height(lineWeight), GUILayout.Width(length));
			}
			else {
				GUILayout.Box("", line, GUILayout.Height(lineWeight));
			}
			break;
		case LineDirection.Vertical:
			if (length > 0f) {
				GUILayout.Box("", line, GUILayout.Width(lineWeight), GUILayout.Height(length));
			}
			else {
				GUILayout.Box("", line, GUILayout.Width(lineWeight));
			}
			break;
		}
		GUI.color = m_GUIColor;
	}
	
	/// <summary>
	/// The line weight.
	/// </summary>
	public float lineWeight = 1f;
	
	// styles
	public GUIStyle line;
	
	public GUIStyle elegantTextDark;
	public GUIStyle elegantTextMedium;
	public GUIStyle elegantTextLight;
	public GUIStyle elegantTextHighlighted;
	public GUIStyle giantTextDark;
	public GUIStyle giantTextMedium;
	public GUIStyle giantTextLight;
	public GUIStyle giantTextHighlighted;
	public GUIStyle largeTextDark;
	public GUIStyle largeTextMedium;
	public GUIStyle largeTextLight;
	public GUIStyle largeTextHighlighted;
	public GUIStyle mediumTextDark;
	public GUIStyle mediumTextMedium;
	public GUIStyle mediumTextLight;
	public GUIStyle mediumTextHighlighted;
	public GUIStyle smallTextDark;
	public GUIStyle smallTextMedium;
	public GUIStyle smallTextLight;
	public GUIStyle smallTextHighlighted;
	
	public GUIStyle mediumTextLightRight;
	
	public GUIStyle roundDarkBox;
	
	public GUIStyle sqLightGreyBox;
	
	public GUIStyle inputField;
	/// <summary>
	/// Gets the height of the input field.
	/// </summary>
	/// <value>
	/// The height of the input field.
	/// </value>
	public float inputFieldHeight { get { return GUIHelpers.GetFontSizeFromName(inputField.font) + inputField.padding.top + inputField.padding.bottom; } }
	
	public GUIStyle button;
	public GUIStyle smallButton;
	public GUIStyle smallButtonFocused;
	public GUIStyle smallButtonDisabled;
	public GUIStyle toggle;
	public GUIStyle foldout;
	
	public GUIStyle histogramBar;
			
	public GUIStyle empty;
	public GUIStyle empty4;
}