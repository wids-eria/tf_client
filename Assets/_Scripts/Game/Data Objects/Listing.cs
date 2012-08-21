/// <summary>
/// A listing for some land.
/// </summary>
public struct Listing
{
	public int id { get; set; }
	public PlayerData owner { get; set; }
	public string status { get; set; }
	public int price { get; set; }
	public int bid_count { get; set; }
	public Megatile[] megatiles { get; set; }
}

/// <summary>
/// A collection of listings.
/// </summary>
public struct Listings
{
	/// <summary>
	/// Gets or sets the listings.
	/// </summary>
	/// <value>
	/// The listings.
	/// </value>
	public Listing[] listings { get; set; }
	
	/// <summary>
	/// Clear this instance.
	/// </summary>
	public void Clear() {
		listings = new Listing[0];
	}
	
	/// <summary>
	/// Array accessor.
	/// </summary>
	/// <returns>
	/// The array.
	/// </returns>
	public Listing[] ToArray() {
		return listings;
	}
	
	/// <summary>
	/// Gets or sets the <see cref="Listing"/> with the specified index.
	/// </summary>
	/// <param name='i'>
	/// Index.
	/// </param>
	public Listing this[int i] {
		get { return listings[i]; }
		set { listings[i] = value; }
	}
	
	/// <summary>
	/// Gets the length of the listings array.
	/// </summary>
	/// <value>
	/// The length of the listings array.
	/// </value>
	public int Length { get { return listings.Length; } }
	
	/// <summary>
	/// Gets the enumerator.
	/// </summary>
	/// <returns>
	/// The enumerator.
	/// </returns>
	public Enumerator GetEnumerator()
	{
		return new Enumerator(this);
	}
	/// <summary>
	/// Enumerator.
	/// </summary>
	public class Enumerator 
	{
		/// <summary>
		/// The current index.
		/// </summary>
		private int i;
		/// <summary>
		/// The listings.
		/// </summary>
		Listings listings;
		/// <summary>
		/// Initializes a new instance of the <see cref="Listings.Enumerator"/> class.
		/// </summary>
		/// <param name='listings'>
		/// Listings.
		/// </param>
		public Enumerator(Listings listings) {
			this.listings = listings;
			if (this.listings.listings == null) {
				this.listings.listings = new Listing[0];
			}
			i = -1;
		}
		/// <summary>
		/// Moves the next.
		/// </summary>
		/// <returns>
		/// The next.
		/// </returns>
		public bool MoveNext() {
			++i;
			return i < listings.listings.Length;
		}
		/// <summary>
		/// Gets the current.
		/// </summary>
		/// <value>
		/// The current.
		/// </value>
		public Listing Current {
			get {
				return listings.listings[i];
			}
		}
	}
}