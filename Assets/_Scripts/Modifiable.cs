using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public delegate void PostChange<T>(ref T obj);

public class Modifiable<T> {
	
	protected T m_prev;
	protected T m_value;
	
	protected Action 		OnModified;
	protected PostChange<T> PostModified;
	
	public T Value { get { return Get(); } set { Set(value); } }
	
	public Modifiable() {
	}
	public Modifiable(T startVal) {
		m_value = startVal;
	}
	public Modifiable(T startVal, params Action[] modDelegate) : this(startVal) {
		foreach(Action m in modDelegate) {
			AddBehavior(m);
		}
	}
	
	public void AddBehavior(Action behavior) {
		OnModified += behavior;
	}
	public void AddBehavior(PostChange<T> behavior) {
		PostModified += behavior;
	}
	
	public void RemoveBehavior(Action behavior) {
		OnModified -= behavior;
	}
	public void RemoveBehavior(PostChange<T> behavior) {
		PostModified -= behavior;
	}
	public T Get() {
		return m_value;
	}
	public void Set(T value) {
		if (!m_value.Equals(value)) {
			DoSet(value);
		}
	}
	
	protected virtual void DoSet(T value) {
		m_prev = m_value; 
		m_value = value; 
		if (OnModified != null) OnModified();
		if (PostModified != null) PostModified(ref m_value);
	}
}

