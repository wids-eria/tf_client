using UnityEngine;
using System;
using System.Reflection;

public class ApplicationEx {
	public readonly static ApplicationEx	Instance = new  ApplicationEx();
	
	public Action		OnPreLevelLoad;
	
	public ApplicationEx() {
	}
	
	public void LoadLevel( int index ) {
		if( OnPreLevelLoad != null ) {
			OnPreLevelLoad();
		}
		
		CleanupStaticReferences();
		Application.LoadLevel( index );
	}
	
	public void LoadLevel( string levelName ) {	
		if( OnPreLevelLoad != null ) {
			OnPreLevelLoad();
		}
		
		CleanupStaticReferences();
		Application.LoadLevel( levelName );
	}
	
	public void LoadLevelAdditive( int index ) {
		if( OnPreLevelLoad != null ) {
			OnPreLevelLoad();
		}
		
		CleanupStaticReferences();
		Application.LoadLevelAdditive( index );
	}
	
	public void LoadLevelAdditive( string levelName ) {
		if( OnPreLevelLoad != null ) {
			OnPreLevelLoad();
		}
		
		CleanupStaticReferences();
		Application.LoadLevelAdditive( levelName );
	}

	public AsyncOperation LoadLevelAsync( int index ) {		
		if( OnPreLevelLoad != null ) {
			OnPreLevelLoad();
		}
		
		CleanupStaticReferences();
		return Application.LoadLevelAsync( index );
	}
	
	public AsyncOperation LoadLevelAsync( string levelName ) {
		if( OnPreLevelLoad != null ) {
			OnPreLevelLoad();
		}
		
		CleanupStaticReferences();
		return Application.LoadLevelAsync( levelName );
	}
	
	public AsyncOperation LoadLevelAdditiveAsync( int index ) {
		if( OnPreLevelLoad != null ) {
			OnPreLevelLoad();
		}
		
		CleanupStaticReferences();
		return Application.LoadLevelAdditiveAsync( index );
	}
	
	public AsyncOperation LoadLevelAdditiveAsync( string levelName ) {
		if( OnPreLevelLoad != null ) {
			OnPreLevelLoad();
		}
		
		CleanupStaticReferences();
		return Application.LoadLevelAdditiveAsync( levelName );
	}
	
	// Need to deal with non-monobehaviour objects also.  Possibly register references to none monobehaviours??
	protected void	CleanupStaticReferences() {
		Type type = null;
		FieldInfo[] fields = null;
		foreach( MonoBehaviour behaviour in GameObject.FindSceneObjectsOfType(typeof(MonoBehaviour)) ) {
			type = behaviour.GetType();
			fields = type.GetFields();
			foreach( FieldInfo info in fields ) {
				if( !info.IsStatic ) {
					continue;
				}
				
				if( info.IsLiteral ) {
					continue;
				}
				
				info.SetValue( behaviour, null );
			}
		}
	}
}