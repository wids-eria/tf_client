using UnityEngine;
using System.Collections;

/// <summary>
/// Effect for the clearcut action
/// </summary>
public class ClearcutEffect : MonoBehaviour
{
	public ParticleEmitter leafChops;
	public AudioClip bzzzz;
	
	public Transform translator;
	
	private ParticleEmitter _particles;
	
	void OnClearcut()
	{
		AudioSource.PlayClipAtPoint(bzzzz, transform.position);
		_particles = Instantiate(leafChops, translator.position, transform.rotation) as ParticleEmitter;
		_particles.transform.parent = translator;
	}
	
	void OnEndAnimation()
	{
		_particles.emit = false;
		_particles.transform.parent = null;
		Destroy(gameObject);
	}
}