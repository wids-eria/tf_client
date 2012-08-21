/// <summary>
/// Survey info.
/// </summary>
using UnityEngine;

public struct Surveys 
{
	public Survey[] surveys { get; set; }
}

[System.Serializable]
public class Survey : System.Object
{
	public string capture_date { get; set; }
	public string created_at { get; set; }
	public string updated_at { get; set; }
	public int id { get; set; }
	public int player_id { get; set; }
	public float num_2in_trees { get; set; }
	public float num_4in_trees { get; set; }
	public float num_6in_trees { get; set; }
	public float num_8in_trees { get; set; }
	public float num_10in_trees { get; set; }
	public float num_12in_trees { get; set; }
	public float num_14in_trees { get; set; }
	public float num_16in_trees { get; set; }
	public float num_18in_trees { get; set; }
	public float num_20in_trees { get; set; }
	public float num_22in_trees { get; set; }
	public float num_24in_trees { get; set; }
	
	public static Survey current {get; set;}
	public bool loaded {get; set;}
	
	public Survey()
	{}
}

public struct SurveyWrapper
{
	public Survey survey {get; set;}
}