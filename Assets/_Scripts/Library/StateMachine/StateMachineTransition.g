grammar StateMachineTransition;

options {
    language=CSharp2;
    output=AST;
    ASTLabelType=CommonTree;
}

tokens {
	PROG;
	PROGEXPR;
	PROGEXPR_RESTART;
	VARDECLBLOCK;
	VARDECL;
	VARTYPE;
	VARNAME;
	VARARGS;
	ARGTYPE_INT;
	ARGTYPE_FLOAT;
	ARGTYPE_DELEGATE;
}

@header {
#pragma warning disable 3021
#pragma warning disable 0414
}

@lexer::header {
#pragma warning disable 3021
#pragma warning disable 0414
}

@members {
public AstParserRuleReturnScope<CommonTree, IToken> process() {
	TreeAdaptor = new CommonTreeAdaptor();
	return prog();
}
}

@init {
}

prog	:	 declBlock? expr -> ^(PROG declBlock? expr)
	;

////////////////////////////////////////////////////

declBlock	:	ignore* '{' ignore* decl+ ignore* '}' ignore* -> ^(VARDECLBLOCK decl+)
	;

decl	:	ignore* type ignore* name ignore* '(' ignore* args? ')' ignore* ';' -> ^(VARDECL name type args?)
	;
	
type	:	ID -> ^(VARTYPE ID)
	;
	
name 
	:	ID -> ^(VARNAME ID)
	;
	
args	:	arg ignore* ( ',' ignore* arg ignore* )* -> ^(VARARGS arg+)
	;

arg	:	ID -> ^(ARGTYPE_DELEGATE ID)
	|	INT -> ^(ARGTYPE_INT INT)
	|	FLOAT -> ^(ARGTYPE_FLOAT FLOAT)
	;

////////////////////////////////////////////////////

expr	:	ignore* orExpr+ ignore* -> ^( PROGEXPR orExpr+ PROGEXPR_RESTART )
	; 

orExpr
	:	(andExpr -> andExpr) (ignore* '|' ignore* a=andExpr -> ^('|' $orExpr $a))*
	;
	
andExpr
	:	(term -> term) (ignore* '&' ignore* a=term -> ^('&' $andExpr $a))*
	;
	
term 	:	NOT ID -> ^( NOT ID )
	|	ID
	;
	
atom
	:	ID
	| 	'(' ignore* orExpr ignore* ')' -> orExpr
	;
	
ignore 	:	(WS|NEWLINE)
	;

NOT		:	'!';
ID		:	('a'..'z'|'A'..'Z'|'_')+;
FLOAT	: 	INT '.' INT ;
INT  		: 	('0'..'9')+;
NEWLINE	:	'\r'? '\n';
WS		:	(' '|'\t')+;