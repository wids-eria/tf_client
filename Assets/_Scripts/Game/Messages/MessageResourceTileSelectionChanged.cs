using UnityEngine;
using System.Collections;

/// <summary>
/// Message sent when the resource tile selection changes.
/// </summary>
public class MessageResourceTileSelectionChanged : MessageAM
{
	/// <summary>
	/// The type of the listener.
	/// </summary>
	new public static readonly string listenerType = MessengerAM.listenTypeInput;
	/// <summary>
	/// Initializes a new instance of the <see cref="MessageResourceTileSelectionChanged"/> class.
	/// </summary>
	public MessageResourceTileSelectionChanged() : base(listenerType)
	{
		
	}
}