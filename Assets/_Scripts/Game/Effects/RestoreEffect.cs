using UnityEngine;
using System.Collections;

/// <summary>
/// Restoration effect
/// </summary>
public class RestoreEffect : MonoBehaviour
{
	/// <summary>
	/// alpha for fade effect
	/// </summary>
	public float alpha = 1f;
	
	/// <summary>
	/// renderer component of the god rays
	/// </summary>
	public Renderer r;
	
	/// <summary>
	/// mesh filter on the god rays
	/// </summary>
	public MeshFilter m;
	
	/// <summary>
	/// colors on the mesh
	/// </summary>
	private Color[] colors;
	
	/// <summary>
	/// speed of texture scroll
	/// </summary>
	public float omega = 0.25f;
	
	/// <summary>
	/// sound effect
	/// </summary>
	public AudioClip laaa;
	
	/// <summary>
	/// Cache god ray colors
	/// </summary>
	void Start()
	{
		colors = m.mesh.colors;
		AudioSource.PlayClipAtPoint(laaa, transform.position);
		TintColors();
	}
	
	/// <summary>
	/// tint vertex colors
	/// </summary>
	void Update()
	{
		TintColors();
	}
	
	/// <summary>
	/// Tint the colors bro
	/// </summary>
	void TintColors()
	{
		Color[] c = MeshHelpers.TintColors(colors, Color.white*alpha);
		m.mesh.colors = c;
		r.material.SetTextureOffset("_MainTex", Vector2.right*Time.time*omega);
	}
	
	/// <summary>
	/// self destruct upon completion
	/// </summary>
	public void OnEndEffect()
	{
		Destroy(gameObject);
	}
}