using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public abstract class GUIObject : Modifiable<GUIObject>
{
	public string name = "Undefined";
	public int id {get; private set;}
	public bool visible = true;
	
	
	
	public CommandQueue animationQueue = new CommandQueue(true);
	
	public Rect rect;
	
	public Modifiable<GUIAnchor> 	anchor;
	public Modifiable<Vector2> 		position;
	public Modifiable<float> 		width;
	public Modifiable<float>		height;
	public Modifiable<float>		x;
	public Modifiable<float>		y;
	
	public Vector2 offset = Vector2.zero;
	public Vector2 bottomLeft {get {return new Vector2(0,height.Get());}}
	
	public enum Direction {
		Clockwise,
		CounterClockWise,
	}
	
	/// <summary>
	/// Gets the styles.
	/// </summary>
	/// <value>
	/// The styles.
	/// </value>
	protected static GUIStyles Styles { get { return GameGUIManager.use.styles; } }
	
	
	
	protected List<GUILayoutOption> layoutOptions = new List<GUILayoutOption>();
	
	public GUIObject() : this(new Rect(0,0,0,0)) {
	}
	
	public GUIObject(Rect rect) {
		SetId(this);
		this.rect = rect;
		
		position = new Modifiable<Vector2>(new Vector2(rect.x,rect.y), OnPositionChanged);
		width = new Modifiable<float>(rect.width, ResetAnchor,RefreshRect);
		height = new Modifiable<float>(rect.height, ResetAnchor,RefreshRect);
		x = new Modifiable<float>(rect.x, OnPositionChanged, RefreshRect);
		y = new Modifiable<float>(rect.y, OnPositionChanged, RefreshRect);
		anchor = new Modifiable<GUIAnchor>(GUIAnchor.TopLeft, ResetAnchor);
	}
	
	private void RefreshRect() {
		rect.x = x.Get();
		rect.y = y.Get();
		rect.width = width.Get();
		rect.height = height.Get();
		if (OnModified != null) OnModified();
	}
	
	private void ResetAnchor() {
     	GUIAnchor anc = anchor.Get();
		if 		( GUIHelpers.CompareAnchor(anc,GUIAnchor.Top)) 		offset.y = 0;
		else if ( GUIHelpers.CompareAnchor(anc,GUIAnchor.Bottom)) 	offset.y = height.Get();
		else if ( GUIHelpers.CompareAnchor(anc,GUIAnchor.Middle)) 	offset.y = height.Get()/2;
		if 		( GUIHelpers.CompareAnchor(anc,GUIAnchor.Left)) 	offset.x = 0;
		else if ( GUIHelpers.CompareAnchor(anc,GUIAnchor.Right)) 	offset.x = width.Get();
		else if ( GUIHelpers.CompareAnchor(anc,GUIAnchor.Center)) 	offset.x = width.Get()/2;
	}
				
	private static int m_nextId = 0;
	private void SetId(GUIObject obj) {
		obj.id = m_nextId;
		m_nextId++;
	}
	
	/// <summary>
	/// Override this method to make any GUI draw calls
	/// </summary>
	public virtual void Draw() {
		if (!visible) return;
	}
	
	/// <summary>
	/// Determines whether this instance is mouse over.
	/// </summary>
	/// <returns>
	/// <c>true</c> if this instance is mouse over; otherwise, <c>false</c>.
	/// </returns>
	public virtual bool IsMouseOver() {
		return rect.Contains(GUIHelpers.MouseToGUIPosition(Input.mousePosition));
	}
	public virtual string GetToolTip() {
		return "";
	}
	
	/// <summary>
	/// Enable visible bool
	/// </summary>
	public virtual void VisibleEnabler(){
		visible = true;
	}
	
	public void AddLayoutOptions(params GUILayoutOption[] options) {
		foreach(GUILayoutOption option in options) {
			layoutOptions.Add(option);
		}
	}
	
	public virtual void SetPosition(Modifiable<Vector2> newPosition)
	{
		position = newPosition;	
	}
	
	private void OnPositionChanged() {
		x.Set(position.Value.x - offset.x);
		y.Set(position.Value.y - offset.y);
	}
}

