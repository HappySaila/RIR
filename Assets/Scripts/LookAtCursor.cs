using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCursor : MonoBehaviour {
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hitPosition;
		if (Physics.Raycast(camRay, out hitPosition, 100))
		{
			Vector3 playerToMouse = hitPosition.point - transform.position;
            transform.LookAt(hitPosition.point);
		}
	}
}
