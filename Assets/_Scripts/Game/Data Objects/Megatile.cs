using UnityEngine;

/// <summary>
/// Data containing a collection of Megatiles.
/// </summary>
public struct Megatiles 
{
	public Megatile[] megatiles { get; set; }
}

/// <summary>
/// Single megatile for individual requests.
/// </summary>
public struct SingleMegatile
{
	public Megatile megatile { get; set; }
}

/// <summary>
/// Data relating to an individual Megatile.
/// </summary>
[System.Serializable]
public class Megatile : System.Object
{
	/// <summary>
	/// Initializes a new instance of the <see cref="Megatile"/> class.
	/// </summary>
	public Megatile()
	{
		
	}
	/// <summary>
	/// Initializes a new instance of the <see cref="Megatile"/> class.
	/// </summary>
	/// <remarks>
	/// TODO: This needs to be destroyed
	/// </remarks>
	/// <param name='x'>
	/// X.
	/// </param>
	/// <param name='z'>
	/// Z.
	/// </param>
	public Megatile(int x, int z)
	{
		this.x = x;
		this.z = z;
		this.id = (this.y + this.x*(TerrainManager.worldRegion.height-1)/Megatile.size)/Megatile.size + 1;
	}
	
	/// <summary>
	/// The size of a megatile along one side.
	/// </summary>
	public static int size { get; private set; }
	
	/// <summary>
	/// Sets the size of the megatile.
	/// </summary>
	/// <param name='val'>
	/// Value.
	/// </param>
	public static void SetMegatileSize(int val)
	{
		size = val;
	}
	
	/// <summary>
	/// Gets or sets the identifier.
	/// </summary>
	/// <value>
	/// The identifier.
	/// </value>
	public int id { get; set; }
	/// <summary>
	/// Gets or sets the x coordinate.
	/// </summary>
	/// <value>
	/// The x coordinate.
	/// </value>
	public int x { get; set; }
	/// <summary>
	/// Gets or sets the y coordinate.
	/// </summary>
	/// <value>
	/// The y coordinate.
	/// </value>
	public int y { get; set; }
	/// <summary>
	/// Gets or sets the z-coordinate (used on client).
	/// </summary>
	/// <value>
	/// The z.
	/// </value>
	public int z {
		get { return ToClientZ(y); }
		set { this.y = ToServerY(value); }
	}
	/// <summary>
	/// Gets the center.
	/// </summary>
	/// <value>
	/// The center.
	/// </value>
	public Vector3 center {
		get {
			return ConvertPointToMegatileCenter(new Vector3(x,0f,z));
//			return new Vector3(
//				x+0.5f*Megatile.size,
//				0f,
//				z+0.5f*Megatile.size
//			);
		}
	}
	
	public string updated_at {get; set;}  //Get Timestamp on megatile and then pass to each resource tile so that we have it when we update. look at resourceTileCache in TerrainManager to determine the timestamps of tiles before a request is sent.
	
	/// <summary>
	/// Converts the point to megatile center.
	/// </summary>
	/// <returns>
	/// The point to megatile center.
	/// </returns>
	/// <param name='point'>
	/// A point in world space.
	/// </param>
	public static Vector3 ConvertPointToMegatileCenter(Vector3 point)
	{
		point.x = (Mathf.FloorToInt(point.x)/Megatile.size+0.5f)*Megatile.size;
		point.y = TerrainManager.use.terrain.SampleHeight(point);
		point.z = (Mathf.FloorToInt(point.z)/Megatile.size+0.5f)*Megatile.size;
		return point;
	}
	
	/// <summary>
	/// Converts the supplied position to an exact megatile coordinate (e.g., in the tile's lower left corner).
	/// </summary>
	/// <remarks>
	/// Note that this point is in the client's space, and may still need to be converted to a server coordinate if it is used to post data.
	/// </remarks>
	/// <returns>
	/// An exact megatile coordinate.
	/// </returns>
	/// <param name='p'>
	/// A point in world space.
	/// </param>
	public static Vector3 ConvertPointToExactMegatileCoordinate(Vector3 p)
	{
		return new Vector3(
			(int)p.x-((int)p.x)%size,
			0f,
			(int)p.z-((int)p.z)%size
			);
	}
	
	/// <summary>
	/// Gets a value indicating whether this <see cref="Megatile"/> is loaded.
	/// </summary>
	/// <remarks>
	/// When a Megatile is loaded from the server, its resource tiles will be populated. They won't be when it was just created as a placeholder.
	/// </remarks>
	/// <value>
	/// <c>true</c> if is loaded; otherwise, <c>false</c>.
	/// </value>
	public bool isLoaded { get { return (this.resource_tiles!=null)?this.resource_tiles.Length>0:false; } }
	
	/// <summary>
	/// Convert z coordinate to a y coordinate for the server, which starts from upper left
	/// </summary>
	/// <param name="z">
	/// A <see cref="System.Int32"/>
	/// </param>
	/// <returns>
	/// A <see cref="System.Int32"/>
	/// </returns>
	public static int ToServerY(int z)
	{
		return TerrainManager.worldRegion.height-1-z;
	}
	/// <summary>
	/// Convert y coordinate to a z coordinate for the client, which starts from lower left
	/// </summary>
	/// <param name="y">
	/// A <see cref="System.Int32"/>
	/// </param>
	/// <returns>
	/// A <see cref="System.Int32"/>
	/// </returns>
	public static int ToClientZ(int y)
	{
		return TerrainManager.worldRegion.height-1-y;
	}
	
	/// <summary>
	/// Gets or sets the owner.
	/// </summary>
	/// <value>
	/// The owner.
	/// </value>
	public PlayerData owner { get; set; }
	
	/// <summary>
	/// Gets a value indicating whether this <see cref="Megatile"/> is owned by current player.
	/// </summary>
	/// <value>
	/// <c>true</c> if is owned by current player; otherwise, <c>false</c>.
	/// </value>
	public bool isOwnedByCurrentPlayer { get { return owner!= null && owner.id == Player.current.id; } }
	
	/// <summary>
	/// Gets or sets the resource_tiles.
	/// </summary>
	/// <value>
	/// The resource_tiles.
	/// </value>
	public ResourceTile[] resource_tiles { get; set; }
	
	/// <summary>
	/// world x-coordinate
	/// </summary>
	public int worldX
	{
		get { return x * Megatile.size; }
	}
	/// <summary>
	/// world y-coordinate
	/// </summary>
	public int worldY
	{
		get { return y * Megatile.size; }
	}
}