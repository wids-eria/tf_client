using UnityEngine;
using System.Collections;

/// <summary>
/// Screen settings manager.
/// </summary>
public class ScreenSettingsManager : MonoBehaviour
{
	/// <summary>
	/// Gets or sets the singleton.
	/// </summary>
	/// <value>
	/// The singleton.
	/// </value>
	public static ScreenSettingsManager use { get; private set; }
	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake()
	{
		if (use != null) Destroy(use.gameObject);
		use = this;
	}
	
	/// <summary>
	/// The supported orientations.
	/// </summary>
	[SerializeField]
#pragma warning disable 414
	private ScreenOrientation[] m_supportedOrientations = new ScreenOrientation[2] {
		ScreenOrientation.Landscape, ScreenOrientation.LandscapeRight
	};
#pragma warning restore 414
	
	/// <summary>
	/// The width of the screen.
	/// </summary>
	private int m_screenWidth;
	/// <summary>
	/// The height of the screen.
	/// </summary>
	private int m_screenHeight;
	
	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start()
	{
		m_screenWidth = Screen.width;
		m_screenHeight = Screen.height;
		
#if UNITY_IPHONE || UNITY_ANDROID
		if (m_supportedOrientations.Length == 0) return;
		else if (m_supportedOrientations.Length == 1)
		{
			switch (m_supportedOrientations[0])
			{
			case ScreenOrientation.Landscape:
				Screen.orientation = ScreenOrientation.Landscape;
				break;
			case ScreenOrientation.LandscapeRight:
				Screen.orientation = ScreenOrientation.LandscapeRight;
				break;
			case ScreenOrientation.Portrait:
				Screen.orientation = ScreenOrientation.Portrait;
				break;
			case ScreenOrientation.PortraitUpsideDown:
				Screen.orientation = ScreenOrientation.PortraitUpsideDown;
				break;
			}
		}
		else
		{
			Screen.orientation = ScreenOrientation.AutoRotation;
			foreach (ScreenOrientation oriention in m_supportedOrientations)
			{
				switch (oriention)
				{
				case ScreenOrientation.Landscape:
					Screen.autorotateToLandscapeLeft = true;
					break;
				case ScreenOrientation.LandscapeRight:
					Screen.autorotateToLandscapeRight = true;
					break;
				case ScreenOrientation.Portrait:
					Screen.autorotateToPortrait = true;
					break;
				case ScreenOrientation.PortraitUpsideDown:
					Screen.autorotateToPortraitUpsideDown = true;
					break;
				}
			}
		}
#endif
	}
	
	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update ()
	{
		if (m_screenWidth!=Screen.width || m_screenHeight!=Screen.height)
		{
			m_screenWidth = Screen.width;
			m_screenHeight = Screen.height;
			MessengerAM.Send(new MessageScreenResolutionChanged());
		}
	}
}