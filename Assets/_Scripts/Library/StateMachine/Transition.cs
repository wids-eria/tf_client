using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Globalization;

public class TransitionPlug<T> {
	public delegate void SetValueDelegate( T val );
	public delegate T GetValueDelegate();
	
	public SetValueDelegate	SetValue;
	public void	Set( T val ) {
		if( SetValue == null ) {
			return;
		}
		
		SetValue( val );
	}
	
	public GetValueDelegate	GetValue;
	public T	Get() {
		if( GetValue == null ) {
			return default(T);
		}
		
		return GetValue();
	}
}

public class Transition<OwnerType> where OwnerType : class {
    public Transition( OwnerType o ) {
		owner = o;
    }

    public Transition( OwnerType o, State<OwnerType> state ) {
		owner = o;
        AddState( state );
    }
	
	public bool	InitFromExpression( string expression ) {
		program.DeclareBehavior = DeclareBehavior;
		program.DereferenceBehavior = DereferenceBehavior;
		return program.LoadProgram( expression );
	}
	
	public bool	InitFromInstructions( List<string> instructions ) {
		program.DeclareBehavior = DeclareBehavior;
		program.DereferenceBehavior = DereferenceBehavior;
		return program.LoadInstructions( instructions );
	}

    public virtual void AddState( State<OwnerType> state ) {
        nextStates.Add( state );
    }

    public virtual State<OwnerType> GetState( int index ) {
		if( nextStates.Count <= 0 ) {
			return null;
		}
        return nextStates[ index ];
    }
	
	public virtual State<OwnerType> GetRandomState() {
		if( nextStates.Count == 0 ) {
			return null;
		}
        return nextStates[ UnityEngine.Random.Range(0, nextStates.Count - 1) ];
    }
	
	public virtual void	AddBehavior( TransitionBehavior<OwnerType> behavior ) {
		behaviors.Add( behavior.Name, behavior );
	}

    public virtual void StateEntered( OwnerType self ) {
		foreach( KeyValuePair< string, TransitionBehavior<OwnerType> > pair in behaviors ) {
			pair.Value.StateEntered( self );
		}
    }

    public virtual void StateExited( OwnerType self ) {
		foreach( KeyValuePair< string, TransitionBehavior<OwnerType> > pair in behaviors ) {
			pair.Value.StateExited( self );
		}
    }

    public virtual State<OwnerType> TransitionTo( OwnerType self ) {
		bool result = false;
		if( !program.Execute(ref result) || !result ) {
			return null;
		}
		
		State<OwnerType> state = GetRandomState();
		foreach( KeyValuePair< string, TransitionBehavior<OwnerType> > pair in behaviors ) {
			pair.Value.SetStateParms( state );
		} 
		
		return state;
	}
	
	public bool	DereferenceBehavior( string name ) {
		try {
			return behaviors[ name ].Evaluate();
		} catch( Exception e ) {
			Debug.LogError( e.Message );
		}
		
		return false;
	}
	
	// Need to put this in a better place
	public Type	TypeOf<T>( T t ) {// this lets us get the type for a null object
		return typeof(T);
	}
	
	#region Find Behavior Args 
	// JESUS GOD THIS NEEDS A LOT OF CLEAN UP!!!
	public void DeclareBehavior( string behaviorName, string name, List<string> args ) {
		Type eventType = null;
		System.Object arg = Find( owner, args[0], out eventType );
		System.Delegate del = arg as Delegate;
		
		// FIX AS SOON AS WORKING - use a Dictionary or something else
		if( behaviorName == "TransitionBehavior_OnEvent" ) {
			AddBehavior( AllocateAndAttachEventBehavior(behaviorName, name, ref del, eventType) );
			Type ownerType = owner.GetType();
			FieldInfo info = ownerType.GetField( args[0] );
			info.SetValue( owner, del );
		} else if( behaviorName == "TransitionBehavior_OnState" ) {
			string behaviorQualifiedName = TransitionBehaviorLibrary.Instance.FindAssemblyQualifiedName( behaviorName, 1 );
			AddBehavior( AllocateStateBehavior(behaviorQualifiedName, name, arg) );
		} else if( behaviorName == "TransitionBehavior_Delayed" ) {
			string behaviorQualifiedName = TransitionBehaviorLibrary.Instance.FindAssemblyQualifiedName( behaviorName, 1 );
			AddBehavior( AllocateDelayBehavior(behaviorQualifiedName, name, arg) );
		} else {
			throw new Exception();
		}
	}
	
	protected System.Object		Find( OwnerType inst, string argName, out Type eventDelegateType ) {
		Type ownerType = inst.GetType();
		
		eventDelegateType = null;

		FieldInfo fieldInfo = ownerType.GetField( argName );
		if( fieldInfo != null ) {
			eventDelegateType = fieldInfo.FieldType;
			return Find( inst, fieldInfo );
		}

		PropertyInfo propInfo = ownerType.GetProperty( argName );
		if( propInfo != null ) {
			return Find( inst, propInfo );
		}
		
		MethodInfo methodInfo = ownerType.GetMethod( argName, BindingFlags.Instance|BindingFlags.NonPublic|BindingFlags.Public );
		if( methodInfo != null ) {
			return Find( inst, methodInfo );
		}
		
		return null;
	}
	
	protected System.Object		Find( OwnerType inst, FieldInfo info ) {
		return info.GetValue( inst );
	}
	
	protected System.Object		Find( OwnerType inst, PropertyInfo info ) {
		return info.GetValue( inst, null );
	}
	
	protected System.Object		Find( OwnerType inst, MethodInfo info ) {
		return Delegate.CreateDelegate( typeof(TransitionOnStateDelegate), inst, info );
	}
	// JESUS GOD THIS NEEDS A LOT OF CLEAN UP!!!
	#endregion

	public void AddBehavior( string behaviorName, string name, ref TransitionEventDelegate del ) {
		Delegate d = del;
		TransitionBehavior<OwnerType> behavior = AllocateAndAttachEventBehavior( behaviorName, name, ref d, TypeOf(del) );
		del = d as TransitionEventDelegate;
		AddBehavior( behavior );
	}
	
	public void AddBehavior<Parm1Type>( string behaviorName, string name, ref TransitionEventDelegate<Parm1Type> del ) {
		Delegate d = del;
		TransitionBehavior<OwnerType> behavior = AllocateAndAttachEventBehavior( behaviorName, name, ref d, TypeOf(del) );
		del = d as TransitionEventDelegate<Parm1Type>;
		AddBehavior( behavior );
	}
	
	public void AddBehavior<Parm1Type, Parm2Type>( string behaviorName, string name, ref TransitionEventDelegate<Parm1Type, Parm2Type> del ) {
		Delegate d = del;
		TransitionBehavior<OwnerType> behavior = AllocateAndAttachEventBehavior( behaviorName, name, ref d, TypeOf(del) );
		del = d as TransitionEventDelegate<Parm1Type, Parm2Type>;
		AddBehavior( behavior );
	}
	
	public void AddBehavior<Parm1Type, Parm2Type, Parm3Type>( string behaviorName, string name, ref TransitionEventDelegate<Parm1Type, Parm2Type, Parm3Type> del ) {
		Delegate d = del;
		TransitionBehavior<OwnerType> behavior = AllocateAndAttachEventBehavior( behaviorName, name, ref d, TypeOf(del) );
		del = d as TransitionEventDelegate<Parm1Type, Parm2Type, Parm3Type>;
		AddBehavior( behavior );
	}
	
	public void AddBehavior( string behaviorName, string name, float delay ) {
		TransitionBehavior<OwnerType> behavior = AllocateDelayBehavior( TransitionBehaviorLibrary.Instance.FindAssemblyQualifiedName(behaviorName, 1), name, delay ) as TransitionBehavior<OwnerType>;
		AddBehavior( behavior );
	}
	
	public void AddBehavior( string behaviorName, string name, TransitionPlug<float> delayPlug ) {
		TransitionBehavior<OwnerType> behavior = AllocateDelayBehavior( TransitionBehaviorLibrary.Instance.FindAssemblyQualifiedName(behaviorName, 1), name, delayPlug ) as TransitionBehavior<OwnerType>;
		AddBehavior( behavior );
	}
	
	public void AddBehavior( string behaviorName, string name, TransitionOnStateDelegate del ) {
		TransitionBehavior<OwnerType> behavior = AllocateStateBehavior( TransitionBehaviorLibrary.Instance.FindAssemblyQualifiedName(behaviorName, 1), name, del ) as TransitionBehavior<OwnerType>;
		AddBehavior( behavior );
	}
	
	protected TransitionBehavior<OwnerType> AllocateAndAttachEventBehavior( string behaviorName, string name, ref System.Delegate delegateInst, Type delegateType ) {
		string behaviorQualifiedName = TransitionBehaviorLibrary.Instance.FindAssemblyQualifiedName( behaviorName, delegateType.GetGenericArguments().Length + 1 );
		TransitionBehavior<OwnerType> behavior = AllocateEventBehavior( behaviorQualifiedName, name, delegateType );

		object[] args = new object[] { delegateInst };
		try {
			Type t = behavior.GetType();
			t.InvokeMember( "AttachHandler", BindingFlags.Public|BindingFlags.Instance|BindingFlags.InvokeMethod, null, behavior, args );
		} catch( Exception e ) {
			Debug.LogError( "User Caught Exception(t.InvokeMember): " + e.Message );
		}
	
		delegateInst = args[0] as System.Delegate;
		return behavior;
	}

	protected TransitionBehavior<OwnerType> AllocateEventBehavior( string qualifiedTypeName, string name, Type delegateType ) {
		try {
			Type t = Type.GetType( qualifiedTypeName );

			Type[] genericArgs = delegateType.GetGenericArguments();
			
			List<Type> behaviorGenericArgs = new List<Type>();
			behaviorGenericArgs.Add( typeof(OwnerType) );
			foreach( Type type in genericArgs ) {
				behaviorGenericArgs.Add( type );
			}
			
			Type behaviorType = null;
			try {
				behaviorType = t.MakeGenericType( behaviorGenericArgs.ToArray() );
			} catch( Exception e ) {
				Debug.LogError( "User Caught Exception(t.MakeGenericType): " + e.Message );
				return null;
			}

			try {
				object obj = Activator.CreateInstance( behaviorType, name );
				return obj as TransitionBehavior<OwnerType>;
			}
			catch( TargetInvocationException e ) {
				Debug.LogError( "User Caught TargetInvocationException(Activator.CreateInstance - " + e.InnerException + "): " + e.Message );
			}
			catch( Exception e ) {
				Debug.LogError( "User Caught Exception(Activator.CreateInstance'" + behaviorType.ToString() + "): " + e.Message );
			}
		} catch( Exception e ) {
			Debug.LogError( "User Caught Exception(1): " + e.Message );
		}
		
		return null;
	}
			
	protected TransitionBehavior<OwnerType> AllocateDelayBehavior( string qualifiedTypeName, string name, object arg ) {
		try {
			Type t = Type.GetType( qualifiedTypeName );
			List<Type> behaviorGenericArgs = new List<Type>();
			behaviorGenericArgs.Add( typeof(OwnerType) );
			
			Type behaviorType = null;
			try {
				behaviorType = t.MakeGenericType( behaviorGenericArgs.ToArray() );
			} catch( Exception e ) {
				Debug.LogError( "User Caught Exception(t.MakeGenericType): " + e.Message );
				return null;
			}

			try {
				object obj = Activator.CreateInstance( behaviorType, name, arg );
				return obj as TransitionBehavior<OwnerType>;
			} 
			catch( TargetInvocationException e ) {
				Debug.LogError( "User Caught TargetInvocationException(Activator.CreateInstance): " + e.Message );
			}
			catch( Exception e ) {
				Debug.LogError( "User Caught Exception(Activator.CreateInstance - Delay): " + e.Message );
			}
		} catch( Exception e ) {
			Debug.LogError( "User Caught Exception(2): " + e.Message );
		}
		
		return null;
	}
	
	protected TransitionBehavior<OwnerType> AllocateStateBehavior( string qualifiedTypeName, string name, object arg ) {
		try {
			Type t = Type.GetType( qualifiedTypeName );
			List<Type> behaviorGenericArgs = new List<Type>();
			behaviorGenericArgs.Add( typeof(OwnerType) );
			
			Type behaviorType = null;
			try {
				behaviorType = t.MakeGenericType( behaviorGenericArgs.ToArray() );
			} catch( Exception e ) {
				Debug.LogError( "User Caught Exception(t.MakeGenericType): " + e.Message );
				return null;
			}

			try {
				object obj = Activator.CreateInstance( behaviorType, name, arg );
				return obj as TransitionBehavior<OwnerType>;
			}
			catch( TargetInvocationException e ) {
				Debug.LogError( "User Caught TargetInvocationException(Activator.CreateInstance): " + e.Message );
			}
			catch( Exception e ) {
				Debug.LogError( "User Caught Exception(Activator.CreateInstance - State): " + e.Message );
			}
		} catch( Exception e ) {
			Debug.LogError( "User Caught Exception(3): " + e.Message );
		}
		
		return null;
	}
	
	//[Save]
    protected 	List< State<OwnerType> > nextStates = new List< State<OwnerType> >();
	
	protected 	Dictionary< string, TransitionBehavior<OwnerType> >	behaviors = new Dictionary< string, TransitionBehavior<OwnerType> >();
	protected	TransitionProgram<OwnerType>	program = new TransitionProgram<OwnerType>();
	protected 	OwnerType	owner = null;
}