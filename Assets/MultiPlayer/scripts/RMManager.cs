﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class RMManager : RobotManagerBehavior {
    public enum types
    {
        FIGHTER = 0,DEADLABOURER = 1,BUILDER = 2,MOVINGTOBASE = 3,DYING = 4
    }
    public AudioSource audio;
    bool canEndGame = true;
    #region fields
        [HideInInspector] public RMMovement robotMovement;
        [HideInInspector] public RMAttack robotAttack;
        [HideInInspector] public RMLabourerController labourerController;
    [Tooltip("Collider that makes robot hover above ground.")] public SphereCollider hoverBase;
    public Camera Camara;
    public Camera frontCamara;
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
        audio = GetComponentInChildren<AudioSource>();
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

    public void playSound(string sound)
    {//this methord exsist so that the audio souce is accses through here

        switch (sound)
        {
            case "hit":
                SoundManager.INSTANCE.PlayRamHit(audio);
                break;
            case "collectLabourer":
                SoundManager.INSTANCE.PlayLabourPickUp(audio);
                break;
            case "hammer":
                SoundManager.INSTANCE.PlayHammer(audio);
                break;
            case "spannereeraSDASF":
                //SoundManager.INSTANCE.PlayRamHit (audioSource);
                break;

        }
    }

    void ChangeToGameOver()
    {
        //look at game over
        SceneManager.LoadScene("GameOver");
        audioMixerScript.INSTANCE.ChangeSnapShot(0);
    }

    // Update is called once per frame
    void Update()
    {
        /* if (!networkObject.IsServer && networkObject.IsOwner)
         {
             BMSLogger.Instance.Log("I am team " + team);
         }*/
        if (canEndGame)
        {
                if (Input.GetKeyDown(KeyCode.E)){
                if(Camara.enabled == true)
                {
                    Camara.enabled = false;
                    frontCamara.enabled = true;
                }
            };
            if (Input.GetKeyUp(KeyCode.E))
            {
                if(frontCamara.enabled == true)
                {
                    Camara.enabled = true;
                    frontCamara.enabled = false;
                }
            }
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
                if (type == types.FIGHTER && frontCamara.enabled == false)
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
            if (TimeMachine.blueTimeMachine.currentProgress > 99)
            {
                networkObject.SendRpc(RPC_SYNC_ROBOT_STATE,Receivers.All,0,5);
            }
            else if (TimeMachine.redTimeMachine.currentProgress > 99)
            {
                networkObject.SendRpc(RPC_SYNC_ROBOT_STATE, Receivers.All, 1, 5);
            }
        }
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
        networkObject.SendRpc("iDied", Receivers.Server, team);
        team = 0;
        Invoke("makeIntoDead", 3f);
        Camara.enabled = false;
    }

    IEnumerator changeTeam(int team)
    {
        yield return new WaitForSeconds(1);
        this.team = team;
    }
    public void makeIntoDead()
    {
        if (networkObject.IsOwner)
            BMSLogger.Instance.Log("now I am  " + team);
        
        
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
        GetComponentInChildren<ColorRobot>().SetColor(team == 1);
        if (networkObject.IsOwner)
        {
            Camara.enabled = true;
        }
        else
        {
            Camara.enabled = false;
        }
        if (team == 1)
        {
            Instantiate(ParticleSystemManager.instance.RespawnRed, networkObject.position, Random.rotation);
        }
        else
        {
            Instantiate(ParticleSystemManager.instance.RespawnBlue, networkObject.position, Random.rotation);
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
        Debug.Log("hello");
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
                    TimeMachine.redTimeMachine.MAddLaborerToAvailableLaborer(this);
                    BMSLogger.Instance.Log(" red labouerer count " + TimeMachine.redTimeMachine.MAvalableLaboreres.Count);
                }
                else
                {
                    
                    TimeMachine.blueTimeMachine.MAddLaborerToAvailableLaborer(this);
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
            case (5):
                //This is really weird. should change the scene at this point
                BMSLogger.Instance.Log("Game over called " + team);
                PlayerPrefs.SetInt("winner", team);
                Invoke("ChangeToGameOver", 3f);
                canEndGame = false;
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

    public override void iDied(RpcArgs args)
    {
        int thisTeam = args.GetNext<int>();
        if (networkObject.IsServer && !networkObject.IsOwner)
        {
            NetworkingPlayer oldOwner = networkObject.Owner;

            if (thisTeam == 1)
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
            team = 0;

            networkObject.SendRpc("syncRobotState", Receivers.AllBuffered, 0, 1);
        }
    }

    #endregion
}
