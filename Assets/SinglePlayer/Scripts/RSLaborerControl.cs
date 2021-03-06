﻿using UnityEngine;
using UnityEngine.AI;

public class RSLaborerControl : MonoBehaviour
{
	//destination that the AI will move to
	public Transform redConstructionSite;
	public Transform blueConstructionSite;
    Vector3 target;
	NavMeshAgent agent;
	SphereCollider trigger;
	Rigidbody rigid;
	Animator anim;
    TimeMachine timeMachine;

	public bool isIdleLaborer;
	[HideInInspector] public bool isBuilding;
	public bool isFighter;

	RSManager robotManager;

	void Start()
	{
        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
		agent.updateRotation = false;
		robotManager = GetComponentInParent<RSManager>();
	}

    private void Awake()
    {
    }

    // Update is called once per frame
    void Update()
	{
		if (agent.enabled && !isFighter)
		{
			transform.forward = -(target - transform.position);
			agent.SetDestination(target);
			if (Vector3.Distance(target, transform.position) < 1.5f)
			{
				//target has reached its destination and must start building
				anim.SetBool("isBuilding", true);
				agent.enabled = false;
                rigid.constraints = RigidbodyConstraints.FreezeAll;
			}
		}

        if (isBuilding){
            timeMachine.Build();


        }

            
	}
	public void playHammerSoundFromAnimation(){
		robotManager.playSound ("hammer");
	}

    public void StartBuilding(TimeMachine t){
		anim.SetBool("isBuilding", true);
        timeMachine = t;
        isBuilding = true;
		agent.enabled = false;

        //add to time machine T laborer available to respawn
		t.AddLaborerToAvailableLaborer(GetComponentInParent<RSManager>());
            
    }

	public void CallSetLaborer()
	{
        if (rigid.velocity.magnitude < float.Epsilon){
            SetLaborer();
        } else {
            Invoke("CallSetLaborer", 0.2f);
        }       
	}

	void SetLaborer()
	{
		if (trigger == null)
		{
			trigger = GetComponent<SphereCollider>();
		}

		trigger.enabled = true;
		isIdleLaborer = true;
        rigid.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ |
            RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
	}

	void OnTriggerStay(Collider col)
	{
        if (col.GetComponentInParent<RSManager>() != null)
		{
            if (isIdleLaborer && col.GetComponentInParent<RSLaborerControl>().isFighter)
			{
				StandUp(col);
			}
		}
	}


    void StandUp(Collider col)
	{
		transform.up = Vector3.Lerp(transform.up, Vector3.up, Time.deltaTime);
		if (Vector3.Distance(transform.up.normalized, Vector3.up) < 0.1)
		{
			//player has completed standing up
			isIdleLaborer = false;
            isFighter = false;
			transform.up = Vector2.up;
			anim.SetTrigger("Spin");
			agent.enabled = true;
            GetComponentInParent<RSManager>().isRed = col.GetComponentInParent<RSManager>().isRed;
            target = col.GetComponentInParent<RSManager>().isRed ? 
				TimeMachine.redTimeMachine.pointsAroundTimeMech[Random.Range(0,TimeMachine.redTimeMachine.pointsAroundTimeMech.Length)].transform.position : 
				TimeMachine.blueTimeMachine.pointsAroundTimeMech[Random.Range(0,TimeMachine.blueTimeMachine.pointsAroundTimeMech.Length)].transform.position ;


            GetComponentInChildren<ColorRobot>().SetColor(col.GetComponentInParent<RSManager>().isRed);
			agent.SetDestination(target);
			trigger.enabled = false;

			robotManager.playSound ("collectLabourer");
		}

	}

    public void Reset()
    {
        isBuilding = false;
        isFighter = true;
        isIdleLaborer = false;
    }

}

