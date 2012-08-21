using UnityEngine;
using System.Collections;

/// <summary>
/// Watering can effect for planting trees.
/// </summary>
public class WateringCanEffect : MonoBehaviour
{
	/// <summary>
	/// Particle effect for the water drops.
	/// </summary>
	public ParticleEmitter waterEffect;
	
	/// <summary>
	/// Audio clip for the trickling watery goodness.
	/// </summary>
	public AudioClip soundEffect;
	
	/// <summary>
	/// Raises the start watering event.
	/// </summary>
	public void OnStartWatering()
	{
		AudioSource.PlayClipAtPoint(soundEffect, transform.position);
	}
	
	/// <summary>
	/// Message for when the animation has concluded.
	/// </summary>
	public void OnEndAnimation()
	{
		// unparent water because it will autodestruct
		waterEffect.transform.parent = null;
		Destroy(gameObject);
	}
}