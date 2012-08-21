grammar JSONRecognizer;

options {
	language=CSharp2;
	output = AST;
	ASTLabelType=CommonTree;
}

tokens {
	STRING; NUMBER; OBJECT; FIELD; ARRAY;
	COMMA = ',';
	TRUE; FALSE; NULL;
}

@header {
#pragma warning disable 0414
#pragma warning disable 3021

using System.Text.RegularExpressions;
using System;
}

@lexer::header {
#pragma warning disable 0414
#pragma warning disable 3021
}

// Optional step: Disable automatic error recovery
@members {
	public CommonTree 	ToTree() {
		TreeAdaptor = new CommonTreeAdaptor();
		return json().Tree;
	}
	
	protected int StartPos() {
		return StartPos(input.LT(1));
    	}
	
	protected int StartPos( IToken t ) {
		return t.StartIndex;
    	}
    	
    	protected int StopPos() {
		return StopPos(input.LT(1));
    	}
    	
    	protected int StopPos( IToken t ) {
		return t.StopIndex;
    	}
    	
    	protected void	LogPos( string prefix, int pos ) {
    		Console.Out.Write( string.Format("{0} = {1}\n", prefix, pos) );
    	}
}
// Alter code generation so catch-clauses get replace with 
// this action. 
@rulecatch { 
}

json 
	:	value EOF;

value
	: string
	| number
	| object
	| array
	| 'true' -> TRUE
	| 'false' -> FALSE
	| 'null' -> NULL
	;

string 	
	: String -> ^(STRING String)
	;

// If you want to conform to the RFC, use a validating semantic predicate to check the result.
number	
	: n=Number {Regex.Match(n.Text, "(0|(-?[1-9]\\d*))(\\.\\d+)?").Success}? Exponent? -> ^(NUMBER Number Exponent?)
	;

object
	: '{' members? '}' -> ^(OBJECT members?)
	;
	
array
	: '[' elements? ']' -> ^(ARRAY elements?)
	;

elements
	: value (COMMA! value)*
	;
	
members
	: pair (COMMA! pair)*
	;
	 
pair	
	: String ':' value -> ^(FIELD String value)
	;

Number
	: '-'? Digit+ ( '.' Digit+)?
	;

Exponent
	: ('e'|'E') '-'? ('1'..'9') Digit*
	;

String 	
	: '"' ( EscapeSequence | ~('\u0000'..'\u001f' | '\\' | '\"' ) )* '"'
	;

WS
	: (' '|'\n'|'\r'|'\t' )+ {Skip();} ; // ignore whitespace 

fragment EscapeSequence
    	:   '\\' (UnicodeEscape |'b'|'t'|'n'|'f'|'r'|'\"'|'\''|'\\'|'\/')
    	;

fragment UnicodeEscape
	: 'u' HexDigit HexDigit HexDigit HexDigit
	;

fragment HexDigit
	: '0'..'9' | 'A'..'F' | 'a'..'f'
	;

fragment Digit
	: '0'..'9'
	;