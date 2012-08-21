using System;
using System.Collections.Generic;
using System.Reflection;

public class ServerCommandCodec {
	public string 		Name;
	public int			Recipient;
	public List<object>	Arguments = new List<object>();
	
	public bool		HasRecipient {
		get {
			return Recipient >= 0;
		}
	}
	
	public ServerCommandCodec() {
		
	}
	
	public ServerCommandCodec( string name ) {
		Name = name;		
	}

	public virtual void	Decode( string[] encodedData ) {
		Parse( encodedData );
	}
	
	protected virtual void	Parse( string[] data ) {
		Name = data[0];
		Recipient = int.Parse( data[1] );
	}
	
	protected virtual object	Parse<T>( string data ) where T : new() {
		string[] args = new string[] { data };
		object val = null;
		try {
			try {
				return typeof(T).InvokeMember( "Parse", BindingFlags.Static|BindingFlags.Public, null, null, args );
			} catch( Exception ) {
				val = new T();
				typeof(T).InvokeMember( "Parse", BindingFlags.Instance|BindingFlags.Public|BindingFlags.InvokeMethod, null, val, args );
				return val;
			}
		} catch( Exception ) {
			return null;
		}
	}
}

public class ServerCommandCodec<Parm1Type> : ServerCommandCodec where Parm1Type : new() {
	public ServerCommandCodec( string name ) : base(name) {
	}
	
	protected override void	Parse( string[] data ) {
		base.Parse( data );

		Arguments.Add( Parse<Parm1Type>(data[2]) );
	}
}

public class ServerCommandCodec<Parm1Type, Parm2Type> : ServerCommandCodec<Parm1Type> where Parm1Type : new() where Parm2Type : new() {
	public ServerCommandCodec( string name ) : base(name) {
	}

	protected override void	Parse( string[] data ) {
		base.Parse( data );

		Arguments.Add( Parse<Parm2Type>(data[3]) );
	}
}