using System;
using System.Text;

public static class StringBuilderExtensions {
	public static void	Replace( this StringBuilder self, int startIndex, int endIndex, string replaceStr ) {
		self.Remove( startIndex, endIndex - startIndex + 1 );
		self.Insert( startIndex, replaceStr );
	}
	
	public static void	DeleteCharAt( this StringBuilder self, int index ) {
		self.Remove( index, 1 );
	}
}