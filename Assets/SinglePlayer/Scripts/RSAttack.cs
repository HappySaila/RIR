﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RSAttack : MonoBehaviour {
    [HideInInspector] public bool isPlayerOne;
	public float ramDelay;
	public float ramDistance;
	public float ramForce;
	Animator anim;
	Rigidbody rigid;
    RSManager robotManager;
	bool isRamming = false;
	public bool canRam = true;
	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
		rigid = GetComponentInChildren<Rigidbody>();
        robotManager = GetComponentInParent<RSManager>();
	}
	
	void Update () {
		UpdateFire();
	}

	void UpdateFire()
	{
        float attack = 0;
        if (!robotManager.isAI){
			if (isPlayerOne){
				attack = Input.GetAxis("Jump");
			} else {
                attack = Input.GetButton("Ram") ? 1 : 0;
			}
		}
            
            
		if (attack > 0 && canRam)
		{
            InitiateRam();
		}
	}

    public void InitiateRam(){
        if (!canRam){
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

	public void OnCollisionEnter(Collision col)
	{
		if (isRamming)
		{
			Hit(col.gameObject, col.contacts[0].point);
		}
	}

	void Hit(GameObject target, Vector3 point)
	{
		if (target.tag == "Hittable")
		{
			target.GetComponent<Rigidbody>().AddForceAtPosition(-transform.forward * ramForce, point, ForceMode.VelocityChange);
            if (target.GetComponentInParent<RSManager>() != null)
			{
                //the player has rammed a robot
				target.GetComponentInParent<RSManager>().Die();
			}
		}
	}

	public void Die()
	{
		//player has just died an must turn into a laborer
		canRam = false;
        isRamming = false;
	}
}
