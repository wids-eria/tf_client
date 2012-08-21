using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class Parser : System.Object {
	
	public const string kHandlerStart = "OnHandlerStart";
	public const string kHandlerDone  = "OnHandlerDone";
	
	private const int    kError        = -1;
	private const string kTokenComment = "#";
	private static readonly string[] kTokenDelimiters = new string[]{ ":" };
	protected static readonly char[] kTrimCharacters = new char[]{ ' ', '\t', ',' };
	
	/// <summary>
	/// Interface for ParseHandler: 
	/// @param: int, string[]
	/// </summary>
	protected delegate bool ParseHandler(int lineNumber, string[] tokens);
	
	protected static void Parse(string filePath, string fileName, Dictionary<string, ParseHandler> handlers) 
	{
		string 		path = string.Format("{0}/{1}", filePath, fileName);
		Debug.Log(path);
		TextAsset 	asset = (TextAsset)Resources.Load(path, typeof(TextAsset));
		TextReader 	input = new StringReader(asset.text);
		string     	aLine;
		int 		lineCurrent = 0;
		
		if (!handlers.ContainsKey(kHandlerStart) || !handlers.ContainsKey(kHandlerDone)) {
			Debug.LogError(string.Format("Handlers must include keys `{0}' and `{1}'",
			                             kHandlerStart, kHandlerDone));
			input.Close();
			return;
		}
		handlers[kHandlerStart](lineCurrent, null);
		while ((aLine = input.ReadLine()) != null) {
			string trimmedLine = aLine.Trim();
			
			// Ignore blank lines
			if (trimmedLine.Length < 1 || trimmedLine.StartsWith(kTokenComment)) {
				++lineCurrent;
				continue;
			}
			
			// Get the tokens and update the current object.
			string[] tokens = aLine.Split(kTokenDelimiters, 
			                              System.StringSplitOptions.RemoveEmptyEntries);
			string tokenType = tokens[0].Trim(kTrimCharacters);
			if (handlers.ContainsKey(tokenType)) {
				if (!handlers[tokenType](lineCurrent+1, tokens)) break;
			}
			else {
				Debug.LogError(string.Format("Unknown token type `{0}' on line {1}.",
				                             tokenType, lineCurrent+1));
				break;
			}
			
			++lineCurrent;
		}
		handlers[kHandlerDone](lineCurrent, null);
		input.Close();
	}
}
