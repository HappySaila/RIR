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
	float currentProgress;
	public float buildSpeed;
	public float buildSpeedMultiplier;
    float maxBuildSpeed;
    bool multiplayer;

    public Queue<RSManager> AvailableLaborers = new Queue<RSManager>();
    public Queue<GameObject> MAvalableLaboreres = new Queue<GameObject>();

	// Use this for initialization
	void Start () {
        if (isRed){
            redTimeMachine = this;
        } else {
            blueTimeMachine = this;
        }
        timeMachine.gameObject.GetComponent<MeshRenderer>().materials[0].color = isRed ? Color.red : Color.blue;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(1))
        {
			Debug.LogFormat("Available {0}", AvailableLaborers.Count);
        }
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
        //team has won. Show end Game Screen
        SceneManager.LoadScene("GameOverMenu");
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
