using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMovementData : MonoBehaviour {
    public float RamDistance;
    public float IdleLaborerCollectionTime;

	void Start(){
		RamDistance = Random.Range (-0.2f, 0.2f) * RamDistance + RamDistance;
	}
    public float GetRamDistance(){
        //will ram at a variable time around RamDistance
        return  RamDistance;
    }

}
