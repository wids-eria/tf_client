using UnityEngine;
using UnityEditor;
using System.Collections;

/// <summary>
/// Clear all non-transform components from selected GameObjects
/// </summary>
public class ClearComponents : ScriptableObject
{
	[MenuItem ("Custom/Library/Objects/Clear Components")]
	static void DoIt()
	{
		foreach (GameObject obj in Selection.gameObjects)
		{
			Component[] comps = obj.GetComponents(typeof(Component));
			foreach (Component comp in comps)
			{
				if (comp.GetType() != typeof(Transform)) DestroyImmediate(comp, true);
			}
		}
	}
}