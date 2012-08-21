using System;

public abstract class TransitionBehavior<OwnerType> where OwnerType : class {
	public TransitionBehavior( string name ) {
		Name = name;
	}
	
	public virtual void StateEntered( OwnerType self ) {
	}
	
	public virtual void StateExited( OwnerType self ) {
	}

	public abstract bool Evaluate();
	
	public abstract void SetStateParms( State<OwnerType> nextState );
	
	public string Name;
}