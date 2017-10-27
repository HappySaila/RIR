using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICamera : MonoBehaviour {
    public Transform[] canvasPositions;
    public Transform target;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 lTargetDir = target.position - transform.position;
		//lTargetDir.y = 0.0f;
		transform.rotation = Quaternion.RotateTowards(transform.rotation, 
                                                      Quaternion.LookRotation(lTargetDir), Time.time * 3);


        //transform.LookAt(position);	
	}

    public void LookAtMultiplayer(){
        target = canvasPositions[1];
    }

    public void LookAtMainMenu(){
		target = canvasPositions[0];
	}
}
