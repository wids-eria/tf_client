using UnityEngine;
using System;

/// <summary>
/// Base class for messages sent through the messenger
/// </summary>
public class MessageAM : System.Object
{
	/// <summary>
	/// The hash for the listeners for the message type.
	/// </summary>
	public string listenerType { get; private set; }
	/// <summary>
	/// Name of the function to be called on listeners when the message is sent.
	/// </summary>
	public string functionName { get; protected set; }
	
	/// <summary>
	/// Initializes a new instance of the <see cref="Message"/> class.
	/// </summary>
	/// <param name='type'>
	/// Type.
	/// </param>
	public MessageAM(string type)
	{
		listenerType = type;
#if !UNITY_FLASH
		// function name for MessageMyMessage becomes _MyMessage()
		functionName = string.Format("_{0}", this.GetType().ToString().Substring(7));
#endif
	}
	
	/// <summary>
	/// Inits a message of type with a specified method name
	/// </summary>
	/// <param name='type'>
	/// Type.
	/// </param>
	/// <param name='methodName'>
	/// Method name.
	/// </param>
	public MessageAM(string type, string methodName)
	{
		listenerType = type;
		functionName = methodName;
	}
}

/*
public abstract class IMessage {
	public string	Name;
	
	protected IMessage( string name ) {
		Name = name;
	}
}

public class Message : IMessage {
	public Message( string name ) : base( name ) {
	}
	
	public void		Broadcast() {
		Messenger.Broadcast( Name );
	}
	
	public void		AddListener( Action del ) {
		Messenger.AddListener( Name, del );
	}
	
	public void		RemoveListener( Action del ) {
		Messenger.RemoveListener( Name, del );
	}
}

public class Message<Parm1Type> : IMessage {
	public Message( string name ) : base( name ) {
	}
	
	public void		Broadcast( Parm1Type parm1 ) {
		Messenger<Parm1Type>.Broadcast( Name, parm1 );
	}
	
	public void		AddListener( Action<Parm1Type> del ) {
		Messenger<Parm1Type>.AddListener( Name, del );
	}
	
	public void		RemoveListener( Action<Parm1Type> del ) {
		Messenger<Parm1Type>.RemoveListener( Name, del );
	}
}

public class Message<Parm1Type, Parm2Type> : IMessage {
	public Message( string name ) : base( name ) {
	}
	
	public void		Broadcast( Parm1Type parm1, Parm2Type parm2 ) {
		Messenger<Parm1Type, Parm2Type>.Broadcast( Name, parm1, parm2 );
	}
	
	public void		AddListener( Action<Parm1Type, Parm2Type> del ) {
		Messenger<Parm1Type, Parm2Type>.AddListener( Name, del );
	}
	
	public void		RemoveListener( Action<Parm1Type, Parm2Type> del ) {
		Messenger<Parm1Type, Parm2Type>.RemoveListener( Name, del );
	}
}

public class Message<Parm1Type, Parm2Type, Parm3Type> : IMessage {
	public Message( string name ) : base( name ) {
	}
	
	public void		Broadcast( Parm1Type parm1, Parm2Type parm2, Parm3Type parm3 ) {
		Messenger<Parm1Type, Parm2Type, Parm3Type>.Broadcast( Name, parm1, parm2, parm3 );
	}
	
	public void		AddListener( Action<Parm1Type, Parm2Type, Parm3Type> del ) {
		Messenger<Parm1Type, Parm2Type, Parm3Type>.AddListener( Name, del );
	}
	
	public void		RemoveListener( Action<Parm1Type, Parm2Type, Parm3Type> del ) {
		Messenger<Parm1Type, Parm2Type, Parm3Type>.RemoveListener( Name, del );
	}
}
*/