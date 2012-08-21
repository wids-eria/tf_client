using UnityEngine;
using System.Collections;

/// <summary>
/// Nturn GUI.
/// </summary>
public class NturnUI : GUIObject
{

	/// <summary>
	/// The nturn box.
	/// </summary>
	[HideInInspector]
	public Rect NturnBox = new Rect();
	
	[HideInInspector]
	public bool isTurnOver {get{return m_isTurnOver;} private set{
		m_isTurnOver = value;
		Messenger<bool>.Broadcast("TurnChanged", m_isTurnOver);}}
	
	/// <summary>
	/// Timer variables with mock data
	/// </summary>
	int turn_day, turn_hour, turn_minute, turn_second, timer = 1;
	TimeState m_TimeState;
	
	protected WebCoroutine PutTurnOverCoroutine;
	protected WebCoroutine StartNewTurnCoroutine;
	protected WebCoroutine GetTurnTimeLeftCoroutine;
	

	#region Flags
	/// <summary>
	/// flag for managing coroutine
	/// </summary>
	private bool TimeIsRunning = false;
	/// <summary>
	/// The is button pressed.
	/// </summary>
	protected bool isButtonPressed = false;
	/// <summary>
	/// The is after put.
	/// </summary>
	protected bool isAfterPut = false;
	/// <summary>
	/// The is after processing.
	/// </summary>
	protected bool isAfterProcessing = false;
	/// <summary>
	/// The is putting.
	/// </summary>
	protected bool isPutting = false;
	#endregion
	
	/// <summary>
	/// The is end turn.
	/// </summary>
	private bool m_isTurnOver = false;
	
	/// <summary>
	/// The delay for completing www request to the server for StartNewTurn()
	/// </summary>
	protected bool ProcessIsRunning = false;
	
	/// <summary>
	/// Initializes a new instance of the <see cref="NturnUI"/> class.
	/// </summary>
	public NturnUI(){
		NturnBox.x = 170f + 5f;
		NturnBox.y = Screen.height - 85f - 80f;
		NturnBox.width = 220f;
		NturnBox.height = 75f;
		
		PutTurnOverCoroutine = new WebCoroutine(PutTurnOver);
		StartNewTurnCoroutine = new WebCoroutine(startNewTurn );
		GetTurnTimeLeftCoroutine = new WebCoroutine(GetTurnTimeLeft);

	}
	
	/// <summary>
	/// Releases unmanaged resources and performs other cleanup operations before the <see cref="NturnUI"/> is reclaimed by
	/// garbage collection.
	/// </summary>
	~NturnUI(){}
	
	/// <summary>
	/// Display the nturn.
	/// </summary>
	public override void Draw(){

		GUILayout.BeginArea(NturnBox, Styles.roundDarkBox);
		{
			//Draw Turn Timer
			GUILayout.Label(TurnTimerGenerator(), Styles.smallTextLight);
			Styles.DrawLine(
				GUIStyles.LineDirection.Horizontal,
				GUIStyles.LineColor.Highlighted
			);
			
			//Draw Turn Over toggle
			isButtonPressed = GUILayout.Toggle(isTurnOver, string.Format("{0}", isTurnOver?"Wait for your turn...":"End Turn"), Styles.smallButton);
			if(isButtonPressed==true){
				isTurnOver = true;
				timer = 0;
			}
			else if(isButtonPressed==false && isAfterPut==false){
				isTurnOver = false;
			}
		}
		GUILayout.EndArea();
		
		if(isTurnOver==true && isAfterPut==false && isPutting==false){;
			
			PutTurnOverCoroutine.Start(GameGUIManager.use);
			//StartCoroutine(PutTurnOver());
			//need something to stop get into this loop
		}
		
		if(isAfterPut==true && ProcessIsRunning==false){
			//StartCoroutine(startNewTurn());
			StartNewTurnCoroutine.Start(GameGUIManager.use);
			if(isAfterProcessing == true){
				//using monobehaviour from GameGUIManager and start coroutine
				GameGUIManager.use.StartCoroutine( GameManager.economyController.ConcludePutAfterTurn(null) );
				//reset all the flags to default
				isButtonPressed = false;
				isTurnOver = false;
				isAfterPut = false;
				isPutting = false;
			}
			//need something that prevent into this loop
			isAfterProcessing = false;
		}

	}
	
	/// <summary>
	/// Sends a turn over message to the server (coroutine)
	/// </summary>
	/// <returns>
	/// what to do
	/// </returns>
	protected IEnumerator PutTurnOver(AWebCoroutine a)
	{
		WWW www = WWWX.Put(WebRequests.urlPutPlayerTurnDone,WebRequests.authenticatedParameters);
		while (!www.isDone) {
			isPutting = true;
			yield return www;
		}
		isAfterPut = true;
		Messenger.Broadcast("TurnOver");
		Debug.Log("Put Turn over message to the server");
		isPutting = false;
		yield break;
	}
	
	/// <summary>
	/// Starts the new turn.
	/// </summary>
	/// <returns>
	/// The new turn.
	/// </returns>
	protected IEnumerator startNewTurn(AWebCoroutine a){
		WWW www = WWWX.Get(WebRequests.urlGetTurnState,WebRequests.authenticatedParameters);
			while (!www.isDone) {
				ProcessIsRunning = true;
				yield return www;
			}
			
			yield return new WaitForSeconds(5);	
		
		m_TimeState = JSONDecoder.Decode<TimeState>(www.text);
		timer = m_TimeState.time_left;
		string state = m_TimeState.state;
		
		if(state == "processing"){
			//make sure that the game had passed through "processing phase"
			isPutting = true;
		}
		
		if(state == "playing" && isPutting == true){
			isAfterProcessing = true;
		}

		ProcessIsRunning = false;
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
	protected string TurnTimerGenerator()
	{
		if(!isTurnOver)
		{
			//Mock data from server (673days,14hr, 26min, 55sec)
			//timer = 58199215;
			if(!TimeIsRunning)
			{
				//StartCoroutine(GetTurnTimeLeft());
				GetTurnTimeLeftCoroutine.Start(GameGUIManager.use);
			}

			if(timer < 0){
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
		if(data<0){
			turn_second = 0;
			return;
		}
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
	protected IEnumerator GetTurnTimeLeft(AWebCoroutine a)
	{
		WWW www = WWWX.Get(WebRequests.urlGetTurnState,WebRequests.authenticatedParameters);
		while (!www.isDone) {
			TimeIsRunning = true;
			yield return www;
		}
		yield return new WaitForSeconds(5);

		m_TimeState = JSONDecoder.Decode<TimeState>(www.text);
		timer = m_TimeState.time_left;
		TimeIsRunning = false; 
		if(timer <= 0){
				isTurnOver = true;
		}
		
		yield break;
	}
	
	/// <summary>
	/// Determines whether this instance is mouse over.
	/// </summary>
	/// <returns>
	/// <c>true</c> if this instance is mouse over; otherwise, <c>false</c>.
	/// </returns>
	public override bool IsMouseOver(){
		if (NturnBox.Contains(GUIHelpers.MouseToGUIPosition( Input.mousePosition))){
			return true;
		}
		else
			return false;
	}
}

