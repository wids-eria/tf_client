using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public interface ISelectable {
	bool 	IsSelectable();
	bool 	GetSelected();
	void	SetSelectable(bool isSelectable);
	void 	SetSelected(bool isSelected);
	
	Action<ISelectable> OnSelected {get; set;}
}

public class GUIRadioGroup
{
	protected List<ISelectable> items = new List<ISelectable>();
	
	public ISelectable 	selectedItem 	{ get; protected set;}
	public int			selectedIndex 	{ get {return items.IndexOf(selectedItem); } }
	
	public int IndexOf(ISelectable item) {
		return items.IndexOf(item);
	}
	
	public GUIRadioGroup(params ISelectable[] baseGroup) {
		foreach(ISelectable item in baseGroup) {
			AddItem(item);
		}
	}
	~GUIRadioGroup() {
		foreach(ISelectable item in items) {
			item.SetSelectable(false); // <--- this seems a little dirty...
			item.OnSelected -= SetSelected;
		}
		items.Clear();
	}
	
	public void AddItem(ISelectable item) {
		if (!items.Contains(item)) {
			items.Add(item);
			item.SetSelected(false);
			item.SetSelectable(true);
			item.OnSelected += SetSelected; 
		}
	}
	public void SetSelected(ISelectable item) {
		if ( !items.Contains(item)) return;
		selectedItem = item;
		foreach(ISelectable i in items) {
			if (i != item) {
				i.SetSelected(false);
			}
		}
		Debug.Log(string.Format("ISelectable `{0}' was selected.",item.ToString()));
	}
	public void ClearSelection() {
		foreach(ISelectable i in items) {
			i.SetSelected(false);
		}
	}
}

