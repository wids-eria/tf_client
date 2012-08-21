/// <summary>
/// Message sent when a harvest is executed.
/// </summary>
public class MessageHarvestExecuted : MessageAM {
	/// <summary>
	/// The type of the listener.
	/// </summary>
	new public static readonly string listenerType = MessageHarvestEnded.listenerType;
	/// <summary>
	/// Gets or sets the harvest.
	/// </summary>
	/// <value>
	/// The harvest.
	/// </value>
	public Harvest harvest { get; private set; }
	/// <summary>
	/// Gets or sets the products yielded by the harvest.
	/// </summary>
	/// <value>
	/// The products.
	/// </value>
	public Harvest.Products products { get; private set; }
	/// <summary>
	/// Initializes a new instance of the <see cref="MessageHarvestExecuted"/> class.
	/// </summary>
	/// <param name='harvest'>
	/// Harvest.
	/// </param>
	/// <param name='products'>
	/// Products.
	/// </param>
	public MessageHarvestExecuted(Harvest harvest, Harvest.Products products):base(listenerType)
	{
		this.harvest = harvest;
		this.products = products;
	}
}