/*
 * using UnityEngine;
using System.Collections;

/// <summary>
/// Nturn GUI.
/// </summary>
public class NturnUI : MonoBehaviour 
{
	
	/// <summary>
	/// The nturn box.
	/// </summary>
	[HideInInspector]
	public Rect NturnBox = new Rect();
	public bool isTurnOver = false;
	
	/// <summary>
	/// Timer variables with mock data
	/// </summary>
	int turn_day, turn_hour, turn_minute, turn_second, timer;
	TimeState m_TimeState;

	
	[HideInInspector]
	public float progress = 1f;
	
	/// <summary>
	/// flag for managing coroutine
	/// </summary>
	private bool TimeIsRunning = false;
	
	
	/// <summary>
	/// Gets the styles.
	/// </summary>
	/// <value>
	/// The styles.
	/// </value>
	private GUIStyles m_styles { get { return GameGUIManager.use.styles; } }
	
	/// <summary>
	/// The is end turn.
	/// </summary>
	private bool m_isTurnOver = false;
	
	/// <summary>
	/// Display the nturn.
	/// </summary>
	public void DisplayNturn(){

		GUILayout.BeginArea(NturnBox, GameGUIManager.use.styleBox);
		{
			//Draw Turn Timer
			GUILayout.Label(TurnTimerGenerator(), m_styles.smallTextLight);
			m_styles.DrawLine(
				GUIStyles.LineDirection.Horizontal,
				GUIStyles.LineColor.Highlighted
			);
			
			//Draw Turn Over toggle
			m_isTurnOver = GUILayout.Toggle(isTurnOver, string.Format("{0}", isTurnOver?"Wait for your turn...":"End Turn"), m_styles.smallButton);
		}
		GUILayout.EndArea();
		
		//Check to see if we finished our turn
		if (!isTurnOver && m_isTurnOver) {
			//Send out the turn over Json (also sets the turn over properties)
			StartCoroutine(PutTurnOver());
			Button_pressed = true;
			//Messenger<Player.CharacterClass>.Broadcast ("EndTurn", GameManager.use.currentPlayer.characterClass);
		}
		
		if(!TimeIsRunning2){
			//start new turn
			StartCoroutine (startNewTurn());
			//update currency
			StartCoroutine(GameManager.economyController.ConcludePut());
			TimeIsRunning = true;
		}
		
		
	}
	
	/// <summary>
	/// Sends a turn over message to the server (coroutine)
	/// </summary>
	/// <returns>
	/// what to do
	/// </returns>
	public IEnumerator PutTurnOver()
	{
		WWW www = WWWX.Put(WebRequests.urlPutPlayerTurnDone,WebRequests.authenticatedParameters);
		while (!www.isDone) {
			yield return www;
		}
		
		Messenger.Broadcast("TurnOver");
		
		isTurnOver = true;
		m_isTurnOver = false;
		Debug.Log("Put Turn over message to the server");
		yield break;
	}
	
	/// <summary>
	/// The delay for completing www request to the server for StartNewTurn()
	/// </summary>
	protected bool TimeIsRunning2 = false;
	
	/// <summary>
	/// Starts the new turn.
	/// </summary>
	/// <returns>
	/// The new turn.
	/// </returns>
	public IEnumerator startNewTurn(){
		WWW www = WWWX.Get(WebRequests.urlGetTurnState,WebRequests.authenticatedParameters);
			while (!www.isDone) {
				TimeIsRunning2 = true;
				yield return www;
			}
			
			yield return new WaitForSeconds(5);	
		
			
			isTurnOver = false;
			m_isTurnOver = false;
			
			if(Button_pressed){
			isTurnOver = true;
			m_isTurnOver = false;
			}
			
			if(JSONDecoder.DecodeValue<TimeState>(www.text).state.Equals("processing")){Button_pressed = false;}
		
			TimeIsRunning2 = false;
			Debug.Log ("New Turn");
		
		TimeIsRunning = false;
		
		yield break;		
	}

	/// <summary>
	/// Generate time based on the timer on the server
	/// </summary>
	/// <returns>
	/// The time in string format
	/// </returns>
	public string TurnTimerGenerator()
	{
		if(!isTurnOver)
		{
			//Mock data from server (673days,14hr, 26min, 55sec)
			//timer = 58199215;
			if(!TimeIsRunning)
			{
				StartCoroutine(GetTurnTimeLeft());
			}

			if(timer<=0){
				isTurnOver = true;
			}
			TurnTimerConverter(timer);
		}
		
		
		if(turn_day == 0)
		{
			if(turn_hour == 0)
			{
				if(turn_minute == 0)
				{
					if(turn_second == 0)
					{
						return "";
					}
					else{
						return string.Format("{0} seconds", turn_second);
					}
				}
				else
				{
					return string.Format("{0} minutes, {1} seconds", turn_minute,turn_second);
				}
			}
			else
			{
				return string.Format("{0} hours, {1} minutes, {2} seconds", turn_hour,turn_minute,turn_second);
			}
		}
		else
		{
			return string.Format("{0} days, {1} hours, {2} minutes, {3} seconds", turn_day,turn_hour,turn_minute,turn_second);
		}
	}
	
	/// <summary>
	/// Converts time from server into int
	/// </summary>
	/// <param name='data'>
	/// Time from server
	/// </param>
	public void TurnTimerConverter(double data){
		double dataContainer = data;
		turn_second = (int)dataContainer % 60;
		
		dataContainer /= 60;
		turn_minute = (int)dataContainer%60;
		
		dataContainer /= 60;
		turn_hour = (int)dataContainer%24;
		
		dataContainer /= 24;
		turn_day = (int)dataContainer;
		
	}
	//overload
	public void TurnTimerConverter(int data){
		TurnTimerConverter((double)data);
	}
	
	/// <summary>
	/// Time state (format from server).
	/// </summary>
	public struct TimeState {
		public int time_left;
		public string state;
	}
	
	/// <summary>
	/// Gets the turn time left.
	/// </summary>
	/// <returns>
	/// The turn time left.
	/// </returns>
	public IEnumerator GetTurnTimeLeft()
	{
		WWW www = WWWX.Get(WebRequests.urlGetTurnState,WebRequests.authenticatedParameters);
		while (!www.isDone) {
			TimeIsRunning = true;
			yield return www;
		}
		yield return new WaitForSeconds(5);
		//timer = JSONDecoder.DecodeValue<TimeLeft>(www.text).time_left;
		m_TimeState = JSONDecoder.DecodeValue<TimeState>(www.text);
		timer = m_TimeState.time_left;
		TimeIsRunning = false; 
		//clamped the timer if the timer is less than zero
		if(timer<=0){
				m_isTurnOver = true;
		}
		
		yield break;
	}
	
	/// <summary>
	/// Status of end turn button being pressed
	/// </summary>
	protected bool Button_pressed = false;
	
}
*/

