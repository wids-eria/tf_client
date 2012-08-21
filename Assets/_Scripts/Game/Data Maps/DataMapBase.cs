using UnityEngine;
using System.Collections;
using System;


/// <summary>
/// Basic 2D projection map
/// </summary>
public class DataMapBase
{
	public string		name;
	public Texture2D 	texture 	 {get; protected set;}
	public Vector2		displayScale {get; protected set;}
	public Vector2		offset		 {get; protected set;}
	
	public DataMapBase() : this(null,Vector2.one) {
	}
	public DataMapBase(Texture2D aTexture, Vector2 scale) : this (aTexture,scale,Vector2.zero) {
	}
	public DataMapBase(Texture2D aTexture, Vector2 scale, Vector2 anOffset) {
		texture = aTexture;
		displayScale = scale;
		offset = anOffset;
		if (texture == null) {
			texture = new Texture2D((int)TerrainManager.terrainData.size.x,(int)TerrainManager.terrainData.size.z, TextureFormat.RGBA32, false);
			TextureHelpers.FloodFill(texture, DataMapController.GetColor(MappingColor.Empty), TextureHelpers.ApplyMode.Apply);
		}
	}
	
	public void SetFilterMode(FilterMode mode) {
		texture.filterMode = mode;
	}
	public void SetWrapMode(TextureWrapMode mode) {
		texture.wrapMode = mode;
	}
}


/// <summary>
/// Base class used to generate a projection texture that will be displayed on the terrain.
/// Implement 'GetColorFromTile' to translate ResourceTiles to a Color
/// </summary>
public abstract class DataMap : DataMapBase
{
	public abstract Color GetColorFromTile(ResourceTileLite tile);
	
	public DataMap() : this(FilterMode.Bilinear) {
	}
	public DataMap(FilterMode mode) : base() {
		Messenger<Region>.AddListener(TerrainManager.kLoadedNewTiles,PaintRegion);
		SetFilterMode(mode);
		SetWrapMode(TextureWrapMode.Clamp);
	}
	~DataMap() {
		Messenger<Region>.RemoveListener(TerrainManager.kLoadedNewTiles,PaintRegion);
	}

	public void PaintRegion(Region region) {
		Color[] pixels = texture.GetPixels(region.left,region.bottom,region.width,region.height);
		Vector3 pos;
		for(int r = 0; r < region.height; r++) {
			for (int c = 0; c < region.width; c++) {
				pos = new Vector3(-1+c+region.left,0,-1+r+region.bottom);
				ResourceTileLite tile;
				if (TerrainManager.GetResourceTileCacheAtWorldPosition(pos,out tile)) {
					pixels[r*region.width+c] = GetColorFromTile(tile);
				} else {
					pixels[r*region.width+c] = DataMapController.GetColor(MappingColor.Empty);
				}
			}
		}
		texture.SetPixels(region.left,region.bottom,region.width,region.height,pixels);
		texture.Apply();
	}
	
	public void Refresh() {
		PaintRegion(TerrainManager.use.mapRegion);
	}
	protected Color GetColor(MappingColor color) {
		return DataMapController.GetColor(color);
	}
}





