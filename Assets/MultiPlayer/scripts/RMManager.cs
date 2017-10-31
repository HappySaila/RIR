using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine.AI;

public class RMManager : RobotManagerBehavior {
    public enum types
    {
        FIGHTER = 0,DEADLABOURER = 1,BUILDER = 2,MOVINGTOBASE = 3,DYING = 4
    }

    #region fields
        [HideInInspector] public RMMovement robotMovement;
        [HideInInspector] public RMAttack robotAttack;
        [HideInInspector] public RMLabourerController labourerController;
    [Tooltip("Collider that makes robot hover above ground.")] public SphereCollider hoverBase;
    public Camera Camara;
    [HideInInspector] public int team = 0;
    [HideInInspector] public types type;
    #endregion
    int oldteam;
    #region unity
    // Use this for initialization
    void Start()
    {
        type = types.FIGHTER;
        robotMovement = GetComponentInChildren<RMMovement>();
        labourerController = GetComponentInChildren<RMLabourerController>();
        robotAttack = GetComponentInChildren<RMAttack>();
        labourerController.isIdleLaborer = false;
        if (networkObject.IsServer)
        {
            BMSLogger.Instance.Log("server bois");
        }
        if (networkObject.IsOwner)
        {
            Camara.enabled = true;
        }
        else
        {
            Camara.enabled = false;
        }

    }

    // Update is called once per frame
    void Update()
    {
       /* if (!networkObject.IsServer && networkObject.IsOwner)
        {
            BMSLogger.Instance.Log("I am team " + team);
        }*/

        if (networkObject.IsOwner && (type == types.FIGHTER || type == types.DEADLABOURER))
        {   
            
            // FrontCamera.enabled = true
            networkObject.position = robotMovement.move(team);
            networkObject.rotation = robotMovement.getRotation();
            networkObject.x = robotMovement.x;
            networkObject.y = robotMovement.y;
            if (team != 0) { 
                robotAttack.attack();
            }
            if (type == types.FIGHTER)
            {
                Camara.enabled = true;
            }
        }else if (networkObject.IsOwner && type == types.MOVINGTOBASE)
        {
            networkObject.position = labourerController.move();
            networkObject.rotation = labourerController.rotation();
            labourerController.tryStartBuilding();
        }
        else
        {
            // FrontCamera.enabled = false;
            robotMovement.move(networkObject.position, networkObject.x, networkObject.y);
            robotMovement.setRotation(networkObject.rotation);
            Camara.enabled = false;
            if (type == types.MOVINGTOBASE)
            {
                
                labourerController.tryStartBuilding();
            }
        }

        if (networkObject.IsServer)
        {
            networkObject.blueTeamTimemachine = TimeMachine.blueTimeMachine.currentProgress;
            networkObject.redTeamTimemachine = TimeMachine.redTimeMachine.currentProgress;
        }
        TimeMachine.blueTimeMachine.currentProgress = networkObject.blueTeamTimemachine;
        TimeMachine.redTimeMachine.currentProgress = networkObject.redTeamTimemachine;

        //d  }
    }

    public float SendRamData()
    {
        float ramVal = 0;
        if (!networkObject.IsOwner)
        {
            ramVal = networkObject.ram;
            return ramVal;
        }

        networkObject.ram = Input.GetAxis("Jump");
        return Input.GetAxis("Jump");
    }

    #endregion
    public void makeIntoLabouer()
    {
        
        Die();
    }
    public void Die()
    {
        robotMovement.Die();
        //robotMovement.enabled = false;
        hoverBase.enabled = false;
        GetComponentInChildren<NavMeshAgent>().enabled = false;
        type = types.DEADLABOURER;

        robotAttack.enabled = false;
        labourerController.isFighter = false;
        Camara.enabled = false;

        GetComponentInChildren<ColorRobot>().SetGrey();

        oldteam = team;
        team = 0;
        Invoke("makeIntoDead", 3f);
        Camara.enabled = false;
        if(networkObject.IsOwner)
        BMSLogger.Instance.Log("I am  " + team);
    }

    public void makeIntoDead()
    {
        if (networkObject.IsOwner)
            BMSLogger.Instance.Log("now I am  " + team);
        if (networkObject.IsServer && !networkObject.IsOwner)
        {
            NetworkingPlayer oldOwner = networkObject.Owner;
            
            if (oldteam == 1)
            {
                gameManager.instance.redTeamDead.Enqueue(oldOwner);
                Debug.Log("amount of dead red players " + gameManager.instance.redTeamDead.Count);
            }
            else
            {
                gameManager.instance.blueTeamDead.Enqueue(oldOwner);
                Debug.Log("amount of dead blue players " + gameManager.instance.blueTeamDead.Count);
            }

            networkObject.TakeOwnership();
        }
        team = 0;

        networkObject.SendRpc("syncRobotState", Receivers.AllBuffered, 0, 1);
        labourerController.SetLaborer();
        
        //type = types.DEADLABOURER;
    }
#region RPC
    public override void ramPlayer(RpcArgs args)
    {
        Vector3 force = args.GetNext<Vector3>();
        Vector3 point = args.GetNext<Vector3>();
        robotAttack.getHit(force, point);
        Debug.Log("I have been hit");
        Die();
    }

    public override void setStarting(RpcArgs args)
    {
        Debug.Log("position set");
        networkObject.position = args.GetNext<Vector3>();
        networkObject.rotation = args.GetNext<Quaternion>();
        robotMovement.setPosition(networkObject.position, networkObject.rotation);
        team = args.GetNext<int>();
        if (networkObject.IsOwner)
            BMSLogger.Instance.Log("starting as team " + team);
        GetComponentInChildren<ColorRobot>().SetColor(team == 1);
        if (networkObject.IsOwner)
        {
            Camara.enabled = true;
        }
        else
        {
            Camara.enabled = false;
        }
    }

    public override void makeIntoLabourer(RpcArgs args)
    {
        robotMovement.rigid.velocity = new Vector3(0, 0, 0);
        networkObject.position = args.GetNext<Vector3>();
        transform.position = networkObject.position;
        makeIntoLabouer();
    }

    public override void syncRobotState(RpcArgs args)
    {
        team = args.GetNext<int>();
        if (networkObject.IsOwner)
            BMSLogger.Instance.Log("sync state called " + team);
        switch (args.GetNext<int>())
        {
            case (0):
                type = types.FIGHTER;
                labourerController.isFighter = true;
                break;
            case (1):
                labourerController.anim.SetTrigger("Die");
                type = types.DEADLABOURER;
                labourerController.isFighter = false;
                break;
            case (2):
                type = types.BUILDER;
                labourerController.isBuilding = true;
                labourerController.isFighter = false;
                labourerController.anim.SetBool("isBuilding", true);
                labourerController.agent.enabled = false;
                if(team == 1)
                {
                    TimeMachine.redTimeMachine.MAddLaborerToAvailableLaborer(gameObject);
                    BMSLogger.Instance.Log(" red labouerer count " + TimeMachine.redTimeMachine.MAvalableLaboreres.Count);
                }
                else
                {
                    
                    TimeMachine.blueTimeMachine.MAddLaborerToAvailableLaborer(gameObject);
                    BMSLogger.Instance.Log(" blue labouerer count " + TimeMachine.blueTimeMachine.MAvalableLaboreres.Count);
                }
                break;
            case (3):
                type = types.MOVINGTOBASE;
                labourerController.anim.SetTrigger("Spin");
                labourerController.agent.enabled = true;
                labourerController.isFighter = false;
                break;
            case (4):
                type = types.DYING;
                labourerController.isFighter = false;
                break;
        }
        if (team != 0) {
            GetComponentInChildren<ColorRobot>().SetColor(team == 1);
        }
        else
        {
            GetComponentInChildren<ColorRobot>().SetGrey();
        }
    }

    public override void destroyMyself(RpcArgs args)
    {
        Destroy(this);
    }

    #endregion
}
