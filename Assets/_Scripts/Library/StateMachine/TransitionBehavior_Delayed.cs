using UnityEngine;
using System;

public class TransitionBehavior_Delayed<OwnerType> : TransitionBehavior<OwnerType> where OwnerType : class {
	public TransitionBehavior_Delayed( string name ) : base( name ) {
	}

	public TransitionBehavior_Delayed( string name, float delay ) : base( name ) {
		this.delay = delay;
	}
	
	public TransitionBehavior_Delayed( string name, TransitionPlug<float> delayPlug ) : base( name ) {
		delayPlug.SetValue += delegate( float delay ) { this.delay = delay; };
		delayPlug.GetValue += delegate() { return this.delay; };
	}
	
	public override void StateEntered( OwnerType self ) {
		startTime = Time.time;
	}
	
	public override void StateExited( OwnerType self ) {
	}

	public override bool Evaluate() {
		return Time.time >= startTime + delay;
	}
	
	public override void SetStateParms( State<OwnerType> nextState ) {
	}
	
	public float	delay = 0.0f;
	protected float	startTime = 0.0f; 
}
