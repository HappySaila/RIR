using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnForgeCanvas : MonoBehaviour {
    public GameObject forgeCanvas;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(1)){
			Instantiate(forgeCanvas);
		}
	}
}
