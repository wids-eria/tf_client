using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;

public abstract class ServerCommandDecoderBase {
	protected ASCIIEncoding 								encoder = new ASCIIEncoding();
	protected Dictionary<string, ServerCommandCodec>		codecs = new Dictionary<string, ServerCommandCodec>();
	protected Dictionary<string, IServerCommandDispatcher>	dispatchers = new Dictionary<string, IServerCommandDispatcher>();
	
	public void	AddCodec( ServerCommandCodec codec ) {
		codecs.Add( codec.Name, codec );
	}
	
	public IServerCommandDispatcher	FindDispatcher( string name ) {
		try {
			return dispatchers[ name ];
		} catch( KeyNotFoundException ) {
			IServerCommandDispatcher d = AllocateDispatcher();
			dispatchers.Add( name, d );
			return d;
		}
	}
	
	public abstract IServerCommandDispatcher	AllocateDispatcher();
	
	public void	OnReceive( byte[] data ) {
		Debug.Log( "ServerCommandDecoder.OnReceive..." );
		string str = encoder.GetString( data );

		string[] encodedData = str.Split( '`' );
		string name = encodedData[0];
		
		ServerCommandCodec codec = codecs[ name ];
		codec.Decode( encodedData );
		
		IServerCommandDispatcher dispatcher;
		if( !dispatchers.TryGetValue(codec.Name, out dispatcher) ) {
			return;// Maybe throw an exception
		}
		dispatcher.Dispatch( codec );
	}
}

/*
public class ServerCommandDecoder : ServerCommandDecoderBase {
	public void	AddHandler( string name, int recipient, Action handler ) {
		ServerCommandDispatcher dispatcher = (ServerCommandDispatcher)FindDispatcher( name );
		dispatcher.AddHandler( recipient, handler );
	}

	public void	RemoveHandler( string name, int recipient, Action handler ) {
		ServerCommandDispatcher dispatcher = (ServerCommandDispatcher)FindDispatcher( name );
		dispatcher.RemoveHandler( recipient, handler );
	}
	
	public override IServerCommandDispatcher	AllocateDispatcher() {
		return new ServerCommandDispatcher();
	}
}

public class ServerCommandDecoder<Parm1Type> : ServerCommandDecoderBase {
	public void	AddHandler( string name, int recipient, Action<Parm1Type> handler ) {
		ServerCommandDispatcher<Parm1Type> dispatcher = (ServerCommandDispatcher<Parm1Type>)FindDispatcher( name );
		dispatcher.AddHandler( recipient, handler );
	}

	public void	RemoveHandler( string name, int recipient, Action<Parm1Type> handler ) {
		ServerCommandDispatcher<Parm1Type> dispatcher = (ServerCommandDispatcher<Parm1Type>)FindDispatcher( name );
		dispatcher.RemoveHandler( recipient, handler );
	}
	
	public override IServerCommandDispatcher	AllocateDispatcher() {
		return new ServerCommandDispatcher<Parm1Type>();
	}
}

public class ServerCommandDecoder<Parm1Type, Parm2Type> : ServerCommandDecoderBase {
	public void	AddHandler( string name, int recipient, Action<Parm1Type, Parm2Type> handler ) {
		ServerCommandDispatcher<Parm1Type, Parm2Type> dispatcher = (ServerCommandDispatcher<Parm1Type, Parm2Type>)FindDispatcher( name );
		dispatcher.AddHandler( recipient, handler );
	}

	public void	RemoveHandler( string name, int recipient, Action<Parm1Type, Parm2Type> handler ) {
		ServerCommandDispatcher<Parm1Type, Parm2Type> dispatcher = (ServerCommandDispatcher<Parm1Type, Parm2Type>)FindDispatcher( name );
		dispatcher.RemoveHandler( recipient, handler );
	}
	
	public override IServerCommandDispatcher	AllocateDispatcher() {
		return new ServerCommandDispatcher<Parm1Type, Parm2Type>();
	}
}

public class ServerCommandDecoder<Parm1Type, Parm2Type, Parm3Type> : ServerCommandDecoderBase {
	public void	AddHandler( string name, int recipient, Action<Parm1Type, Parm2Type, Parm3Type> handler ) {
		ServerCommandDispatcher<Parm1Type, Parm2Type, Parm3Type> dispatcher = (ServerCommandDispatcher<Parm1Type, Parm2Type, Parm3Type>)FindDispatcher( name );
		dispatcher.AddHandler( recipient, handler );
	}

	public void	RemoveHandler( string name, int recipient, Action<Parm1Type, Parm2Type, Parm3Type> handler ) {
		ServerCommandDispatcher<Parm1Type, Parm2Type, Parm3Type> dispatcher = (ServerCommandDispatcher<Parm1Type, Parm2Type, Parm3Type>)FindDispatcher( name );
		dispatcher.RemoveHandler( recipient, handler );
	}
	
	public override IServerCommandDispatcher	AllocateDispatcher() {
		return new ServerCommandDispatcher<Parm1Type, Parm2Type, Parm3Type>();
	}
}

public class ServerCommandDecoder<Parm1Type, Parm2Type, Parm3Type, Parm4Type> : ServerCommandDecoderBase {
	public void	AddHandler( string name, int recipient, Action<Parm1Type, Parm2Type, Parm3Type, Parm4Type> handler ) {
		ServerCommandDispatcher<Parm1Type, Parm2Type, Parm3Type, Parm4Type> dispatcher = (ServerCommandDispatcher<Parm1Type, Parm2Type, Parm3Type, Parm4Type>)FindDispatcher( name );
		dispatcher.AddHandler( recipient, handler );
	}

	public void	RemoveHandler( string name, int recipient, Action<Parm1Type, Parm2Type, Parm3Type, Parm4Type> handler ) {
		ServerCommandDispatcher<Parm1Type, Parm2Type, Parm3Type, Parm4Type> dispatcher = (ServerCommandDispatcher<Parm1Type, Parm2Type, Parm3Type, Parm4Type>)FindDispatcher( name );
		dispatcher.RemoveHandler( recipient, handler );
	}
	
	public override IServerCommandDispatcher	AllocateDispatcher() {
		return new ServerCommandDispatcher<Parm1Type, Parm2Type, Parm3Type, Parm4Type>();
	}
}
*/