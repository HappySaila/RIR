using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking;

public class RMManager : RobotManagerBehavior {
    #region fields
        [HideInInspector] public RMMovement robotMovement;
        [HideInInspector] public RMAttack robotAttack;
        public Camera Camara;
        int team = 0;
    #endregion

    #region unity
    // Use this for initialization
    void Start()
    { 
        robotMovement = GetComponentInChildren<RMMovement>();
        robotAttack = GetComponentInChildren<RMAttack>();
        if (networkObject.IsServer)
        {
            Debug.Log("server bois");
        }        
    }

    // Update is called once per frame
    void Update()
    {
        if (!networkObject.IsServer)
        {
            if (networkObject.IsOwner)
            {
                // FrontCamera.enabled = true
                networkObject.position = robotMovement.move();
                networkObject.rotation = robotMovement.getRotation();
                networkObject.x = robotMovement.x;
                networkObject.y = robotMovement.y;
            }
            else
            {
                // FrontCamera.enabled = false;
                robotMovement.move(networkObject.position, networkObject.x, networkObject.y);
                robotMovement.setRotation(networkObject.rotation);
            }
        }
    }

#endregion

#region RPC
    public override void ramPlayer(RpcArgs args)
    {
        
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

    #endregion
}
