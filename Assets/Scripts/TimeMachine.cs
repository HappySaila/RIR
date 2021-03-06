﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimeMachine : MonoBehaviour {
	public static TimeMachine redTimeMachine;
	public static TimeMachine blueTimeMachine;

    public bool isRed;
    public Transform timeMachine;
    public Transform initialPosition;
    public Transform targetPosition;
    public Transform spawnPosition;
	public float currentProgress;
	public float buildSpeed;
	public float buildSpeedMultiplier;
    float maxBuildSpeed;
    bool multiplayer;
    public static bool canEndGame = true;

    public Queue<RSManager> AvailableLaborers = new Queue<RSManager>();
    public Queue<RMManager> MAvalableLaboreres = new Queue<RMManager>();

	[HideInInspector] public GameObject[] pointsAroundTimeMech;
    ParticleSystem[] steamParticles;


	// Use this for initialization
	void Start () {
		if(!isRed){
			pointsAroundTimeMech = GameObject.FindGameObjectsWithTag ("BlueTMPoint");
		}else{
			pointsAroundTimeMech = GameObject.FindGameObjectsWithTag ("RedTMPoint");
		}
	
        if (isRed){
            redTimeMachine = this;
        } else {
            blueTimeMachine = this;
        }
        steamParticles = GetComponentsInChildren<ParticleSystem>();
        StartCoroutine(AdjustParticles());
	}

    IEnumerator AdjustParticles(){
        yield return new WaitForSeconds(1);
        foreach (ParticleSystem s in steamParticles)
        {
            var e = s.emission;
            e.rateOverTime = currentProgress / 100f * 150;
        }
        StartCoroutine(AdjustParticles());
    }
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown("escape") && !isRed){
            EndGame();
        }
	}

    void ChangeToGameOver(){
        //look at game over
        SceneManager.LoadScene("GameOver");
    }

    public void Build(){
        currentProgress += buildSpeedMultiplier * buildSpeed;
        currentProgress = Mathf.Clamp(currentProgress, 0, 100);
        if (currentProgress > 99 && canEndGame){
            EndGame();
            canEndGame = false;
        }

        timeMachine.position = new Vector3(timeMachine.position.x, 
            (currentProgress/100)*(targetPosition.position.y-initialPosition.position.y)+initialPosition.position.y, 
            timeMachine.position.z);
    }

    public void EndGame()
    {
        print("GameOver");
        if (!multiplayer)
        {
            Instantiate(InitialSpawnManager.instance.FadeBlack, transform.position, Quaternion.identity);
            audioMixerScript.INSTANCE.Mute();
		    SoundManager.INSTANCE.PlayGameOverSound ();
            Invoke("ChangeToGameOver", 3f);
            PlayerPrefs.SetInt("Winner", isRed ? 1 : 0);
        }
    }

    private void OnTriggerEnter(Collider col)
    {
            if (col.GetComponentInParent<RSManager>() != null)
            {
                if (!col.GetComponentInParent<RSManager>().robotLaborerControl.isFighter)
                {
                    col.GetComponentInParent<RSManager>().robotLaborerControl.StartBuilding(this);
                }
            }
        
        if (col.GetComponentInParent<RMManager>() != null)
            {
            if (col.GetComponentInParent<RMManager>().type == RMManager.types.MOVINGTOBASE)
                {
                BMSLogger.Instance.Log("building this");
                col.GetComponentInParent<RMManager>().labourerController.StartBuilding(this);
                }
           
        }

    }

    public void AddLaborerToAvailableLaborer(RSManager controller)
	{
        multiplayer = false;
		AvailableLaborers.Enqueue(controller);
	}

    public void MAddLaborerToAvailableLaborer(RMManager controller)
    {
        multiplayer = true;
        MAvalableLaboreres.Enqueue(controller);
    }
}
