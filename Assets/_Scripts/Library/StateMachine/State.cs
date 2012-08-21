using UnityEngine;
using System.Collections.Generic;

public class State<OwnerType> where OwnerType : class {
    public virtual void Enter( OwnerType self, State<OwnerType> prevState ) {
        timeEnterState = Time.time;
		Debug.Log(string.Format(" --- --- Entered State `{0}' --- ---  ",this.ToString()));
        foreach( Transition<OwnerType> transition in exitTransitionList ) {
            transition.StateEntered( self );
        }
    }

    public virtual void Exit( OwnerType self, State<OwnerType> nextState ) {
        foreach( Transition<OwnerType> transition in exitTransitionList ) {
            transition.StateExited( self );
        }
    }

    public virtual void Update( OwnerType self ) {
    }
	
	public virtual void FixedUpdate( OwnerType self ) {
    }
	
	public virtual void OnDrawGizmos( OwnerType self ) {
	}
	
	public virtual void OnGUI( OwnerType self ) {
	}

    public State<OwnerType> AttemptStateChange( OwnerType self ) {
        State<OwnerType>    nextState = null;

        foreach( Transition<OwnerType> transition in exitTransitionList ) {
            nextState = transition.TransitionTo( self );
            if( nextState != null ) {
                return nextState;
            }
        }

        return null;
    }

    public  void        AddTransition( Transition<OwnerType> transition ) {
        exitTransitionList.Add( transition );
    }
	
	public void			AddPlug( string name, TransitionPlug<float> plug ) {
		transitionPlugs.Add( name, plug );
	}
	
	public void			SetTransitionValue( string name, float val ) {
		try {
			transitionPlugs[ name ].Set( val );
		} catch( KeyNotFoundException e ) {
			Debug.LogError( e );
		}
	}
	
	//[Save]
    protected List< Transition<OwnerType> >     exitTransitionList = new List< Transition<OwnerType> >();
	//[Save]
    protected float         timeEnterState = 0.0f;
    protected float         durationInState {
		get {
			return Time.time - timeEnterState;
		}
	}
	
	protected Dictionary<string, TransitionPlug<float>>	transitionPlugs = new Dictionary<string, TransitionPlug<float>>();
}

public class State<OwnerType, Parm1Type> : State<OwnerType> where OwnerType : class {
	//[Save]
	public Parm1Type	Parm1;
}

public class State<OwnerType, Parm1Type, Parm2Type> : State<OwnerType, Parm1Type> where OwnerType : class {
	//[Save]
	public Parm2Type	Parm2;
}

public class State<OwnerType, Parm1Type, Parm2Type, Parm3Type> : State<OwnerType, Parm1Type, Parm2Type> where OwnerType : class {
	//[Save]
	public Parm3Type	Parm3;
}

public class State<OwnerType, Parm1Type, Parm2Type, Parm3Type, Parm4Type> : State<OwnerType, Parm1Type, Parm2Type, Parm3Type> where OwnerType : class {
	//[Save]
	public Parm4Type	Parm4;
}