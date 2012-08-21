tree grammar JSONRecognizerTree;

options {
language=CSharp2;
tokenVocab=JSONRecognizer; // reuse token types 
ASTLabelType=CommonTree; // $label will have type CommonTree 
} 

@header {
#pragma warning disable 0414
#pragma warning disable 3021

using System;
using System.Text;
using System.Linq;
using System.IO;
}

@members {
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
}

jsonTree returns [object node]
	:	n=value EOF { node = n; }
	;

value returns [object node]
	: s=string { node = s; }
	| n=number { node = n; }
	| o=object { node = o; }
	| a=array { node = a; }
	| TRUE { node = true; }
	| FALSE { node = false; }
	| NULL { node = null; }
	;

string returns [string node]
	: ^(STRING String)
	  { node = extractString($String); }
	;
	
number returns [object node]
	: ^(NUMBER Number Exponent?)
	  { node = extractNumber($Number, $Exponent); }
	;

array returns [ List<object> node ]
@init { node = new List<object>(); }
	: ^(ARRAY (v=value { $node.Add( v); })*)
	;
	
object returns [ Dictionary<string, object> node ]
@init { node = new Dictionary<string, object>(); }
	: ^(OBJECT pair[$node]*)
	;
	
pair [Dictionary<string, object> dict]
	: ^(FIELD key=String n=value) 
	   { $dict.Add(extractString($key), n); }
	;