/// <summary>
/// A class to describe a bid for zone lobbying
/// </summary>
public class LobbyBid : System.Object
{	
	public int id { get; set; }
	public PlayerData bidder { get; set; }
	public bool active { get; set; }
	public int money { get; set; }
	public string status { get; set; }
	public string updated_at { get; set; }
	public int zoning_type { get; set; }
}