using UnityEngine;
using System.Collections;

/// <summary>
/// A message to send when the screen resolution changes.
/// </summary>
public class MessageScreenResolutionChanged : MessageAM
{
	/// <summary>
	/// Initializes a new instance of the <see cref="MessageScreenResolutionChanged"/> class.
	/// </summary>
	public MessageScreenResolutionChanged():base(MessengerAM.listenTypeConfig)
	{
#if UNITY_FLASH
		functionName = "_ScreenResolutionChanged";
#endif
	}
}