using System;

public delegate bool    TransitionOnStateDelegate();

public class TransitionBehavior_OnState<OwnerType> : TransitionBehavior<OwnerType> where OwnerType : class {
	public TransitionBehavior_OnState( string name ) : base(name) {
    }
	
	public TransitionBehavior_OnState( string name, TransitionOnStateDelegate d ) : base(name) {
		AttachHandler( d );
    }
	
	public void	AttachHandler( TransitionOnStateDelegate d ) {
		CanTransition = d;
	}
	
	public override bool Evaluate() {
		return CanTransition();
	}
	
	public override void SetStateParms( State<OwnerType> nextState ) {
	}
	
	protected TransitionOnStateDelegate	CanTransition;
}
