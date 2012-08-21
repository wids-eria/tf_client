using UnityEngine;
using System.Collections;

public class StateMachine<OwnerType> where OwnerType : class {
    public StateMachine() {
        CurrentState = null;
        PreviousState = null;
    }

    public bool Update( OwnerType self ) {
		bool stateChanged = false;

        if( CurrentState == null ) {
            return stateChanged;
        }

        State<OwnerType> nextState = CurrentState.AttemptStateChange( self );
        if( nextState != null ) {
            ChangeState( nextState, self );
			stateChanged = true;
        }
            
		CurrentState.Update( self );
		return stateChanged;
    }
	
	public bool FixedUpdate( OwnerType self ) {
        if( CurrentState == null ) {
            return false;
        }

        State<OwnerType> nextState = CurrentState.AttemptStateChange( self );
		bool stateChanged = false;

        if( nextState != null ) {
            ChangeState( nextState, self );
			stateChanged = true;
        }
		
        CurrentState.FixedUpdate( self );
        return stateChanged;
    }
	
	public bool OnDrawGizmos( OwnerType self ) {
		bool stateChanged = false;

        if( CurrentState == null ) {
            return stateChanged;
        }
		
		State<OwnerType> nextState = CurrentState.AttemptStateChange( self );
        if( nextState != null ) {
            ChangeState( nextState, self );
			stateChanged = true;
        }

        CurrentState.OnDrawGizmos( self );
		return stateChanged;
    }
	
	public bool OnGUI( OwnerType self ) {
		bool stateChanged = false;

        if( CurrentState == null ) {
            return stateChanged;
        }
		
		State<OwnerType> nextState = CurrentState.AttemptStateChange( self );
        if( nextState != null ) {
            ChangeState( nextState, self );
			stateChanged = true;
        }

        CurrentState.OnGUI( self );
		return stateChanged;
    }

    public void ChangeState( State<OwnerType> nextState, OwnerType self ) {
        if( CurrentState != null ) {
            CurrentState.Exit( self, nextState );
        }

        PreviousState = CurrentState;
        CurrentState = nextState;

        if( CurrentState != null ) {
            CurrentState.Enter( self, PreviousState );
        }
    }
	
	//[Save]
    public State<OwnerType>    CurrentState;
	//[Save]
    public State<OwnerType>    PreviousState;
}