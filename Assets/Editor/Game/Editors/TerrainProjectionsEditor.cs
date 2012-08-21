using UnityEditor;
using UnityEngine;
using System.Collections;
/*
/// <summary>
/// Terrain projections editor.
/// </summary>
[CustomEditor(typeof(TerrainProjections))]
public class TerrainProjectionsEditor : Editor
{
	/// <summary>
	/// The component being inspected.
	/// </summary>
	private TerrainProjections m_projections;
	
	/// <summary>
	/// Raises the enable event.
	/// </summary>
	void OnEnable()
	{
		m_projections = target as TerrainProjections;
	}
	
	/// <summary>
	/// Raises the inspector GUI event.
	/// </summary>
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		
		// labels for zoning colors
		string[] labels = System.Enum.GetNames(typeof(ZoneType));
		
		// make sure array lengths are correct
		if (m_projections.zoningColors.Count != labels.Length) {
			if (m_projections.zoningColors.Count > labels.Length) {
				while (m_projections.zoningColors.Count < labels.Length) {
					m_projections.zoningColors.RemoveAt(m_projections.zoningColors.Count-1);
				}
			}
			else {
				while (m_projections.zoningColors.Count < labels.Length) {
					m_projections.zoningColors.Add(new Color());
				}
			}
		}
		
		// zoning color editor
		for (int i=0; i<labels.Length; ++i) {
			EditorGUILayout.BeginHorizontal();
			{
				GUILayout.Label(labels[i], GUILayout.Width(150f));
				m_projections.zoningColors[i] = EditorGUILayout.ColorField(m_projections.zoningColors[i]);
			}
			EditorGUILayout.EndHorizontal();
		}
		
		EditorUtility.SetDirty(target);
	}
}*/