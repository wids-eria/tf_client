using UnityEngine;
using System.Collections;
/*
/// <summary>
/// A class to describe a quest (e.g., mission card)
/// </summary>
public class Quest : MonoBehaviour
{
	/// <summary>
	/// enum to describe a quest's difficulty
	/// </summary>
	public enum Difficulty { Easy, Medium, Hard }
	
	/// <summary>
	/// the quest's difficulty
	/// </summary>
	public Difficulty difficulty = Difficulty.Easy;
	
	/// <summary>
	/// a description of the quest
	/// </summary>
	public string description;
	
	/// <summary>
	/// who owns this quest?
	/// </summary>
	public Player quester;
	
	/// <summary>
	/// Initialize
	/// </summary>
	void Awake()
	{
		Messenger.Listen("actions", this);
		Messenger.Listen("game flow", this);
	}
	void Start()
	{
		// if no quester is already specified, search the hierarchy for the first Player
		if (quester != null) return;
		Transform root = transform.parent;
		while (root.parent != null && root.GetComponent<Player>() == null) root = root.parent;
		quester = root.gameObject.GetComponent<Player>();
	}
	
	/// <summary>
	/// A class to describe a quest objective
	/// </summary>
	[System.Serializable]
	public class Objective : System.Object
	{
		/// <summary>
		/// the name of the objective
		/// </summary>
		public string name;
		
		/// <summary>
		/// is the objective complete
		/// </summary>
		[HideInInspector]
		public bool isComplete = false;
		
		/// <summary>
		/// enum to describe when an objective should be evaluated
		/// </summary>
		public enum EvaluateOn
		{
			PlayerAction,	// when an action is performed
//			WorldModified,	// when the world is modified
			RoundEnd,		// when the turn ends
			GameEnd			// when the game ends
		}
		
		/// <summary>
		/// when should the objective be evaluated
		/// </summary>
		public EvaluateOn evaluateOn;
		
		/// <summary>
		/// enum to describe how often the objective should be evaluated
		/// </summary>
		public enum EvaluationFrequency
		{
			Once,	// objective is complete upon first successful evaluation
			Always	// objective is evaluated every time a message is sent
		}
		
		/// <summary>
		/// how often should the objective be evaluated
		/// </summary>
		public EvaluationFrequency evaluationFrequency;
		
		/// <summary>
		/// what type of comparison does the objective use
		/// </summary>
		public enum Comparison { Equals, LessOrEqual, GreaterOrEqual }
		
		/// <summary>
		/// different quantities
		/// </summary>
		public enum QuantityType
		{
			Cash,
			AcresOwned,
			AcresCleared,
			AcresClearcut,
			AcresDeveloped,
			AcresRestored,
			AcresConserved,
			AcresSurveyed,
			AcresPark,
			FishPopulation,
			BirdPopulation
		}
		
		/// <summary>
		/// different attributes
		/// </summary>
		public enum QuantityAttribute
		{
			Age,
			Density,
			Intensity,
			Quality,
			Size
		}
		
		/// <summary>
		/// A class for describing attribute requirements for a quantity
		/// </summary>
		[System.SerializableAttribute]
		public class AttributeRequirement
		{
			/// <summary>
			/// The attribute to evaluate
			/// </summary>
			public QuantityAttribute attribute;
			/// <summary>
			/// the desired effect
			/// </summary>
			public Comparison requirement = Comparison.GreaterOrEqual;
			/// <summary>
			/// the desired amount
			/// </summary>
			public float requiredAmount;
		}
		
		/// <summary>
		/// A class for describing a quantitative completion requirement
		/// </summary>
		[System.SerializableAttribute]
		public class QuantityRequirement
		{
			/// <summary>
			/// the type of quantity
			/// </summary>
			public QuantityType quantityType;
			/// <summary>
			/// the desired effect
			/// </summary>
			public Comparison requirement = Comparison.GreaterOrEqual;
			/// <summary>
			/// the desired amount
			/// </summary>
			public int requiredAmount;
			/// <summary>
			/// any further qualifiers for the quantity
			/// </summary>
			public AttributeRequirement[] attributeRequirements;
			/// <summary>
			/// Get the min and max density and intensity using the attribute requirements
			/// </summary>
			/// <param name="densityLow">
			/// A <see cref="System.Single"/>
			/// </param>
			/// <param name="densityHigh">
			/// A <see cref="System.Single"/>
			/// </param>
			/// <param name="intensityLow">
			/// A <see cref="System.Single"/>
			/// </param>
			/// <param name="intensityHigh">
			/// A <see cref="System.Single"/>
			/// </param>
			public void GetMinMaxDensityIntensity(out float densityLow, out float densityHigh, out float intensityLow, out float intensityHigh)
			{
				densityLow = 0f;
				densityHigh = 1f;
				intensityLow = 0f;
				intensityHigh = 1f;
				foreach (AttributeRequirement req in attributeRequirements)
				{
					switch (req.attribute)
					{
					case QuantityAttribute.Density:
						switch (req.requirement)
						{
						case Comparison.Equals:
							densityLow = densityHigh = req.requiredAmount;
							break;
						case Comparison.GreaterOrEqual:
							densityLow = req.requiredAmount;
							break;
						case Comparison.LessOrEqual:
							densityHigh = req.requiredAmount;
							break;
						}
						break;
					case QuantityAttribute.Intensity:
						switch (req.requirement)
						{
						case Comparison.Equals:
							intensityHigh = intensityLow = req.requiredAmount;
							break;
						case Comparison.GreaterOrEqual:
							intensityLow = req.requiredAmount;
							break;
						case Comparison.LessOrEqual:
							intensityHigh = req.requiredAmount;
							break;
						}
						break;
					}
				}
			}
		}
		
		/// <summary>
		/// quantity requirement for completion
		/// </summary>
		public QuantityRequirement quantityRequirement;
		
		/// <summary>
		/// what is the objective's completion percentage?
		/// </summary>
		public float completionPercentage { get { return _pct; } }
		private float _pct = 0f;
		
		/// <summary>
		/// Evaluate the objective's state using the supplied player
		/// </summary>
		/// <param name="quester">
		/// A <see cref="Player"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		public bool EvaluateForQuester(Player quester)
		{
			return false; // disable this since this will be done on the server
			/* TODO
			 
			// if the objective only evaluates once and is already complete, then return true
			if (evaluationFrequency==Quest.Objective.EvaluationFrequency.Once && isComplete) return isComplete;
			
			// get the comparison amount depending upon the type of quantity required
			int amt = 0;
			switch (quantityRequirement.quantityType)
			{
			case QuantityType.AcresCleared:
				amt = quester.actionLogger.totalBulldoze*Megatile.size*Megatile.size;
				break;
			case QuantityType.AcresClearcut:
				amt = quester.actionLogger.totalCleartCut*Megatile.size*Megatile.size;
				break;
			case QuantityType.AcresConserved:
				// TODO
				break;
			case QuantityType.AcresDeveloped:
				float densityLow, densityHigh, intensityLow, intensityHigh;
				quantityRequirement.GetMinMaxDensityIntensity(out densityLow, out densityHigh, out intensityLow, out intensityHigh);
				amt = quester.actionLogger.GetNumMicrotilesStructures(densityLow, densityHigh, intensityLow, intensityHigh);
				break;
			case QuantityType.AcresOwned:
				amt = quester.actionLogger.GetNumMicrotiles();
				break;
			case QuantityType.AcresRestored:
				amt = quester.actionLogger.totalRestoreLand*Megatile.size*Megatile.size;
				break;
			case QuantityType.AcresSurveyed:
				amt = quester.actionLogger.totalSurveyLand*Megatile.size*Megatile.size;
				// TODO: require that they have up-to-date info for a Maintain frequency
				break;
			case QuantityType.AcresPark:
				//float densityLow, densityHigh, intensityLow, intensityHigh;
				quantityRequirement.GetMinMaxDensityIntensity(out densityLow, out densityHigh, out intensityLow, out intensityHigh);
				amt = quester.actionLogger.GetNumMicrotilesParks(densityLow, densityHigh);
				break;
			case QuantityType.BirdPopulation:
				// TODO
				break;
			case QuantityType.Cash:
				amt = quester.cash;
				break;
			case QuantityType.FishPopulation:
				// TODO
				break;
			}
			
			// determine whether or not the requirement has been met
			switch (quantityRequirement.requirement)
			{
			case Comparison.Equals:
				isComplete = amt == quantityRequirement.requiredAmount;
				break;
			case Comparison.GreaterOrEqual:
				isComplete = amt >= quantityRequirement.requiredAmount;
				break;
			case Comparison.LessOrEqual:
				isComplete = amt <= quantityRequirement.requiredAmount;
				break;
			}
			
			// compute completion percentage
			_pct = (float)amt/(float)quantityRequirement.requiredAmount;
			
			// return the new value for isComplete
			return isComplete;
			
		}
		
		/// <summary>
		/// A string representation of the objective
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/>
		/// </returns>
		public override string ToString()
		{
			string amtStr = string.Format("{0:#,#}", quantityRequirement.requiredAmount);
			switch (quantityRequirement.requirement)
			{
			case Comparison.GreaterOrEqual:
				amtStr += " or more";
				break;
			case Comparison.LessOrEqual:
				amtStr += "or fewer";
				break;
			}
			string ret = "";
			float densityLow, densityHigh, intensityLow, intensityHigh;
			switch (quantityRequirement.quantityType)
			{
			case QuantityType.AcresCleared:
				ret = (evaluationFrequency==EvaluationFrequency.Once)?
					string.Format("Clear {0} acres of land for development.", amtStr):
					string.Format("Maintain {0} acres of land cleared for development.", amtStr);
				break;
			case QuantityType.AcresClearcut:
				ret = (evaluationFrequency==EvaluationFrequency.Once)?
					string.Format("Clearcut {0} acres of land.", amtStr):
					string.Format("Maintain {0} acres of clearcut land.", amtStr);
				break;
			case QuantityType.AcresConserved:
				ret = (evaluationFrequency==EvaluationFrequency.Once)?
					string.Format("Bring {0} acres of land into conservation.", amtStr):
					string.Format("Maintain {0} acres of land in conservation.", amtStr);
				break;
			case QuantityType.AcresDeveloped:
				ret = (evaluationFrequency==EvaluationFrequency.Once)?
					string.Format("Develop structures on {0} acres of land.", amtStr):
					string.Format("Maintain structures on {0} acres of land.", amtStr);
//				quantityRequirement.GetMinMaxDensityIntensity(out densityLow, out densityHigh, out intensityLow, out intensityHigh);
//				ret = string.Format("{0} (Density [{1:0.00}, {2:0.00}], Intensity [{3:0.00}, {4:0.00}]).", ret, densityLow, densityHigh, intensityLow, intensityHigh);
				break;
			case QuantityType.AcresOwned:
				ret = (evaluationFrequency==EvaluationFrequency.Once)?
					string.Format("Bring {0} acres of land under your control.", amtStr):
					string.Format("Maintain {0} acres of land under your control.", amtStr);
				break;
			case QuantityType.AcresRestored:
				ret = (evaluationFrequency==EvaluationFrequency.Once)?
					string.Format("Restore {0} acres of fallow land.", amtStr):
					string.Format("Maintain {0} acres of restored land.", amtStr);
				break;
			case QuantityType.AcresSurveyed:
				ret = (evaluationFrequency==EvaluationFrequency.Once)?
					string.Format("Survey {0} acres of land.", amtStr):
					string.Format("Maintain up-to-date survey info on {0} acres land.", amtStr);
				break;
			case QuantityType.AcresPark:
				ret = (evaluationFrequency==EvaluationFrequency.Once)?
					string.Format("Develop parks on {0} acres of land.", amtStr):
					string.Format("Maintain parks on {0} acres of land.", amtStr);
				break;
			case QuantityType.BirdPopulation:
				ret = (evaluationFrequency==EvaluationFrequency.Once)?
					string.Format("Attain a bird population of {0} individuals.", amtStr):
					string.Format("Maintain a bird population of {0} individuals.", amtStr);
				// TODO: account for attributes
				break;
			case QuantityType.Cash:
				ret = (evaluationFrequency==EvaluationFrequency.Once)?
					string.Format("Attain ${0}.", amtStr):
					string.Format("Maintain ${0}.", amtStr);
				break;
			case QuantityType.FishPopulation:
				ret = (evaluationFrequency==EvaluationFrequency.Once)?
					string.Format("Attain a fish population of {0} individuals.", amtStr):
					string.Format("Maintain a fish population of {0} individuals.", amtStr);
				// TODO: account for attributes
				break;
			}
			// add attribute requirements as needed
			if (quantityRequirement.attributeRequirements.Length > 0)
			{
				quantityRequirement.GetMinMaxDensityIntensity(out densityLow, out densityHigh, out intensityLow, out intensityHigh);
				System.Text.StringBuilder sb = new System.Text.StringBuilder();
				bool isDensityAdded = false;
				bool isIntensityAdded = false;
				foreach (AttributeRequirement req in quantityRequirement.attributeRequirements)
				{
					switch (req.attribute)
					{
					case QuantityAttribute.Age: // TODO: account for this as needed
						break;
					case QuantityAttribute.Density:
						if (!isDensityAdded)
						{
							sb.Append(string.Format(", {0} [{1:0.00}, {2:0.00}]", req.attribute, densityLow, densityHigh));
							isDensityAdded = true;
						}
						break;
					case QuantityAttribute.Intensity:
						if (!isIntensityAdded)
						{
							sb.Append(string.Format(", {0} [{1:0.00}, {2:0.00}]", req.attribute, intensityLow, intensityHigh));
							isIntensityAdded = true;
						}
						break;
					case QuantityAttribute.Quality: // TODO: account for this as needed
						break;
					case QuantityAttribute.Size: // TODO: account for this as needed
						break;
					}
				}
				if (sb.Length > 0)
				{
					ret = string.Format("{0} ({1}).", ret.Substring(0, ret.Length-1), sb.ToString().Substring(2));
				}
			}
			return ret;
		}
	}
	
	/// <summary>
	/// objectives in the quest
	/// </summary>
	public Objective[] objectives = new Objective[1];
	
	/// <summary>
	/// is the quest complete?
	/// </summary>
	public bool isComplete
	{
		get
		{
			foreach (Objective objective in objectives)
			{
				if (!objective.isComplete) return false;
			}
			return true;
		}
	}
	
	/// <summary>
	/// what is the quest's completion percentage?
	/// </summary>
	public float completionPercentage
	{
		get
		{
			float ret = 0f;
			foreach (Objective o in objectives) ret += o.completionPercentage;
			return ret/objectives.Length;
		}
	}
	
	/// <summary>
	/// Evaluate all objectives that evaluate for the specified type of event
	/// </summary>
	/// <param name="evaluateOn">
	/// A <see cref="Quest.Objective.EvaluateOn"/>
	/// </param>
	void EvaluateObjectivesOn(Quest.Objective.EvaluateOn evaluateOn)
	{
		bool wasDone = isComplete;
		foreach (Objective objective in objectives) if (objective.evaluateOn == evaluateOn) objective.EvaluateForQuester(quester);
		if (!wasDone && isComplete) Messenger.Send(new MessageQuestComplete(this));
	}
}
*/