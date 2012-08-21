using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CommandController {
	
	static List<CommandBase> commandsRunning = new List<CommandBase>();
	
	public static void Run(CommandBase command, IEnumerator routine) {
		
		if (commandsRunning.Contains(command)) return;
		
		commandsRunning.Add(command);
		
		// ******** THIS MAY BE A STUPID WAY TO DO THIS ********
		
		GameManager.use.StartCoroutine(AsyncRun(command, routine));
	}
	
	public static bool IsRunning(CommandBase command) {
		return commandsRunning.Contains(command);
	}
	
	protected static IEnumerator AsyncRun(CommandBase command, IEnumerator routine) {
		yield return routine.Current;
		while (routine.MoveNext()) {
			yield return routine.Current;
		}
		commandsRunning.Remove(command);
		Debug.Log(string.Format("Command Controller finished running command `{0}'.",command.ToString()));
		yield break;
	}
}


public abstract class CommandBase {
	
	public bool isRunning { get { return CommandController.IsRunning(this); } }
	
	protected virtual IEnumerator Execute() { 
		yield break;
	}
	public virtual void Run() {
		CommandController.Run(this, Execute());
	}
}

public class CommandQueue : CommandBase {
	protected Queue<CommandBase> commands = new Queue<CommandBase>();
	protected CommandBase currentCommand;
	public bool autoRun = false;
	
	public CommandQueue(params CommandBase[] initialCommands) : this (false,initialCommands) {
	}
	public CommandQueue(bool autoRun) {
		this.autoRun = autoRun;
	}
	public CommandQueue(bool autoRun, params CommandBase[] initialCommands) {
		this.autoRun = autoRun;
		Enqueue(initialCommands);
	}
	protected override IEnumerator Execute() {
		currentCommand = commands.Dequeue();
		while (commands.Count > 0 || currentCommand != null) {
			currentCommand.Run();
			while(currentCommand.isRunning) {
				yield return 0;
			}
			currentCommand = null;
			if (commands.Count > 0) {
				currentCommand = commands.Dequeue();
			}
		}
		yield break;
	}
	public void Enqueue(params CommandBase[] commandsToAdd) {
		foreach (CommandBase command in commandsToAdd) {
			commands.Enqueue(command);
		}
		if (autoRun && !isRunning) {
			Run();
		}
	}
}

public class CommandList : CommandBase {
	protected List<CommandBase> commands = new List<CommandBase>();
	
	public CommandList(params CommandBase[] initialCommands) {
		foreach (CommandBase command in initialCommands) {
			commands.Add(command);
		}
	}
	
	protected override IEnumerator Execute()
	{
		foreach(CommandBase command in commands) {
			command.Run();
		}
		bool allDone = false;
		while (!allDone) {
			allDone = true;
			foreach(CommandBase command in commands) {
				if (command.isRunning) {
					allDone = false; break;
				}
			}
			yield return 0;
		}
		yield break;
	}
	
	public void Add(params CommandBase[] commandsToAdd) {
		foreach (CommandBase command in commandsToAdd) {
			commands.Add(command);
		}
	}
}

public class CommandCoroutine : CommandBase {
	
	protected IEnumerator coroutine;
	public CommandCoroutine() {
		this.coroutine = Execute();
	}
	public CommandCoroutine(IEnumerator coroutine) {
		this.coroutine = coroutine;
	}
	public override void Run() {
		CommandController.Run(this, coroutine);
	}
}

public class CommandWait : CommandCoroutine {
	protected float time;
	public CommandWait(float time) : base() {
		this.time = time;
	}
	protected override IEnumerator Execute() {
		yield return new WaitForSeconds(time);
		yield break;
	}
}



/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * 
 * 				MOVEMENT COMMAND BASE CLASSES
 * 
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */ 

public class MoveSettings<TStore,TVal> {
	
	public TStore obj;
	public TVal valA;
	public TVal valB;
	public float time = 1.0f;
	public AnimationCurve curve = AnimationCurve.Linear(0,0,1,1);
	
	public MoveSettings() {
	}
	public MoveSettings(TStore obj, TVal pointA, TVal pointB) {
		this.obj = obj;
		this.valA = pointA;
		this.valB = pointB;
	}
	public MoveSettings(TStore obj, TVal pointA, TVal pointB, float time) : this(obj,pointA,pointB) {
		this.time = time;
	}
	public MoveSettings(TStore obj, TVal pointA, TVal pointB, float time, AnimationCurve curve) : this(obj,pointA,pointB,time) {
		this.curve = curve;
	}
}

public abstract class Move<TStore,TVal> : CommandCoroutine
{ 
	protected MoveSettings<TStore,TVal> settings = new MoveSettings<TStore, TVal>();
	
	private TVal 			m_A 			{ get { return settings.valA; } }
	private TVal			m_B 			{ get { return settings.valB; } }
	private TStore 			m_obj 			{ get { return settings.obj; } }
	private AnimationCurve 	m_curve 		{ get { return settings.curve; } }
	private float 			m_animationTime { get { return settings.time; } }
	
	protected Func<TVal,TVal,float,TVal> 	Lerp;
	protected Action<TStore,TVal> 			Set;
	
	public Move() {
		coroutine = Execute();
	}
	
	protected override IEnumerator Execute () {
		float elapsed = 0f;
		while (elapsed < m_animationTime) {
			elapsed += Time.deltaTime; 
			Set(m_obj,Lerp(m_A, m_B, m_curve.Evaluate(elapsed/m_animationTime)));
			yield return 0;
		}
		Set(m_obj,m_B);
		yield break;
	}
}

