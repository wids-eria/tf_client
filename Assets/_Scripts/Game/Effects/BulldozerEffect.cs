using UnityEngine;
using System.Collections;

/// <summary>
/// Component for the bulldoze effect
/// </summary>
public class BulldozerEffect : MonoBehaviour
{
	/// <summary>
	/// Particle effect for object being destroyed
	/// </summary>
	public ParticleEmitter destructionEffect;
	
	/// <summary>
	/// Audio clip for the destruction sound effect
	/// </summary>
	public AudioClip destructionSound;
	
	/// <summary>
	/// Message for when the bulldozer has reached its target
	/// </summary>
	public void OnDestroyMicrotile()
	{
		Instantiate(destructionEffect, transform.position, transform.rotation);
		AudioSource.PlayClipAtPoint(destructionSound, transform.position);
	}
	
	/// <summary>
	/// Message for when the animation has concluded
	/// </summary>
	public void OnEndAnimation()
	{
		Destroy(gameObject);
	}
}