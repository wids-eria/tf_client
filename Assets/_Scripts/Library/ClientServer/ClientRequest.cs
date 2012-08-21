using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;

public class ClientRequestBase {
	public Action<object>	OnResponse;
	
	protected void	OnComplete( AWebCoroutine self ) {
		//OnResponse( JSONDeserializer.Deserialize(self.ResponseText) );
	}
}

public class ClientRequest : ClientRequestBase {
	protected WebCoroutine	webCoroutine;
	
	public ClientRequest( WebCoroutine.OnExecuteDelegate func ) {
		webCoroutine = new WebCoroutine( func );
		webCoroutine.OnComplete = OnComplete;
	}

	public void		Send( MonoBehaviour owner ) {
		webCoroutine.Start( owner );
	}
}

public class ClientRequest<Parm1Type> : ClientRequestBase {
	protected WebCoroutine<Parm1Type>	webCoroutine;
	
	public ClientRequest( WebCoroutine<Parm1Type>.OnExecuteDelegate func ) {
		webCoroutine = new WebCoroutine<Parm1Type>( func );
		webCoroutine.OnComplete = OnComplete;
	}
	
	public void		Send( Parm1Type parm1, MonoBehaviour owner ) {
		webCoroutine.Start( owner, parm1 );
	}
}

public class ClientRequest<Parm1Type, Parm2Type> : ClientRequestBase {
	protected WebCoroutine<Parm1Type, Parm2Type>	webCoroutine;
	
	public ClientRequest( WebCoroutine<Parm1Type, Parm2Type>.OnExecuteDelegate func ) {
		webCoroutine = new WebCoroutine<Parm1Type, Parm2Type>( func );
		webCoroutine.OnComplete = OnComplete;
	}
	
	public void		Send( Parm1Type parm1, Parm2Type parm2, MonoBehaviour owner ) {
		webCoroutine.Start( owner, parm1, parm2 );
	}
}

public class ClientRequest<Parm1Type, Parm2Type, Parm3Type> : ClientRequestBase {
	protected WebCoroutine<Parm1Type, Parm2Type, Parm3Type>	webCoroutine;
	
	public ClientRequest( WebCoroutine<Parm1Type, Parm2Type, Parm3Type>.OnExecuteDelegate func ) {
		webCoroutine = new WebCoroutine<Parm1Type, Parm2Type, Parm3Type>( func );
		webCoroutine.OnComplete = OnComplete;
	}
	
	public void		Send( Parm1Type parm1, Parm2Type parm2, Parm3Type parm3, MonoBehaviour owner ) {
		webCoroutine.Start( owner, parm1, parm2, parm3 );
	}
}