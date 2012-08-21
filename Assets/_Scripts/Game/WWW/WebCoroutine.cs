using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public abstract class AWebCoroutine {
	public Action<AWebCoroutine>	OnComplete;
	protected bool					isDone = true;
	
	public bool cancelled = false;

	public AWebCoroutine() {
	}
	
	public HTTP.Request		Request;
	public HTTP.Response	Response {
		get {
			return Request.response;
		}
	}

	public virtual bool		IsDone {
		get {
			return isDone;
		}
	}
	
	public virtual bool		RequestIsDone {
		get {
			return Request.isDone;
		}
	}
	
	private float		progress = 0.0f;
	public float		Progress {
		get {
			if( Request.isDone ) {
				progress = 1.0f;
			}
			return progress;
		}
		
		protected set {
			float val = Mathf.Clamp( value, 0.0f, 1.0f );
			progress = val;
			WebRequests.tileDownloadProgress = val;
		}
	}
	
	public string		ResponseText {
		get {
			if( Response == null ) {
				return "";
			}
			return Response.Text;
		}
	}
	
	public int		Status {
		get {
			if( Response == null ) {
				return 0;
			}
			return Response.status;
		}
	}
	
	public bool		ProducedError {
		get {
			if( Request == null ) {
				return false;
			}

			return Request.ProducedError;
		}
	}
	
	protected virtual void	SignalSystemBusy() {
		if( GameGUIManager.use != null ) {
			GameGUIManager.use.SystemIsBusy = true;
		}
	}
	
	protected virtual void	SignalSystemIdle() {
		if( GameGUIManager.use != null ) {
			GameGUIManager.use.SystemIsBusy = false;
		}
	}
	
	protected virtual void	OnStart() {
		isDone = false;
		cancelled = false;
		Request = null;
	}
	
	protected virtual void	BroadcastOnComplete() {
		isDone = true;
		
		if( OnComplete != null ) {
			OnComplete( this );
		}
	}
}

public class WebCoroutine : AWebCoroutine {
	public delegate IEnumerator		OnExecuteDelegate(  AWebCoroutine self );
	
	protected List<OnExecuteDelegate>		executionList = new List<OnExecuteDelegate>();
	
	public WebCoroutine() {
	}
	
	public WebCoroutine( OnExecuteDelegate func ) {
		executionList.Add( func );
	}
	
	public void	AddExecutionHandler( OnExecuteDelegate func ) {
		executionList.Add( func );
	}
	
	public void	Start( MonoBehaviour owner ) {
		OnStart();
		owner.StartCoroutine( Execute() );
	}
	
	protected IEnumerator	Execute() {
		IEnumerator enumerator = null;

		OnExecuteDelegate d;
		for( int ix = 0; ix < executionList.Count; ++ix ) {
			if (cancelled) yield break;
			
			Progress = (float)ix / (float)executionList.Count;

			d = executionList[ ix ];

			enumerator = d( this );
			
			if( ProducedError ) {
				BroadcastOnComplete();
				yield break;
			}

			while( enumerator.MoveNext() ) {
				if (cancelled) yield break;
				yield return enumerator.Current;
			}
		}
		
		BroadcastOnComplete();
	}
}

public class WebCoroutine<Parm1Type> : AWebCoroutine {
	public delegate IEnumerator		OnExecuteDelegate( Parm1Type parm1, AWebCoroutine self );
	
	protected List<OnExecuteDelegate>		executionList = new List<OnExecuteDelegate>();
	
	public WebCoroutine() {
	}
	
	public WebCoroutine( OnExecuteDelegate func ) {
		executionList.Add( func );
	}
	
	public void	AddExecutionHandler( OnExecuteDelegate func ) {
		executionList.Add( func );
	}
	
	public void	Start( MonoBehaviour owner, Parm1Type parm1 ) {
		OnStart();
		owner.StartCoroutine( Execute(parm1) );
	}
	
	protected IEnumerator	Execute( Parm1Type parm1 ) {
		IEnumerator enumerator = null;

		OnExecuteDelegate d;
		for( int ix = 0; ix < executionList.Count; ++ix ) {
			if (cancelled) yield break;
			Progress = (float)ix / (float)executionList.Count;

			d = executionList[ ix ];

			enumerator = d( parm1, this );
			
			if( ProducedError ) {
				BroadcastOnComplete();
				yield break;
			}
			
			while( enumerator.MoveNext() ) {
				if (cancelled) yield break;
				yield return enumerator.Current;
			}
		}
		
		BroadcastOnComplete();
	}
}

public class WebCoroutine<Parm1Type, Parm2Type> : AWebCoroutine {
	public delegate IEnumerator		OnExecuteDelegate( Parm1Type parm1, Parm2Type parm2, AWebCoroutine self );
	
	protected List<OnExecuteDelegate>		executionList = new List<OnExecuteDelegate>();
	
	public WebCoroutine() {
	}
	
	public WebCoroutine( OnExecuteDelegate func ) {
		executionList.Add( func );
	}
	
	public void	AddExecutionHandler( OnExecuteDelegate func ) {
		executionList.Add( func );
	}
	
	public void	Start( MonoBehaviour owner, Parm1Type parm1, Parm2Type parm2 ) {
		OnStart();
		owner.StartCoroutine( Execute(parm1, parm2) );
	}
	
	protected IEnumerator	Execute( Parm1Type parm1, Parm2Type parm2 ) {
		IEnumerator enumerator = null;
		
		OnExecuteDelegate d;
		for( int ix = 0; ix < executionList.Count; ++ix ) {
			if (cancelled) yield break;
			Progress = (float)ix / (float)executionList.Count;

			d = executionList[ ix ];
			enumerator = d( parm1, parm2, this );
			
			if( ProducedError ) {
				BroadcastOnComplete();
				yield break;
			}
			
			while( enumerator.MoveNext() ) {
				if (cancelled) yield break;
				yield return enumerator.Current;
			}
		}
		
		BroadcastOnComplete();
	}
}

public class WebCoroutine<Parm1Type, Parm2Type, Parm3Type> : AWebCoroutine {
	public delegate IEnumerator		OnExecuteDelegate( Parm1Type parm1, Parm2Type parm2, Parm3Type parm3, AWebCoroutine self );
	
	protected List<OnExecuteDelegate>		executionList = new List<OnExecuteDelegate>();
	
	public WebCoroutine() {
	}
	
	public WebCoroutine( OnExecuteDelegate func ) {
		executionList.Add( func );
	}
	
	public void	AddExecutionHandler( OnExecuteDelegate func ) {
		executionList.Add( func );
	}
	
	public void	Start( MonoBehaviour owner, Parm1Type parm1, Parm2Type parm2, Parm3Type parm3 ) {
		OnStart();
		owner.StartCoroutine( Execute(parm1, parm2, parm3) );
	}
	
	protected IEnumerator	Execute( Parm1Type parm1, Parm2Type parm2, Parm3Type parm3 ) {
		IEnumerator enumerator = null;

		OnExecuteDelegate d;
		for( int ix = 0; ix < executionList.Count; ++ix ) {
			if (cancelled) yield break;
			Progress = (float)ix / (float)executionList.Count;

			d = executionList[ ix ];

			enumerator = d( parm1, parm2, parm3, this );
			
			if( ProducedError ) {
				BroadcastOnComplete();
				yield break;
			}
			
			while( enumerator.MoveNext() ) {
				if (cancelled) yield break;
				yield return enumerator.Current;
			}
		}
		
		BroadcastOnComplete();
	}
}