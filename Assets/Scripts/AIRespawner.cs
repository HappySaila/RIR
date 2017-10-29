using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIRespawner : MonoBehaviour {
    //keeps a list of all AI needed to respawner and respawns them
    // Use this for initialization
    public static AIRespawner instance;

    //AI waiting to respawn for each team
	public int RedAICount = 0;
	public int BlueAICount = 0;

    private void Awake()
    {
        instance = this;
        Invoke("SetUp", 0.3f);
    }
    void SetUp(){
        RedAICount = 0;
        BlueAICount = 0;
    }

    public void AIDied(bool isRed){
        if (isRed){
            RedAICount++;
        } else {
            BlueAICount++;
        }
    }

    void Start () {
        StartCoroutine(Respawn());
	}

    IEnumerator Respawn(){
        RSManager robotManager;
        if (RedAICount > 0){
            if (TimeMachine.redTimeMachine.AvailableLaborers.Count > 0){
                robotManager = TimeMachine.redTimeMachine.AvailableLaborers.Dequeue();
                GameObject robot = Instantiate(InitialSpawnManager.instance.RobotAI, 
                                               robotManager.GetComponentInChildren<Rigidbody>().transform.position, Quaternion.identity);
				robot.GetComponent<RSController>().SetTeam(true);
				Destroy(robotManager.gameObject);
                RedAICount--;
            }
        }

        if (BlueAICount > 0){
            if (TimeMachine.blueTimeMachine.AvailableLaborers.Count > 0)
			{
                robotManager = TimeMachine.blueTimeMachine.AvailableLaborers.Dequeue();
				GameObject robot = Instantiate(InitialSpawnManager.instance.RobotAI,
											   robotManager.GetComponentInChildren<Rigidbody>().transform.position, Quaternion.identity);
                robot.GetComponent<RSController>().SetTeam(false);
				Destroy(robotManager.gameObject);
                BlueAICount--;
			}
        }

        if (TimeMachine.redTimeMachine != TimeMachine.blueTimeMachine){
			HUDManager.instance.SetScores();
		}
            
        yield return new WaitForSeconds(3);
        StartCoroutine(Respawn());
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
