using UnityEngine;
using System.Collections;

/// <summary>
/// An enum to describe different types of ground surfaces
/// </summary>
public enum GroundTextureType {
	Loading,
	None,
	Barren,
	Crops,
	Developed,
	Forest,
	Grass,
	Water,
	Wetland
};

/// <summary>
/// A class to describe a ground texture's attributes for use on the terrain
/// (i.e., a serializable SplatPrototype)
/// </summary>
[System.SerializableAttribute]
public class GroundTexture
{
	/// <summary>
	/// The name of the ground texture (to e.g. appear in inspector array)
	/// </summary>
	public string name = "Ground Texture";
	/// <summary>
	/// The texture for the ground type
	/// </summary>
	public Texture2D texture;
	/// <summary>
	/// The texture's offset
	/// </summary>
	public Vector2 tileOffset;
	/// <summary>
	/// The texture's tiling size
	/// </summary>
	public Vector2 tileSize = 8f*Vector2.one;
	/// <summary>
	/// The type of the ground
	/// </summary>
	public GroundTextureType groundTextureType;
	/// <summary>
	/// Convert to SplatPrototype
	/// </summary>
	/// <returns>
	/// A <see cref="SplatPrototype"/>
	/// </returns>
	public SplatPrototype ToSplat()
	{
		SplatPrototype splat = new SplatPrototype();
		splat.texture = texture;
		splat.tileOffset = tileOffset;
		splat.tileSize = tileSize;
		return splat;
	}
}