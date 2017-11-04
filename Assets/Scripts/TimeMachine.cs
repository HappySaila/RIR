using System.Collections;
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
	public float currentProgress;
	public float buildSpeed;
	public float buildSpeedMultiplier;
    float maxBuildSpeed;
    bool multiplayer;

    public Queue<RSManager> AvailableLaborers = new Queue<RSManager>();
    public Queue<GameObject> MAvalableLaboreres = new Queue<GameObject>();

	[HideInInspector] public GameObject[] pointsAroundTimeMech;


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
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(1) && !isRed){
            EndGame();
        }
	}

    void ChangeToGameOver(){
        //look at game over
		SceneManager.LoadScene("GameOver");
        audioMixerScript.INSTANCE.ChangeSnapShot(0);
    }

    public void Build(){
        currentProgress += buildSpeedMultiplier * buildSpeed;
        currentProgress = Mathf.Clamp(currentProgress, 0, 100);
        if (currentProgress > 99){
            EndGame();
        }

        timeMachine.position = new Vector3(timeMachine.position.x, 
            (currentProgress/100)*(targetPosition.position.y-initialPosition.position.y)+initialPosition.position.y, 
            timeMachine.position.z);
    }

    public void EndGame(){
		Instantiate(InitialSpawnManager.instance.FadeBlack, transform.position, Quaternion.identity);
		audioMixerScript.INSTANCE.Mute();
		Invoke("ChangeToGameOver", 3f);
        PlayerPrefs.SetInt("Winner", isRed ? 1 : 0);
    }

    private void OnTriggerEnter(Collider col)
    {
        if (!multiplayer)
        {
            if (col.GetComponentInParent<RSManager>() != null)
            {
                if (!col.GetComponentInParent<RSManager>().robotLaborerControl.isFighter)
                {
                    col.GetComponentInParent<RSManager>().robotLaborerControl.StartBuilding(this);
                }
            }
        }
        else
        {
            if (col.GetComponentInParent<RMManager>() != null)
            {
                if (!col.GetComponentInParent<RMManager>().labourerController.isFighter)
                {
                    col.GetComponentInParent<RMManager>().labourerController.StartBuilding(this);
                }
            }
        }

    }

    public void AddLaborerToAvailableLaborer(RSManager controller)
	{
		AvailableLaborers.Enqueue(controller);
	}

    public void MAddLaborerToAvailableLaborer(GameObject controller)
    {
        MAvalableLaboreres.Enqueue(controller);
        
    }


}
