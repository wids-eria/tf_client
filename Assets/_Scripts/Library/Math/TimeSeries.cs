using UnityEngine;
using System.Collections;

/// <summary>
/// A class for describing a time series in a graph.
/// </summary>
[System.Serializable]
public class TimeSeries : System.Object
{
	/// <summary>
	/// The color.
	/// </summary>
	public Color color;
	/// <summary>
	/// The graphic.
	/// </summary>
	/// <remarks>
	/// A point on a line,for example.
	/// </remarks>
	public Texture2D graphic;
}