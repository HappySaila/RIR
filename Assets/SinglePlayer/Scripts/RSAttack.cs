using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RSAttack : MonoBehaviour
{
	[HideInInspector] public bool isPlayerOne;
	public float ramDelay;
	public float ramDistance;
	public float ramForce;
	Animator anim;
	Rigidbody rigid;
	RSManager robotManager;
	bool isRamming = false;
	public bool canRam = true;
	public float timeRamStarted;

	// Use this for initialization
	void Start ()
	{
		anim = GetComponent<Animator> ();
		rigid = GetComponentInChildren<Rigidbody> ();
		robotManager = GetComponentInParent<RSManager> ();
	}

	void Update ()
	{
		UpdateFire ();
	}

	void UpdateFire ()
	{
		float attack = 0;
		if (!robotManager.isAI) {
			if (isPlayerOne) {
				attack = Input.GetAxis ("Jump");

			} else {
				attack = Input.GetButton ("Ram") ? 1 : 0;

			}
		}
		if (attack > 0 && canRam) {
			InitiateRam ();
		}
	}

	public void InitiateRam ()
	{
        if (!canRam){
            return;
        }
		canRam = false;
		timeRamStarted = Time.time;
		anim.SetTrigger ("Ram");
	}

	public void Ram ()
	{
		isRamming = true;
        rigid.AddForce (-transform.forward * 50, ForceMode.Impulse);
	}

	public void OnRamComplete ()
	{
		canRam = true;
        isRamming = false;
	}

	public void OnCollisionEnter (Collision col)
	{
		if (isRamming) {
			Hit (col.gameObject, col.contacts [0].point);
            isRamming = false;
		}
	}

    void Hit(GameObject target, Vector3 point)
    {
        if (target.tag == "Hittable")
        {
            target.GetComponent<Rigidbody>().AddForceAtPosition(-transform.forward * ramForce, point, ForceMode.VelocityChange);
            RSManager rsManagerofTarget = target.GetComponentInParent<RSManager>();

            if (rsManagerofTarget != null)
            { //the player has rammed a robot
                if (rsManagerofTarget.isRed != robotManager.isRed)
                {
                    if (rsManagerofTarget.robotAttack.isRamming)
                    {//he is also ramming
                        if (timeRamStarted <= rsManagerofTarget.robotAttack.timeRamStarted)
                        {
                            target.GetComponentInParent<RSManager>().Die();
                        }
                    } else if (robotManager.robotLaborerControl.isFighter){
						target.GetComponentInParent<RSManager>().Die();
					}
                }
                robotManager.playSound("hit");
            }
        }
    }

	public void Die ()
	{
		//player has just died an must turn into a laborer
		canRam = false;
		isRamming = false;
	}
}
