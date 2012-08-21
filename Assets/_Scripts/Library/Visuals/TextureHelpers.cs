using UnityEngine;
using System.Collections;

/// <summary>
/// Static helper methods for working with textures
/// </summary>
public static class TextureHelpers : System.Object
{
	/// <summary>
	/// Return a duplicate of tex
	/// </summary>
	/// <param name="tex">
	/// A <see cref="Texture2D"/>
	/// </param>
	/// <returns>
	/// A <see cref="Texture2D"/>
	/// </returns>
	public static Texture2D Duplicate(Texture2D tex)
	{
		Texture2D t = new Texture2D(tex.width, tex.height, tex.format, false);
		t.name = tex.name + " (Clone)";
		try
		{
			t.SetPixels(tex.GetPixels());
		}
		catch (System.Exception e)
		{
			Debug.LogError(e);
		}
		return t;
	}
	
	/// <summary>
	/// an enum to describe whether changes should be applied
	/// </summary>
	public enum ApplyMode { Apply, NoApply }
	
	/// <summary>
	/// Flood fill the specified texture with the specified color
	/// </summary>
	/// <param name="tex">
	/// A <see cref="Texture2D"/>
	/// </param>
	/// <param name="col">
	/// A <see cref="Color"/>
	/// </param>
	/// <param name="apply">
	/// A <see cref="ApplyMode"/>
	/// </param>
	public static void FloodFill(Texture2D tex, Color col, ApplyMode apply)
	{
		FloodFill(tex, 0, 0, tex.width, tex.height, col, apply);
	}
	
	/// <summary>
	/// Flood fill the specified block on the specified texture using the specified color
	/// </summary>
	/// <param name="tex">
	/// A <see cref="Texture2D"/>
	/// </param>
	/// <param name="x">
	/// A <see cref="System.Int32"/>
	/// </param>
	/// <param name="y">
	/// A <see cref="System.Int32"/>
	/// </param>
	/// <param name="width">
	/// A <see cref="System.Int32"/>
	/// </param>
	/// <param name="height">
	/// A <see cref="System.Int32"/>
	/// </param>
	/// <param name="col">
	/// A <see cref="Color"/>
	/// </param>
	/// <param name="apply">
	/// A <see cref="ApplyMode"/>
	/// </param>
	public static void FloodFill(Texture2D tex, int x, int y, int width, int height, Color col, ApplyMode apply)
	{
		int size = width*height;
		Color[] colors = new Color[size];
		for (int i=0; i<size; i++) colors[i] = col;
		tex.SetPixels(x, y, width, height, colors);
		if (apply==TextureHelpers.ApplyMode.Apply) tex.Apply();
	}
}