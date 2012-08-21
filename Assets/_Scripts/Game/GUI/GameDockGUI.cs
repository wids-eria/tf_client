using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameDockGUI : GUIObject {
	/// <summary>
	/// The rect of game dock .
	/// </summary>
	[HideInInspector]
	public Rect GameDockRect = new Rect();
	
	#region string constant for messenger
	public static string kOnQuestSelector = "OnQuestSelector";
	public static string kOnProjectionInterface = "OnProjectionInterface";
	public static string kOnGameDock = "OnGameDock";
	#endregion

	/// <summary>
	/// The flag of which item being invoked in game dock.
	/// </summary>
	private bool[] isInvokeItem = new bool[(int) GameDockIconCount];
	/// <summary>
	/// Constant game dock icon count.
	/// </summary>
	[SerializeField]
	private const int GameDockIconCount = 2;

	/// <summary>
	/// Initializes a new instance of the <see cref="GameDockGUI"/> class.
	/// </summary>
	public GameDockGUI(){
		Messenger.AddListener(kOnGameDock, resetAllGameDockItems);
		GameDockRect.x = Styles.roundDarkBox.margin.left;
		GameDockRect.y = Styles.roundDarkBox.margin.top;
		GameDockRect.width = 50f;
		GameDockRect.height = Screen.height/2;
		resetAllGameDockItems();
	}
	
	/// <summary>
	/// Releases unmanaged resources and performs other cleanup operations before the <see cref="GameDockGUI"/> is
	/// reclaimed by garbage collection.
	/// </summary>
	~GameDockGUI(){
		Messenger.RemoveListener(kOnGameDock, resetAllGameDockItems);
	}
	
	/// <summary>
	/// Resets all game dock items.
	/// </summary>
	public void resetAllGameDockItems(){
		for(int i=0; i<GameDockIconCount; i++){
			isInvokeItem[i]=false;
		}
		visible = true;
	}
	
	/// <summary>
	/// Displays the game dock.
	/// </summary>
	public override void Draw(){
		GUILayout.BeginArea(GameDockRect, Styles.roundDarkBox);
		{
			GUILayout.BeginVertical();
			{
				for(int i=0 ; i<GameDockIconCount ; i++){
					isInvokeItem[i] = GUILayout.Toggle (isInvokeItem[i], string.Format("{0}",i) ,Styles.smallButton);
				}
			}
			GUILayout.EndVertical();
		}GUILayout.EndArea();
		
		if(isInvokeItem.Length > 0 || isInvokeItem != null)
		{
			if(isInvokeItem[0]){
				Messenger.Broadcast(kOnQuestSelector);
				this.visible = false;
			}
			else if(isInvokeItem[1]){
				Messenger.Broadcast(kOnProjectionInterface);
				this.visible = false;
			}


		}
		
	}
	
	/// <summary>
	/// Determines whether this instance is mouse over.
	/// </summary>
	/// <returns>
	/// <c>true</c> if this instance is mouse over; otherwise, <c>false</c>.
	/// </returns>
	public override bool IsMouseOver(){
		if (GameDockRect.Contains(GUIHelpers.MouseToGUIPosition( Input.mousePosition))){
			return true;
		}
		else
			return false;
	}

}
