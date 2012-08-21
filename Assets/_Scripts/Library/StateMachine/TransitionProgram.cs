using System;
using System.Collections.Generic;
using Antlr.Runtime;
using Antlr.Runtime.Misc;
using Antlr.Runtime.Tree;

public class TransitionProgram<OwnerType> where OwnerType : class {
	public TransitionProgram() {
	}
	
	public bool	LoadProgram( string expression ) {
		ANTLRStringStream input = new ANTLRStringStream( expression );
        StateMachineTransitionLexer lexer = new StateMachineTransitionLexer( input );
        CommonTokenStream tokens = new CommonTokenStream( lexer );
        StateMachineTransitionParser parser = new StateMachineTransitionParser( tokens );
		
		CommonTree t = parser.process().Tree;
		CommonTreeNodeStream nodes = new CommonTreeNodeStream( t );
		StateMachineTransitionTree tree = new StateMachineTransitionTree( nodes );
		
		tree.process();
		
		if( !LoadInstructions(tree.Instructions) ) {
			return false;
		}
		
		return m_Interpreter.Execute();// Create all declared behaviors
	}
	
	public bool LoadInstructions( List<string> instructions ) {
		return m_Interpreter.InitFromInstructions( instructions );
	}
	
	public bool LoadInstructions( string[] instructions ) {
		return m_Interpreter.InitFromInstructions( instructions );
	}
	
	public bool	Execute( ref bool result ) {
		if( !m_Interpreter.Execute() ) {
			return false;
		}
		result = m_Interpreter.DataStack.Pop();
		return true;
	}
	
	public TransitionInterpreter.DeclareBehaviorDelegate	DeclareBehavior {
		get {
			return m_Interpreter.DeclareBehavior;
		}
		
		set {
			m_Interpreter.DeclareBehavior = value;
		}
	}

	public TransitionInterpreter.DereferenceBehaviorDelegate	DereferenceBehavior {
		get {
			return m_Interpreter.DereferenceBehavior;
		}
		
		set {
			m_Interpreter.DereferenceBehavior = value;
		}
	}
	
	protected TransitionInterpreter	m_Interpreter = new TransitionInterpreter();
}