using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Quest GUI.
/// </summary>
public class QuestGUI : GUIObject
{
	/// <summary>
	/// The rect for the quest selector box.
	/// </summary>
	[HideInInspector]
	public Rect selectorBox = new Rect();
	/// <summary>
	/// The rect for the quest details flyout.
	/// </summary>
	[HideInInspector]
	public Rect detailsBox = new Rect();
	
	/// <summary>
	/// The index of the currently selected quest.
	/// </summary>
	private int m_currentlySelectedQuestIndex {
		get { return m_currentlySelectedQuestIndexBackingField; }
		set {
			Quest[] quests = QuestManager.Instance.GetViewableQuests();
			if (quests == null || quests.Length == 0) {
				m_currentlySelectedQuestIndexBackingField = 0;
			}
			else {
				m_currentlySelectedQuestIndexBackingField = Mathf.Min(value, quests.Length-1);
			}
		}
	}
	/// <summary>
	/// The currently selected quest index backing field.
	/// </summary>
	private int m_currentlySelectedQuestIndexBackingField = 0;
	/// <summary>
	/// Gets the currently selected quest.
	/// </summary>
	/// <value>
	/// The currently selected quest.
	/// </value>
	private Quest m_currentlySelectedQuest {
		get {
			Quest[] quests = QuestManager.Instance.GetViewableQuests();
			if (quests == null || quests.Length == 0) {
				return null;
			}
			return QuestManager.Instance.GetViewableQuests()[m_currentlySelectedQuestIndex];
		}
	}
	
	/// <summary>
	/// Gets or sets the index of the currently selected objective.
	/// </summary>
	/// <value>
	/// The index of the currently selected objective.
	/// </value>
	private int m_currentlySelectedObjectiveIndex {
		get { return m_currentlySelectedObjectiveIndexBackingField; }
		set {
			if (m_currentlySelectedQuest == null) {
				m_currentlySelectedObjectiveIndexBackingField = 0;
			}
			else {
				m_currentlySelectedObjectiveIndexBackingField = value;
			}
		}
	}
	/// <summary>
	/// The currently selected objective index backing field.
	/// </summary>
	private int m_currentlySelectedObjectiveIndexBackingField;

	/// <summary>
	/// Flag specifying whether the quest details flyout is visible.
	/// </summary>
	public bool isDetailsflyoutVisible {get; private set; }

	/// <summary>
	/// The flag to invoke game dock and close quest GUI.
	/// </summary>
	private bool isInvokeGameDock = false;
	
	
	/// <summary>
	/// The quest selection scroll position.
	/// </summary>
	private Vector2 m_questSelectionScrollPosition = Vector2.zero;
	/// <summary>
	/// The objectives box scroll position.
	/// </summary>
	private Vector2 m_objectivesBoxScrollPosition = Vector2.zero;
	/// <summary>
	/// The quest description scroll position.
	/// </summary>
	private Vector2 m_questDescriptionScrollPosition = Vector2.zero;
	
	public static GUITextures texture = GameGUIManager.use.iconTexture;
	/// <summary>
	/// The progress bar fill tex.
	/// </summary>
    public Texture2D progressBarFillTex = texture.ProgressBarFillTex;
	/// <summary>
	/// The incomplete quest icon.
	/// </summary>
	public Texture2D incompleteIcon = texture.IncompleteIcon;
	/// <summary>
	/// The complete quest icon.
	/// </summary>
	public Texture2D completeIcon = texture.CompleteIcon;
	/// <summary>
	/// Gets the size of the m_completion icon.
	/// </summary>
	/// <value>
	/// The size of the m_completion icon.
	/// </value>
	private float m_completionIconSize {
		get {
			return 2f + // no idea why I need to add the extra 2...
				Styles.smallButton.padding.top +
				Styles.smallButton.padding.bottom +
				GUIHelpers.GetFontSizeFromName(Styles.smallButton.font); // use this because not currently using dynamic fonts
		}
	}
	
	/// <summary>
	/// Initializes a new instance of the <see cref="QuestGUI"/> class.
	/// </summary>
	public QuestGUI(){
		Messenger.AddListener(GameDockGUI.kOnQuestSelector, VisibleEnabler);
		selectorBox.x = Styles.roundDarkBox.margin.left;
		selectorBox.y = Styles.roundDarkBox.margin.top;
		selectorBox.width = 180f;
		selectorBox.height = Screen.height/2;
		detailsBox.x = selectorBox.x + selectorBox.width;
		detailsBox.y = selectorBox.y;
		detailsBox.width = selectorBox.width * 1.8f;
		detailsBox.height = selectorBox.height*0.9f;
		this.visible = false;
	}
	
	/// <summary>
	/// Releases unmanaged resources and performs other cleanup operations before the <see cref="QuestGUI"/> is reclaimed
	/// by garbage collection.
	/// </summary>
	~QuestGUI(){
		Messenger.RemoveListener(GameDockGUI.kOnQuestSelector, VisibleEnabler);
	}
	
	/// <summary>
	/// Draw this instance.
	/// </summary>
	public override void Draw(){
		DisplayQuestSelectorBox();
		DisplayQuestDetails();
	}
		
	/// <summary>
	/// Displays the quest box.
	/// </summary>
	public void DisplayQuestSelectorBox() {

		GUILayout.BeginArea(selectorBox, Styles.roundDarkBox); {
			// display header
			GUILayout.Label(string.Format(
				"Quests\t\t\t\t[{0}/20]QP",GameManager.use.currentPlayer.questPoints
				), Styles.largeTextLight);
			Styles.DrawLine(
				GUIStyles.LineDirection.Horizontal,
				GUIStyles.LineColor.Highlighted
			);
			
			m_questSelectionScrollPosition = GUILayout.BeginScrollView(
				m_questSelectionScrollPosition, false, true
			); {
				//Merge completed and unlocked Quest
				Quest[] quests = QuestManager.Instance.GetViewableQuests();
				
				for (int i=0; i<quests.Length; ++i) {
					GUILayout.BeginHorizontal(); {
						// completion icon
						GUILayout.Box(
							quests[i].IsComplete?completeIcon:incompleteIcon,
							Styles.empty4,
							GUILayout.Width(m_completionIconSize), GUILayout.Height(m_completionIconSize)
						);

						// selector button
						if (GUILayout.Button(
							quests[i].Name,
							m_currentlySelectedQuestIndex==i?Styles.smallButtonFocused:Styles.smallButton)
						) {
							m_currentlySelectedQuestIndex = i;
							//when new Quest selected, quest details activated and the details index being flushed
							isDetailsflyoutVisible = true;
							m_currentlySelectedObjectiveIndex = 0;
						}
							
					} GUILayout.EndHorizontal();
				}
			} GUILayout.EndScrollView();
			isDetailsflyoutVisible = GUILayout.Toggle(
				isDetailsflyoutVisible, string.Format(
					"{0} Quest Details", isDetailsflyoutVisible?"Hide":"Show"
				),
				Styles.smallButton
			);
			
			isInvokeGameDock = GUILayout.Toggle(
				isInvokeGameDock, string.Format(
					"{0} Game Dock", isInvokeGameDock?"Hide":"Show"
				),
				Styles.smallButton
			);
			if(isInvokeGameDock){
				Messenger.Broadcast(GameDockGUI.kOnGameDock);
				this.visible = false;
				isDetailsflyoutVisible = false;
				isInvokeGameDock = false;
			}
			
		} GUILayout.EndArea();
	}
	
	/// <summary>
	/// The quest progress bar background rect.
	/// </summary>
	private Rect m_progressBarBackground;
	
	/// <summary>
	/// Displays the quest details.
	/// </summary>
	public void DisplayQuestDetails()
	{
		
		// early out if not visible
		if (!isDetailsflyoutVisible) {
			return;
		}
		
	
		GUILayout.BeginArea(detailsBox, Styles.roundDarkBox); {
			if (m_currentlySelectedQuest == null) {
				GUILayout.Label("No quest selected.", Styles.largeTextLight);
			}
			else {
				// title
				GUILayout.BeginHorizontal(); {
					GUILayout.Label(m_currentlySelectedQuest.Name, Styles.largeTextLight);
					GUILayout.FlexibleSpace();
					GUILayout.Label(string.Format("({0} QP)", m_currentlySelectedQuest.PointValue), Styles.largeTextHighlighted);
				} GUILayout.EndHorizontal();
				Styles.DrawLine(
					GUIStyles.LineDirection.Horizontal,
					GUIStyles.LineColor.Highlighted
				);
				// progress
				GUILayout.BeginHorizontal(); {
					GUILayout.Label (string.Format("Progress: {0:0%}", m_currentlySelectedQuest.Progress), Styles.mediumTextLight);
					GUILayout.Box("", Styles.roundDarkBox, GUILayout.Width(200f), GUILayout.Height(20f));
					if (Event.current.type == EventType.Repaint) {
						m_progressBarBackground = GUILayoutUtility.GetLastRect();
					}
					Rect fill = m_progressBarBackground;
					fill.x += Styles.roundDarkBox.padding.left;
					fill.y += Styles.roundDarkBox.padding.top;
					fill.height -= Styles.roundDarkBox.padding.top + Styles.roundDarkBox.padding.bottom;
					fill.width = (fill.width-Styles.roundDarkBox.padding.top-Styles.roundDarkBox.padding.bottom);
					fill.width *= m_currentlySelectedQuest.Progress;
					GUI.DrawTexture(fill, progressBarFillTex);
				} GUILayout.EndHorizontal();
				// description
				GUILayout.BeginHorizontal(); {
					GUILayout.Label ("Description: ", Styles.mediumTextLight);
					m_questDescriptionScrollPosition = GUILayout.BeginScrollView(
						m_questDescriptionScrollPosition,
						GUILayout.Width(m_progressBarBackground.width)
					); {
						GUILayout.Label(m_currentlySelectedQuest.Description, Styles.mediumTextLight);
					} GUILayout.EndScrollView();
				} GUILayout.EndHorizontal();
				// objectives
				Styles.DrawLine(
					GUIStyles.LineDirection.Horizontal,
					GUIStyles.LineColor.Medium
				);
				GUILayout.Label("Objectives:", Styles.mediumTextLight);
				System.Collections.Generic.List<IObjective> objectives = m_currentlySelectedQuest.GetObjectives();
				GUILayout.BeginHorizontal(); {
					m_objectivesBoxScrollPosition = GUILayout.BeginScrollView(
						m_objectivesBoxScrollPosition,
						false, true,
						GUILayout.Width(detailsBox.width*0.5f)
					); {
						for (int i=0; i<objectives.Count; ++i) {
							GUILayout.BeginHorizontal(); {
								// completion icon
								GUILayout.Box(
									objectives[i].IsComplete()?completeIcon:incompleteIcon,
									Styles.empty4,
									GUILayout.Width(m_completionIconSize), GUILayout.Height(m_completionIconSize)
								);
								// selector button
								if (GUILayout.Button(
									objectives[i].Name,
									m_currentlySelectedObjectiveIndex==i?Styles.smallButtonFocused:Styles.smallButton)
								) {
									m_currentlySelectedObjectiveIndex = i;
								}
							} GUILayout.EndHorizontal();
						}
					} GUILayout.EndScrollView();
					// objective description
					GUILayout.Label(objectives[m_currentlySelectedObjectiveIndex].Description, Styles.mediumTextLight);
				} GUILayout.EndHorizontal();
			}
		}
		GUILayout.EndArea();
	}
	
	/// <summary>
	/// Determines whether this instance is mouse over.
	/// </summary>
	/// <returns>
	/// <c>true</c> if this instance is mouse over; otherwise, <c>false</c>.
	/// </returns>
	public override bool IsMouseOver(){
		if (visible && selectorBox.Contains(GUIHelpers.MouseToGUIPosition( Input.mousePosition))){
			return true;
		}
		
		if (isDetailsflyoutVisible && detailsBox.Contains(GUIHelpers.MouseToGUIPosition(Input.mousePosition))){
			return true;
		}
		
		return false;
	}
}