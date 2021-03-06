//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 3.4
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// $ANTLR 3.4 /Users/Abahr/TrailsForward/Assets/_Scripts/Library/JSON/Internal/JSONRecognizerTree.g 2012-07-27 09:12:07

// The variable 'variable' is assigned but its value is never used.
#pragma warning disable 168, 219
// Unreachable code detected.
#pragma warning disable 162
// Missing XML comment for publicly visible type or member 'Type_or_Member'
#pragma warning disable 1591


#pragma warning disable 0414
#pragma warning disable 3021

using System;
using System.Text;
using System.Linq;
using System.IO;


using System.Collections.Generic;
using Antlr.Runtime;
using Antlr.Runtime.Misc;
using Antlr.Runtime.Tree;
using RewriteRuleITokenStream = Antlr.Runtime.Tree.RewriteRuleTokenStream;
using ConditionalAttribute = System.Diagnostics.ConditionalAttribute;

[System.CodeDom.Compiler.GeneratedCode("ANTLR", "3.4")]
//[System.CLSCompliant(false)]
public partial class JSONRecognizerTree : Antlr.Runtime.Tree.TreeParser
{
	internal static readonly string[] tokenNames = new string[] {
		"<invalid>", "<EOR>", "<DOWN>", "<UP>", "ARRAY", "COMMA", "Digit", "EscapeSequence", "Exponent", "FALSE", "FIELD", "HexDigit", "NULL", "NUMBER", "Number", "OBJECT", "STRING", "String", "TRUE", "UnicodeEscape", "WS", "':'", "'['", "']'", "'false'", "'null'", "'true'", "'{'", "'}'"
	};
	public const int EOF=-1;
	public const int T__21=21;
	public const int T__22=22;
	public const int T__23=23;
	public const int T__24=24;
	public const int T__25=25;
	public const int T__26=26;
	public const int T__27=27;
	public const int T__28=28;
	public const int ARRAY=4;
	public const int COMMA=5;
	public const int Digit=6;
	public const int EscapeSequence=7;
	public const int Exponent=8;
	public const int FALSE=9;
	public const int FIELD=10;
	public const int HexDigit=11;
	public const int NULL=12;
	public const int NUMBER=13;
	public const int Number=14;
	public const int OBJECT=15;
	public const int STRING=16;
	public const int String=17;
	public const int TRUE=18;
	public const int UnicodeEscape=19;
	public const int WS=20;

	#if ANTLR_DEBUG
		private static readonly bool[] decisionCanBacktrack =
			new bool[]
			{
				false, // invalid decision
				false, false, false, false
			};
	#else
		private static readonly bool[] decisionCanBacktrack = new bool[0];
	#endif
	public JSONRecognizerTree(ITreeNodeStream input)
		: this(input, new RecognizerSharedState())
	{
	}
	public JSONRecognizerTree(ITreeNodeStream input, RecognizerSharedState state)
		: base(input, state)
	{
		OnCreated();
	}

	public override string[] TokenNames { get { return JSONRecognizerTree.tokenNames; } }
	public override string GrammarFileName { get { return "/Users/Abahr/TrailsForward/Assets/_Scripts/Library/JSON/Internal/JSONRecognizerTree.g"; } }


		public object	Deserialize() {
			return jsonTree();
		}

	    private Object extractNumber(CommonTree numberToken, CommonTree exponentToken) {
	        String numberBody = numberToken.Text;
	        String exponent = (exponentToken == null) ? null : exponentToken.Text.Substring(1); // remove the 'e' prefix if there
	        bool isReal = numberBody.IndexOf('.') >= 0 || exponent != null;
	        if (!isReal) {
	            return int.Parse(numberBody);
	        } else {
	            double result = double.Parse(numberBody);
	            if (exponent != null) {
	                result = result * Math.Pow(10.0f, double.Parse(exponent));
	            }
	            return result;
	        }
	    }
	    
	    private String extractString(CommonTree token) {
	        // StringBuffers are an efficient way to modify strings
	        StringBuilder sb = new StringBuilder(token.Text);
	        // Process character escapes
	        int startPoint = 1; // skip initial quotation mark
	        for (;;) {
	            int slashIndex = sb.ToString().IndexOf("\\", startPoint); // search for a single backslash
	            if (slashIndex == -1) break;
	            // Else, we have a backslash
	            char escapeType = Enumerable.ToArray<char>(sb.ToString())[slashIndex + 1];
	            switch (escapeType) {
	                case'u':
	                    // Unicode escape.
	                    String unicode = extractUnicode(sb, slashIndex);
	                    sb.Replace(slashIndex, slashIndex + 6, unicode); // backspace
	                    break; // back to the loop

	                    // note: Java's character escapes match JSON's, which is why it looks like we're replacing
	                // "\b" with "\b". We're actually replacing 2 characters (slash-b) with one (backspace).
	                case 'b':
	                    sb.Replace(slashIndex, slashIndex + 2, "\b"); // backspace
	                    break;

	                case 't':
	                    sb.Replace(slashIndex, slashIndex + 2, "\t"); // tab
	                    break;

	                case 'n':
	                    sb.Replace(slashIndex, slashIndex + 2, "\n"); // newline
	                    break;

	                case 'f':
	                    sb.Replace(slashIndex, slashIndex + 2, "\f"); // form feed
	                    break;

	                case 'r':
	                    sb.Replace(slashIndex, slashIndex + 2, "\r"); // return
	                    break;

	                case '\'':
	                    sb.Replace(slashIndex, slashIndex + 2, "\'"); // single quote
	                    break;

	                case '\"':
	                    sb.Replace(slashIndex, slashIndex + 2, "\""); // double quote
	                    break;

	                case '\\':
	                    sb.Replace(slashIndex, slashIndex + 2, "\\"); // backslash
	                    break;
	                    
	                case '/':
	                    sb.Replace(slashIndex, slashIndex + 2, "/"); // solidus
	                    break;

	            }
	            startPoint = slashIndex+1;

	        }

	        // remove surrounding quotes
	        sb.DeleteCharAt(0);
	        sb.DeleteCharAt(sb.Length - 1);

	        return sb.ToString();
	    }

	    private String extractUnicode(StringBuilder sb, int slashIndex) {
	        // Gather the 4 hex digits, convert to an integer, translate the number to a unicode char, replace
	        String result;
	        String code = sb.ToString(slashIndex + 2, 4);
	        int charNum = int.Parse(code); // hex to integer
	        // There's no simple way to go from an int to a unicode character.
	        // We'll have to pass this through an output stream writer to do
	        // the conversion.
	        try {
	            MemoryStream baos = new MemoryStream();
	            StreamWriter osw = new StreamWriter(baos, Encoding.UTF8);
	            osw.Write(charNum);
	            osw.Flush();
	            result = baos.ToString(); // Thanks to Silvester Pozarnik for the tip about adding "UTF-8" here
	        } catch (Exception e) {
	            //e.StackTrace;
	            result = null;
	        }
	        return result;
	    }


	[Conditional("ANTLR_TRACE")]
	protected virtual void OnCreated() {}
	[Conditional("ANTLR_TRACE")]
	protected virtual void EnterRule(string ruleName, int ruleIndex) {}
	[Conditional("ANTLR_TRACE")]
	protected virtual void LeaveRule(string ruleName, int ruleIndex) {}

	#region Rules

	[Conditional("ANTLR_TRACE")]
	protected virtual void EnterRule_jsonTree() {}
	[Conditional("ANTLR_TRACE")]
	protected virtual void LeaveRule_jsonTree() {}

	// $ANTLR start "jsonTree"
	// /Users/Abahr/TrailsForward/Assets/_Scripts/Library/JSON/Internal/JSONRecognizerTree.g:128:1: jsonTree returns [object node] : n= value EOF ;
	[GrammarRule("jsonTree")]
	private object jsonTree()
	{
		EnterRule_jsonTree();
		EnterRule("jsonTree", 1);
		TraceIn("jsonTree", 1);
	    object node = default(object);


	    object n = default(object);

		try { DebugEnterRule(GrammarFileName, "jsonTree");
		DebugLocation(128, 1);
		try
		{
			// /Users/Abahr/TrailsForward/Assets/_Scripts/Library/JSON/Internal/JSONRecognizerTree.g:129:2: (n= value EOF )
			DebugEnterAlt(1);
			// /Users/Abahr/TrailsForward/Assets/_Scripts/Library/JSON/Internal/JSONRecognizerTree.g:129:4: n= value EOF
			{
			DebugLocation(129, 5);
			PushFollow(Follow._value_in_jsonTree54);
			n=value();
			PopFollow();

			DebugLocation(129, 12);
			Match(input,EOF,Follow._EOF_in_jsonTree56); 
			DebugLocation(129, 16);
			 node = n; 

			}

		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		}
		finally
		{
			TraceOut("jsonTree", 1);
			LeaveRule("jsonTree", 1);
			LeaveRule_jsonTree();
	    }
	 	DebugLocation(130, 1);
		} finally { DebugExitRule(GrammarFileName, "jsonTree"); }
		return node;

	}
	// $ANTLR end "jsonTree"


	[Conditional("ANTLR_TRACE")]
	protected virtual void EnterRule_value() {}
	[Conditional("ANTLR_TRACE")]
	protected virtual void LeaveRule_value() {}

	// $ANTLR start "value"
	// /Users/Abahr/TrailsForward/Assets/_Scripts/Library/JSON/Internal/JSONRecognizerTree.g:132:1: value returns [object node] : (s= string |n= number |o= object |a= array | TRUE | FALSE | NULL );
	[GrammarRule("value")]
	private object value()
	{
		EnterRule_value();
		EnterRule("value", 2);
		TraceIn("value", 2);
	    object node = default(object);


	    string s = default(string);
	    object n = default(object);
	    Dictionary<string, object> o = default(Dictionary<string, object>);
	    List<object> a = default(List<object>);

		try { DebugEnterRule(GrammarFileName, "value");
		DebugLocation(132, 1);
		try
		{
			// /Users/Abahr/TrailsForward/Assets/_Scripts/Library/JSON/Internal/JSONRecognizerTree.g:133:2: (s= string |n= number |o= object |a= array | TRUE | FALSE | NULL )
			int alt1=7;
			try { DebugEnterDecision(1, decisionCanBacktrack[1]);
			switch (input.LA(1))
			{
			case STRING:
				{
				alt1 = 1;
				}
				break;
			case NUMBER:
				{
				alt1 = 2;
				}
				break;
			case OBJECT:
				{
				alt1 = 3;
				}
				break;
			case ARRAY:
				{
				alt1 = 4;
				}
				break;
			case TRUE:
				{
				alt1 = 5;
				}
				break;
			case FALSE:
				{
				alt1 = 6;
				}
				break;
			case NULL:
				{
				alt1 = 7;
				}
				break;
			default:
				{
					NoViableAltException nvae = new NoViableAltException("", 1, 0, input);
					DebugRecognitionException(nvae);
					throw nvae;
				}
			}

			} finally { DebugExitDecision(1); }
			switch (alt1)
			{
			case 1:
				DebugEnterAlt(1);
				// /Users/Abahr/TrailsForward/Assets/_Scripts/Library/JSON/Internal/JSONRecognizerTree.g:133:4: s= string
				{
				DebugLocation(133, 5);
				PushFollow(Follow._string_in_value75);
				s=@string();
				PopFollow();

				DebugLocation(133, 13);
				 node = s; 

				}
				break;
			case 2:
				DebugEnterAlt(2);
				// /Users/Abahr/TrailsForward/Assets/_Scripts/Library/JSON/Internal/JSONRecognizerTree.g:134:4: n= number
				{
				DebugLocation(134, 5);
				PushFollow(Follow._number_in_value84);
				n=number();
				PopFollow();

				DebugLocation(134, 13);
				 node = n; 

				}
				break;
			case 3:
				DebugEnterAlt(3);
				// /Users/Abahr/TrailsForward/Assets/_Scripts/Library/JSON/Internal/JSONRecognizerTree.g:135:4: o= object
				{
				DebugLocation(135, 5);
				PushFollow(Follow._object_in_value93);
				o=@object();
				PopFollow();

				DebugLocation(135, 13);
				 node = o; 

				}
				break;
			case 4:
				DebugEnterAlt(4);
				// /Users/Abahr/TrailsForward/Assets/_Scripts/Library/JSON/Internal/JSONRecognizerTree.g:136:4: a= array
				{
				DebugLocation(136, 5);
				PushFollow(Follow._array_in_value102);
				a=array();
				PopFollow();

				DebugLocation(136, 12);
				 node = a; 

				}
				break;
			case 5:
				DebugEnterAlt(5);
				// /Users/Abahr/TrailsForward/Assets/_Scripts/Library/JSON/Internal/JSONRecognizerTree.g:137:4: TRUE
				{
				DebugLocation(137, 4);
				Match(input,TRUE,Follow._TRUE_in_value109); 
				DebugLocation(137, 9);
				 node = true; 

				}
				break;
			case 6:
				DebugEnterAlt(6);
				// /Users/Abahr/TrailsForward/Assets/_Scripts/Library/JSON/Internal/JSONRecognizerTree.g:138:4: FALSE
				{
				DebugLocation(138, 4);
				Match(input,FALSE,Follow._FALSE_in_value116); 
				DebugLocation(138, 10);
				 node = false; 

				}
				break;
			case 7:
				DebugEnterAlt(7);
				// /Users/Abahr/TrailsForward/Assets/_Scripts/Library/JSON/Internal/JSONRecognizerTree.g:139:4: NULL
				{
				DebugLocation(139, 4);
				Match(input,NULL,Follow._NULL_in_value123); 
				DebugLocation(139, 9);
				 node = null; 

				}
				break;

			}
		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		}
		finally
		{
			TraceOut("value", 2);
			LeaveRule("value", 2);
			LeaveRule_value();
	    }
	 	DebugLocation(140, 1);
		} finally { DebugExitRule(GrammarFileName, "value"); }
		return node;

	}
	// $ANTLR end "value"


	[Conditional("ANTLR_TRACE")]
	protected virtual void EnterRule_string() {}
	[Conditional("ANTLR_TRACE")]
	protected virtual void LeaveRule_string() {}

	// $ANTLR start "string"
	// /Users/Abahr/TrailsForward/Assets/_Scripts/Library/JSON/Internal/JSONRecognizerTree.g:142:1: string returns [string node] : ^( STRING String ) ;
	[GrammarRule("string")]
	private string @string()
	{
		EnterRule_string();
		EnterRule("string", 3);
		TraceIn("string", 3);
	    string node = default(string);


	    CommonTree String1 = default(CommonTree);

		try { DebugEnterRule(GrammarFileName, "string");
		DebugLocation(142, 1);
		try
		{
			// /Users/Abahr/TrailsForward/Assets/_Scripts/Library/JSON/Internal/JSONRecognizerTree.g:143:2: ( ^( STRING String ) )
			DebugEnterAlt(1);
			// /Users/Abahr/TrailsForward/Assets/_Scripts/Library/JSON/Internal/JSONRecognizerTree.g:143:4: ^( STRING String )
			{
			DebugLocation(143, 4);
			DebugLocation(143, 6);
			Match(input,STRING,Follow._STRING_in_string141); 

			Match(input, TokenTypes.Down, null); 
			DebugLocation(143, 13);
			String1=(CommonTree)Match(input,String,Follow._String_in_string143); 

			Match(input, TokenTypes.Up, null); 

			DebugLocation(144, 4);
			 node = extractString(String1); 

			}

		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		}
		finally
		{
			TraceOut("string", 3);
			LeaveRule("string", 3);
			LeaveRule_string();
	    }
	 	DebugLocation(145, 1);
		} finally { DebugExitRule(GrammarFileName, "string"); }
		return node;

	}
	// $ANTLR end "string"


	[Conditional("ANTLR_TRACE")]
	protected virtual void EnterRule_number() {}
	[Conditional("ANTLR_TRACE")]
	protected virtual void LeaveRule_number() {}

	// $ANTLR start "number"
	// /Users/Abahr/TrailsForward/Assets/_Scripts/Library/JSON/Internal/JSONRecognizerTree.g:147:1: number returns [object node] : ^( NUMBER Number ( Exponent )? ) ;
	[GrammarRule("number")]
	private object number()
	{
		EnterRule_number();
		EnterRule("number", 4);
		TraceIn("number", 4);
	    object node = default(object);


	    CommonTree Number2 = default(CommonTree);
	    CommonTree Exponent3 = default(CommonTree);

		try { DebugEnterRule(GrammarFileName, "number");
		DebugLocation(147, 1);
		try
		{
			// /Users/Abahr/TrailsForward/Assets/_Scripts/Library/JSON/Internal/JSONRecognizerTree.g:148:2: ( ^( NUMBER Number ( Exponent )? ) )
			DebugEnterAlt(1);
			// /Users/Abahr/TrailsForward/Assets/_Scripts/Library/JSON/Internal/JSONRecognizerTree.g:148:4: ^( NUMBER Number ( Exponent )? )
			{
			DebugLocation(148, 4);
			DebugLocation(148, 6);
			Match(input,NUMBER,Follow._NUMBER_in_number166); 

			Match(input, TokenTypes.Down, null); 
			DebugLocation(148, 13);
			Number2=(CommonTree)Match(input,Number,Follow._Number_in_number168); 
			DebugLocation(148, 20);
			// /Users/Abahr/TrailsForward/Assets/_Scripts/Library/JSON/Internal/JSONRecognizerTree.g:148:20: ( Exponent )?
			int alt2=2;
			try { DebugEnterSubRule(2);
			try { DebugEnterDecision(2, decisionCanBacktrack[2]);
			int LA2_0 = input.LA(1);

			if ((LA2_0==Exponent))
			{
				alt2 = 1;
			}
			} finally { DebugExitDecision(2); }
			switch (alt2)
			{
			case 1:
				DebugEnterAlt(1);
				// /Users/Abahr/TrailsForward/Assets/_Scripts/Library/JSON/Internal/JSONRecognizerTree.g:148:20: Exponent
				{
				DebugLocation(148, 20);
				Exponent3=(CommonTree)Match(input,Exponent,Follow._Exponent_in_number170); 

				}
				break;

			}
			} finally { DebugExitSubRule(2); }


			Match(input, TokenTypes.Up, null); 

			DebugLocation(149, 4);
			 node = extractNumber(Number2, Exponent3); 

			}

		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		}
		finally
		{
			TraceOut("number", 4);
			LeaveRule("number", 4);
			LeaveRule_number();
	    }
	 	DebugLocation(150, 1);
		} finally { DebugExitRule(GrammarFileName, "number"); }
		return node;

	}
	// $ANTLR end "number"


	[Conditional("ANTLR_TRACE")]
	protected virtual void EnterRule_array() {}
	[Conditional("ANTLR_TRACE")]
	protected virtual void LeaveRule_array() {}

	// $ANTLR start "array"
	// /Users/Abahr/TrailsForward/Assets/_Scripts/Library/JSON/Internal/JSONRecognizerTree.g:152:1: array returns [ List<object> node ] : ^( ARRAY (v= value )* ) ;
	[GrammarRule("array")]
	private List<object> array()
	{
		EnterRule_array();
		EnterRule("array", 5);
		TraceIn("array", 5);
	    List<object> node = default(List<object>);


	    object v = default(object);

	     node = new List<object>(); 
		try { DebugEnterRule(GrammarFileName, "array");
		DebugLocation(152, 1);
		try
		{
			// /Users/Abahr/TrailsForward/Assets/_Scripts/Library/JSON/Internal/JSONRecognizerTree.g:154:2: ( ^( ARRAY (v= value )* ) )
			DebugEnterAlt(1);
			// /Users/Abahr/TrailsForward/Assets/_Scripts/Library/JSON/Internal/JSONRecognizerTree.g:154:4: ^( ARRAY (v= value )* )
			{
			DebugLocation(154, 4);
			DebugLocation(154, 6);
			Match(input,ARRAY,Follow._ARRAY_in_array198); 

			if (input.LA(1) == TokenTypes.Down)
			{
				Match(input, TokenTypes.Down, null); 
				DebugLocation(154, 12);
				// /Users/Abahr/TrailsForward/Assets/_Scripts/Library/JSON/Internal/JSONRecognizerTree.g:154:12: (v= value )*
				try { DebugEnterSubRule(3);
				while (true)
				{
					int alt3=2;
					try { DebugEnterDecision(3, decisionCanBacktrack[3]);
					int LA3_0 = input.LA(1);

					if ((LA3_0==ARRAY||LA3_0==FALSE||(LA3_0>=NULL && LA3_0<=NUMBER)||(LA3_0>=OBJECT && LA3_0<=STRING)||LA3_0==TRUE))
					{
						alt3 = 1;
					}


					} finally { DebugExitDecision(3); }
					switch ( alt3 )
					{
					case 1:
						DebugEnterAlt(1);
						// /Users/Abahr/TrailsForward/Assets/_Scripts/Library/JSON/Internal/JSONRecognizerTree.g:154:13: v= value
						{
						DebugLocation(154, 14);
						PushFollow(Follow._value_in_array203);
						v=value();
						PopFollow();

						DebugLocation(154, 21);
						 node.Add( v); 

						}
						break;

					default:
						goto loop3;
					}
				}

				loop3:
					;

				} finally { DebugExitSubRule(3); }


				Match(input, TokenTypes.Up, null); 
			}


			}

		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		}
		finally
		{
			TraceOut("array", 5);
			LeaveRule("array", 5);
			LeaveRule_array();
	    }
	 	DebugLocation(155, 1);
		} finally { DebugExitRule(GrammarFileName, "array"); }
		return node;

	}
	// $ANTLR end "array"


	[Conditional("ANTLR_TRACE")]
	protected virtual void EnterRule_object() {}
	[Conditional("ANTLR_TRACE")]
	protected virtual void LeaveRule_object() {}

	// $ANTLR start "object"
	// /Users/Abahr/TrailsForward/Assets/_Scripts/Library/JSON/Internal/JSONRecognizerTree.g:157:1: object returns [ Dictionary<string, object> node ] : ^( OBJECT ( pair[$node] )* ) ;
	[GrammarRule("object")]
	private Dictionary<string, object> @object()
	{
		EnterRule_object();
		EnterRule("object", 6);
		TraceIn("object", 6);
	    Dictionary<string, object> node = default(Dictionary<string, object>);


	     node = new Dictionary<string, object>(); 
		try { DebugEnterRule(GrammarFileName, "object");
		DebugLocation(157, 1);
		try
		{
			// /Users/Abahr/TrailsForward/Assets/_Scripts/Library/JSON/Internal/JSONRecognizerTree.g:159:2: ( ^( OBJECT ( pair[$node] )* ) )
			DebugEnterAlt(1);
			// /Users/Abahr/TrailsForward/Assets/_Scripts/Library/JSON/Internal/JSONRecognizerTree.g:159:4: ^( OBJECT ( pair[$node] )* )
			{
			DebugLocation(159, 4);
			DebugLocation(159, 6);
			Match(input,OBJECT,Follow._OBJECT_in_object230); 

			if (input.LA(1) == TokenTypes.Down)
			{
				Match(input, TokenTypes.Down, null); 
				DebugLocation(159, 13);
				// /Users/Abahr/TrailsForward/Assets/_Scripts/Library/JSON/Internal/JSONRecognizerTree.g:159:13: ( pair[$node] )*
				try { DebugEnterSubRule(4);
				while (true)
				{
					int alt4=2;
					try { DebugEnterDecision(4, decisionCanBacktrack[4]);
					int LA4_0 = input.LA(1);

					if ((LA4_0==FIELD))
					{
						alt4 = 1;
					}


					} finally { DebugExitDecision(4); }
					switch ( alt4 )
					{
					case 1:
						DebugEnterAlt(1);
						// /Users/Abahr/TrailsForward/Assets/_Scripts/Library/JSON/Internal/JSONRecognizerTree.g:159:13: pair[$node]
						{
						DebugLocation(159, 13);
						PushFollow(Follow._pair_in_object232);
						pair(node);
						PopFollow();


						}
						break;

					default:
						goto loop4;
					}
				}

				loop4:
					;

				} finally { DebugExitSubRule(4); }


				Match(input, TokenTypes.Up, null); 
			}


			}

		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		}
		finally
		{
			TraceOut("object", 6);
			LeaveRule("object", 6);
			LeaveRule_object();
	    }
	 	DebugLocation(160, 1);
		} finally { DebugExitRule(GrammarFileName, "object"); }
		return node;

	}
	// $ANTLR end "object"


	[Conditional("ANTLR_TRACE")]
	protected virtual void EnterRule_pair() {}
	[Conditional("ANTLR_TRACE")]
	protected virtual void LeaveRule_pair() {}

	// $ANTLR start "pair"
	// /Users/Abahr/TrailsForward/Assets/_Scripts/Library/JSON/Internal/JSONRecognizerTree.g:162:1: pair[Dictionary<string, object> dict] : ^( FIELD key= String n= value ) ;
	[GrammarRule("pair")]
	private void pair(Dictionary<string, object> dict)
	{
		EnterRule_pair();
		EnterRule("pair", 7);
		TraceIn("pair", 7);
	    CommonTree key = default(CommonTree);
	    object n = default(object);

		try { DebugEnterRule(GrammarFileName, "pair");
		DebugLocation(162, 1);
		try
		{
			// /Users/Abahr/TrailsForward/Assets/_Scripts/Library/JSON/Internal/JSONRecognizerTree.g:163:2: ( ^( FIELD key= String n= value ) )
			DebugEnterAlt(1);
			// /Users/Abahr/TrailsForward/Assets/_Scripts/Library/JSON/Internal/JSONRecognizerTree.g:163:4: ^( FIELD key= String n= value )
			{
			DebugLocation(163, 4);
			DebugLocation(163, 6);
			Match(input,FIELD,Follow._FIELD_in_pair250); 

			Match(input, TokenTypes.Down, null); 
			DebugLocation(163, 15);
			key=(CommonTree)Match(input,String,Follow._String_in_pair254); 
			DebugLocation(163, 24);
			PushFollow(Follow._value_in_pair258);
			n=value();
			PopFollow();


			Match(input, TokenTypes.Up, null); 

			DebugLocation(164, 5);
			 dict.Add(extractString(key), n); 

			}

		}
		catch (RecognitionException re)
		{
			ReportError(re);
			Recover(input,re);
		}
		finally
		{
			TraceOut("pair", 7);
			LeaveRule("pair", 7);
			LeaveRule_pair();
	    }
	 	DebugLocation(165, 1);
		} finally { DebugExitRule(GrammarFileName, "pair"); }
		return;

	}
	// $ANTLR end "pair"
	#endregion Rules


	#region Follow sets
	private static class Follow
	{
		public static readonly BitSet _value_in_jsonTree54 = new BitSet(new ulong[]{0x0UL});
		public static readonly BitSet _EOF_in_jsonTree56 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _string_in_value75 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _number_in_value84 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _object_in_value93 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _array_in_value102 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _TRUE_in_value109 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _FALSE_in_value116 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _NULL_in_value123 = new BitSet(new ulong[]{0x2UL});
		public static readonly BitSet _STRING_in_string141 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _String_in_string143 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _NUMBER_in_number166 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _Number_in_number168 = new BitSet(new ulong[]{0x108UL});
		public static readonly BitSet _Exponent_in_number170 = new BitSet(new ulong[]{0x8UL});
		public static readonly BitSet _ARRAY_in_array198 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _value_in_array203 = new BitSet(new ulong[]{0x5B218UL});
		public static readonly BitSet _OBJECT_in_object230 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _pair_in_object232 = new BitSet(new ulong[]{0x408UL});
		public static readonly BitSet _FIELD_in_pair250 = new BitSet(new ulong[]{0x4UL});
		public static readonly BitSet _String_in_pair254 = new BitSet(new ulong[]{0x5B210UL});
		public static readonly BitSet _value_in_pair258 = new BitSet(new ulong[]{0x8UL});
	}
	#endregion Follow sets
}
