using UnityEngine;
using System;
using System.Collections.Generic;

public class TransitionBehaviorLibrary {
	public static TransitionBehaviorLibrary Instance = new TransitionBehaviorLibrary();
	
	public TransitionBehaviorLibrary() {
		Register( typeof(TransitionBehavior_OnEvent<>) );
		Register( typeof(TransitionBehavior_OnEvent<,>) );
		Register( typeof(TransitionBehavior_OnEvent<,,>) );
		Register( typeof(TransitionBehavior_OnEvent<,,,>) );
		Register( typeof(TransitionBehavior_Delayed<>) );
		Register( typeof(TransitionBehavior_OnState<>) );
	}
	
	protected void	Register( string name, string assemblyQualifiedName ) {
		if( qualifiedNames.ContainsKey(name) ) {
			return;
		}
		qualifiedNames.Add( name, assemblyQualifiedName );
	}
	
	protected void	Register( Type type ) {
		string[] parts = type.Name.Split( '`' );
		Register( parts[0] + BuildEmptyGenericArgs(int.Parse(parts[1])), type.AssemblyQualifiedName.ToString() );
	}
	
	protected string	BuildEmptyGenericArgs( int numArgs ) {
		string genericParms = "";
		for( int ix = 0; ix < numArgs; ++ix ) {
			genericParms += ",";
		}
		genericParms = "<" + genericParms + ">";
		return genericParms;
	}
	
	public string	FindAssemblyQualifiedName( string name, int numGenericArgs ) {
		try {
			string fullName = name + BuildEmptyGenericArgs(numGenericArgs);
			return qualifiedNames[ fullName ];
		} catch( KeyNotFoundException e ) {
			Debug.LogError( e.Message );
			return "";
		}
	}
	
	protected Dictionary<string, string>	qualifiedNames = new Dictionary<string, string>();
}
