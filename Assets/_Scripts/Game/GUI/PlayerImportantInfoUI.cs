using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerImportantInfoUI : GUIObject {

	
	/// <summary>
	/// The current marten count.
	/// </summary>
	private int currentMartenCount {get {return GameManager.worldData.marten_suitable_tile_count;}}
	/// <summary>
	/// The human_ population.
	/// </summary>
	private int humanPopulation {get {return GameManager.worldData.human_population;}}
	/// <summary>
	/// The timber count.
	/// </summary>
	private int timberCount {get { return GameManager.worldData.timber_count;}}
	

	/// <summary>
	/// The info box.
	/// </summary>
	[HideInInspector]
	public Rect InfoBox = new Rect();
	
	/// <summary>
	/// Gets the styles.
	/// </summary>
	/// <value>
	/// The styles.
	/// </value>
	private GUIStyles m_styles { get { return Styles; } }
	
	/// <summary>
	/// The height of the player info box.
	/// </summary>
	[HideInInspector]
	public float playerInfoBoxHeight{get; private set;}
	
	/// <summary>
	/// The m_info icon button.
	/// </summary>
	[HideInInspector]
	protected GUIContent[] m_infoIconButton = null;
	
	/// <summary>
	/// Gets or sets content of player information
	/// </summary>
	[HideInInspector]
	public string[] InformationIndex{get; private set;}
	
	/// <summary>
	/// The info description.
	/// </summary>
	private string[] InfoDescription;
	
	/// <summary>
	/// Initializes a new instance of the <see cref="PlayerImportantInfoUI"/> class.
	/// </summary>
	public PlayerImportantInfoUI(){
		Messenger<WorldData>.AddListener(GameManager.kWorldDataChanged, LoadInformationIndex);
		InfoBox.width = 150f;
		InfoBox.height = 0f;
		InfoBox.y = 5f;
		InfoBox.x = Screen.width - InfoBox.width;
		
	}
	
	/// <summary>
	/// Releases unmanaged resources and performs other cleanup operations before the <see cref="PlayerImportantInfoUI"/>
	/// is reclaimed by garbage collection.
	/// </summary>
	~PlayerImportantInfoUI(){
		Messenger<WorldData>.RemoveListener(GameManager.kWorldDataChanged, LoadInformationIndex);
	}
	
	/// <summary>
	/// Displaies the player important info.
	/// </summary>
	public override void Draw(){
		if(InfoBox.height == 0f){
			LoadPlayerInfoIcon();
		}
		LoadInformationIndex(null);
		int sumPadding = 0;
		GUILayout.BeginArea(InfoBox, Styles.roundDarkBox);
		{		
			for(int i=0; i<InformationIndex.Length ; i++){
				GUILayout.BeginHorizontal();
					GUILayout.Button(m_infoIconButton[i],m_styles.smallButtonDisabled);
					GUILayout.BeginVertical();
						GUILayout.TextArea(string.Format("{0}\n{1}", InfoDescription[i],InformationIndex[i]), m_styles.mediumTextHighlighted);
					GUILayout.EndVertical();
					GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
				sumPadding += m_styles.smallButton.padding.top + m_infoIconButton[i].image.height + m_styles.smallButton.padding.bottom
					+ (int)(1.6*(m_styles.smallButton.margin.top + m_styles.smallButton.padding.bottom));
			}
		}
		GUILayout.EndArea();
		InfoBox.height = (float)sumPadding;
	}
	
	/// <summary>
	/// Loads the player info icon.
	/// </summary>
	protected void LoadPlayerInfoIcon(){
		List<GUIContent> actionButtons = new List<GUIContent>();
		List<PlayerAction> actions = new List<PlayerAction>();
		foreach (PlayerAction a in Player.current.actions) {
			actionButtons.Add(new GUIContent(a.icon, ""));
			actions.Add(a);
		}
		m_infoIconButton = actionButtons.ToArray();
	}
	

	/// <summary>
	/// Loads the index of the information in string.
	/// Modify: if the world is updated
	/// </summary>
	protected void LoadInformationIndex(WorldData data){
		List<string> statusList = new List<string>();
		List<string> titleList = new List<string>();
		
		if(GamePrefs.GetCharacterClass().Equals(Player.CharacterClass.Conservationist)){
			titleList.Add ("Marten Population");
			statusList.Add (string.Format ("{0}", currentMartenCount));		
		}
		else if(GamePrefs.GetCharacterClass().Equals(Player.CharacterClass.Developer)){
			titleList.Add ("Population");
			statusList.Add (string.Format ("{0}", humanPopulation ));	
			titleList.Add ("Timber Count");
			statusList.Add (string.Format ("{0}", timberCount));
		}
		else {
			titleList.Add ("Timber Count");
			statusList.Add (string.Format ("{0}", timberCount));
		}
		InformationIndex = statusList.ToArray();
		InfoDescription = titleList.ToArray();
		
	}
	
	/// <summary>
	/// Determines whether this instance is mouse over.
	/// </summary>
	/// <returns>
	/// <c>true</c> if this instance is mouse over; otherwise, <c>false</c>.
	/// </returns>
	public override bool IsMouseOver(){
		if(visible && InfoBox.Contains(GUIHelpers.MouseToGUIPosition(Input.mousePosition))){
			return true;
		}
		else 
			return false;
	}
	
}
