using UnityEngine;
using System.Collections;

/// <summary>
/// Megatile selection; used for e.g. submitting ids of tiles to perform an action.
/// </summary>
public struct MegatileSelection {
	/// <summary>
	/// The megatiles.
	/// </summary>
	public int[] megatiles;
	/// <summary>
	/// Initializes a new instance of the <see cref="MegatileSelection"/> struct.
	/// </summary>
	/// <param name='ids'>
	/// Identifiers.
	/// </param>
	public MegatileSelection(int[] ids)
	{
		megatiles = ids;
	}
	/// <summary>
	/// Converts to json.
	/// </summary>
	/// <returns>
	/// The json representation.
	/// </returns>
	public string ToJson()
	{
		return JsonMapper.ToJson(this);
	}
}