/// <summary>
/// Message sent when a harvest ended.
/// </summary>
public class MessageHarvestEnded : MessageAM {
	/// <summary>
	/// The type of the listener.
	/// </summary>
	new public static readonly string listenerType = "harvest";
	/// <summary>
	/// Gets or sets the harvest.
	/// </summary>
	/// <value>
	/// The harvest.
	/// </value>
	public Harvest harvest { get; private set; }
	/// <summary>
	/// Initializes a new instance of the <see cref="MessageHarvestEnded"/> class.
	/// </summary>
	/// <param name='harvest'>
	/// Harvest.
	/// </param>
	public MessageHarvestEnded(Harvest harvest):base(listenerType)
	{
		this.harvest = harvest;
	}
}