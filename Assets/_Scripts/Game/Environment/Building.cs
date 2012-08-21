using UnityEngine;
using System.Collections;

/// <summary>
/// Component to describe a building
/// </summary>
public class Building : MonoBehaviour
{
	/// <summary>
	/// a description of the building to appear in e.g. tooltips
	/// </summary>
	public string description = "This building is awesome.";
	/// <summary>
	/// The supported number of occupants.
	/// </summary>
	public int capacity { get { return m_capacity; } }
	/// <summary>
	/// The supported number of occupants backing field.
	/// </summary>
	[SerializeField]
	private int m_capacity;
	
	/// <summary>
	/// The icon.
	/// </summary>
	public Texture2D icon;
}