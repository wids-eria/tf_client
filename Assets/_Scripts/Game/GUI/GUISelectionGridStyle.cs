using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class GUISelectionGridStyleBase {
	[SerializeField]
	protected GUIStyleEx	style;
	
	public List<GUIContent>		Labels = new List<GUIContent>();
	
	public Rect			DrawRect {
		get {
			if( style == null ) {
				style = new GUIStyleEx();
			}
			return style.DrawRect;
		}
	}
	
	public string		Name {
		get {
			if( style == null ) {
				style = new GUIStyleEx();
			}
			return style.Name;
		}
	}
	
	public GUIStyle		Style {
		get {
			if( style == null ) {
				style = new GUIStyleEx();
			}
			return style.Style;
		}
	}
}

[Serializable]
public class GUISelectionGridStyle {
	public Rect				DrawRect;

	[SerializeField]
	protected List<GUISelectionGridStyleBase>	columns = new List<GUISelectionGridStyleBase>();

	public GUISelectionGridStyleBase		GetColumn( int col ) {
		return columns[ col ];
	}
	
	[HideInInspector]
	public int				SelectedIndex = 0;
	
	public void				AddRow( params GUIContent[] row ) {
		if( row.Length != columns.Count ) {
			throw new System.ArrayTypeMismatchException();
		}
		
		GUISelectionGridStyleBase col = null;
		for( int ix = 0; ix < columns.Count; ++ix ) {
			col = columns[ix];
			col.Labels.Add( row[ix] );
		}
	}
	
	public int	NumColumns {
		get {
			return columns.Count;
		}
		
		set {
			columns.Clear();
			for( int ix = 0; ix < value; ++ix ) {
				columns.Add( new GUISelectionGridStyleBase() );
			}
		}
	}
	
	public void			Clear() {
		columns.Clear();
	}
	
	public void	Draw() {
		GUILayout.BeginHorizontal();
		for( int ix = 0; ix < columns.Count; ++ix ) {
			SelectedIndex = GUI.SelectionGrid( DrawRect, SelectedIndex, GetColumn(ix).Labels.ToArray(), 1 );
		}
		GUILayout.EndHorizontal();
	}
}