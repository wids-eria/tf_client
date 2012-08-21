using System;
using System.Collections.Generic;

/// <summary>
/// Information about the current player.
/// </summary>
public class UserData : System.Object, IJSONDeserilizable
{
	/// <summary>
	/// The identifier.
	/// </summary>
	public int id = 1;
	/// <summary>
	/// The authentication token.
	/// </summary>
	public string auth_token = "FpydyyrZaCkZ7HWhCxWh"; // NOTE: update default value when world is rebuilt to bypass login for testing
	/// <summary>
	/// The world number.
	/// </summary>
	public static int worldNumber = 1; // NOTE: update default value when world is rebuilt to bypass login for testing
	/// <summary>
	/// Gets or sets the current user data.
	/// </summary>
	/// <value>
	/// The current user data.
	/// </value>
	public static UserData current { get; set; }
	
	public void	Decode( object node ) {
		Dictionary<string, object> dict;
		
		ExceptionHelper.Throw<Exception>( JSONDecoder.Decode(node, out dict) );
		
		foreach( KeyValuePair<string, object> kv in dict ) {
			JSONDecoder.SetValue( this, kv.Key, kv.Value );
		}
	}
}