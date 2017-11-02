using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICamera : MonoBehaviour {
    public Transform[] canvasPositions;
    public Transform target;
    bool isMoving;
    Transform CurrentRoom;
    public Transform MainRoom;
	public Transform JoiningRoom;
	public Transform GameOverRoom;

    // Use this for initialization
    void Start () {
        transform.LookAt(canvasPositions[0]);
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 lTargetDir = target.position - transform.position;
		//lTargetDir.y = 0.0f;
		transform.rotation = Quaternion.RotateTowards(transform.rotation, 
                                                      Quaternion.LookRotation(lTargetDir), 10);

        if (isMoving){
			transform.position = Vector3.Lerp(transform.position, CurrentRoom.position, 0.1f);
			transform.rotation = Quaternion.Slerp(transform.rotation, CurrentRoom.rotation, 0.02f);
            if (Vector3.Distance(transform.position, CurrentRoom.position) < 0.001f){
                isMoving = false;
            }
        }
       
	}

    public void LookAtMultiplayer(){
        target = canvasPositions[1];
    }

    public void LookAtMainMenu(){
		target = canvasPositions[0];
        CurrentRoom = MainRoom;
        isMoving = true;
	}

    public void LookAtCreateRoom(){
        CurrentRoom = MainRoom;
		isMoving = true;
        target = canvasPositions[2];
    }

    public void LookAtWaitingRoom(){
        CurrentRoom = JoiningRoom;
		isMoving = true;
        target = canvasPositions[3];
    }

	public void LookAtGameOverRoom()
	{
        CurrentRoom = GameOverRoom;
		isMoving = true;
		target = canvasPositions[4];
	}
}
