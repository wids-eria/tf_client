using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class GUILayoutSelectionGridStyleBase {
	public float		Width;

	public GUIStyle		Style;

	public List<string>	Labels = new List<string>();
	
	public string		Name {
		get {
			return Style.name;
		}
	}
}

[Serializable]
public class GUILayoutSelectionGridStyle {
	[SerializeField]
	protected float		cellHeight;
	
	public float		Height {
		get {
			if( NumColumns <= 0 ) {
				return 0;
			}
			return cellHeight * columns[0].Labels.Count;
		}
	}
	
	[SerializeField]
	protected List<GUILayoutSelectionGridStyleBase>	columns = new List<GUILayoutSelectionGridStyleBase>();

	public GUILayoutSelectionGridStyleBase		GetColumn( int col ) {
		return columns[ col ];
	}
	
	[HideInInspector]
	public int				SelectedIndex = 0;
	
	public void				AddRow( params string[] row ) {
		if( row.Length != columns.Count ) {
			throw new System.ArrayTypeMismatchException();
		}
		
		GUILayoutSelectionGridStyleBase col = null;
		for( int ix = 0; ix < columns.Count; ++ix ) {
			col = columns[ix];
			col.Labels.Add( row[ix] );
		}
	}
	
	public int	NumRows {
		get {
			if( columns.Count <= 0 ) {
				return 0;
			}
			
			return columns[0].Labels.Count;
		}
	}
	
	public int	NumColumns {
		get {
			return columns.Count;
		}
	}
	
	public bool		Empty {
		get {
			return NumRows <= 0;
		}
	}
	
	public void			Clear() {
		columns.Clear();
	}
	
	public void			ClearRows() {
		for( int ix = 0; ix < columns.Count; ++ix ) {
			columns[ix].Labels.Clear();
		}
	}
	
	public void	Draw() {
		GUILayout.BeginHorizontal();
		for( int ix = 0; ix < columns.Count; ++ix ) {
			SelectedIndex = GUILayout.SelectionGrid( SelectedIndex, GetColumn(ix).Labels.ToArray(), 1, GetColumn(ix).Style, GUILayout.Width(GetColumn(ix).Width), GUILayout.Height(Height) );
		}
		GUILayout.EndHorizontal();
	}
}
