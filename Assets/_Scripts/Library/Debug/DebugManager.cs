using UnityEngine;
using System.Collections;

/// <summary>
/// A manager for global debug settings
/// </summary>
public class DebugManager : MonoBehaviour
{
	// flag to enable/disable global debug mode
	public bool isDebug = false;
	
	// singleton
	public static DebugManager use { get; private set; }
	
	/// <summary>
	/// Initialize singleton
	/// </summary>
	void Awake()
	{
		if (use != null) Destroy(use.gameObject);
		use = this;
	}
}