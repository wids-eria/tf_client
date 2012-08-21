using System;
using System.Collections.Generic;

// For temporary variables only. 
public class DataStructureLibrary<T> where T : class, new() {
	public readonly static DataStructureLibrary<T>	Instance = new DataStructureLibrary<T>();
	
	protected LinkedList<T>	available = new LinkedList<T>();
	protected LinkedList<T>	checkedOut = new LinkedList<T>();
	
	public T	CheckOut() {
		if( available.Count == 0 ) {
			available.AddLast( new T() );
		}
		
		LinkedListNode<T> node = available.Last;
		available.RemoveLast();

		checkedOut.AddLast( node );
		
		return node.Value;
	}
	
	public void	Return( T obj ) {
		LinkedListNode<T> node = checkedOut.Find( obj );
		if( node == null ) {
			return;
		}
		
		checkedOut.Remove( node );
		available.AddLast( node );
	}
	
	// Hiding the contructor
	protected DataStructureLibrary() {
	}
}