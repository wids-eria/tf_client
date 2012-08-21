using UnityEngine;
using System.Collections;

public class GUIHorizontal : GUIContainer
{
	public GUIHorizontal(Rect zoneRect, bool drawBackground) : base(zoneRect,drawBackground) {
	}
	
	public override void Draw ()
	{
		if (!visible) return;
		if (!drawBackground) { 
			GUILayout.BeginArea(rect);
		} else {
			GUILayout.BeginArea(rect,Styles.roundDarkBox);
		}
		GUILayout.BeginHorizontal(layoutOptions.ToArray());
			foreach(GUIObject obj in elements.Keys) {
				obj.Draw();
			}
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}
}

