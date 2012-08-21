using UnityEngine;
using System.Collections;

public enum AnimalType {
	Marten,
	Deer,
	WoodTurtle,
}
public struct Animal {
	
	string 		name  		{get; set;}
	AnimalType 	animalType 	{get; set;}
	int			count 		{get; set;}
	
}