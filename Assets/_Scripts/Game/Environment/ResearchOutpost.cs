using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Research outpost.
/// </summary>
public class ResearchOutpost : MonoBehaviour
{
	/// <summary>
	/// Determines whether world position is in the radius of this instance.
	/// </summary>
	/// <returns>
	/// <c>true</c> if world position is in the radius tof this instance; otherwise, <c>false</c>.
	/// </returns>
	/// <param name='worldPosition'>
	/// A point in world space.
	/// </param>
	public static bool IsWorldPositionInRadius(Vector3 center, Vector3 worldPosition, float radius)
	{
		Vector3 v = center - worldPosition;
		v.y = 0f;
		return v.sqrMagnitude < radius*radius;
	}
	public static List<int> GetTileIdsInRadius(ResourceTile tile, float radius)
	{
		List<int> tiles = new List<int>();
		Vector3 pos;
		for (int rr=-(int)radius; rr<(int)radius; rr++) {
			for (int cc=-(int)radius; cc<(int)radius; cc++) {
				pos = new Vector3(tile.x,tile.y,tile.z);
				if (!IsWorldPositionInRadius(pos,pos + new Vector3(cc,0,rr), 20f)) {
					continue;
				}
				try {
					Debug.Log("Added a Tile");
					tiles.Add(TerrainManager.use.resourceTileCache[tile.y+rr,tile.x+cc].id);
				}
				catch (System.IndexOutOfRangeException) {
					continue;
				}
			}
		}
		return tiles;
	}
}