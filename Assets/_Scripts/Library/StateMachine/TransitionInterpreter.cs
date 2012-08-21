using UnityEngine;
using System;
using System.Collections.Generic;

public abstract class Operation {
	public abstract bool	Parse( string[] opDesc );
	
	public abstract bool	Execute( TransitionInterpreter interpreter );
	
	public virtual void		UpdateInstructionIndex( TransitionInterpreter interpreter ) {
		++interpreter.InstructionIndex;
	}
}

public class DeclareBehaviorOperation : Operation {
	public static void	Register( OperationLibrary library ) {
		library.Register( "DeclareBehavior", delegate() { return new DeclareBehaviorOperation(); } );
	}
	
	public override bool	Parse( string[] opDesc ) {
		behaviorType = opDesc[ 1 ];
		behaviorName = opDesc[ 2 ];
		
		for( int ix = 3; ix < opDesc.Length; ++ix ) {
			behaviorArgs.Add( opDesc[ix] );
		}
		return true;
	}
	
	public override bool	Execute( TransitionInterpreter interpreter ) {
		try {
			interpreter.DeclareBehavior( behaviorType, behaviorName, behaviorArgs );
		} catch( Exception e ) {
			Debug.LogError( e.Message );
			return false;
		}
		return true;
	}
	
	protected string		behaviorType;
	protected string		behaviorName;
	protected List<string>	behaviorArgs = new List<string>();
}

public class PushBehaviorOperation : Operation {
	public static void	Register( OperationLibrary library ) {
		library.Register( "PushBehavior", delegate() { return new PushBehaviorOperation(); } );
	}
	
	public override bool	Parse( string[] opDesc ) {
		behaviorName = opDesc[ 1 ];
		return true;
	}
	
	public override bool	Execute( TransitionInterpreter interpreter ) {
		try {
			interpreter.DataStack.Push( interpreter.DereferenceBehavior(behaviorName) );
		} catch( Exception e ) {
			Debug.LogError( e.Message );
			return false;
		}
		return true;
	}
	
	protected string	behaviorName;
}

public class NotOperation : Operation {
	public static void	Register( OperationLibrary library ) {
		library.Register( "Not", delegate() { return new NotOperation(); } );
	}
	
	public override bool	Parse( string[] opDesc ) {
		return true;
	}
	
	public override bool	Execute( TransitionInterpreter interpreter ) {
		bool	res = interpreter.DataStack.Pop();
		interpreter.DataStack.Push( !res );
		return true;
	}
}

public class AndOperation : Operation {
	public static void	Register( OperationLibrary library ) {
		library.Register( "And", delegate() { return new AndOperation(); } );
	}
	
	public override bool	Parse( string[] opDesc ) {
		return true;
	}
	
	public override bool	Execute( TransitionInterpreter interpreter ) {
		bool	lhs = interpreter.DataStack.Pop();
		bool	rhs = interpreter.DataStack.Pop();
		bool 	res = lhs && rhs;
		interpreter.DataStack.Push( res );
		return true;
	}
}

public class OrOperation : Operation {
	public static void	Register( OperationLibrary library ) {
		library.Register( "Or", delegate() { return new OrOperation(); } );
	}
	
	public override bool	Parse( string[] opDesc ) {
		return true;
	}
	
	public override bool	Execute( TransitionInterpreter interpreter ) {
		bool	lhs = interpreter.DataStack.Pop();
		bool	rhs = interpreter.DataStack.Pop();
		bool 	res = lhs || rhs;
		interpreter.DataStack.Push( res );
		return true;
	}
}

public class NoOpOperation : Operation {
	public static void	Register( OperationLibrary library ) {
		library.Register( "NoOp", delegate() { return new NoOpOperation(); } );
	}
	
	public override bool	Parse( string[] opDesc ) {
		return true;
	}
	
	public override bool	Execute( TransitionInterpreter interpreter ) {
		return true;
	}
	
	public override void	UpdateInstructionIndex( TransitionInterpreter interpreter ) {
		base.UpdateInstructionIndex( interpreter );
		throw DataStructureLibrary<InterpreterWaitFrame>.Instance.CheckOut();
	}
}

// Currently we are assuming that there is only one Jump and its the last instruction.  When this changes please look thru the Execute code
public class JumpOperation : Operation {
	public static void	Register( OperationLibrary library ) {
		library.Register( "Jump", delegate() { return new JumpOperation(); } );
	}
	
	public override bool	Parse( string[] opDesc ) {
		jumpDistance = int.Parse( opDesc[1] );
		return true;
	}
	
	public override bool	Execute( TransitionInterpreter interpreter ) {
		return true;
	}
	
	public override void	UpdateInstructionIndex( TransitionInterpreter interpreter ) {
		interpreter.InstructionIndex += jumpDistance;
		throw DataStructureLibrary<InterpreterWaitFrame>.Instance.CheckOut();
	}
	
	protected int	jumpDistance = 0;
}

public class InterpreterWaitFrame : Exception {
	
}

public class OperationLibrary {
	public static readonly OperationLibrary	Instance = new OperationLibrary();
	
	public OperationLibrary() {
		RegisterBehaviors();
	}
	
	protected void	RegisterBehaviors() {
		DeclareBehaviorOperation.Register( this );
		PushBehaviorOperation.Register( this );
		OrOperation.Register( this );
		AndOperation.Register( this );
		NotOperation.Register( this );
		JumpOperation.Register( this );
		NoOpOperation.Register( this );
	}
	
	public delegate	Operation	AllocOperationDelegate();
	
	public void	Register( string name, AllocOperationDelegate alloc ) {
		allocators[ name ] = alloc;
	}
	
	public Operation	Allocate( string name ) {
		return allocators[ name ]();
	}
	
	protected	Dictionary<string, AllocOperationDelegate>	allocators = new Dictionary<string, AllocOperationDelegate>();
}

public class TransitionInterpreter {
	public TransitionInterpreter() {
	}

	public bool	InitFromInstructions( List<string> lines ) {
		return InitFromInstructions( lines.ToArray() );
	}
	
	public bool	InitFromInstructions( string[] lines ) {
		string[]	instruction = null;
		Operation	operation = null;

		foreach( string line in lines ) {
			instruction = line.Split( ' ', '\t' );
			operation = OperationLibrary.Instance.Allocate( instruction[0] );
			operation.Parse( instruction );
			
			operations.Add( operation );
		}
		return true;
	}

	public bool	Execute() {
		Operation op = null;
		
		try {
			while( InstructionIndex < operations.Count ) {
				op = operations[ InstructionIndex ];
				if( !op.Execute(this) ) {
					return false;
				}
			
				op.UpdateInstructionIndex( this );
			}
		}
		catch( InterpreterWaitFrame e ) {
			DataStructureLibrary<InterpreterWaitFrame>.Instance.Return( e );
			return true;
		}
		catch( Exception ) {
			return false;
		}

		return true;
	}

	protected List<Operation>	operations = new List<Operation>();
	public Stack<bool>			DataStack = new Stack<bool>();
	
	public delegate void		DeclareBehaviorDelegate( string type, string name, List<string> args );
	public DeclareBehaviorDelegate	DeclareBehavior;
	
	public delegate bool		DereferenceBehaviorDelegate( string name );
	public DereferenceBehaviorDelegate	DereferenceBehavior;
	
	internal int				InstructionIndex;
}