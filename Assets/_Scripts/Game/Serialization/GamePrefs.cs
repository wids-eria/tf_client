using UnityEngine;
using System.Collections;

/// <summary>
/// A static class that wraps PlayerPrefs calls
/// </summary>
public static class GamePrefs : System.Object
{
	/// <summary>
	/// The preference key for the user name
	/// </summary>
	private static readonly string m_userNamePrefKey = "User Name";
	
	/// <summary>
	/// Get the name of the last user to successfully have logged in
	/// </summary>
	/// <returns>
	/// A <see cref="System.String"/>
	/// </returns>
	public static string GetUserName()
	{
		return PlayerPrefs.GetString(m_userNamePrefKey, "");
	}
	
	/// <summary>
	/// Cache the name of the most recent user
	/// </summary>
	/// <param name="name">
	/// A <see cref="System.String"/>
	/// </param>
	public static void SetUser(string name)
	{
		PlayerPrefs.SetString(m_userNamePrefKey, name);
	}
	
	/// <summary>
	/// The preference key for the server URL
	/// </summary>
	private static readonly string m_urlServerPrefKey = "Server URL";
	
	/// <summary>
	/// The default server.
	/// </summary>
	public static readonly string defaultServer = "http://tf.staging.eriainteractive.com/";
	
	/// <summary>
	/// Get the url of the last server used
	/// </summary>
	/// <returns>
	/// A <see cref="System.String"/>
	/// </returns>
	public static string GetServerURL()
	{
		return PlayerPrefs.GetString(m_urlServerPrefKey, defaultServer);
	}
	
	/// <summary>
	/// Cache the url of the most recent server accessed
	/// </summary>
	/// <param name="url">
	/// A <see cref="System.String"/>
	/// </param>
	public static void SetServerURL(string url)
	{
		if (url.LastIndexOf("/") == url.Length-1) {
			url = url.Substring(0, url.Length-2);
		}
		PlayerPrefs.SetString(m_urlServerPrefKey, url);
	}
	
	/// <summary>
	/// The preference key for the most recent world id
	/// </summary>
	private static readonly string m_worldIdKey = "World ID";
	
	/// <summary>
	/// Get the id of the most recently used world
	/// </summary>
	/// <returns>
	/// A <see cref="System.Int32"/>
	/// </returns>
	public static int GetWorldId()
	{
		return PlayerPrefs.GetInt(m_worldIdKey, 1);
	}
	
	/// <summary>
	/// Set the id of the most recently used world
	/// </summary>
	/// <param name="id">
	/// A <see cref="System.Int32"/>
	/// </param>
	public static void SetWorldId(int id)
	{
		PlayerPrefs.SetInt(m_worldIdKey, id);
	}
	
	/// <summary>
	/// The character class key.
	/// </summary>
	private static readonly string m_characterClassKey = "Character Class";
	
	/// <summary>
	/// Gets the character class.
	/// </summary>
	/// <returns>
	/// The character class.
	/// </returns>
	public static Player.CharacterClass GetCharacterClass()
	{
		return (Player.CharacterClass)PlayerPrefs.GetInt(m_characterClassKey, (int)Player.CharacterClass.TheAlmighty);
	}
	
	/// <summary>
	/// Sets the character class.
	/// </summary>
	/// <param name='characterClass'>
	/// Character class.
	/// </param>
	public static void SetCharacterClass(Player.CharacterClass characterClass)
	{
		PlayerPrefs.SetInt(m_characterClassKey, (int)characterClass);
	}
	
	/// <summary>
	/// The master volume key.
	/// </summary>
	private static readonly string m_masterVolumeKey = "Master Volume";
	
	/// <summary>
	/// Gets the master volume.
	/// </summary>
	/// <returns>
	/// The master volume.
	/// </returns>
	public static float GetMasterVolume()
	{
		return PlayerPrefs.GetFloat(m_masterVolumeKey, 1f);
	}
	
	/// <summary>
	/// Sets the master volume.
	/// </summary>
	/// <param name='volume'>
	/// Volume.
	/// </param>
	public static void SetMasterVolume(float volume)
	{
		PlayerPrefs.SetFloat(m_masterVolumeKey, volume);
		AudioListener.volume = volume;
	}
}