/// <summary>
/// A class to describe a bid on a piece of land.
/// </summary>
public struct Bid
{
	public int id { get; set; }
	public PlayerData bidder { get; set; }
	public string updated_at { get; set; }
	public string status { get; set; }
	public bool pending { get; set; }
	public int money { get; set; }
	public LandRequest[] requested_land { get; set; }
	/// <summary>
	/// Land requested in a bid.
	/// </summary>
	public struct LandRequest
	{
		public int id { get; set; }
		public int x { get; set; }
		public int y { get; set; }
	}
	
	/// <summary>
	/// Determines whether one specified <see cref="Bid"/> is lower than another specfied <see cref="Bid"/>.
	/// </summary>
	/// <param name='bid1'>
	/// The first <see cref="Bid"/> to compare.
	/// </param>
	/// <param name='bid2'>
	/// The second <see cref="Bid"/> to compare.
	/// </param>
	/// <returns>
	/// <c>true</c> if <c>bid1</c> is lower than <c>bid2</c>; otherwise, <c>false</c>.
	/// </returns>
	public static bool operator <(Bid bid1, Bid bid2)
	{
		return bid1.money < bid2.money;
    }
	
	/// <summary>
	/// Determines whether one specified <see cref="Bid"/> is greater than another specfied <see cref="Bid"/>.
	/// </summary>
	/// <param name='bid1'>
	/// The first <see cref="Bid"/> to compare.
	/// </param>
	/// <param name='bid2'>
	/// The second <see cref="Bid"/> to compare.
	/// </param>
	/// <returns>
	/// <c>true</c> if <c>bid1</c> is greater than <c>bid2</c>; otherwise, <c>false</c>.
	/// </returns>
    public static bool operator >(Bid bid1, Bid bid2)
	{
		return bid1.money > bid2.money;
    }
	
	/// <summary>
	/// Determines whether a specified instance of <see cref="Bid"/> is equal to another specified <see cref="Bid"/>.
	/// </summary>
	/// <param name='bid1'>
	/// The first <see cref="Bid"/> to compare.
	/// </param>
	/// <param name='bid2'>
	/// The second <see cref="Bid"/> to compare.
	/// </param>
	/// <returns>
	/// <c>true</c> if <c>bid1</c> and <c>bid2</c> are equal; otherwise, <c>false</c>.
	/// </returns>
    public static bool operator ==(Bid bid1, Bid bid2)
	{
		return bid1.money == bid2.money;
    }
	
	/// <summary>
	/// Determines whether a specified instance of <see cref="Bid"/> is not equal to another specified <see cref="Bid"/>.
	/// </summary>
	/// <param name='bid1'>
	/// The first <see cref="Bid"/> to compare.
	/// </param>
	/// <param name='bid2'>
	/// The second <see cref="Bid"/> to compare.
	/// </param>
	/// <returns>
	/// <c>true</c> if <c>bid1</c> and <c>bid2</c> are not equal; otherwise, <c>false</c>.
	/// </returns>
    public static bool operator !=(Bid bid1, Bid bid2)
	{
        return bid1.money != bid2.money;
    }
	
	/// <summary>
	/// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="Bids"/>.
	/// </summary>
	/// <param name='obj'>
	/// The <see cref="System.Object"/> to compare with the current <see cref="Bids"/>.
	/// </param>
	/// <returns>
	/// <c>true</c> if the specified <see cref="System.Object"/> is equal to the current <see cref="Bids"/>; otherwise, <c>false</c>.
	/// </returns>
    public override bool Equals(object obj) {

        if (!(obj is Bid)) return false;
        return this == (Bid)obj;
    }
	
	/// <summary>
	/// Determines whether one specified <see cref="Bid"/> is lower than or equal to another specfied <see cref="Bid"/>.
	/// </summary>
	/// <param name='bid1'>
	/// The first <see cref="Bid"/> to compare.
	/// </param>
	/// <param name='bid2'>
	/// The second <see cref="Bid"/> to compare.
	/// </param>
	/// <returns>
	/// <c>true</c> if <c>bid1</c> is lower than or equal to <c>bid2</c>; otherwise, <c>false</c>.
	/// </returns>
    public static bool operator <=(Bid bid1, Bid bid2)
	{
		return bid1.money <= bid2.money;
    }
	
	/// <summary>
	/// Determines whether one specified <see cref="Bid"/> is greater than or equal to another specfied <see cref="Bid"/>.
	/// </summary>
	/// <param name='bid1'>
	/// The first <see cref="Bid"/> to compare.
	/// </param>
	/// <param name='bid2'>
	/// The second <see cref="Bid"/> to compare.
	/// </param>
	/// <returns>
	/// <c>true</c> if <c>bid1</c> is greater than or equal to <c>bid2</c>; otherwise, <c>false</c>.
	/// </returns>
    public static bool operator >=(Bid bid1, Bid bid2)
	{
		return bid1.money >= bid2.money;
    }
	
	/// <summary>
	/// Serves as a hash function for a <see cref="Bid"/> object.
	/// </summary>
	/// <returns>
	/// A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a hash table.
	/// </returns>
	public override int GetHashCode()
	{
		return base.GetHashCode();
	}
}

/// <summary>
/// A struct to capture a list of bids.
/// </summary>
public struct Bids
{
	/// <summary>
	/// The bid list.
	/// </summary>
	public Bid[] bids;
	
	/// <summary>
	/// Gets the highest bid amount.
	/// </summary>
	/// <returns>
	/// The highest bid amount.
	/// </returns>
	public int GetHighestBidAmount()
	{
		if (bids.Length == 0) {
			return 0;
		}
		int x = 0;
		for (int i=0; i<bids.Length; ++i) {
			if (bids[i].money > bids[x].money) {
				x = i;
			}
		}
		return bids[x].money;
	}
	
	/// <summary>
	/// Array accessor.
	/// </summary>
	/// <returns>
	/// The array.
	/// </returns>
	public Bid[] ToArray() {
		return bids;
	}
	
	/// <summary>
	/// Gets or sets the <see cref="Bids"/> with the specified index.
	/// </summary>
	/// <param name='i'>
	/// Index.
	/// </param>
	public Bid this[int i] {
		get { return bids[i]; }
		set { bids[i] = value; }
	}
	
	/// <summary>
	/// Gets a value indicating whether this <see cref="Bids"/> is null or empty.
	/// </summary>
	/// <value>
	/// <c>true</c> if is null or empty; otherwise, <c>false</c>.
	/// </value>
	public bool isNullOrEmpty {
		get {
			return bids == null || bids.Length == 0;
		}
	}
	
	/// <summary>
	/// Gets the length of the bid array.
	/// </summary>
	/// <value>
	/// The length of the bid array.
	/// </value>
	public int Length { get { return bids.Length; } }
	
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
		/// The bids.
		/// </summary>
		Bids bids;
		/// <summary>
		/// Initializes a new instance of the <see cref="Bids.Enumerator"/> class.
		/// </summary>
		/// <param name='bids'>
		/// Bids.
		/// </param>
		public Enumerator(Bids bids) {
			this.bids = bids;
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
			return i < bids.bids.Length;
		}
		/// <summary>
		/// Gets the current.
		/// </summary>
		/// <value>
		/// The current.
		/// </value>
		public Bid Current {
			get {
				return bids.bids[i];
			}
		}
	}
}