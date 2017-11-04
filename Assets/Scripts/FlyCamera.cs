using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyCamera : MonoBehaviour {
    Camera cam;
    Rigidbody rigid;
    public bool inFreeFlyMode;
    public float flySpeed;
    public bool isPlayerOne;
	// Use this for initialization
	void Start () {
        cam = GetComponentInChildren<Camera>();
        rigid = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
        if (inFreeFlyMode){
            float y = 0;
			float xRot = 0;
			float yRot = 0;
            if (isPlayerOne){
                y = Input.GetButton("Jump") ? 1 : 0;
                //xRot = Input.GetAxis("Mouse X");
                xRot = Input.GetAxis("Horizontal2") * 5f;
                yRot = Input.GetAxis("Vertical2") * 2f;
			} else {
				y = Input.GetAxis("Ram");
				xRot = Input.GetAxis("Horizontal") * 4;
				yRot = Input.GetAxis("Vertical") * 4;
			}
                
            rigid.velocity = cam.transform.forward * flySpeed * y;
			cam.transform.Rotate(new Vector3(-yRot,0,0));
			transform.Rotate(new Vector3(0, xRot, 0));
		}
	}

    public void Activate(Transform t){
        //will activate camera at position t
        inFreeFlyMode = true;
        cam.enabled = true;
        transform.position = t.position;
    }

    public void Disable(){
        inFreeFlyMode = false;
        cam.enabled = false;
    }
}
