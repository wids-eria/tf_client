/// <summary>
/// Resource tile selection; used for e.g. submitting ids of tiles to perform an action.
/// </summary>
public struct ResourceTileSelection {
	/// <summary>
	/// Gets the current resource tile selection.
	/// </summary>
	/// <returns>
	/// The current resource tile selection.
	/// </returns>
	public static ResourceTileSelection GetCurrent() 
	{
		try {
			return InputManager.use.resourceTileSelection;
		}
		catch (UnityEngine.UnassignedReferenceException) {
			return new ResourceTileSelection(new int[0]);
		}
	}
	
	/// <summary>
	/// Initializes a new instance of the <see cref="ResourceTileSelection"/> struct.
	/// </summary>
	/// <param name='ids'>
	/// Identifiers.
	/// </param>
	public ResourceTileSelection(int[] ids)
	{
		this.resource_tile_ids = new System.Collections.Generic.List<int>(ids);
	}
	/// <summary>
	/// Initializes a new instance of the <see cref="ResourceTileSelection"/> struct.
	/// </summary>
	/// <param name='ids'>
	/// Identifiers.
	/// </param>
	public ResourceTileSelection(System.Collections.Generic.List<int> ids)
	{
		this.resource_tile_ids = new System.Collections.Generic.List<int>(ids);
	}
	/*
	public ResourceTileSelection() {
		this.resource_tile_ids = new System.Collections.Generic.List<int>();
	}*/
		
	/// <summary>
	/// The resource_tile_ids field (on the server model).
	/// </summary>
	public System.Collections.Generic.List<int> resource_tile_ids;
		
	/// <summary>
	/// Converts to an array.
	/// </summary>
	/// <returns>
	/// The resource_tile_ids array.
	/// </returns>
	public int[] ToArray()
	{
		return this.resource_tile_ids.ToArray();
	}
	/// <summary>
	/// Add the specified id.
	/// </summary>
	/// <param name='id'>
	/// Identifier.
	/// </param>
	public void Add(int id)
	{
		resource_tile_ids.Add(id);
	}
	/// <summary>
	/// Clear this instance.
	/// </summary>
	public void Clear()
	{
		resource_tile_ids.Clear();
	}
	/// <summary>
	/// Removes at index.
	/// </summary>
	/// <param name='index'>
	/// Index.
	/// </param>
	public void RemoveAt(int index)
	{
		resource_tile_ids.RemoveAt(index);
	}
	public bool Remove( int id )
	{
		return resource_tile_ids.Remove(id);
	}
	/// <summary>
	/// Gets the count.
	/// </summary>
	/// <remarks>
	/// This is explicitly a method rather than a property, so it is not serialized when written to JSON
	/// </remarks>
	/// <value>
	/// The count.
	/// </value>
	public int Count()
	{
		return resource_tile_ids.Count;
	}
	/// <summary>
	/// Test whether selection contains the specified id.
	/// </summary>
	/// <param name='id'>
	/// Identifier
	/// </param>
	/// <returns>
	/// true if id is in selection, otherwise false.
	/// </returns>
	public bool Contains(int id)
	{
		return this.resource_tile_ids.Contains(id);
	}
	/// <summary>
	/// Gets or sets the <see cref="ResourceTileSelection"/> at the specified index.
	/// </summary>
	/// <param name='index'>
	/// Index.
	/// </param>
	public int this[int index] {
		get { return resource_tile_ids[index]; }
		set { resource_tile_ids[index] = value; }
	}
	/// <summary>
	/// Converts to json.
	/// </summary>
	/// <returns>
	/// The json representation.
	/// </returns>
	public string ToJson()
	{
		return JsonMapper.ToJson(this);
	}
	/// <summary>
	/// Gets the enumerator.
	/// </summary>
	/// <returns>
	/// The enumerator.
	/// </returns>
	public Enumerator GetEnumerator()
	{
		return new Enumerator(this);
	}
	/// <summary>
	/// Enumerator.
	/// </summary>
	public class Enumerator 
	{
		/// <summary>
		/// The current index.
		/// </summary>
		private int i;
		/// <summary>
		/// The resource tile selection.
		/// </summary>
		ResourceTileSelection resourceTileSelection;
		/// <summary>
		/// Initializes a new instance of the <see cref="ResourceTileSelection.Enumerator"/> class.
		/// </summary>
		/// <param name='selection'>
		/// Selection.
		/// </param>
		public Enumerator(ResourceTileSelection selection) {
			resourceTileSelection = selection;
			i = -1;
		}
		/// <summary>
		/// Moves the next.
		/// </summary>
		/// <returns>
		/// The next.
		/// </returns>
		public bool MoveNext() {
			++i;
			return i < resourceTileSelection.resource_tile_ids.Count;
		}
		/// <summary>
		/// Gets the current.
		/// </summary>
		/// <value>
		/// The current.
		/// </value>
		public int Current {
			get {
				return resourceTileSelection.resource_tile_ids[i];
			}
		}
	}
}