using UnityEngine;
using System.Collections;

/// <summary>
/// Audio manager.
/// </summary>
public class AudioManager : MonoBehaviour
{
	/// <summary>
	/// Gets or sets the singleton.
	/// </summary>
	/// <value>
	/// The singleton.
	/// </value>
	public static AudioManager use { get; private set; }
	/// <summary>
	/// Initialize the singleton.
	/// </summary>
	void Awake()
	{
		if (use != null) {
			Destroy(use.gameObject);
		}
		use = this;
	}
	/// <summary>
	/// Initialize volume
	/// </summary>
	void Start()
	{
		AudioListener.volume = GamePrefs.GetMasterVolume();
	}
	/// <summary>
	/// Secret key press to toggle volume.
	/// </summary>
	void Update()
	{
		if (Input.GetKey(KeyCode.LeftApple) && Input.GetKeyDown(KeyCode.V)) {
			GamePrefs.SetMasterVolume(GamePrefs.GetMasterVolume()==0f?1:0f);
		}
	}
}