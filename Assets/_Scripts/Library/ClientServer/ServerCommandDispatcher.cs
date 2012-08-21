using System;
using System.Collections.Generic;

public interface IServerCommandDispatcher {
	void	Dispatch( ServerCommandCodec sc );
}

public abstract class ServerCommandDispatcherBase : IServerCommandDispatcher {
	protected Dictionary<int, Delegate>	listeners = new Dictionary<int, Delegate>();
	
	public void	AddHandler( int recipientId, Delegate handler ) {
		Delegate action = null;
		
		try {
			action = listeners[ recipientId ];
		} catch( KeyNotFoundException ) {
			return;
		}

		listeners.Add( recipientId, Delegate.Combine(action, handler) );
	}
	
	public void	RemoveHandler( int recipientId, Delegate handler ) {
		Delegate action = null;
		
		if( !listeners.TryGetValue(recipientId, out action) ) {
			return;
		}
		
		action = Delegate.Remove( action, handler );
		listeners[ recipientId ] = action;
	}
	
	public void	Dispatch( ServerCommandCodec sc ) {
		if( sc.HasRecipient ) {	
			Send( sc );
		} else {
			Broadcast( sc );
		}
	}
	
	protected abstract void	Send( ServerCommandCodec sc );
	
	protected abstract void	Broadcast( ServerCommandCodec sc );
}

public class ServerCommandDispatcher : ServerCommandDispatcherBase {
	protected override void	Send( ServerCommandCodec sc ) {
		Action a = (Action)listeners[ sc.Recipient ];
		a();
	}
	
	protected override void	Broadcast( ServerCommandCodec sc ) {
		Action a;
		foreach( KeyValuePair<int, Delegate> kv in listeners ) {
			a = (Action)kv.Value;
			a();
		}
	}
}

public class ServerCommandDispatcher<Parm1Type> : ServerCommandDispatcherBase {
	protected override void	Send( ServerCommandCodec sc ) {
		Action<Parm1Type> a = listeners[ sc.Recipient ] as Action<Parm1Type>;
		a( (Parm1Type)sc.Arguments[0] );
	}
	
	protected override void	Broadcast( ServerCommandCodec sc ) {
		Action<Parm1Type> a;
		foreach( KeyValuePair<int, Delegate> kv in listeners ) {
			a = (Action<Parm1Type>)kv.Value;
			a( (Parm1Type)sc.Arguments[0] );
		}
	}
}

public class ServerCommandDispatcher<Parm1Type, Parm2Type> : ServerCommandDispatcherBase {
	protected override void	Send( ServerCommandCodec sc ) {
		Action<Parm1Type, Parm2Type> a = listeners[ sc.Recipient ] as Action<Parm1Type, Parm2Type>;
		a( (Parm1Type)sc.Arguments[0], (Parm2Type)sc.Arguments[1] );
	}
	
	protected override void	Broadcast( ServerCommandCodec sc ) {
		Action<Parm1Type, Parm2Type> a;
		foreach( KeyValuePair<int, Delegate> kv in listeners ) {
			a = kv.Value as Action<Parm1Type, Parm2Type>;
			a( (Parm1Type)sc.Arguments[0], (Parm2Type)sc.Arguments[1] );
		}
	}
}

public class ServerCommandDispatcher<Parm1Type, Parm2Type, Parm3Type> : ServerCommandDispatcherBase {
	protected override void	Send( ServerCommandCodec sc ) {
		Action<Parm1Type, Parm2Type, Parm3Type> a = listeners[ sc.Recipient ] as Action<Parm1Type, Parm2Type, Parm3Type>;
		a( (Parm1Type)sc.Arguments[0], (Parm2Type)sc.Arguments[1], (Parm3Type)sc.Arguments[2] );
	}
	
	protected override void	Broadcast( ServerCommandCodec sc ) {
		Action<Parm1Type, Parm2Type, Parm3Type>	a;
		foreach( KeyValuePair<int, Delegate> kv in listeners ) {
			a = kv.Value as Action<Parm1Type, Parm2Type, Parm3Type>;
			a( (Parm1Type)sc.Arguments[0], (Parm2Type)sc.Arguments[1], (Parm3Type)sc.Arguments[2] );
		}
	}
}

public class ServerCommandDispatcher<Parm1Type, Parm2Type, Parm3Type, Parm4Type> : ServerCommandDispatcherBase {
	protected override void	Send( ServerCommandCodec sc ) {
		Action<Parm1Type, Parm2Type, Parm3Type, Parm4Type> a = listeners[ sc.Recipient ] as Action<Parm1Type, Parm2Type, Parm3Type, Parm4Type>;
		a( (Parm1Type)sc.Arguments[0], (Parm2Type)sc.Arguments[1], (Parm3Type)sc.Arguments[2], (Parm4Type)sc.Arguments[3] );
	}
	
	protected override void	Broadcast( ServerCommandCodec sc ) {
		Action<Parm1Type, Parm2Type, Parm3Type, Parm4Type> a;
		foreach( KeyValuePair<int, Delegate> kv in listeners ) {
			a = kv.Value as Action<Parm1Type, Parm2Type, Parm3Type, Parm4Type>;
			a( (Parm1Type)sc.Arguments[0], (Parm2Type)sc.Arguments[1], (Parm3Type)sc.Arguments[2], (Parm4Type)sc.Arguments[3] );
		}
	}
}