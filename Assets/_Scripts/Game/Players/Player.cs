using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Player.
/// </summary>
public class Player : MonoBehaviour
{
	/// <summary>
	/// Character class.
	/// </summary>
	/// 
	[System.Flags]
	public enum CharacterClass {
		TheAlmighty,
		Conservationist,
		Developer,
		TimberCompany
	}
	
	
	
	/// <summary>
	/// The character class.
	/// </summary>
	public CharacterClass characterClass {
		get {
			return m_characterClass;
		}
		
		set {
			if( m_characterClass != value ) {
				GamePrefs.SetCharacterClass( value );
				m_characterClass = value;
			}
		}
	}
	/// <summary>
	/// The character class backing field.
	/// </summary>
	[SerializeField]
	private CharacterClass m_characterClass;
	
	/// <summary>
	/// accessor for the current player
	/// </summary>
	public static Player current { get { return GameManager.use.currentPlayer; } }
	
	/// <summary>
	/// The player's actions.
	/// </summary>
	public List<PlayerAction> actions = new List<PlayerAction>();
	
	/// <summary>
	/// Sets the data.
	/// </summary>
	/// <value>
	/// The data.
	/// </value>
	public PlayerData data {
		get {
			return m_data;
		}
		set {
			m_data = value;
			this.name = value.name;
			QuestManager.Instance.questPoints = value.quest_points;
		}
	}
	private PlayerData m_data;
	
	/// <summary>
	/// Gets the cash.
	/// </summary>
	/// <value>
	/// The cash.
	/// </value>
	public int balance { get { return data.balance;}}
	
	public int pendingBalance { get {return data.pending_balance;}}
	/// <summary>
	/// Gets or sets the quest points.
	/// </summary>
	/// <value>
	/// The quest points.
	/// </value>
	public int questPoints { get {return QuestManager.Instance.questPoints; } }
	
	/// <summary>
	/// Gets the identifier.
	/// </summary>
	/// <value>
	/// The identifier.
	/// </value>
	public int id { get {return data.id;} }
	
	/// <summary>
	/// Return the action of the specified type associated with this player
	/// </summary>
	/// <param name="actionType">
	/// A <see cref="System.Type"/>
	/// </param>
	/// <returns>
	/// A <see cref="PlayerAction"/>
	/// </returns>
	public PlayerAction GetActionByType(System.Type actionType)
	{
		foreach(PlayerAction a in actions) {
			if (a.GetType() == actionType) return a;
		}
		return null;
	}
	
	/// <summary>
	/// The permission bitmask by action type.
	/// </summary>
	public Dictionary<System.Type, int> permissionBitmaskByActionType = new Dictionary<System.Type, int>();
	
	protected static Dictionary<Player.CharacterClass, string>	localClassToServerClassName = null;
	protected static Dictionary<string, Player.CharacterClass>	serverClassNameToLocalClass = null;
	
	public static string	GetCharacterClassName( Player.CharacterClass c ) {
		if( localClassToServerClassName == null ) {
			localClassToServerClassName = new Dictionary<Player.CharacterClass, string>();
			localClassToServerClassName.Add( Player.CharacterClass.Conservationist, "Conserver" );
			localClassToServerClassName.Add( Player.CharacterClass.TimberCompany, "Lumberjack" );
			localClassToServerClassName.Add( Player.CharacterClass.Developer, "Developer" );
		}
		
		return localClassToServerClassName[ c ];
	}
	
	public static Player.CharacterClass	GetCharacterClass( string name ) {
		if( serverClassNameToLocalClass == null ) {
			serverClassNameToLocalClass = new Dictionary<string, Player.CharacterClass>();
			serverClassNameToLocalClass.Add( "Conserver", Player.CharacterClass.Conservationist );
			serverClassNameToLocalClass.Add( "Lumberjack", Player.CharacterClass.TimberCompany );
			serverClassNameToLocalClass.Add( "Developer", Player.CharacterClass.Developer );
		}
		
		return serverClassNameToLocalClass[ name ];
	}
	
	/// <summary>
	/// Initialize
	/// </summary>
	void Awake()
	{
		m_characterClass = GamePrefs.GetCharacterClass();
		
		// instantiate action prefabs and population action list
		List<PlayerAction> actionInstances = new List<PlayerAction>();
		foreach (PlayerAction action in actions) {
			PlayerAction instance = Instantiate(action, transform.position, transform.rotation) as PlayerAction;
			instance.name = action.name;
			instance.enabled = false;
			instance.transform.parent = transform;
			actionInstances.Add(instance);
		}
		actions = actionInstances;
		
		// populate permissions mask dictionary
		for (int i=0; i<actions.Count; ++i) {
			permissionBitmaskByActionType.Add(actions[i].GetType(), i);
		}
		
		QuestManager.Instance.OnAwake();
	}
	
	[HideInInspector]
	public bool isActionRunning {
		get{
			foreach(PlayerAction pA in actions) {
				if (pA.isActionInProgress) return true;
			}
			return false;
		}
	}
}