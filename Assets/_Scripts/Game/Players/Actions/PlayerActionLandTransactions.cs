using UnityEngine;
using System.Collections;

//THIS IS NO LONGER A VALID PLAYER ACTION WE ARE USING

/*

/// <summary>
/// The action to bid on the selected piece of land
/// </summary>
public class PlayerActionLandTransactions : PlayerAction
{
	/// <summary>
	/// Gets or sets all listings.
	/// </summary>
	/// <value>
	/// All listings.
	/// </value>
	public static Listings allListings { get; private set; }
	
	/// <summary>
	/// Gets or sets the bids on my listings, where key is the Megatile id.
	/// </summary>
	/// <value>
	/// The bids on my listings.
	/// </value>
	public static System.Collections.Generic.Dictionary<int, Bids> bidsOnMyListings { get; private set; }
	
	/// <summary>
	/// Gets my listings, where key is the Megatile id.
	/// </summary>
	/// <value>
	/// My listings.
	/// </value>
	public static System.Collections.Generic.Dictionary<int, Listing> myListings {
		get {
			System.Collections.Generic.Dictionary<int, Listing> myStuff = new System.Collections.Generic.Dictionary<int, Listing>();
			foreach (Listing listing in allListings) {
				if (listing.owner.id == Player.current.id) {
					myStuff[listing.megatiles[0].id] = listing;
				}
			}
			return myStuff;
		}
	}
	
	/// <summary>
	/// Gets the other players' listings, where key is the Megatile id.
	/// </summary>
	/// <value>
	/// The other players' listings.
	/// </value>
	public static System.Collections.Generic.Dictionary<int, Listing> otherListings {
		get {
			System.Collections.Generic.Dictionary<int, Listing> theirStuff = new System.Collections.Generic.Dictionary<int, Listing>();
			foreach (Listing listing in allListings) {
				if (listing.owner.id != Player.current.id) {
					theirStuff.Add(listing.megatiles[0].id, listing);
				}
			}
			return theirStuff;
		}
	}
	
	/// <summary>
	/// Initialize singleton.
	/// </summary>
	void Awake()
	{
		// clear listings and bids in case e.g. loading a new level
		allListings.Clear();
		bidsOnMyListings = new System.Collections.Generic.Dictionary<int, Bids>();
	}
	
	/// <summary>
	/// Gets the listing URL.
	/// </summary>
	/// <value>
	/// The listing URL.
	/// </value>
	private string m_listingUrl {
		get {
			return string.Format(
				"{0}/worlds/{1}/listings.json",
				WebRequests.urlServer,
				UserData.worldNumber
			);
		}
	}
	
	/// <summary>
	/// Gets the active listings URL.
	/// </summary>
	/// <value>
	/// The active listings URL.
	/// </value>
	private string m_activeListingsUrl {
		get {
			return string.Format(
				"{0}/worlds/{1}/listings/active.json",
				WebRequests.urlServer,
				UserData.worldNumber
			);
		}
	}
	
	/// <summary>
	/// Gets the bids placed URL.
	/// </summary>
	/// <value>
	/// The bids placed URL.
	/// </value>
	private string m_bidsPlacedUrl {
		get {
			return string.Format(
				"{0}/worlds/{1}/players/{2}/bids_placed.json",
				WebRequests.urlServer,
				UserData.worldNumber,
				UserData.current.id
			);
		}
	}
	
	/// <summary>
	/// Gets the bids received URL.
	/// </summary>
	/// <value>
	/// The bids received URL.
	/// </value>
	private string m_bidsReceivedUrl {
		get {
			return string.Format(
				"{0}/worlds/{1}/players/{2}/bids_received.json",
				WebRequests.urlServer,
				UserData.worldNumber,
				UserData.current.id
			);
		}
	}
	
	/// <summary>
	/// Gets the world listing bids URL.
	/// </summary>
	/// <returns>
	/// The world listing bids URL.
	/// </returns>
	/// <param name='listingId'>
	/// Listing identifier.
	/// </param>
	private string GetWorldListingBidsUrl(int listingId)
	{
		return string.Format(
			"{0}/worlds/{1}/listings/{2}/bids.json",
			WebRequests.urlServer,
			UserData.worldNumber,
			listingId
		);
	}
	
	/// <summary>
	/// Gets the accept world listing bid URL.
	/// </summary>
	/// <returns>
	/// The accept world listing bid URL.
	/// </returns>
	/// <param name='listingId'>
	/// Listing identifier.
	/// </param>
	/// <param name='bidId'>
	/// Bid identifier.
	/// </param>
	private string GetAcceptWorldListingBidUrl(int listingId, int bidId)
	{
		return string.Format(
			"{0}/worlds/{1}/listings/{2}/bids/{3}/accept.json",
			WebRequests.urlServer,
			UserData.worldNumber,
			listingId,
			bidId
		);
	}
	
	/// <summary>
	/// Gets the megatile bids URL.
	/// </summary>
	/// <returns>
	/// The megatile bids URL.
	/// </returns>
	/// <param name='megatileId'>
	/// Megatile identifier.
	/// </param>
	private string GetMegatileBidsUrl(int megatileId)
	{
		return string.Format(
			"{0}/worlds/{1}/megatiles/{2}/bids.json",
			WebRequests.urlServer,
			UserData.worldNumber,
			megatileId
		);
	}
	
	/// <summary>
	/// Gets the accept megatile bid URL.
	/// </summary>
	/// <returns>
	/// The accept megatile bid URL.
	/// </returns>
	/// <param name='megatileId'>
	/// Megatile identifier.
	/// </param>
	/// <param name='bidId'>
	/// Bid identifier.
	/// </param>
	private string GetAcceptMegatileBidUrl(int megatileId, int bidId)
	{
		return string.Format(
			"{0}/worlds/{1}/megatiles/{2}/bids/{3}/accept.json",
			WebRequests.urlServer,
			UserData.worldNumber,
			megatileId,
			bidId
		);
	}
	
	/// <summary>
	/// Gets the do it URL.
	/// </summary>
	/// <returns>
	/// The do it URL.
	/// </returns>
	override protected WWW GetDoItWWW(MegatileSelection selection)
	{
		System.Collections.Generic.Dictionary<string, object> headers = WebRequests.authenticatedParameters;
		switch (m_currentTab) {
		case Tab.Buy:
			headers.Add("money", m_inputValue);
			return WWWX.Post(GetMegatileBidsUrl(m_selectedMegatile.id), headers);
		case Tab.Sell:
			headers.Add("price", m_inputValue);
			MegatileSelection sel = new MegatileSelection(new int[1] { m_selectedMegatile.id });
			return WWWX.PostWithJson(m_listingUrl, sel.ToJson(), headers);
		default:
			return null;
		}
	}
	
	/// <summary>
	/// Tab.
	/// </summary>
	internal enum Tab { Sell, Buy }
	
	/// <summary>
	/// The current tab.
	/// </summary>
	private Tab m_currentTab;
	
	/// <summary>
	/// Raises the set megatile selection event.
	/// </summary>
	public override void OnSetMegatileSelection()
	{
		base.OnSetMegatileSelection();
		// get any bids on the tile
		StartCoroutine(DownloadBidsOnMegatileWithId(m_selectedMegatile.id));
	}
	
	/// <summary>
	/// Downloads the bids on megatile with identifier.
	/// </summary>
	/// <returns>
	/// The bids on megatile with identifier.
	/// </returns>
	/// <param name='id'>
	/// Identifier of the megatile.
	/// </param>
	private IEnumerator DownloadBidsOnMegatileWithId(int id)
	{
		WWW www = WWWX.Get(
			GetMegatileBidsUrl(id),
			WebRequests.authenticatedParameters
		);
		yield return www;
		if (!string.IsNullOrEmpty(www.error)) {
			if (WWWX.GetErrorCode(www.error) != 500) {
				Debug.LogError(www.error);
			}
			yield break;
		}
		try {
			Bids bids = JSONDecoder.DecodeValue<Bids>(www.text);
			m_minBid = bids.GetHighestBidAmount()+1;
			// no bids on it, so set to asking price
			if (m_minBid == 0) {
				foreach (Listing listing in otherListings.Values) {
					if (listing.megatiles[0].id == m_selectedMegatile.id) {
						m_minBid = listing.price;
					}
				}
			}
		}
		catch (JsonException) {
			Debug.LogError(www.text);
			yield break;
		}
	}
	
	/// <summary>
	/// Sound effect when a bid is submitted
	/// </summary>
	public AudioClip submitBid;
	
	/// <summary>
	/// The input value
	/// </summary>
	private int m_inputValue = 100;
	
	/// <summary>
	/// The minimum bid.
	/// </summary>
	private int m_minBid = 0;
	
	/// <summary>
	/// The scroll position.
	/// </summary>
	private Vector2 m_scrollPosition;
	
	/// <summary>
	/// The height of the scroll area.
	/// </summary>
	private float m_scrollAreaHeight;
	
	/// <summary>
	/// The scroll area width scale factor.
	/// </summary>
	[SerializeField]
	private float m_scrollAreaWidthScaleFactor = 0.75f;
	
	/// <summary>
	/// Display the contents of the dialog
	/// </summary>
	protected override void DisplayControlsContents()
	{
		m_currentTab = (Tab)GUILayout.SelectionGrid(
			(int)m_currentTab,
			System.Enum.GetNames(typeof(Tab)),
			System.Enum.GetValues(typeof(Tab)).Length,
			m_button
		);
		
		GUILayout.BeginHorizontal();
		{
			GUILayout.FlexibleSpace();
			
			// determine listings to display and headers for scroll view
			GUILayout.BeginVertical(GUILayout.Width(
				GameGUIManager.use.rectActionControls.width*m_scrollAreaWidthScaleFactor)
			); {
				switch (m_currentTab) {
				case Tab.Buy:
					DisplayOtherListings();
					break;
				case Tab.Sell:
					GUILayout.BeginVertical(GUILayout.Height(m_scrollAreaHeight)); {
						DisplayMyListings(new PlayerActionLandTransactions.SelectMyListingDelegate(OnSelectMyListing), true);
					} GUILayout.EndVertical();
					m_styles.DrawLine(
						GUIStyles.LineDirection.Horizontal,
						GUIStyles.LineColor.Light
					);
					DisplayBidsForMyListing();
					break;
				}
			} GUILayout.EndVertical();
			if (Event.current.type == EventType.Repaint) {
				m_scrollAreaHeight = GUILayoutUtility.GetLastRect().height*0.45f;
			}
			GUILayout.FlexibleSpace();
		}
		GUILayout.EndHorizontal();
		
		GUILayout.FlexibleSpace();
		
		// bottom input area
		if (m_selectedMegatile != null) {
			switch (m_currentTab) {
			case Tab.Buy:
				DisplayPostBid();
				break;
			case Tab.Sell:
				DisplayPostListing();
				break;
			}
		}
		else {
			GUILayout.Label("No tile selected.", m_mainText);
		}
		
		// display footer button as needed
		bool displayFooter = m_selectedMegatile != null;
		if (displayFooter) {
			if (!m_selectedMegatile.isLoaded) {
				GUILayout.Label("Loading tile data...", m_mainText);
			}
			else if (m_currentTab == Tab.Buy) {
				if (m_selectedMegatile.isOwnedByCurrentPlayer) {
					displayFooter = false;
				}
			}
			else {
				if (!m_selectedMegatile.isOwnedByCurrentPlayer) {
					displayFooter = false;
				}
			}
		}
		if (displayFooter && m_selectedMegatile.isLoaded) {
			base.DisplayControlsContents();
		}
	}
	
	/// <summary>
	/// Displays my listings.
	/// </summary>
	/// <param name='onSelect'>
	/// Delegate for when select button is pressed.
	/// </param>
	/// <param name='showListingsWithoutBids'>
	/// Show listings without bids?
	/// </param>
	public void DisplayMyListings(SelectMyListingDelegate onSelect, bool showListingsWithoutBids)
	{
		// cache the listings to display
		System.Collections.Generic.Dictionary<int, Listing> listingsToDisplay = myListings;
		// header
		GUILayout.BeginHorizontal(); {
			GUILayout.Label("Number of Offers", m_mainText);
			GUILayout.FlexibleSpace();
			GUILayout.Label("Asking Price", m_mainText);
		} GUILayout.EndHorizontal();
		// divider
		m_styles.DrawLine(
			GUIStyles.LineDirection.Horizontal,
			GUIStyles.LineColor.Medium
		);
		// scroll area
		m_scrollPosition = GUILayout.BeginScrollView(m_scrollPosition); {
			// if there are no listings...
			if (listingsToDisplay.Keys.Count == 0) {
				GUILayout.Label("No listings.", m_minorText);
			}
			else {
				foreach (Listing listing in listingsToDisplay.Values) {
					int id = listing.megatiles[0].id;
					int numBids = 0;
					if (bidsOnMyListings.ContainsKey(id) && !bidsOnMyListings[id].isNullOrEmpty) {
						numBids = bidsOnMyListings[id].Length;
					}
					if (!showListingsWithoutBids && numBids == 0) {
						continue;
					}
					GUILayout.BeginHorizontal(); {
//						GUIStyle buttonStyle = m_activeButton;
						GUIStyle buttonStyle = m_button;
						if (m_selectedMegatile == null ||
							m_selectedMegatile.id != id
						) {
							buttonStyle = m_button;
						}
						if (listingsToDisplay.ContainsKey(id) &&
							GUILayout.Button(
								numBids.ToString(),
								buttonStyle,
								GUILayout.Width(GameGUIManager.use.rectActionControls.width*0.3f)
							)
						) {
							onSelect(listing);
						}
						GUILayout.FlexibleSpace();
						GUILayout.Label(
							string.Format("{0:c}", listing.price),
							m_minorText,
							GUILayout.Height(m_buttonHeight)
						);
					} GUILayout.EndHorizontal();
				}
			}
		} GUILayout.EndScrollView();
	}
	
	/// <summary>
	/// Raises the select my listing event.
	/// </summary>
	/// <param name='listing'>
	/// Listing.
	/// </param>
	public void OnSelectMyListing(Listing listing)
	{
		m_selectedMegatile = listing.megatiles[0];
	}
	
	/// <summary>
	/// Delegate for selecting a listing from my listings.
	/// </summary>
	public delegate void SelectMyListingDelegate(Listing listing);
	
	/// <summary>
	/// The bid scroll position.
	/// </summary>
	private Vector2 m_bidScrollPosition;
	
	/// <summary>
	/// Displays the bids for my listing.
	/// </summary>
	private void DisplayBidsForMyListing()
	{
		// early out if no tile is selected or tile is not loaded
		if (m_selectedMegatile == null || !m_selectedMegatile.isLoaded) {
			return;
		}
		int id = m_selectedMegatile.id;
		// early out if there are no bids on the selected tile
		if (!bidsOnMyListings.ContainsKey(id)) {
			return;
		}
		Bids bids = bidsOnMyListings[id];
		float btnWid = 80f;
		// header
		GUILayout.BeginHorizontal(); {
			GUILayout.Label("Bidder", m_mainText);
			GUILayout.FlexibleSpace();
			GUILayout.Label("Bid Amount", m_mainText);
			GUILayout.FlexibleSpace();
			GUILayout.Box("", m_styles.empty, GUILayout.Width(btnWid));
		} GUILayout.EndHorizontal();
		// divider
		m_styles.DrawLine(
			GUIStyles.LineDirection.Horizontal,
			GUIStyles.LineColor.Medium
		);
		// scroll view
		m_bidScrollPosition = GUILayout.BeginScrollView(m_bidScrollPosition); {
			Color c = GUI.color;
			if (m_isAcceptingBid) {
				GUI.color = new Color(c.r, c.g, c.b, GameGUIManager.use.inactiveGUIFade);
			}
			// display controls for each bid
			foreach (Bid bid in bids) {
				GUILayout.BeginHorizontal(); {
					GUILayout.Label(bid.bidder.name, m_minorText);
					GUILayout.Label(string.Format("{0:c}", bid.money), m_minorText);
					if (GUILayout.Button("Accept", m_button, GUILayout.Width(btnWid)) &&
						!m_isAcceptingBid
					) {
						StartCoroutine(AcceptBidForMegatileWithId(m_selectedMegatile.id, bid));
					}
				} GUILayout.EndHorizontal();
			}
			GUI.color = c;
		} GUILayout.EndScrollView();
	}
	
	/// <summary>
	/// Flag specifying whether a bid is currently being accepted.
	/// </summary>
	private bool m_isAcceptingBid = false;
	
	/// <summary>
	/// Accepts the bid for the Megatile with the supplied identifier.
	/// </summary>
	/// <param name='id'>
	/// Identifier of the Megatile to which the bid belongs.
	/// </param>
	/// <param name='bid'>
	/// Bid.
	/// </param>
	private IEnumerator AcceptBidForMegatileWithId(int id, Bid bid) 
	{
		WWW www = WWWX.Post(GetAcceptMegatileBidUrl(id, bid.id), WebRequests.authenticatedParameters);
		m_isAcceptingBid = true;
		yield return www;
		
		if (!string.IsNullOrEmpty(www.error)) {
			GameGUIManager.use.SetErrorMessage("Error accepting bid.");
			Debug.LogError(www.error);
		}		
		// refresh bids on megatile
		yield return StartCoroutine(DownloadBidsOnMegatileWithId(id));
		
		// indicate that the bid has finished processing
		m_isAcceptingBid = false;
	}
	
	/// <summary>
	/// Displays the other listings.
	/// </summary>
	private void DisplayOtherListings()
	{
		// cache the listings to display
		System.Collections.Generic.Dictionary<int, Listing> listingsToDisplay = otherListings;
		// header
		GUILayout.BeginHorizontal(); {
			GUILayout.Label("Seller (Tiles)", m_mainText);
			GUILayout.FlexibleSpace();
			GUILayout.Label("Asking Price", m_mainText);
		} GUILayout.EndHorizontal();
		// divider
		m_styles.DrawLine(
			GUIStyles.LineDirection.Horizontal,
			GUIStyles.LineColor.Medium
		);
		// scroll area
		m_scrollPosition = GUILayout.BeginScrollView(
			m_scrollPosition
		); {
			// if there are no listings...
			if (listingsToDisplay.Keys.Count == 0) {
				GUILayout.Label("No listings.", m_minorText);
			}
			else {
				foreach (Listing listing in listingsToDisplay.Values) {
					int id = listing.megatiles[0].id;
					GUILayout.BeginHorizontal(); {
						GUIStyle buttonStyle = m_button;
						if (m_selectedMegatile == null ||
							m_selectedMegatile.id != id
						) {
							buttonStyle = m_button;
						}
						if (GUILayout.Button(
								string.Format(
									"{0} ({1} tile{2})",
									listing.owner.name,
									listing.megatiles.Length,
									listing.megatiles.Length==1?"":"s"
								),
								buttonStyle,
								GUILayout.Width(GameGUIManager.use.rectActionControls.width*0.3f)
							)
						) {
							m_selectedMegatile = listing.megatiles[0];
						}
						GUILayout.FlexibleSpace();
						GUILayout.Label(
							string.Format("{0:c}", listing.price),
							m_minorText,
							GUILayout.Height(m_buttonHeight)
						);
					} GUILayout.EndHorizontal();
				}
			}
		} GUILayout.EndScrollView();
	}
	
	/// <summary>
	/// Gets or sets the selected megatile.
	/// </summary>
	/// <value>
	/// The selected megatile.
	/// </value>
	private Megatile m_selectedMegatile {
		get { return InputManager.use.selectedMegatile; }
		set { InputManager.use.SelectMegatileAt(value.center); }
	}
	
	/// <summary>
	/// Displaies the selected tile owner.
	/// </summary>
	private void DisplaySelectedTileOwner()
	{
		if (!m_selectedMegatile.isLoaded) return;
		GUILayout.BeginHorizontal(); {
			GUILayout.Label("Owner:", m_mainTextAlt);
			GUILayout.Label(
				m_selectedMegatile.isOwnedByCurrentPlayer?"You":(
					m_selectedMegatile.owner==null?"None":m_selectedMegatile.owner.name
				), m_mainText
			);
			GUILayout.FlexibleSpace();
		} GUILayout.EndHorizontal();
	}
	
	/// <summary>
	/// Display dialog controls for posting a bid.
	/// </summary>
	private void DisplayPostBid()
	{
		// display owner
		DisplaySelectedTileOwner();
		// set bottom button text
		dialogConfirmButtonDo.text = "BID";
		// early out if not loaded
		if (!m_selectedMegatile.isLoaded) {
			return;
		}
		// bid submission
		if (!m_selectedMegatile.isOwnedByCurrentPlayer) {
			// bid input
			GUILayout.BeginHorizontal(); {
				GUILayout.Label(
					"$",
					m_styles.elegantTextLight,
					GUILayout.Width(10f),
					GUILayout.Height(m_styles.inputFieldHeight)
				);
				m_inputValue = Mathf.Max(GameGUIManager.DrawNumericInputField(m_inputValue), m_minBid);
			} GUILayout.EndHorizontal();
			// info on the current bids
			GUILayout.BeginHorizontal(); {
				GUILayout.FlexibleSpace();
				GUILayout.Label(string.Format("The current minimum bid is {0:c}", m_minBid), m_mainText);
			} GUILayout.EndHorizontal();
		}
		// option to list
		else if (GUILayout.Button("List it for sale", m_button)) {
			m_currentTab = Tab.Sell;
		}
	}
	
	/// <summary>
	/// Display dialog controls for posting a new listing.
	/// </summary>
	private void DisplayPostListing()
	{
		// display owner
		DisplaySelectedTileOwner();
		// set bottom button text
//		dialogConfirmButtonDo.text = myListings.ContainsKey(m_selectedMegatile.id)?"DE-LIST":"LIST";
		dialogConfirmButtonDo.text = "LIST";
		// TODO: update listing?
		// early out if not loaded
		if (!m_selectedMegatile.isLoaded) return;
		// create a new listing
		if (m_selectedMegatile.isOwnedByCurrentPlayer) {
			// price input
			GUILayout.BeginHorizontal();
			{
				GUILayout.Label(
					"$",
					m_styles.elegantTextLight,
					GUILayout.Width(10f),
					GUILayout.Height(m_styles.inputFieldHeight)
				);
				m_inputValue = Mathf.Max(GameGUIManager.DrawNumericInputField(m_inputValue), m_minBid);
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(); {
				GUILayout.FlexibleSpace();
				GUILayout.Label("How much are you asking for this land?", m_mainText);
			} GUILayout.EndHorizontal();
		}
		else if (GUILayout.Button("Offer to buy", m_button)) {
			m_currentTab = Tab.Buy;
		}
	}
	
	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start()
	{
		MessengerAM.Listen("www", this);
	}
	
	/// <summary>
	/// Raises the enable event.
	/// </summary>
	public override void OnEnable()
	{
		base.OnEnable();
		// issue an immediate refresh
		StartCoroutine(ForceRefreshOnEnable());
	}
	
	/// <summary>
	/// Force refresh necessary data when tool is enabled.
	/// </summary>
	private IEnumerator ForceRefreshOnEnable()
	{
		// ensure listings are valid before requesting bids for them
		yield return StartCoroutine(RefreshListings());
		StartCoroutine(RefreshBidsOnMyListings());
	}
	
	/// <summary>
	/// The refresh interval.
	/// </summary>
	public float refreshInterval = 2f;
	
	/// <summary>
	/// The most recent listing refresh.
	/// </summary>
	protected WWW m_mostRecentListingRefresh;
	
	/// <summary>
	/// Refresh this instance.
	/// </summary>
	private IEnumerator RefreshListings()
	{
		// early out if the action is not enabled
		if (!enabled) yield break;
		// update listings
		WWW www = WWWX.Get(
			string.Format("{0}/worlds/{1}/listings.json", WebRequests.urlServer, UserData.worldNumber),
			WebRequests.authenticatedParameters
		);
		m_mostRecentListingRefresh = www;
		yield return www;
				
		// ensure this is the freshest request
		if (www != m_mostRecentListingRefresh) yield break;
		
		if (string.IsNullOrEmpty(www.error)) {
			try {
				allListings = JSONDecoder.DecodeValue<Listings>(www.text);
			}
			catch (JsonException) {
				Debug.LogError(string.Format("Unable to parse json data:\n{0}", www.text));
			}
		}
		else {
			Debug.LogError(www.error);
		}
		
		// refresh again
		yield return new WaitForSeconds(refreshInterval);
		StartCoroutine(RefreshListings());
	}
	
	/// <summary>
	/// The time of last bid refresh request.
	/// </summary>
	System.DateTime m_timeOfLastBidRefreshRequest;
	
	/// <summary>
	/// Refreshes the bids on my listings.
	/// </summary>
	private IEnumerator RefreshBidsOnMyListings()
	{
		// store time of request in case it needs to be restarted with a fresher one
		m_timeOfLastBidRefreshRequest = System.DateTime.Now;
		System.DateTime startTime = m_timeOfLastBidRefreshRequest;
		
		// cache my listings in case they mutate
		System.Collections.Generic.Dictionary<int, Listing> myStuff = myListings;
		foreach (int id in myStuff.Keys) {
			WWW www = WWWX.Get(
				GetMegatileBidsUrl(id),
				WebRequests.authenticatedParameters
			);
			yield return www;
			
			// early out if there is a newer request
			if (startTime != m_timeOfLastBidRefreshRequest) {
				yield break;
			}
			
			// skip if there is an error
			if (!string.IsNullOrEmpty(www.error)) {
				Debug.LogError(www.error);
				continue;
			}
			try {
				bidsOnMyListings[id] = JSONDecoder.DecodeValue<Bids>(www.text);
			}
			catch (JsonException) {
				Debug.LogError(www.text);
				continue;
			}
		}
		
		// refresh again
		yield return new WaitForSeconds(refreshInterval);
		StartCoroutine(RefreshBidsOnMyListings());
	}
	
	/// <summary>
	/// Refresh all necessary information.
	/// </summary>
	protected override IEnumerator Refresh()
	{
		yield return StartCoroutine(base.Refresh());
		yield return StartCoroutine(DownloadBidsOnMegatileWithId(m_selectedMegatile.id));
		yield return StartCoroutine(RefreshListings());
	}
}

*/