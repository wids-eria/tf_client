using UnityEngine;
using UnityEditor;
using System.Collections;

/// <summary>
/// Print the assetPath for all selected source assets
/// </summary>
public class PrintAssetPath : ScriptableObject
{
	[MenuItem ("Custom/Library/Objects/Print Asset Path")]
	static void Clear()
	{
		foreach (UnityEngine.Object obj in Selection.objects)
		{
			if (AssetDatabase.Contains(obj))
				Debug.Log(AssetDatabase.GetAssetPath(obj));
			else Debug.LogWarning("Object "+obj.name+" is not a source asset.");
		}
	}
}