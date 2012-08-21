using UnityEditor;
using UnityEngine;
using System.Collections;

/// <summary>
/// An AssetPostprocessor class to simply disable importation of materials from models
/// </summary>
public class ImportWithoutMaterials : AssetPostprocessor
{
	/// <summary>
	/// Raises the preprocess model event.
	/// </summary>
	void OnPreprocessModel()
	{
		ModelImporter importer = assetImporter as ModelImporter;
		importer.importMaterials = false;
	}
}