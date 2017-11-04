using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RMLabourerController : MonoBehaviour {
    public Transform redConstructionSite;
    public Transform blueConstructionSite;
    Vector3 target;
    public NavMeshAgent agent;
    TimeMachine timeMachine;
    public SphereCollider trigger;
    Rigidbody rigid;
    public Animator anim;

    public bool isIdleLaborer;
    [HideInInspector] public bool isBuilding;
    public bool isFighter;

    RMManager robotManager;

    void Start () {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        robotManager = GetComponentInParent<RMManager>();
    }

    public void OnTriggerStay(Collider col)
    {
        if (col.GetComponentInParent<RMManager>() != null)
        {
            if (col.GetComponentInParent<RMManager>().team != 0)
            {

                if (col.GetComponentInParent<RMManager>().type == RMManager.types.FIGHTER)
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
            robotManager.networkObject.SendRpc("syncRobotState", BeardedManStudios.Forge.Networking.Receivers.All, GetComponentInParent<RMManager>().team, 3);
            target = col.GetComponentInParent<RMManager>().team == 1 ?
                        TimeMachine.redTimeMachine.targetPosition.position :
                        TimeMachine.blueTimeMachine.targetPosition.position;
            Debug.Log(GetComponentInParent<RMManager>().team);
            GetComponentInChildren<ColorRobot>().SetColor(col.GetComponentInParent<RMManager>().team == 1);
            agent.SetDestination(target);
            trigger.enabled = false;

            //robotManager.playSound("collectLabourer");

        }

    }

    public Quaternion rotation()
    {
        return transform.rotation;
    }
    
    public void SetLaborer()
    {
        if (trigger == null)
        {
            trigger = GetComponent<SphereCollider>();
        }
        trigger.enabled = true;
        isIdleLaborer = true;
        
        rigid.constraints = RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationZ |
            RigidbodyConstraints.FreezeRotationY;
    }

    public Vector3 move()
    {
        if (agent.enabled && !isFighter)
        {
            agent.SetDestination(target);
            if (Vector3.Distance(target, transform.position) > 1.5f)
            {
                transform.forward = -(target - transform.position);
               
            }
        }
        return transform.position;
    }

    

    // Update is called once per frame
    void Update () { 
        if (isBuilding)
        {
            if (robotManager.team == 1)
            {
                TimeMachine.redTimeMachine.Build();
            }
            else
            {
                TimeMachine.blueTimeMachine.Build();
            }
        }
    }

    public void playHammerSoundFromAnimation()
    {
        //robotManager.playSound("hammer");
    }
    public void tryStartBuilding()
    {
        //Debug.Log(Vector3.Distance(target, transform.position));
        if (Vector3.Distance(target, transform.position) < 3f)
        {
            //target has reached its destination and must start building
            BMSLogger.Instance.Log("building ");
            anim.SetBool("isBuilding", true);
            robotManager.type = RMManager.types.BUILDER;
            agent.enabled = false;
            robotManager.robotMovement.enabled = false;
            isBuilding = true;
            if (robotManager.networkObject.IsServer)
            {
                robotManager.networkObject.SendRpc("syncRobotState", BeardedManStudios.Forge.Networking.Receivers.All, robotManager.team, 2);
            }
        }
    }

    public void StartBuilding(TimeMachine t)
    {
        anim.SetBool("isBuilding", true);
        timeMachine = t;
        isBuilding = true;
        agent.enabled = false;
        Debug.Log("I am building like a beast");
        robotManager.type = RMManager.types.BUILDER;
        //add to time machine T laborer available to respawn
        t.MAddLaborerToAvailableLaborer(gameObject);
    }
}
