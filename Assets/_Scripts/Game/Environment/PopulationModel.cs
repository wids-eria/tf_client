using UnityEngine;
using System.Collections;

public class PopulationModel
{
	public int unhousedPeople;
	public int totalPeople;
	
	public int housedPeople {
		get {return totalPeople - unhousedPeople;}
	}
}

/*
public class Desirablility
{
	public float desirability;
	
	
	protected int landCoverScore;
	protected int treeCount;
	protected int animalCount;
}*/
