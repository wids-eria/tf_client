using UnityEngine;
using System;
using System.Collections.Generic;
using System.Globalization;

public class JSONConverterManager {
	private static JSONConverterManager	instance = null;
	public static JSONConverterManager	Instance {
		get {
			if( instance == null ) {
				instance = new JSONConverterManager();
			}
			return instance;
		}
	}
	
	protected JSONConverterManager() {
		BuildConversionMap();
	}
	
	internal delegate object ConvertionDelegate( object src );

	private Dictionary<Type, Dictionary<Type, ConvertionDelegate>> conversionMap = new Dictionary<Type, Dictionary<Type, ConvertionDelegate>>();
	public void	BuildConversionMap() {
		ConvertionDelegate importer;

        importer = delegate( object input ) {
            return Convert.ToByte( input );
        };
        BuildConverter( typeof(int), typeof(byte), importer );

        importer = delegate( object input ) {
            return Convert.ToUInt64( (int)input );
        };
        BuildConverter( typeof(int), typeof(ulong), importer );

        importer = delegate( object input ) {
            return Convert.ToSByte( (int)input );
        };
        BuildConverter( typeof(int), typeof(sbyte), importer );

        importer = delegate( object input ) {
            return Convert.ToInt16( (int)input );
        };
        BuildConverter( typeof(int), typeof(short), importer );

        importer = delegate( object input ) {
            return Convert.ToUInt16( (int)input );
        };
        BuildConverter( typeof(int), typeof(ushort), importer );

        importer = delegate( object input ) {
            return Convert.ToUInt32( (int)input );
        };
        BuildConverter( typeof(int), typeof(uint), importer );

        importer = delegate( object input ) {
            return Convert.ToSingle( (int)input );
        };
        BuildConverter( typeof(int), typeof(float), importer );

        importer = delegate( object input ) {
            return Convert.ToDouble( (int)input );
        };
        BuildConverter( typeof(int), typeof(double), importer );

        importer = delegate( object input ) {
            return Convert.ToDecimal( (double)input );
        };
        BuildConverter( typeof(double), typeof(decimal), importer );

        importer = delegate( object input ) {
            return Convert.ToUInt32( (long)input );
        };
        BuildConverter( typeof(long), typeof(uint), importer);

        importer = delegate (object input) {
            return Convert.ToChar( (string)input );
        };
        BuildConverter ( typeof(string), typeof(char), importer );

        importer = delegate( object input ) {
            return Convert.ToDateTime( (string)input, DateTimeFormatInfo.InvariantInfo );
        };
        BuildConverter( typeof(string), typeof(DateTime), importer );
		
		importer = delegate (object input) {
            return Convert.ToSingle( (double)input );
        };
        BuildConverter ( typeof(System.Double), typeof(System.Single), importer );
	}
	
	private void	BuildConverter( Type srcType, Type destType, ConvertionDelegate converter ) {
		Dictionary<Type, ConvertionDelegate>	converterMap = null;
		try {
			converterMap = conversionMap[ srcType ];
		} catch( KeyNotFoundException ) {
			converterMap = new Dictionary<Type, ConvertionDelegate>();
			conversionMap.Add( srcType, converterMap );
		}
		
		converterMap.Add( destType, converter );
	}
	
	public object	ConvertValue( Type srcType, Type destType, object val ) {
		ConvertionDelegate converter;
		try {
			converter = conversionMap[srcType][destType];
		} catch( Exception e ) {
			Debug.LogWarning( e );
			return null;
		}
		return converter( val );
	}
	
	public ToType	ConvertValue<FromType, ToType>( FromType val ) {
		return (ToType)ConvertValue( typeof(FromType), typeof(ToType), val );
	}
}