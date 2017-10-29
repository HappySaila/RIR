using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RMLabourerController : MonoBehaviour {
    public Transform redConstructionSite;
    public Transform blueConstructionSite;
    Vector3 target;
    NavMeshAgent agent;
    public SphereCollider trigger;
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

    public void OnTriggerStay(Collider col)
    {
        if (col.GetComponentInParent<RMManager>() != null)
        {
            if (col.GetComponentInParent<RMManager>().team != 0)
            {

                if (col.GetComponentInParent<RMManager>().type != RMManager.types.DYING)
                {
                    if (isIdleLaborer)
                    {
                        StandUp(col);
                    }
                }
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
            GetComponentInParent<RMManager>().team = col.GetComponentInParent<RMManager>().team;
            GetComponentInParent<RMManager>().type = RMManager.types.MOVINGTOBASE;
            target = col.GetComponentInParent<RMManager>().team == 1 ?
                        TimeMachine.redTimeMachine.targetPosition.position :
                        TimeMachine.blueTimeMachine.targetPosition.position;
            GetComponentInChildren<ColorRobot>().SetColor(col.GetComponentInParent<RMManager>().team == 1);
            agent.SetDestination(target);
            trigger.enabled = false;

            //robotManager.playSound("collectLabourer");

        }

    }

    public void SetLaborer()
    {
        if (trigger == null)
        {
            trigger = GetComponent<SphereCollider>();
        }
        trigger.enabled = true;
        Debug.Log("enabled trigger");
        isIdleLaborer = true;
        rigid.constraints = RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationZ |
            RigidbodyConstraints.FreezeRotationY;
    }

    public Vector3 move()
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
            }
        }
        return transform.position;
    }
    // Update is called once per frame
    void Update () {
        if (isBuilding)
        {
            timeMachine.Build();
        }
    }

    public void playHammerSoundFromAnimation()
    {
        //robotManager.playSound("hammer");
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
