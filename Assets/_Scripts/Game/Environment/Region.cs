using UnityEngine;
using System.Collections;

/// <summary>
/// A struct to describe a region in the world grid
/// </summary>
public struct Region
{
	/// <summary>
	/// inclusive left boundary for the region
	/// </summary>
	public int left;
	/// <summary>
	/// inclusive right boundary for the region
	/// </summary>
	public int right;
	/// <summary>
	/// inclusive top boundary for the region
	/// </summary>
	public int top;
	/// <summary>
	/// inclusive bottom boundary for the region
	/// </summary>
	public int bottom;
	
	/// <summary>
	/// Construct using specified bounds
	/// </summary>
	/// <param name="left">
	/// A <see cref="System.Int32"/>
	/// </param>
	/// <param name="right">
	/// A <see cref="System.Int32"/>
	/// </param>
	/// <param name="top">
	/// A <see cref="System.Int32"/>
	/// </param>
	/// <param name="bottom">
	/// A <see cref="System.Int32"/>
	/// </param>
	public Region(int left, int right, int top, int bottom)
	{
		this.left = left;
		this.right = right;
		this.top = top;
		this.bottom = bottom;
	}
	/// <summary>
	/// Create a region that includes all of the specified tiles
	/// </summary>
	/// <param name="tiles">
	/// A <see cref="ResourceTile"/>
	/// </param>
	public Region(ResourceTile[] tiles)
	{
		left = tiles[0].x;
		right = tiles[0].x;
		top = tiles[0].z;
		bottom = tiles[0].z;
		foreach (ResourceTile tile in tiles) {
			bottom = Mathf.Min(bottom, tile.z);
			top = Mathf.Max(top, tile.z);
			left = Mathf.Min(left, tile.x);
			right = Mathf.Max(right, tile.x);
		}
	}
	public Region(ResourceTileLite[] tiles)
	{
		left = tiles[0].x;
		right = tiles[0].x;
		top = tiles[0].y;
		bottom = tiles[0].y;
		foreach (ResourceTileLite tile in tiles) {
			bottom = Mathf.Min(bottom, tile.y);
			top = Mathf.Max(top, tile.y);
			left = Mathf.Min(left, tile.x);
			right = Mathf.Max(right, tile.x);
		}
	}
	public Region(params int[] ids) : this(TerrainManager.GetResourceTileCacheGroup(out status, ids)) {
	}
	
	public static TerrainManager.Status status = TerrainManager.Status.Succeeded;
	
	/// <summary>
	/// get the width of the region
	/// </summary>
	public int width { get { return right-left+1; } }
	/// <summary>
	/// get the height of the region
	/// </summary>
	public int height { get { return top-bottom+1; } }
	
	/// <summary>
	/// Convert to a region clamped in the space of the terrain
	/// </summary>
	public void ToTerrainRegion( out Region reg )
	{
		Region offset = TerrainManager.use.currentTerrainRegion;
		reg.left = Mathf.Max(0, left-offset.left);
		reg.right = Mathf.Min((int)TerrainManager.terrainData.size.x-1, right-offset.left);
		reg.top = Mathf.Min((int)TerrainManager.terrainData.size.z-1, top-offset.bottom);
		reg.bottom = Mathf.Max(0, bottom-offset.bottom);
	}
	
	/// <summary>
	/// Grow the region to include the specified point
	/// </summary>
	/// <param name="x">
	/// A <see cref="System.Int32"/>
	/// </param>
	/// <param name="z">
	/// A <see cref="System.Int32"/>
	/// </param>
	public void Encapsulate(int x, int z)
	{
		left = Mathf.Min(x, left);
		right = Mathf.Max(x, right);
		top = Mathf.Max(z, top);
		bottom = Mathf.Min(z, bottom);
	}
	
	/// <summary>
	/// Encapsulates extents of all contained megatiles.
	/// </summary>
	public void EncapsulateMegatiles()
	{
		left = Mathf.Max(TerrainManager.worldRegion.left, left-left%Megatile.size);
		right = Mathf.Min(TerrainManager.worldRegion.right, right+Megatile.size-right%Megatile.size-1);
		// NOTE: workaround since coordinates start at top presently; will need fix when it changes
		top = (TerrainManager.worldRegion.height-1)-top;
		top = Mathf.Max(TerrainManager.worldRegion.bottom, top-top%Megatile.size);
		top = (TerrainManager.worldRegion.height-1)-top;
		bottom = (TerrainManager.worldRegion.height-1)-bottom;
		bottom = Mathf.Min(TerrainManager.worldRegion.top, bottom+Megatile.size-bottom%Megatile.size-1);
		bottom = (TerrainManager.worldRegion.height-1)-bottom;
	}
	
	/// <summary>
	/// Get the center of the region
	/// </summary>
	/// <param name="x">
	/// A <see cref="System.Int32"/>
	/// </param>
	/// <param name="z">
	/// A <see cref="System.Int32"/>
	/// </param>
	public void GetCenter(out int x, out int z)
	{
		x = left+width/2;
		z = bottom+height/2;
	}
	/// <summary>
	/// Get the center of the region
	/// </summary>
	/// <param name="x">
	/// A <see cref="System.Single"/>
	/// </param>
	/// <param name="z">
	/// A <see cref="System.Single"/>
	/// </param>
	public void GetCenter(out float x, out float z)
	{
		x = left+width*0.5f;
		z = bottom+height*0.5f;
	}
	
	public void GetCenter(out Vector3 pos) {
		pos = Vector3.zero;
		pos.x = left+width*0.5f;
		pos.z = bottom+height*0.5f;
	}
	
	/// <summary>
	/// Tests whether the point is in the region
	/// </summary>
	/// <param name="p">
	/// A <see cref="Vector3"/>
	/// </param>
	/// <returns>
	/// A <see cref="System.Boolean"/> true if the point is in the region; otherwise false
	/// </returns>
	public bool Contains(Vector3 p)
	{
		return !(p.x < left || p.x > right+1 || p.z < bottom || p.z > top+1);
	}
	
	/// <summary>
	/// String reprensetation of the region
	/// </summary>
	/// <returns>
	/// A <see cref="System.String"/>
	/// </returns>
	public override string ToString()
	{
		 return new RectOffset(left, right, top, bottom).ToString();
	}
	
	public bool Intersects(Region otherRegion)
	{
		return !(otherRegion.left > right
				|| otherRegion.right < left
				|| otherRegion.top > bottom
				|| otherRegion.bottom < top);	
	}
	
	public Region GetOverlap(Region otherRegion)
	{
		if(this.Intersects(otherRegion))
		{
			return new Region(Mathf.Max(left, otherRegion.left), Mathf.Min(right, otherRegion.right), Mathf.Max(top, otherRegion.top), Mathf.Min(bottom, otherRegion.bottom));
		}
		else
		{
			return new Region(0,0,0,0);	
		}
	}
}