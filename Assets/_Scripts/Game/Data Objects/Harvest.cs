using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Cut type.
/// </summary>
public enum CutType {
	Clearcut,
	DiameterLimitCut,
	QRatioCut
}

/// <summary>
/// Diameter limit cut direction.
/// </summary>
public enum DiameterLimitCutDirection {
	Less = -1,
	Greater = 1
}

/// <summary>
/// Scheduled harvest.
/// </summary>
public class Harvest : System.Object
{
	/// <summary>
	/// Products yielded by a harvest.
	/// </summary>
	/// <remarks>
	/// Fields are doubles because json deserializer is not smart enough to coerce to float.
	/// </remarks>
	public struct Products {
		public double poletimber_value;
		public float poletimberValue { get { return (float)poletimber_value; } }
		public double poletimber_volume;
		public float poletimberVolume { get { return (float)poletimber_volume; } }
		public double sawtimber_value;
		public float sawtimberValue { get { return (float)sawtimber_value; } }
		public double sawtimber_volume;
		public float sawtimberVolume { get { return (float)sawtimber_volume; } }
		public override string ToString()
		{
			return string.Format(
				"[Harvest.Products]: poletimber=({0:0.0000} cords, {1:c}), sawtimber=({2:0.0000} board feet ,{3:c})",
				this.poletimber_volume, this.poletimber_value,
				this.sawtimber_volume, this.sawtimber_value
			);
		}
	}
	
	/// <summary>
	/// Gets or sets the duration.
	/// </summary>
	/// <value>
	/// The number of times the harvest operation will execute.
	/// </value>
	public int duration { get; private set; }
	/// <summary>
	/// The history.
	/// </summary>
	/// <remarks>
	/// Each element is an array of the amount of timber extracted for each size class at the time point.
	/// </remarks>
	private List<Harvest.Products> m_history;
	/// <summary>
	/// Gets the history.
	/// </summary>
	/// <remarks>
	/// Each element is an array of the amount of timber extracted for each size class.
	/// </remarks>
	/// <value>
	/// The history.
	/// </value>
	public Harvest.Products[] history {
		get {
			if (m_history == null) {
				m_history = new List<Harvest.Products>();
			}
			return m_history.ToArray();
		}
	}
	public int yearsInOperation {
		get {
			return turnsInOperation*4;
		}
	}
	public int turnsInOperation {
		get {
			return history.Length;
		}
	}
	public float totalSawTimberProduction {
		get {
			float count = 0;
			foreach (Products p in history) {
				count += p.sawtimberVolume;
			}
			return count;
		}
	}
	public float totalPoleTimberProduction {
		get {
			float count = 0;
			foreach (Products p in history) {
				count += p.poletimberVolume;
			}
			return count;
		}
	}
	
	/// <summary>
	/// The identifiers of the tiles included in the harvest.
	/// </summary>
	public readonly int[] ids;
	/// <summary>
	/// The type of the cut.
	/// </summary>
	public readonly CutType cutType;
	/// <summary>
	/// The q ratio, if applicable.
	/// </summary>
	public readonly float qRatio;
	/// <summary>
	/// The basal area, if applicable.
	/// </summary>
	public readonly float basalArea;
	/// <summary>
	/// The diameter limit, if applicable.
	/// </summary>
	public readonly int diameterLimit;
	/// <summary>
	/// The diameter limit cut direction, if applicable.
	/// </summary>
	public readonly DiameterLimitCutDirection diameterLimitCutDirection;
	/// <summary>
	/// The center point.
	/// </summary>
	public Vector3 centerPoint { get; private set; }
	/// <summary>
	/// Initializes a new instance of the <see cref="Harvest"/> struct for a clearcut.
	/// </summary>
	/// <param name='ids'>
	/// Identifiers.
	/// </param>
	/// <param name='duration'>
	/// Duration.
	/// </param>
	public Harvest(int[] ids, int duration)
	{
		this.ids = ids;
		this.duration = duration;
		this.cutType = CutType.Clearcut;
	}
	/// <summary>
	/// Initializes a new instance of the <see cref="Harvest"/> struct for a q ratio cut.
	/// </summary>
	/// <param name='ids'>
	/// Identifiers.
	/// </param>
	/// <param name='duration'>
	/// Duration.
	/// </param>
	/// <param name='qRatio'>
	/// Q ratio.
	/// </param>
	/// <param name='targetBasalArea'>
	/// Target basal area.
	/// </param>
	public Harvest(int[] ids, int duration, float qRatio, float targetBasalArea)
	{
		this.ids = ids;
		this.duration = duration;
		this.cutType = CutType.QRatioCut;
		this.qRatio = qRatio;
		this.basalArea = targetBasalArea;
	}
	/// <summary>
	/// Initializes a new instance of the <see cref="Harvest"/> struct for a diameter limit cut.
	/// </summary>
	/// <param name='ids'>
	/// Identifiers.
	/// </param>
	/// <param name='duration'>
	/// Duration.
	/// </param>
	/// <param name='diameterLimit'>
	/// Diameter limit.
	/// </param>
	/// <param name='cutDirection'>
	/// Cut direction.
	/// </param>
	public Harvest(int[] ids, int duration, int diameterLimit, DiameterLimitCutDirection cutDirection)
	{
		this.ids = ids;
		this.duration = duration;
		this.cutType = CutType.DiameterLimitCut;
		this.diameterLimit = diameterLimit;
		this.diameterLimitCutDirection = cutDirection;
	}
	
	/// <summary>
	/// Computes the center point based on the bounds of the tile positions.
	/// </summary>
	/// <returns>
	/// The center point.
	/// </returns>
	public IEnumerator ComputeCenterPoint()
	{
		HTTP.Request request = new HTTP.Request( "Get", WebRequests.GetURLToResourceTile(this.ids[0]) );
		request.AddParameters( WebRequests.authenticatedGodModeParameters );
		request.Send();
		while( !request.isDone ) {
			yield return 0;
		}
		
		if( request.ProducedError ) {
			Debug.LogError( "Unable to compute center point of harvest: " + request.Error );
			yield break;
		};
		ResourceTile tile = new ResourceTile();
		try {
			tile = JSONDecoder.Decode<IndividualResourceTile>(request.response.Text).resource_tile;
		}
		catch (JsonException) {
			Debug.LogError("Error parsing json data:\n"+request.response.Text);
		}
		Bounds bounds = new Bounds(tile.GetSimpleCenterPoint(), new Vector3(1f,0f,1f));
		foreach (int id in this.ids) {
			request = new HTTP.Request( "Get", WebRequests.GetURLToResourceTile(id) );
			request.AddParameters( WebRequests.authenticatedGodModeParameters );
			request.Send();
			
			while( !request.isDone ) {
				yield return 0;
			}
			
			if( request.ProducedError ) {
				Debug.LogError( "Unable to compute center point of harvest: " + request.Error );
				yield break;
			}
			try {
				tile = JSONDecoder.Decode<IndividualResourceTile>(request.response.Text).resource_tile;
				bounds.Encapsulate(tile.GetSimpleCenterPoint());
			}
			catch (JsonException) {
				Debug.LogError("Error parsing json data:\n"+request.response.Text);
			}
		}
		// set the center point from the bounding box
		this.centerPoint = bounds.center;
	}
	
	/// <summary>
	/// Gets the partial selection curve.
	/// </summary>
	/// <remarks>
	/// This method replicates the math in partial_selection_curve in tree_harvesting.rb.
	/// </remarks>
	/// <returns>
	/// The partial selection curve, where each index corresponds to a size class, and each value is the desired n for the class.
	/// </returns>
	/// <param name='qRatio'>
	/// Q ratio.
	/// </param>
	/// <param name='basalArea'>
	/// Basal area.
	/// </param>
	public static float[] GetPartialSelectionCurve(float qRatio, float basalArea)
	{
		int x = ResourceTile.treeSizeClassCount;
		float[] targetDistribution = new float[x];
		float[] normalizedTargetDiameterDistribution = new float[x];
		float[] products = new float[x];
		float sizeClassCenterOffset = 0.5f*ResourceTile.treeSizeClassInterval;
		for (int i=0; i<x; ++i) {
			float individualBasalArea = Mathf.Pow(
				(i+1)*ResourceTile.treeSizeClassInterval-sizeClassCenterOffset, 2f
			) * 0.005454154f; // NOTE: conversion of inches diameter to square feet area
			normalizedTargetDiameterDistribution[i] = Mathf.Pow(qRatio, x-i-1);
			products[i] = normalizedTargetDiameterDistribution[i] * individualBasalArea;
		}
		float normalizedTargetDiameterDistributionBasalAreaSum = 0f;
		for (int i=0; i<x; ++i) {
			normalizedTargetDiameterDistributionBasalAreaSum += products[i];
		}
		float sumOverBasalArea = normalizedTargetDiameterDistributionBasalAreaSum / ((basalArea==0)?1:basalArea);
		for (int i=0; i<x; ++i) {
			targetDistribution[i] = normalizedTargetDiameterDistribution[i] * sumOverBasalArea;
		}
		return targetDistribution;
	}
	
	/// <summary>
	/// Computes the trees to be cut.
	/// </summary>
	/// <param name='frequencyDistribution'>
	/// Frequency distribution.
	/// </param>
	/// <param name='diameterLimit'>
	/// Diameter limit.
	/// </param>
	/// <param name='direction'>
	/// Direction of the diameter limit cut.
	/// </param>
	/// <param name='treesToCut'>
	/// Trees to cut.
	/// </param>
	/// <param name='treesToKeep'>
	/// Trees to keep.
	/// </param>
	public static void ComputeTreesToBeCut(
		int[] frequencyDistribution, int diameterLimit, DiameterLimitCutDirection direction,
		out int[] treesToCut, out int[] treesToKeep
	)
	{
		treesToCut = new int[frequencyDistribution.Length];
		treesToKeep = new int[frequencyDistribution.Length];
		int limit = diameterLimit/ResourceTile.treeSizeClassInterval-1;
		switch (direction) {
		case DiameterLimitCutDirection.Greater:
			// cut all trees greater than limit
			for (int i=0; i<frequencyDistribution.Length; ++i) {
				if (i <= limit) {
					treesToKeep[i] = frequencyDistribution[i];
				}
				else {
					treesToCut[i] = frequencyDistribution[i];
				}
			}
			break;
		case DiameterLimitCutDirection.Less:
			// cut all trees less than limit
			for (int i=0; i<frequencyDistribution.Length; ++i) {
				if (i >= limit) {
					treesToKeep[i] = frequencyDistribution[i];
				}
				else {
					treesToCut[i] = frequencyDistribution[i];
				}
			}
			break;
		}
	}
	
	/// <summary>
	/// Computes the trees to be cut.
	/// </summary>
	/// <param name='frequencyDistribution'>
	/// Frequency distribution.
	/// </param>
	/// <param name='basalArea'>
	/// Basal area.
	/// </param>
	/// <param name='qRatio'>
	/// Q ratio.
	/// </param>
	/// <param name='treesToCut'>
	/// Trees to cut.
	/// </param>
	/// <param name='treesToKeep'>
	/// Trees to keep.
	/// </param>
	public static void ComputeTreesToBeCut(
		int[] frequencyDistribution, float[] qRatioCurve,
		out int[] treesToCut, out int[] treesToKeep
	)
	{
		treesToCut = new int[frequencyDistribution.Length];
		treesToKeep = new int[frequencyDistribution.Length];
		for (int i=0; i<ResourceTile.treeSizeClassCount; ++i) {
			treesToKeep[i] = Mathf.Min((int)qRatioCurve[i], frequencyDistribution[i]);
			treesToCut[i] = frequencyDistribution[i]-treesToKeep[i];
		}
	}
	
	/// <summary>
	/// Clearcut modified fields.
	/// </summary>
	/// <remarks>
	/// Only use this class when the land cover should be modified.
	/// </remarks>
	public class ClearcutModifiedFields : System.Object
	{
		public readonly int landcover_class_code = (int)LandCoverType.Barren;
	}
	
	/// <summary>
	/// Gets a WWW object do perform the harvest, configured for the associated api point on the server.
	/// </summary>
	/// <returns>
	/// The do it WWW.
	/// </returns>
	public HTTP.Request GetDoItWWW()
	{
		string apiPoint = "";
		Dictionary<string, string> parameters = WebRequests.authenticatedParameters;
		switch (this.cutType) {
		case CutType.Clearcut:
			apiPoint = "clearcut";
			break;
		case CutType.DiameterLimitCut:
			apiPoint = "diameter_limit_cut";
			parameters.Add(this.diameterLimitCutDirection==DiameterLimitCutDirection.Greater?"above":"below", this.diameterLimit.ToString());
			break;
		case CutType.QRatioCut:
			apiPoint = "partial_selection_cut";
			parameters.Add("qratio", this.qRatio.ToString());
			parameters.Add("target_basal_area", this.basalArea.ToString());
			break;
		}
		string url = string.Format(
			"{0}/worlds/{1}/resource_tiles/{2}.json",
			WebRequests.urlServer, UserData.worldNumber, apiPoint
		);
		HTTP.Request request = new HTTP.Request( "Post", url );
		request.AddParameters( parameters );
		
		HTTP.JSON json = new HTTP.JSON();
		json.Data = JsonMapper.ToJson( new ResourceTileSelection(this.ids) );
		request.SetText(json);   ///.ToJson() );
		//Debug.Log(new ResourceTileSelection(this.ids).ToJson());
		
		return request;
	}
	
	/// <summary>
	/// Gets or sets the progress when the action is under way.
	/// </summary>
	/// <value>
	/// The progress.
	/// </value>
	public float progress { get; private set; }
	/// <summary>
	/// Gets or sets the do it error, if any.
	/// </summary>
	/// <value>
	/// The do it error.
	/// </value>
	public string doItError { get; private set; }
	
	/// <summary>
	/// Execute the harvest
	/// </summary>
	public IEnumerator DoIt()
	{
		// submit harvest to API point
		progress = 0f; doItError = "";
		HTTP.Request request = GetDoItWWW();
		
		
		//Debug.Log(request.bytes);
		
		request.Send();
		while (!request.isDone) {
			//progress = Mathf.Min(www.uploadProgress+www.progress, 0.99f);
			yield return 0;
		}

		// deserialize the product values
		Harvest.Products products;
		
		Debug.Log( request.response.status );
		
		try {
			products = JSONDecoder.Decode<Harvest.Products>( request.response.Text );
		}
		catch (JsonException e) {
			Debug.LogError(e.ToString()+"\n"+request.response.Text);
			//doItError = www.error;
			progress = 1f;
			yield break;
		}
		// TODO: since server api does not presently modify cover type, modify it with god mode here
		if (this.cutType == CutType.Clearcut) {
			PlayerAction.ResourceTileModification modification = new PlayerAction.ResourceTileModification();
			modification.resource_tile = new ClearcutModifiedFields();
			string json = JsonMapper.ToJson(modification);
			foreach (int id in this.ids) {
				request = new HTTP.Request( "Get", WebRequests.GetURLToResourceTile(id) );
				request.AddParameters( WebRequests.authenticatedParameters );
				request.Send();
				
				while( !request.isDone ) {
					yield return 0;
				}
				
				//if (!string.IsNullOrEmpty(www.error)) {
				//	progress = 1f; doItError = www.error;
				//	yield break;
				//}
				ResourceTile tile;
				try {
					tile = JSONDecoder.Decode<IndividualResourceTile>(request.response.Text).resource_tile;
					switch (tile.baseCoverType) {
//					case BaseCoverType.Barren:
//					case BaseCoverType.CultivatedCrops:
					case BaseCoverType.Developed:
					case BaseCoverType.Excluded:
//					case BaseCoverType.Forest:
//					case BaseCoverType.Herbaceous:
					case BaseCoverType.Unknown:
					case BaseCoverType.Water:
//					case BaseCoverType.Wetland:
						continue;
					}
				}
				catch (JsonException e) {
					Debug.LogError(e.ToString()+"\n"+request.response.Text);
				}
				request = new HTTP.Request( "Put", WebRequests.GetURLToResourceTile(id) );
				request.SetText( json );
				request.AddParameters( WebRequests.authenticatedGodModeParameters );
				request.Send();
				
				while( !request.isDone ) {
					yield return 0;
				}
				
				//if (!string.IsNullOrEmpty(www.error)) {
				//	progress = 1f; doItError = www.error;
				//	yield break;letmein
				//}
			}
		}
		// add products to harvest history
		if (m_history == null) {
			m_history = new List<Products>();
		}
		m_history.Add(products);
		// set progress to done
		progress = 1f; doItError = "";
		// broadcast message that harvest was executed
		MessengerAM.Send(new MessageHarvestExecuted(this, products));
		
		// TODO: Collapse these two messages into one
		Messenger<Products>.Broadcast("HarvestProducts",products);
	}
	
	/// <summary>
	/// Returns a <see cref="System.String"/> that represents the current <see cref="Harvest"/>.
	/// </summary>
	/// <returns>
	/// A <see cref="System.String"/> that represents the current <see cref="Harvest"/>.
	/// </returns>
	public override string ToString()
	{
		return string.Format ("[Harvest: duration={0}, centerPoint={1}, cutType={2}]", duration, centerPoint, cutType);
	}
}