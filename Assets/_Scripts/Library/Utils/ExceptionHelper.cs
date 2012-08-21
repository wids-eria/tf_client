using System;

public class ExceptionHelper {
	public static void	Throw<T>( bool exp, string msg ) where T : System.Exception {
		if( exp ) {
			return;
		}
		
		System.Exception e = System.Activator.CreateInstance( typeof(T), new object[]{msg} ) as System.Exception;
		throw e;
	}
	
	public static void	Throw<T>( bool exp ) where T : System.Exception {
		Throw<T>( exp, "" );
	}
}