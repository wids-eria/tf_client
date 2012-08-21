using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;

[CustomEditor(typeof(DataMapController))]
public class DataMapEditor : Editor {
	
	DataMapController dmc;
	
	void OnEnable() {
		dmc = (DataMapController)target;
	}
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		
		//Color Pallette
		DrawColorPallette();
		DrawProjectionPallette();
		GUILayout.Space(15f);
		EditorUtility.SetDirty(target);
	}
	
	public void DrawColorPallette()
	{
		DrawPallette<Color>("Projection Color Pallette", dmc.colors, typeof(MappingColor), BaseAlloc<Color>, ModifyColor);
	}
	
	public void DrawProjectionPallette()
	{
		DrawPallette<Material>("Projection Material Pallette", dmc.projectionTypes, typeof(ProjectionStyle), MaterialAlloc, ModifyObject<Material>);
	}
	
	public void DrawPallette<T>(string name, List<T> list, Type enumType, Func<T> allocator, Func<T,T> handler)
	{
		string[] labels = SetupEnumList<T>(list,enumType,allocator);
		
		EditorGUILayout.BeginVertical();
		GUILayout.Space(8f);
		GUILayout.Label(name);
		GUILayout.Space(5f);
		for(int i=0; i<labels.Length; i++)
		{
			EditorGUILayout.BeginHorizontal();
			{
				GUILayout.Label(labels[i],GUILayout.Width(150));
				list[i] = handler(list[i]);
			}
			EditorGUILayout.EndHorizontal();
		}
		GUILayout.Space(5f);
		EditorGUILayout.EndVertical();
	}
		
	public T ModifyObject<T>(T obj) where T: UnityEngine.Object
	{
		return EditorGUILayout.ObjectField(obj,typeof(T),false) as T;
	}
	public Color ModifyColor(Color color)
	{
		return EditorGUILayout.ColorField(color);
	}
	
	public T BaseAlloc<T>() where T: new()
	{
		return new T();
	}
	public Material MaterialAlloc()
	{
		string path = "/Environment/Materials/";
		return AssetDatabase.LoadAssetAtPath(string.Format(path,"MapProjector.mat"),typeof(Material)) as Material;
	}
	
	public string[] SetupEnumList<T>(List<T> list, Type enumSet) where T: new()
	{
		return SetupEnumList<T>(list,enumSet,BaseAlloc<T>);
	}
	
	public string[] SetupEnumList<T>(List<T> list, Type enumSet, Func<T> allocator)
	{
		string[] labels = Enum.GetNames(enumSet);
		// make sure array lengths are correct
		if (list.Count != labels.Length) {
			if (list.Count > labels.Length) {
				while (list.Count < labels.Length) {
					list.RemoveAt(dmc.colors.Count-1);
				}
			}
			else {
				while (list.Count < labels.Length) {
					list.Add(allocator());
				}
			}
		}
		return labels;
	}
	
}
