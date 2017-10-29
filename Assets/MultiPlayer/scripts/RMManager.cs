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
        FIGHTER,DEADLABOURER,BUILDER,MOVINGTOBASE,DYING
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

    #region unity
    // Use this for initialization
    void Start()
    {
            type = types.FIGHTER;
            robotMovement = GetComponentInChildren<RMMovement>();
            labourerController = GetComponentInChildren<RMLabourerController>();
            robotAttack = GetComponentInChildren<RMAttack>();
            labourerController.isIdleLaborer = false;
            if (networkObject.NetworkId == 1)
            {
                GetComponentInChildren<ColorRobot>().SetColor(true);
                team = 1;
            }
            else
            {
                GetComponentInChildren<ColorRobot>().SetColor(false);
                team = 2;
            }
            if (networkObject.IsServer)
            {
                Debug.Log("server bois");
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
        }else if (networkObject.IsOwner && type == types.MOVINGTOBASE)
        {
            networkObject.position = labourerController.move();
        }
        else
        {
            // FrontCamera.enabled = false;
            robotMovement.move(networkObject.position, networkObject.x, networkObject.y);
            robotMovement.setRotation(networkObject.rotation);
        }
        

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
        

        robotAttack.enabled = false;

        Camara.enabled = false;

        GetComponentInChildren<ColorRobot>().SetGrey();
        team = 0;
        if (networkObject.IsServer) {
            networkObject.TakeOwnership();
        }
        Invoke("makeIntoDead", 1f);
    }

    public void makeIntoDead()
    {
        Debug.Log("dead dead dead");
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
        Debug.Log("make into labourer called");
        robotMovement.rigid.velocity = new Vector3(0, 0, 0);
        networkObject.position = args.GetNext<Vector3>();
        transform.position = networkObject.position;
        makeIntoLabouer();
    }

    #endregion
}
