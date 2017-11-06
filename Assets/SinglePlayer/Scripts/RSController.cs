using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RSController : MonoBehaviour {
    public RSManager robotManager;
    public FlyCamera flyCamera;
	public bool isAI;
    public bool isRed;
    public bool isPlayerOne;
    public bool isLaborer;

    private void Start()
    {
        flyCamera = GetComponent<FlyCamera>();
        robotManager = GetComponentInChildren<RSManager>();
        robotManager.isRed = isRed;
        if (isLaborer){
            robotManager.Invoke("Die", 0.2f);
		}
            
    }

    public void Die(){
        //place into construction site queue to respawn as laborer
        robotManager.transform.parent = null;
        if (!isAI){
            flyCamera.Activate(robotManager.robotFollow.BackCamera.transform);
            Invoke("StartRespawn", 3);
        } else {
            Destroy(gameObject);
            AIRespawner.instance.AIDied(robotManager.isRed);
        }
    }

    void StartRespawn(){
		StartCoroutine(RespawnE());
	}

    IEnumerator RespawnE(){
        bool spawned = false;
        if (isRed){
            if (TimeMachine.redTimeMachine.AvailableLaborers.Count > 0){
                robotManager = TimeMachine.redTimeMachine.AvailableLaborers.Dequeue();
                Respawn();
                spawned = true;
            }
        } else {
			if (TimeMachine.blueTimeMachine.AvailableLaborers.Count > 0)
			{
				robotManager = TimeMachine.blueTimeMachine.AvailableLaborers.Dequeue();
				Respawn();
				spawned = true;
			}
        }

        if (!spawned){
            yield return new WaitForSeconds(3);
            StartCoroutine(RespawnE());
        }
    }
    public void SetPlayer(bool isPlayerOne){
        this.isPlayerOne = isPlayerOne;
        Invoke("SetPlayerL", 0.1f);
    }

    void SetPlayerL(){
        robotManager.SetPlayerOne(isPlayerOne);
    }

    public void Respawn()
    {
        //delete current controller
        Transform RespawnPosition;
        if (robotManager.isRed){
            RespawnPosition = TimeMachine.redTimeMachine.spawnPosition;
            Instantiate(ParticleSystemManager.instance.RespawnRed, RespawnPosition.position, Quaternion.identity);
        } else {
            RespawnPosition = TimeMachine.blueTimeMachine.spawnPosition;
			Instantiate(ParticleSystemManager.instance.RespawnBlue, RespawnPosition.position, Quaternion.identity);
		}
        GameObject newRobot = Instantiate(InitialSpawnManager.instance.RobotPlayer, RespawnPosition.position, RespawnPosition.rotation);
        //set players color and team 
        newRobot.GetComponent<RSController>().SetTeam(robotManager.isRed);
        newRobot.GetComponentInChildren<RSController>().SetPlayer(isPlayerOne);
        if (InitialSpawnManager.instance.isSplitScreen){
			newRobot.GetComponent<RSController>().SetViewPort(isPlayerOne);
		}

        if (robotManager!=null){
			Destroy(robotManager.gameObject);
		}

        Destroy(gameObject);
    }

	public void SetTeam(bool isRed)
	{
		this.isRed = isRed;
        GetComponentInChildren<RSManager>().isRed = isRed;
        GetComponentInChildren<ColorRobot>().SetColor(isRed);
	}

    public void SetViewPort(bool playerOne){
        if (playerOne){
            Rect rect = new Rect(-0.5f, 0, 1, 1);
			GetComponentInChildren<RSManager>().GetComponentInChildren<RobotFollow>().BackCamera.rect = rect;
			Camera[] cams = GetComponentsInChildren<Camera>();
            foreach (Camera c in cams)
            {
                c.rect = rect;
            }
        } else {
            Rect rect = new Rect(0.5f, 0, 1, 1);
            GetComponentInChildren<RSManager>().GetComponentInChildren<RobotFollow>().BackCamera.rect = rect;
			Camera[] cams = GetComponentsInChildren<Camera>();
			foreach (Camera c in cams)
			{
				c.rect = rect;
			}
        }
		GetComponentInChildren<RSManager>().GetComponentInChildren<RobotFollow>().BackCamera.enabled = true;
	}
}
