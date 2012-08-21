using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public interface IJSONDeserilizable {
	void	Decode( object node );
}

public class JSONDecoder {
	protected static bool	SetField( Type selfType, object self, string fieldName, object val ) {
		FieldInfo info = selfType.GetField( fieldName );
		if( info == null ) {
			return false;
		}
		
		info.SetValue( self, val );
		return true;
	}
	
	protected static bool	SetProperty( Type selfType, object self, string propName, object val ) {
		PropertyInfo info = selfType.GetProperty( propName );
		if( info == null ) {
			return false;
		}
		
		info.SetValue( self, val, null );
		return true;
	}
	
	public static bool	SetValue( object inst, string name, object val ) {
		Type instType = inst.GetType();
		
		if( SetProperty(instType, inst, name, val) ) {
			return true;
		}
		
		if( SetField(instType, inst, name, val) ) {
			return true;
		}
		
		return false;
	}
	
	public static T		Decode<T>( string json ) {
		return (T)Decode( typeof(T), JSONDeserializer.Deserialize(json) );
	}
	
	public static T		Decode<T>( object node ) {
		return (T)Decode( typeof(T), node );
	}
	
	public static object		Decode( Type destType, object node ) {
		if( node == null ) {
			return null;
		}
		
		Type nodeType = node.GetType();
		
		if( destType.IsAssignableFrom(nodeType) ) {
			return node;
		}

		if( nodeType == typeof(Dictionary<string, object>) ) {
			return DecodeValue( destType, (Dictionary<string, object>)node );
		}
		

		if( nodeType == typeof(List<object>) ) {
			return DecodeValue( destType, (List<object>)node );
		}
		
		if( destType.IsEnum ) {
			return Enum.ToObject( destType, node );
		}
		
		return JSONConverterManager.Instance.ConvertValue( nodeType, destType, node );
	}
	
	protected static Type		GetMemberType( Type declaringType, string name ) {
		PropertyInfo propInfo = declaringType.GetProperty( name );
		if( propInfo != null ) {
			return propInfo.PropertyType;
		}
		
		FieldInfo fieldInfo = declaringType.GetField( name );
		if( fieldInfo != null ) {
			return fieldInfo.FieldType;
		}
		
		return null;
	}
	
	protected static object		DecodeValue( Type destType, Dictionary<string, object> node ) {
		object destInst = null;
		try {
			destInst = Activator.CreateInstance( destType );
		}
		catch( Exception ) {
			//Debug.LogWarning( e );
			return null;
		}
		
		Type memberType = null;
		foreach( KeyValuePair<string, object> kv in node ) {
			memberType = GetMemberType( destType, kv.Key );
			if( memberType == null ) {
				continue;
			}
			
			SetValue( destInst, kv.Key, Decode(memberType, kv.Value) );
		}
		
		return destInst;
	}
	
	protected static object		DecodeValue( Type destType, List<object> node ) {
		return ( destType.IsArray ) ? DecodeArray(destType.GetElementType(), node) : DecodeList(destType, node);
	}
	
	protected static object		DecodeList( Type destType, List<object> node ) {
		IList destInst = null;
		try {
			destInst = (IList)Activator.CreateInstance( destType );
		}
		catch( Exception ) {
			//Debug.LogWarning( e );
			return null;
		}
		
		Type elementType;
		if( destType.IsGenericType ) {
			elementType = destType.GetGenericArguments()[0];
		} else {
			elementType = destType.GetElementType();
		}
		foreach( object element in node ) {
			destInst.Add( Decode(elementType, element) );
		}
		
		return destInst;
	}
	
	protected static object		DecodeArray( Type elemType, List<object> node ) {
		Array destInst = null;
		try {
			destInst = Array.CreateInstance( elemType, node.Count );
		}
		catch( Exception ) {
			//Debug.LogWarning( e );
			return null;
		}
		
		for( int ix = 0; ix < node.Count; ++ix ) {
			destInst.SetValue( Decode(elemType, node[ix]), ix );
		}
		
		return destInst;
	}
	
	////////////////////////Below code should be in better place.  And may need better names////////////////////
	protected static object		GetFieldValue( object inst, string name ) {
		return GetFieldValue( inst, inst.GetType(), name );
	}
	
	protected static object		GetFieldValue( object inst, Type instType, string name ) {
		FieldInfo info = instType.GetField( name );
		if( info == null ) {
			return null;
		}
		
		return info.GetValue( inst );
	}
	
	protected static object		GetPropertyValue( object inst, string name ) {
		return GetPropertyValue( inst, inst.GetType(), name );
	}
	
	protected static object		GetPropertyValue( object inst, Type instType, string name ) {
		PropertyInfo info = instType.GetProperty( name );
		if( info == null ) {
			return null;
		}
		
		return info.GetValue( inst, null );
	}
	
	protected static object		GetValue( object inst, string name ) {
		Type	instType = inst.GetType();
		object obj = GetPropertyValue( inst, instType, name );
		if( obj != null ) {
			return obj;
		}
		
		obj = GetFieldValue( inst, instType, name );
		if( obj != null ) {
			return obj;
		}
		
		return null;
	}
	
	public static bool			Decode( object node, out Dictionary<string, object> dict ) {
		if( node.GetType() != typeof(Dictionary<string, object>) ) {
			dict = null;
			return false;
		}
		
		dict = node as Dictionary<string, object>;
		return true;
	}
	
	public static bool			Decode( object node, out List<object> list ) {
		if( node.GetType() != typeof(List<object>) ) {
			list = null;
			return false;
		}
		
		list = node as List<object>;
		return true;
	}
	
	public static bool			Decode( object node, out object[] list ) {
		if( node.GetType() != typeof(object[]) ) {
			list = null;
			return false;
		}
		
		list = node as object[];
		return true;
	}
	
	public static bool			Decode<FromType, ToType>( object node, out ToType val ) {
		Type fromType = typeof(FromType);
		Type toType = typeof(ToType);
		Type nodeType = node.GetType();
		
		if( fromType.IsAssignableFrom(nodeType) ) {
			val = (ToType)node;
			return true;
		}
		
		if( fromType != nodeType ) {
			val = default(ToType);
			return false;
		}
		
		try {
			val = (ToType)JSONConverterManager.Instance.ConvertValue( fromType, toType, node );
		} 
		catch( Exception ) {
			val = default(ToType);
			return false;
		}
		
		return true;
	}
	
	public static bool			Decode( object node, object callerInst, string memberName ) {
		Dictionary<string, object>	objDict;
		if( !Decode(node, out objDict) ) {
			return false;
		}
		
		object obj = JSONDecoder.GetValue( callerInst, memberName );
		Decode( node, obj );
		return true;
	}
	
	public static bool			Decode( object node, object inst ) {
		Type instType = inst.GetType();
		instType.InvokeMember( "Decode", BindingFlags.Instance|BindingFlags.Public|BindingFlags.InvokeMethod, null, inst, new object[]{ node } );
		return true;
	}
}