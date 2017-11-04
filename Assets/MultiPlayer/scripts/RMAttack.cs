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
    RMManager robotManger;
    // Use this for initialization
    void Start () {
        anim = GetComponent<Animator>();
        rigid = GetComponentInChildren<Rigidbody>();
        robotManger = GetComponentInParent<RMManager>();
    }
	
	// Update is called once per frame
	void Update () {
        UpdateFire();

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

    public void OnCollisionEnter(Collision col)
    {
        if (isRamming)
            Hit(col.gameObject, col.contacts[0].point);
    }

    void Hit(GameObject target, Vector3 point)
    {
        if (target.tag == "Hittable")
        {
            Debug.Log("hit");
            //robotManger.networkObject.SendRpc(robotManger.networkObject.Owner,"ramPlayer",BeardedManStudios.Forge.Networking.Receivers.Target, -transform.forward * ramForce, point);
            target.GetComponent<Rigidbody>().AddForceAtPosition(-transform.forward * ramForce, point, ForceMode.VelocityChange);
            if (target.GetComponentInParent<RMManager>() != null)
            {
                //the player has rammed a robot
                robotManger.playSound("hit");
                target.GetComponentInParent<RMManager>().Die();
            }
        }

    }

    void UpdateFire()
    {
        float ramVal = robotManger.SendRamData();
        //set network objects jumpval
        if (ramVal > 0 && canRam)
        {
            canRam = false;
            anim.SetTrigger("Ram");
        }
    }
    public void getHit(Vector3 force, Vector3 point)
    {
        anim.SetTrigger("Ram");
        rigid.GetComponent<Rigidbody>().AddForceAtPosition(-force, point, ForceMode.VelocityChange);
    }

    public void Die()
    {
        //player has just died an must turn into a laborer
        canRam = false;
        isRamming = false;
    }

}
