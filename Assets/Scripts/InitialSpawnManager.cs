﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialSpawnManager : MonoBehaviour {
    public static InitialSpawnManager instance;
	public Transform[] RedPositions;
	public Transform[] BluePositions;
    public BoxCollider LaborerSpawn;
	public GameObject RobotAI;
	public GameObject RobotPlayer;
	public GameObject IdleLaborer;
    public GameObject FadeBlack;
    public bool isSplitScreen;
	[Range(0, 4)]
    public int blueBots;
	[Range(0,4)]
	public int redBots;
	public int LaborerCount;
	public int laborersToSPawnByBase;
	public int LaborerCountThroughGame;

	public TimeMachine redTimeMachine;
	public TimeMachine blueTimeMachine;

    private void Awake()
    {
        instance = this;
    }

    void Start () {
        audioMixerScript.INSTANCE.ChangeSnapShot(2);
        SpawnRobots();
        SpawnLaborers();
		SpawnBaseLaborers ();
		StartCoroutine (SpawnLaborersThroughtGame ());

		StartCoroutine (checkOneTeamIsOut ());
    }

    void SpawnRobots(){
        int i = 0;
        GameObject robot;
        //spawn players
        if (isSplitScreen){
            //spawn player 1
            robot = Instantiate(RobotPlayer, RedPositions[1].position, RedPositions[1].rotation);
			robot.GetComponent<RSController>().SetTeam(true);
            //only allowed 1 audio listener
            robot.GetComponentInChildren<RobotFollow>().DisableAudioListener();
            //set camera for split screen
            robot.GetComponent<RSController>().SetPlayer(false);
			robot.GetComponent<RSController>().SetViewPort(false);
            //robot.GetComponentInChildren<RSMovement>().GetComponentInChildren<Rigidbody>().transform.position = Vector3.zero;

			//spawn player 2
			robot = Instantiate(RobotPlayer, BluePositions[1].position, BluePositions[1].rotation);
			robot.GetComponent<RSController>().SetTeam(false);
			robot.GetComponent<RSController>().SetPlayer(true);
			robot.GetComponent<RSController>().SetViewPort(true);
			//robot.GetComponentInChildren<RSMovement>().GetComponentInChildren<Rigidbody>().transform.position = Vector3.zero;

		} else {
            //just spawn 1 player
			robot = Instantiate(RobotPlayer, BluePositions[3].position, BluePositions[3].rotation);
			robot.GetComponent<RSController>().SetTeam(false);
            robot.GetComponent<RSController>().SetPlayer(true);
        }
		


		//spawn bots
		foreach (Transform t in RedPositions)
		{
           // print(i);
            if (i != 1 && i < redBots){
				robot = Instantiate(RobotAI, t.position, t.rotation);
				robot.GetComponent<RSController>().SetTeam(true);
            }
            i++;
		}

        if (isSplitScreen){
			i = 0;
		}
            
		foreach (Transform t in BluePositions)
		{
            if (i != 1 && i%4 < blueBots)
			{
				robot = Instantiate(RobotAI, t.position, t.rotation);
				robot.GetComponent<RSController>().SetTeam(false);
			}
            i++;
		}
    }

    void SpawnLaborers(){
        for (int i = 0; i < LaborerCount; i++)
        {
			Vector3 bounds = LaborerSpawn.size;
            float x = LaborerSpawn.transform.position.x + Random.Range(-bounds.x, bounds.x);
			float z = LaborerSpawn.transform.position.z + Random.Range(-bounds.z, bounds.z);
			Vector3 spawnPos = new Vector3(x, LaborerSpawn.transform.position.y, z);
            Instantiate(IdleLaborer, spawnPos, Random.rotation);
        }

        AIRespawner.instance.BlueAICount = 0;
        AIRespawner.instance.RedAICount = 0;
        if (isSplitScreen){
			AIRespawner.instance.RedAICount = redBots -1;
		}
    }
	void SpawnBaseLaborers(){

		GameObject[] spawnPointsB = GameObject.FindGameObjectsWithTag("BaseLabourerSpawnPointB");
		GameObject[] spawnPointsR = GameObject.FindGameObjectsWithTag("BaseLabourerSpawnPointR");

		for (int i = 0; i < spawnPointsB.Length ; i++)
		{
			
			Instantiate(IdleLaborer, spawnPointsB[i].transform.position, Random.rotation);
		}
	
		for (int i = 0; i < spawnPointsR.Length ; i++)
		{

			Instantiate(IdleLaborer, spawnPointsR[i].transform.position, Random.rotation);
		}

		AIRespawner.instance.BlueAICount = 0;
		AIRespawner.instance.RedAICount = 0;
		if (isSplitScreen){
			AIRespawner.instance.RedAICount = redBots -1;
		}
	}


	public IEnumerator SpawnLaborersThroughtGame ()
	{
		
		yield return new WaitForSeconds (4);



		for (int i = 0; i < LaborerCountThroughGame; i++)
		{
			Vector3 bounds = LaborerSpawn.size;
			float x = LaborerSpawn.transform.position.x + Random.Range(-bounds.x, bounds.x);
			float z = LaborerSpawn.transform.position.z + Random.Range(-bounds.z, bounds.z);
			Vector3 spawnPos = new Vector3(x, LaborerSpawn.transform.position.y, z);
			Instantiate(IdleLaborer, spawnPos, Random.rotation);

			AIRespawner.instance.BlueAICount--;
		}
			StartCoroutine (SpawnLaborersThroughtGame ());


	}


	//checks if either team has no players left. If so ends game.
	public IEnumerator checkOneTeamIsOut ()
	{
		yield return new WaitForSeconds (5);

		RSManager[] robots = FindObjectsOfType<RSManager> ();
		bool redleft = false;
		bool blueleft = false;

		for (int i = 0; i < robots.Length; i++) {
			if(!robots[i].robotLaborerControl.isIdleLaborer){
				if (!robots [i].isRed  ) {
					blueleft = true;
				} 
				else {
					redleft = true;
				}
			}

		}

		if(!blueleft){
			redTimeMachine.EndGame ();
		}
		else if(!redleft){
			blueTimeMachine.EndGame ();
		}

		StartCoroutine (checkOneTeamIsOut ());
	}

}
