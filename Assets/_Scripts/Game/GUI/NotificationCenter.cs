using UnityEngine;
using System.Collections;

/// <summary>
/// Player message center.
/// </summary>
public class NotificationCenter : GUIObject
{
	/// <summary>
	/// Gets or sets the singleton.
	/// </summary>
	/// <value>
	/// The singleton.
	/// </value>
	public static NotificationCenter use { get; private set; }
	
	/// <summary>
	/// Initializes a new instance of the <see cref="NotificationCenter"/> class.
	/// </summary>
	public NotificationCenter(){
		use = this;
		m_rect.width = Screen.width * 0.25f;
		m_rect.x = Screen.width - rect.width - m_box.margin.right;
		m_rect.y = m_box.margin.top;
	}

	/// <summary>
	/// Gets the height of the closed rect.
	/// </summary>
	/// <value>
	/// The height of the closed rect.
	/// </value>
	private float m_closedRectHeight {
		get {
			return m_box.padding.top + 
				m_smallButton.margin.top + 
				m_smallButton.padding.top + 
				GUIHelpers.GetFontSizeFromName(m_smallButton.font) +
				m_smallButton.padding.bottom + 
				m_smallButton.margin.bottom + 
				m_box.padding.bottom;
		}
	}
	
	/// <summary>
	/// Gets the height of the open rect.
	/// </summary>
	/// <value>
	/// The height of the open rect.
	/// </value>
	private float m_openRectHeight {get {return 150f;}	}
	
	/// <summary>
	/// The rect in the GUI.
	/// </summary>
	public Rect rect {
		get {
			m_rect.height = (m_isNotificationBrowserExpanded)?m_openRectHeight:m_closedRectHeight;
			return m_rect;
		}
	}
	/// <summary>
	/// The rect backing field.
	/// </summary>
	private Rect m_rect;
	
	/// <summary>
	/// Specifies if the listing browser is expanded.
	/// </summary>
	private bool m_isNotificationBrowserExpanded = false;
	/*
	/// <summary>
	/// Gets the number of notifications.
	/// </summary>
	/// <value>
	/// The number of notifications.
	/// </value>
	protected int numberOfNotifications {
		get {
			int i = 0;
			foreach (Bids bids in PlayerActionLandTransactions.bidsOnMyListings.Values) {
				i += bids.Length;
			}
			return i;
		}
	}*/
	
	/// <summary>
	/// The scroll position.
	/// </summary>
	private Vector2 m_scrollPosition;
	
	/// <summary>
	/// Gets the box style.
	/// </summary>
	/// <value>
	/// The box style.
	/// </value>
	private GUIStyle m_box { get { return Styles.roundDarkBox; } }
	
	/// <summary>
	/// Gets the small button style.
	/// </summary>
	/// <value>
	/// The small button style.
	/// </value>
	private GUIStyle m_smallButton { get { return Styles.smallButton; } }
	
	/// <summary>
	/// Gets the large text style.
	/// </summary>
	/// <value>
	/// The large text style.
	/// </value>
	private GUIStyle m_largeText { get { return Styles.largeTextLight; } }
	
	/// <summary>
	/// Gets the small text style.
	/// </summary>
	/// <value>
	/// The small text style.
	/// </value>
	private GUIStyle m_smallText { get { return Styles.smallTextLight; } }
	
	/// <summary>
	/// Displays the player info.
	/// </summary>
	public override void Draw()
	{
		// NOTE: temporarily disabling this
#pragma warning disable 162
		return;
		GUILayout.BeginArea(rect, m_box);
		{
			// header
			GUILayout.BeginHorizontal();
			{
				GUILayout.Label(
					Player.current.name,
					m_largeText,
					GUILayout.Height(m_closedRectHeight - m_box.padding.top - m_box.padding.bottom)
				);
				GUILayout.FlexibleSpace();
				m_isNotificationBrowserExpanded = GUILayout.Toggle(
					m_isNotificationBrowserExpanded,
					new GUIContent(
					),
					m_smallButton
				);
			}
			GUILayout.EndHorizontal();
			
			// show scroll area if expanded
			if (m_isNotificationBrowserExpanded) {
				GameGUIManager.use.styles.DrawLine(
					GUIStyles.LineDirection.Horizontal,
					GUIStyles.LineColor.Highlighted
				);
			}
		}
		GUILayout.EndArea();
#pragma warning restore 162
	}
	
	/// <summary>
	/// Raises the select my listing event.
	/// </summary>
	/// <param name='listing'>
	/// Listing.
	/// </param>
	protected void OnSelectMyListing(Listing listing)
	{
		m_isNotificationBrowserExpanded = false;
	}
	
	/// <summary>
	/// Determines whether this instance is mouse over.
	/// </summary>
	/// <returns>
	/// <c>true</c> if this instance is mouse over; otherwise, <c>false</c>.
	/// </returns>
	public override bool IsMouseOver(){
		if(m_rect.Contains (GUIHelpers.MouseToGUIPosition(Input.mousePosition))){
			return true;
		}
		else
			return false;
	}
}
