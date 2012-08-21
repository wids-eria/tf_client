using System.Collections.Generic;

public static class ListExtensions {
	public static void AssureCountOf<T>( this List<T> self, int amount ) {
		if( self.Count >= amount ) {
			return;
		}
		
		int count = self.Count;
		for( int ix = count; ix < amount; ++ix ) {
			self.Add( default(T) );
		}
	}
}