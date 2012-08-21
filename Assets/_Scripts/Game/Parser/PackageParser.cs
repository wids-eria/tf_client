using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PackageParser: Parser {
	
	private static PackageParser m_Singleton;
	
	static Dictionary<string, string> 	m_definitions;
	public static readonly string kTokenDef = "def";
	static string filePath = "Dictionary";
	static string filename = "Definitions_Inventory";
	
	static bool hasInit = false;
	public static void InitParse()
	{
		//Start parsing the file
		Parser.Parse(filePath, filename, GetHandlers());
		hasInit = true;
	}
	
	static Dictionary<string,ParseHandler> GetHandlers()
	{
		return new Dictionary<string, ParseHandler>() {
			{ kHandlerStart, 	HandleStart },
			{ kTokenDef, 		HandleDefinition },
			{ kHandlerDone, 	HandleDone },
		};
	}
	
	public static string GetDefinition(string key){
		
		if (!hasInit){
			InitParse();
		}
		
		if (m_definitions.ContainsKey(key.ToUpper())){
			return m_definitions[key.ToUpper()];	
		}else{
			Debug.LogWarning("No dictionary definition found for " + key);
			return "";	
		}
		
		return "";	
	}
	
	public static bool IsDefined(string key) {
		if (!hasInit) {
			InitParse();
		}
		return m_definitions.ContainsKey(key.ToUpper());
	}
	
	
	#region Handlers
	
	static bool HandleStart(int lineNumber, string[] tokens)
	{
		m_definitions = new Dictionary<string, string>();
		return true;
	}
	
	static bool HandleDone(int lineNumber, string[] tokens)
	{
		return true;
	}
	
	static bool HandleDefinition(int lineNumber, string[] tokens)
	{	
		if (tokens.Length != 3) {
			Debug.LogError(string.Format("Error in Dictionary Parser [Line {0}]: Item requires 3 tokens (has {1})",lineNumber, tokens.Length));
			return false;
		}
		for(int i = 0; i < tokens.Length; i++) {
			tokens[i] = tokens[i].Trim();
		}
		

		m_definitions.Add(tokens[1].ToUpper(), tokens[2]);
		
		return true;
	}
	
	#endregion
}
