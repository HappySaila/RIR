﻿    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotFollow : MonoBehaviour {
	public Transform RobotToLookAt;
	public Camera FrontCamera;
	public Camera BackCamera;
    FlyCamera flyCamera;

	bool camIsInFront;
	public bool isSplitScreen;
    public float flySpeed;
	
    void Start(){
        flyCamera = GetComponentInChildren<FlyCamera>();
        isSplitScreen = InitialSpawnManager.instance.isSplitScreen;
    }
	// Update is called once per frame
	void Update () {
		Orientate ();
        Follow ();
        CameraSwitch();
		
	}

    public void EnableFreeFly(){
        DisableCameras();
    }

    void CameraSwitch(){
        if (!isSplitScreen){
			if (Input.GetKeyDown(KeyCode.E))
			{
				SwitchCamera();
			}
			
			if (Input.GetKeyUp(KeyCode.E))
			{
				SwitchCamera();
			}
        } 
    }

    public void DisableCameras(){
		FrontCamera.enabled = false;
        BackCamera.enabled = false;
		BackCamera.gameObject.GetComponent<AudioListener>().enabled = false;
    }

    public void EnableCameras()
    {
        FrontCamera.enabled = true;
        BackCamera.enabled = true;
    }

    public void DisableAudioListener(){
		GetComponentInChildren<AudioListener>().enabled = false;
	}

	void Follow(){
		//camera will lerp tranform to back/front position
	}

	void Orientate (){
		//camera will always look at RobotToLookAt
		FrontCamera.transform.LookAt (RobotToLookAt);
		BackCamera.transform.LookAt (RobotToLookAt);
	}

	void SwitchCamera(){
		camIsInFront = !camIsInFront;

		if (camIsInFront){
			FrontCamera.enabled = true;
			BackCamera.enabled = false;
		} else {
			FrontCamera.enabled = false;
			BackCamera.enabled = true;
		}
	}

	public void DisableCameraColliders(){
		FrontCamera.GetComponent <Collider>().enabled = false;
		BackCamera.GetComponent <Collider>().enabled = false;
	}

    public void SetColliders(bool state){
		FrontCamera.gameObject.GetComponent<Collider>().enabled = state;
		BackCamera.gameObject.GetComponent<Collider>().enabled = state;
    }
}
