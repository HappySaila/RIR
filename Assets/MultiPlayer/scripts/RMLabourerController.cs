using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RMLabourerController : MonoBehaviour {
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

    void Start () {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        robotManager = GetComponentInParent<RSManager>();
    }
	
	// Update is called once per frame
	void Update () {
        if (agent.enabled && !isFighter)
        {
            transform.forward = -(target - transform.position);
            agent.SetDestination(target);
            if (Vector3.Distance(target, transform.position) < 1.5f)
            {
                //target has reached its destination and must start building
                anim.SetBool("isBuilding", true);
                agent.enabled = false;
            }
        }

        if (isBuilding)
        {
            timeMachine.Build();
        }
    }

    public void playHammerSoundFromAnimation()
    {
        robotManager.playSound("hammer");
    }

    public void StartBuilding(TimeMachine t)
    {
        anim.SetBool("isBuilding", true);
        timeMachine = t;
        isBuilding = true;
        agent.enabled = false;

        //add to time machine T laborer available to respawn
        t.AddLaborerToAvailableLaborer(GetComponentInParent<RSManager>());
    }
}
