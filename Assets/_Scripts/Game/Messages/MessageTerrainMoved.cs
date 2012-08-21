using UnityEngine;
using System.Collections;

/// <summary>
/// Message sent when the terrain moves
/// </summary>
public class MessageTerrainMoved : MessageAM
{
	/// <summary>
	/// the new position to which the terrain moved
	/// </summary>
	public Vector3 newPosition { get; private set; }
	
	/// <summary>
	/// Initializes a new instance of the <see cref="MessageTerrainMoved"/> class.
	/// </summary>
	/// <param name='newPosition'>
	/// New position.
	/// </param>
	public MessageTerrainMoved(Vector3 newPosition) : base("world")
	{
		this.newPosition = newPosition;
	}
}