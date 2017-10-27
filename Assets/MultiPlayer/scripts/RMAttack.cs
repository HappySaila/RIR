using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RMAttack : MonoBehaviour {

    public float ramDelay;
    public float ramDistance;
    public float ramForce;
    Animator anim;
    Rigidbody rigid;
    public bool canRam = true;
    bool isRamming = false;
    // Use this for initialization
    void Start () {
        anim = GetComponent<Animator>();
        rigid = GetComponentInChildren<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update () {
        
		
	}
    public void attack()
    {
        float attack = Input.GetAxis("Jump");

        if (attack > 0 && canRam)
        {
            InitiateRam();
        }
    }

    public void InitiateRam()
    {
        if (!canRam)
        {
            return;
        }
        canRam = false;
        anim.SetTrigger("Ram");
        rigid.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY
            | RigidbodyConstraints.FreezeRotationZ;
    }

    public void Ram()
    {
        if (isRamming)
        {
            isRamming = false;
            rigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY
                | RigidbodyConstraints.FreezeRotationZ;
            return;
        }

        Invoke("Ram", 0.5f);
        rigid.AddForce(-transform.forward * 50, ForceMode.Impulse);
        isRamming = true;
    }

    public void OnRamComplete()
    {
        canRam = true;
    }
}
