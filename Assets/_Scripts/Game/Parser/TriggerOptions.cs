using UnityEngine;
using System.Collections;

public class CutTreeTrigger: Trigger{
	
	public override void DoAction(){
		progress += 10f;
		isTriggered=true;
	}
}

public class PlantTreeTrigger: Trigger{
	public override void DoAction(){
		progress += 20f;
		isTriggered=true;
	}
}

public class DestroyLandTrigger : Trigger{
	
}

