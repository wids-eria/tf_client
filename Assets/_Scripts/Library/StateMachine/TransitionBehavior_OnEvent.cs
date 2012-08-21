using UnityEngine;
using System;
using System.Collections.Generic;

public delegate void    TransitionEventDelegate();
public delegate void    TransitionEventDelegate<Parm1Type>( Parm1Type parm1 );
public delegate void    TransitionEventDelegate<Parm1Type, Parm2Type>( Parm1Type parm1, Parm2Type parm2 );
public delegate void    TransitionEventDelegate<Parm1Type, Parm2Type, Parm3Type>( Parm1Type parm1, Parm2Type parm2, Parm3Type parm3 );

public abstract class ITransitionBehavior_OnEvent<OwnerType> : TransitionBehavior<OwnerType> where OwnerType : class {
	public ITransitionBehavior_OnEvent( string name ) : base( name ) {
	}
}

public class TransitionBehavior_OnEvent<OwnerType> : ITransitionBehavior_OnEvent<OwnerType> where OwnerType : class {
    public TransitionBehavior_OnEvent( string name ) : base(name) {
    }
	
	public TransitionBehavior_OnEvent( string name, ref TransitionEventDelegate d ) : base(name) {
		AttachHandler( ref d );
    }
	
	public void	AttachHandler( ref TransitionEventDelegate d ) {
		d += OnEvent;
	}
	
    public override void StateEntered( OwnerType self ) {
        base.StateEntered( self );
		
		active = true;
		triggered = false;
    }
	
	public override void StateExited( OwnerType self ) {
		base.StateExited( self );
		
		active = false;
		triggered = false;
    }
	
	public override void SetStateParms( State<OwnerType> nextState ) {
	}

    protected virtual void  OnEvent() {
		//try {
			if( !active ) {
		//		throw new Exception( "OnEvent '" + Name + "' called while not active" );
				return;
			}
		//} catch( Exception e ) {
		//	Debug.LogError( e.ToString() );
		//}

		triggered = active;
    }

    public override bool Evaluate() {
        return triggered;
    }
	
    protected bool 	triggered = false;
	protected bool	active = false;
}

public class TransitionBehavior_OnEvent<OwnerType, Parm1Type> : ITransitionBehavior_OnEvent<OwnerType> where OwnerType : class {
    public TransitionBehavior_OnEvent( string name ) : base(name) {
    }
	
	public TransitionBehavior_OnEvent( string name, ref TransitionEventDelegate<Parm1Type> d ) : base(name) {
		AttachHandler( ref d );
    }
	
	public void	AttachHandler( ref TransitionEventDelegate<Parm1Type> d ) {
		d += OnEvent;
	}

    public override void StateEntered( OwnerType self ) {
        base.StateEntered( self );

		active = true;
		triggered = false;
    }
	
	public override void StateExited( OwnerType self ) {
		base.StateExited( self );

		active = false;
		triggered = false;
    }
	
	public override void SetStateParms( State<OwnerType> nextState ) {
		SetStateParms( (State<OwnerType, Parm1Type>)nextState );
	}

	public virtual void SetStateParms( State<OwnerType, Parm1Type> nextState ) {
		if( !active || !triggered ) {
			return;
		}

		nextState.Parm1 = Parm1;
	}

    protected virtual void  OnEvent( Parm1Type parm1 ) {
		//try {
			if( !active ) {
		//		throw new Exception( "OnEvent '" + Name + "' called while not active" );
				return;
			}
		//} catch( Exception e ) {
		//	Debug.LogError( e.ToString() );
		//}

		triggered = active;
		Parm1 = parm1;
    }

    public override bool Evaluate() {
        return triggered;
    }
	
    protected bool		triggered = false;
	protected bool		active = false;
	public Parm1Type	Parm1;
}

public class TransitionBehavior_OnEvent<OwnerType, Parm1Type, Parm2Type> : ITransitionBehavior_OnEvent<OwnerType> where OwnerType : class {
	public TransitionBehavior_OnEvent( string name ) : base(name) {
    }
	
	public TransitionBehavior_OnEvent( string name, ref TransitionEventDelegate<Parm1Type, Parm2Type> d ) : base(name) {
		AttachHandler( ref d );
    }
	
	public void	AttachHandler( ref TransitionEventDelegate<Parm1Type, Parm2Type> d ) {
		d += OnEvent;
	}

    public override void StateEntered( OwnerType self ) {
        base.StateEntered( self );

		active = true;
		triggered = false;
    }
	
	public override void StateExited( OwnerType self ) {
		base.StateExited( self );

		active = false;
		triggered = false;
    }
	
	public override void SetStateParms( State<OwnerType> nextState ) {
		SetStateParms( (State<OwnerType, Parm1Type, Parm2Type>)nextState );
	}
	
	public void SetStateParms( State<OwnerType, Parm1Type, Parm2Type> nextState ) {
		if( !active || !triggered ) {
			return;
		}

		nextState.Parm1 = Parm1;
		nextState.Parm2 = Parm2;
	}

    protected virtual void  OnEvent( Parm1Type parm1, Parm2Type parm2 ) {
		//try {
			if( !active ) {
		//		throw new Exception( "OnEvent '" + Name + "' called while not active" );
				return;
			}
		//} catch( Exception e ) {
		//	Debug.LogError( e.ToString() );
		//}
		
		triggered = active;
		Parm1 = parm1;
		Parm2 = parm2;	
    }

    public override bool Evaluate() {
        return triggered;
    }

    protected bool		triggered = false;
	protected bool		active = false;
	public Parm1Type	Parm1;
	public Parm2Type	Parm2;
}

public class TransitionBehavior_OnEvent<OwnerType, Parm1Type, Parm2Type, Parm3Type> : ITransitionBehavior_OnEvent<OwnerType> where OwnerType : class {
	public TransitionBehavior_OnEvent( string name ) : base(name) {
    }
	
	public TransitionBehavior_OnEvent( string name, ref TransitionEventDelegate<Parm1Type, Parm2Type, Parm3Type> d ) : base(name) {
		AttachHandler( ref d );
    }
	
	public void	AttachHandler( ref TransitionEventDelegate<Parm1Type, Parm2Type, Parm3Type> d ) {
		d += OnEvent;
	}

    public override void StateEntered( OwnerType self ) {
        base.StateEntered( self );

		active = true;
		triggered = false;
    }
	
	public override void StateExited( OwnerType self ) {
		base.StateExited( self );

		active = false;
		triggered = false;
    }
	
	public override void SetStateParms( State<OwnerType> nextState ) {
		SetStateParms( (State<OwnerType, Parm1Type, Parm2Type, Parm3Type>)nextState );
	}
	
	public void SetStateParms( State<OwnerType, Parm1Type, Parm2Type, Parm3Type> nextState ) {
		if( !active || !triggered ) {
			return;
		}

		nextState.Parm1 = Parm1;
		nextState.Parm2 = Parm2;
		nextState.Parm3 = Parm3;
	}

    protected virtual void  OnEvent( Parm1Type parm1, Parm2Type parm2, Parm3Type parm3 ) {
		//try {
			if( !active ) {
		//		throw new Exception( "OnEvent '" + Name + "' called while not active" );
				return;
			}
		//} catch( Exception e ) {
		//	Debug.LogError( e.ToString() );
		//}

		triggered = active;
		Parm1 = parm1;
		Parm2 = parm2;
		Parm3 = parm3;
    }

    public override bool Evaluate() {
        return triggered;
    }

    protected bool	triggered = false;
	protected bool	active = false;
	public Parm1Type	Parm1;
	public Parm2Type	Parm2;
	public Parm3Type	Parm3;
}