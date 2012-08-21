using UnityEngine;
using System.Collections;

/// <summary>
/// String helpers.
/// </summary>
public static class StringHelpers : System.Object
{
	/// <summary>
	/// Converts camelCaseWord to Camel Case Words.
	/// </summary>
	/// <returns>
	/// The camel case representation as individual words.
	/// </returns>
	/// <param name='camelCaseWord'>
	/// Camel case word.
	/// </param>
	public static string CamelCaseToWords(string camelCaseWord)
	{
		return System.Text.RegularExpressions.Regex.Replace(
			camelCaseWord,
			"([A-Z])",
			" $1"/*,
			System.Text.RegularExpressions.RegexOptions.Compiled*/).Trim();
	}
	
	/// <summary>
	/// Gets the enum names as camel case.
	/// </summary>
	/// <returns>
	/// The enum names as camel case.
	/// </returns>
	/// <param name='t'>
	/// Type of the enum.
	/// </param>
	public static string[] GetEnumNamesAsCamelCase(System.Type t)
	{
		string[] ret = System.Enum.GetNames(t);
		for (int i=0; i<ret.Length; ++i) {
			ret[i] = CamelCaseToWords(ret[i]);
		}
		return ret;
	}
}