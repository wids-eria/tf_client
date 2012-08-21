using System.Collections.Generic;

public static class DictionaryExtensions {
	public static void CopyTo<TKey, TValue>( this Dictionary<TKey,TValue> self, TKey key, Dictionary<TKey,TValue> dst ) {
		TValue val = self[ key ];
		
		self.Remove( key );
		dst.Add( key, val );
	}
}