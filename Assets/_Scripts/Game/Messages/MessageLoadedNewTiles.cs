/// <summary>
/// Message sent when new tiles have been downloaded and have finished applying to the terrain
/// </summary>
public class MessageLoadedNewTiles : MessageAM
{
	/// <summary>
	/// The terrain-space region of the newly loaded tiles.
	/// </summary>
	public Region region;
	
	/// <summary>
	/// Initializes a new instance of the <see cref="MessageLoadedNewTiles"/> class.
	/// </summary>
	/// <param name='region'>
	/// Terrain-space region of the newly loaded tiles.
	/// </param>
	public MessageLoadedNewTiles(Region region) : base("world")
	{
		this.region = region;
	}
}