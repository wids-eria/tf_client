using UnityEngine;
using System.Collections;

/// <summary>
/// The message to be broadcast when a round has concluded
/// </summary>
public class MessageQuestComplete : MessageAM
{
	public Quest quest;
	public MessageQuestComplete(Quest quest) : base("game flow")
	{
		this.quest = quest;
	}
}