using UnityEngine;
using System.Collections;

/// <summary>
/// Class to manage updating components when certain game events happen
/// </summary>
public static class MessengerAM : System.Object
{
	/// <summary>
	/// All the objects listening for messages.
	/// </summary>
	private static Hashtable m_listeners = new Hashtable();
	
	/// <summary>
	/// A listener type for input-related events.
	/// </summary>
	public static readonly string listenTypeInput = "input";
	/// <summary>
	/// A listener type for player-related events.
	/// </summary>
	public static readonly string listenTypePlayer = "player";
	/// <summary>
	/// A listener type for item collection events.
	/// </summary>
	public static readonly string listenTypeCollectItem = "collect item";
	/// <summary>
	/// A listener type for configuration-related events (quality, resolution, etc.)
	/// </summary>
	public static readonly string listenTypeConfig = "config";
	
	/// <summary>
	/// Clears all registered listeners. Should be called when a new scene is loaded, etc.
	/// </summary>
	public static void ClearAll()
	{
		m_listeners = new Hashtable();
	}
	
	/// <summary>
	/// Adds a listener for a particular type of message.
	/// </summary>
	/// <param name="listenerType">
	/// A <see cref="System.String"/>
	/// </param>
	/// <param name="go">
	/// A <see cref="GameObject"/>
	/// </param>
	public static void Listen(string listenerType, GameObject go)
	{	
		// if there's no array for this tracking category, make a new one
		if(m_listeners[listenerType] == null)
		{
			m_listeners[listenerType] = new ArrayList();
		}
	
		ArrayList listener = m_listeners[listenerType] as ArrayList;
		
		// only add to the array if it isn't already being tracked
		if(!listener.Contains(go))
		{
			listener.Add(go);
		}
	}

	/// <summary>
	/// Register implicitly with this instead of gameObject.
	/// </summary>
	/// <param name="listenerType">
	/// A <see cref="System.String"/>
	/// </param>
	/// <param name="component">
	/// A <see cref="Component"/>
	/// </param>
	public static void Listen(string listenerType, Component component)
	{
		Listen(listenerType, component.gameObject);
	}

	/// <summary>
	/// Removes a listener for the specified type of message.
	/// </summary>
	/// <param name="listenerType">
	/// A <see cref="System.String"/>
	/// </param>
	/// <param name="go">
	/// A <see cref="GameObject"/>
	/// </param>
	public static void StopListen(string listenerType, GameObject go)
	{
		ArrayList listener = m_listeners[listenerType] as ArrayList;
	
		if(listener != null)
		{
			listener.Remove(go);
		}
	}

	/// <summary>
	/// Sends a message (calls the function denoted by methodName) to all registered listeners for the given message type.
	/// </summary>
	/// <param name="msg">
	/// A <see cref="Message"/>
	/// </param>
	public static void Send(MessageAM msg)
	{	
		ArrayList sendTo = m_listeners[msg.listenerType] as ArrayList;
		if(sendTo != null)
		{
			foreach(GameObject listener in sendTo)
			{
				if(listener != null)
					listener.SendMessage(msg.functionName, msg, SendMessageOptions.DontRequireReceiver);
			}
		}
	}
}