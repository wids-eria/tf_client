using UnityEngine;
using System.Collections;

/// <summary>
/// An enum to specify whether terrain should be flushed after changes or not
/// </summary>
public enum SetTerrainMode { Flush, NoFlush }

/// <summary>
/// Utility class for working with terrains
/// </summary>
public static class TerrainHelpers : System.Object
{
	/// <summary>
	/// Clone TerrainData object
	/// </summary>
	/// <param name="source">
	/// A <see cref="TerrainData"/>
	/// </param>
	/// <param name="detailResolutionPerPatch">
	/// A <see cref="System.Int32"/>
	/// </param>
	/// <returns>
	/// A <see cref="TerrainData"/>
	/// </returns>
	public static TerrainData Clone(TerrainData source, int detailResolutionPerPatch)
	{
		TerrainData dest = new TerrainData();
		dest.alphamapResolution = source.alphamapResolution;
		dest.baseMapResolution = source.baseMapResolution;
		dest.SetDetailResolution(source.detailResolution, detailResolutionPerPatch);
		DetailPrototype[] dp = new DetailPrototype[source.detailPrototypes.Length];
		for (int i=0; i<dp.Length; i++) dp[i] = new DetailPrototype();
		dest.detailPrototypes = dp;
		for (int i=0; i<dp.Length; i++)
		{
			dest.detailPrototypes[i].bendFactor =			source.detailPrototypes[i].bendFactor;
			dest.detailPrototypes[i].dryColor =			source.detailPrototypes[i].dryColor;
			dest.detailPrototypes[i].healthyColor =		source.detailPrototypes[i].healthyColor;
			dest.detailPrototypes[i].maxHeight =			source.detailPrototypes[i].maxHeight;
			dest.detailPrototypes[i].maxWidth =			source.detailPrototypes[i].maxWidth;
			dest.detailPrototypes[i].minHeight =			source.detailPrototypes[i].minHeight;
			dest.detailPrototypes[i].minWidth =			source.detailPrototypes[i].minWidth;
			dest.detailPrototypes[i].noiseSpread =			source.detailPrototypes[i].noiseSpread;
			dest.detailPrototypes[i].prototype =			source.detailPrototypes[i].prototype;
			dest.detailPrototypes[i].prototypeTexture =	source.detailPrototypes[i].prototypeTexture;
			dest.detailPrototypes[i].renderMode =			source.detailPrototypes[i].renderMode;
			dest.detailPrototypes[i].usePrototypeMesh =	source.detailPrototypes[i].usePrototypeMesh;
			dest.SetDetailLayer(0,0,i,source.GetDetailLayer(0,0,source.detailWidth,source.detailHeight,i));
		}
		dest.RefreshPrototypes();
		dest.heightmapResolution = source.heightmapResolution;
		dest.SetHeights(0,0,source.GetHeights(0,0,source.heightmapWidth,source.heightmapHeight));
		dest.hideFlags = source.hideFlags;
		dest.name = source.name+" (Clone)";
		dest.size = source.size;
		dest.splatPrototypes = source.splatPrototypes;
		dest.treeInstances = source.treeInstances;
		dest.treePrototypes = source.treePrototypes;
		dest.wavingGrassAmount = source.wavingGrassAmount;
		dest.wavingGrassSpeed = source.wavingGrassSpeed;
		dest.wavingGrassStrength = source.wavingGrassStrength;
		dest.wavingGrassTint = source.wavingGrassTint;
		return dest;
	}
	
	/// <summary>
	/// Return a control texture as an int[height,width] for e.g., detail distribution
	/// </summary>
	/// <param name="tex">
	/// A <see cref="Texture2D"/>
	/// </param>
	/// <param name="xBase">
	/// A <see cref="System.Int32"/>
	/// </param>
	/// <param name="yBase">
	/// A <see cref="System.Int32"/>
	/// </param>
	/// <param name="width">
	/// A <see cref="System.Int32"/>
	/// </param>
	/// <param name="height">
	/// A <see cref="System.Int32"/>
	/// </param>
	/// <param name="res">
	/// A <see cref="System.Int32"/> detail resolution
	/// </param>
	/// <returns>
	/// A <see cref="System.Int32[,]"/>
	/// </returns>
	public static int[,] ControlTextureToIntArray(Texture2D tex, int xBase, int yBase, int width, int height, int res)
	{
		Color[] pixels = tex.GetPixels();
		int[,] density = new int[height, width];
		if (tex.format == TextureFormat.Alpha8)
		{
			for (int r=0; r<height; r++)
			{
				for (int c=0; c<width; c++)
				{
					density[r,c] = (int)(res*pixels[(r+yBase)*tex.width+(c+xBase)].a);
				}
			}
		}
		else
		{
			for (int r=0; r<height; r++)
			{
				for (int c=0; c<width; c++)
				{
					density[r,c] = (int)(res*pixels[(r+yBase)*tex.width+(c+xBase)].grayscale);
				}
			}
		}
		return density;
	}
	
	/// <summary>
	/// Update a grayscale control texture using the supplied information
	/// </summary>
	/// <param name="tex">
	/// A <see cref="Texture2D"/>
	/// </param>
	/// <param name="xBase">
	/// A <see cref="System.Int32"/>
	/// </param>
	/// <param name="yBase">
	/// A <see cref="System.Int32"/>
	/// </param>
	/// <param name="details">
	/// A <see cref="System.Int32[,]"/>
	/// </param>
	/// <param name="res">
	/// A <see cref="System.Int32"/>
	/// </param>
	public static void SetTextureFromDetails(Texture2D tex, int xBase, int yBase, int[,] details, int res)
	{
		float div = 1f/res;
		Color[] colors = new Color[(details.GetUpperBound(0)+1)*(details.GetUpperBound(1)+1)];
		for (int c=0; c<=details.GetUpperBound(1); c++)
		{
			for (int r=0; r<=details.GetUpperBound(0); r++)
			{
				colors[r*(details.GetUpperBound(1)+1)+c] = Color.white*(details[r,c]*div);
			}
		}
		tex.SetPixels(xBase, yBase, details.GetUpperBound(1)+1, details.GetUpperBound(0)+1, colors);
		tex.Apply();
	}
	
	/// <summary>
	/// Return a control texture as a float[height,width] for e.g., height or splat
	/// </summary>
	/// <param name="tex">
	/// A <see cref="Texture2D"/>
	/// </param>
	/// <param name="xBase">
	/// A <see cref="System.Int32"/>
	/// </param>
	/// <param name="yBase">
	/// A <see cref="System.Int32"/>
	/// </param>
	/// <param name="width">
	/// A <see cref="System.Int32"/>
	/// </param>
	/// <param name="height">
	/// A <see cref="System.Int32"/>
	/// </param>
	/// <param name="scale">
	/// A <see cref="System.Single"/>
	/// </param>
	/// <returns>
	/// A <see cref="System.Single[,]"/>
	/// </returns>
	public static float[,] ControlTextureToFloatArray(Texture2D tex, int xBase, int yBase, int width, int height, float scale)
	{
		Color[] pixels = tex.GetPixels();
		float[,] vals = new float[height, width];
		if (tex.format == TextureFormat.Alpha8)
		{
			for (int r=0; r<height; r++)
			{
				for (int c=0; c<width; c++)
				{
					vals[r,c] = scale*pixels[(r+yBase)*tex.width+(c+xBase)].a;
				}
			}
		}
		else
		{
			for (int r=0; r<height; r++)
			{
				for (int c=0; c<width; c++)
				{
					vals[r,c] = scale*pixels[(r+yBase)*tex.width+(c+xBase)].grayscale;
				}
			}
		}
		return vals;
	}
	
	/// <summary>
	/// Convert terrainSpacePosition into a tree-space position
	/// </summary>
	/// <param name="terrain">
	/// A <see cref="Terrain"/>
	/// </param>
	/// <param name="terrainSpacePosition">
	/// A <see cref="Vector3"/> in terrain's local space
	/// </param>
	/// <returns>
	/// A <see cref="Vector3"/> homogenous coordinate
	/// </returns>
	public static Vector3 TerrainToTreePosition(Terrain terrain, Vector3 terrainSpacePosition)
	{
		Vector3 scale = new Vector3(
		                            1f/terrain.terrainData.size.x,
		                            1f/terrain.terrainData.size.y,
		                            1f/terrain.terrainData.size.z
		                            );
		return TerrainToTreePosition(scale, terrainSpacePosition);
	}
	
	/// <summary>
	/// Convert terrainSpacePosition into a tree-space position
	/// </summary>
	/// <param name="inverseTerrainSize">
	/// A <see cref="Vector3"/> inverse of the size of the terrain of interest (e.g., (1/x, 1/y, 1/z))
	/// </param>
	/// <param name="terrainSpacePosition">
	/// A <see cref="Vector3"/> in the local space of the terrain of interest
	/// </param>
	/// <returns>
	/// A <see cref="Vector3"/> homogenous coordinate
	/// </returns>
	public static Vector3 TerrainToTreePosition(Vector3 inverseTerrainSize, Vector3 terrainSpacePosition)
	{
		terrainSpacePosition.Scale(inverseTerrainSize);
		return terrainSpacePosition;
	}
	
	/// <summary>
	/// Convert treeSpacePosition into a terrain-space position
	/// </summary>
	/// <param name="terrain">
	/// A <see cref="Terrain"/>
	/// </param>
	/// <param name="treeSpacePosition">
	/// A <see cref="Vector3"/> homogenous coordinate
	/// </param>
	/// <returns>
	/// A <see cref="Vector3"/> in terrain's local space
	/// </returns>
	public static Vector3 TreeToTerrainPosition(Terrain terrain, Vector3 treeSpacePosition)
	{
		treeSpacePosition.Scale(terrain.terrainData.size);
		return treeSpacePosition;
	}
}