using UnityEngine;
using System.Collections;

/// <summary>
/// the type of tree
/// </summary>
public enum TreeGraphicType {
	SmallConifer,
	LargeConifer,
	SmallDeciduous,
	LargeDeciduous
}

/// <summary>
/// Component for describing a tree species and its distribution
/// (i.e., a serializable DetailPrototype)
/// </summary>
public class TreeProperties : MonoBehaviour
{
	/// <summary>
	/// the tree type
	/// </summary>
	public TreeGraphicType treeType;
	/// <summary>
	/// The texture associated with the tree
	/// </summary>
	public Texture2D prototypeTexture;
	/// <summary>
	/// minimum billboard width
	/// </summary>
	public float minWidth = 0.2f;
	/// <summary>
	/// maximum billboard width
	/// </summary>
	public float maxWidth = 0.5f;
	/// <summary>
	/// minimum billboard height
	/// </summary>
	public float minHeight = 0.2f;
	/// <summary>
	/// maximum billboard height
	/// </summary>
	public float maxHeight = 0.5f;
	/// <summary>
	/// noise spread
	/// </summary>
	public float noiseSpread = 0.1f;
	/// <summary>
	/// healthy tint for the billboard's vertices
	/// </summary>
	public Color healthyColor = new Color(0.5f, 1f, 0.35f); // greenish
	/// <summary>
	/// dry tint for the billboard's vertices
	/// </summary>
	public Color dryColor = new Color(0.5f, 0.4f, 0.2f); // brownish
	/// <summary>
	/// Create a DetailPrototype from the tree properties
	/// </summary>
	/// <returns>
	/// A <see cref="DetailPrototype"/>
	/// </returns>
	public DetailPrototype ToDetailPrototype()
	{
		DetailPrototype dp = new DetailPrototype();
		dp.prototypeTexture = prototypeTexture;
		dp.minWidth = minWidth;
		dp.maxWidth = maxWidth;
		dp.minHeight = minHeight;
		dp.maxHeight = maxHeight;
		dp.noiseSpread = noiseSpread;
		dp.healthyColor = healthyColor;
		dp.dryColor = dryColor;
		dp.renderMode = DetailRenderMode.GrassBillboard;
		return dp;
	}
}