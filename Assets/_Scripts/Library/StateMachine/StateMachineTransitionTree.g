tree grammar StateMachineTransitionTree;

options {
    language=CSharp2;
    tokenVocab=StateMachineTransition;
    ASTLabelType=CommonTree;
    //output=AST;
}

@header {
#pragma warning disable 3021
#pragma warning disable 0414
}

@members {
public Dictionary<string, int> Labels = new Dictionary<string, int>();
public List<string>	Instructions = new List<string>();

public void process() {
	prog();
}
}

@init {
}

prog	:	^(PROG declBlock? expr)
	;
	
////////////////////////////////////////////////////

declBlock	:	^(VARDECLBLOCK decl+)
	;

decl	:	^(VARDECL n=name t=type a=args?) {
			Instructions.Add( "DeclareBehavior " + t + " " + n + " " +  a );
		}
	;

type returns[ string type ]
	:	^(VARTYPE ID) {
			$type = $ID.text;
		}
	;
	
name returns[ string name ]
	:	^(VARNAME ID) {
			$name = $ID.text;
		}
	;

args returns[ string val ]
	:	^(VARARGS a=arg+) { $val = a; }
	;

arg returns[ string val ]
	:	^(ARGTYPE_DELEGATE ID) { $val = $ID.text; }
	|	^(ARGTYPE_INT INT) { $val = $INT.text; }
	|	^(ARGTYPE_FLOAT FLOAT) { $val = $FLOAT.text; }
	;

////////////////////////////////////////////////////

expr 	:
	^( PROGEXPR { Instructions.Add("NoOp"); Labels.Add("Start", Instructions.Count); } orExpr+ PROGEXPR_RESTART { Instructions.Add( "Jump " + (Labels["Start"] - Instructions.Count) ); } )
	;

orExpr
 	:	^('|' andExpr andExpr) { Instructions.Add( "Or" ); }
 	|	andExpr
	;
	
andExpr
	:	^('&' term term) { Instructions.Add( "And" ); }
	|	term
	;
	
term 	:	^( NOT atom ) { Instructions.Add( "Not" ); }
	|	atom
	;
	
atom	
	:	ID { Instructions.Add( "PushBehavior " + $ID.text ); }
	;