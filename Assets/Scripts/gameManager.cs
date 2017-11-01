using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking.Unity;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;

public class gameManager : MonoBehaviour {
    public Transform[] RedPositions;
    public Transform[] BluePositions;
    public BoxCollider LaborerSpawn;
    public Queue<NetworkingPlayer> redTeamDead = new Queue<NetworkingPlayer>();
    public Queue<NetworkingPlayer> blueTeamDead = new Queue<NetworkingPlayer>();
    public static gameManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Invoke("spawnPlayers", 0.2f);
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    // Use this for initialization
    IEnumerator setLabourer(RobotManagerBehavior rmb, float delay){

        yield return new WaitForSeconds(delay);
        Vector3 bounds = LaborerSpawn.size;
        float x = LaborerSpawn.transform.position.x + Random.Range(-bounds.x, bounds.x);
        float z = LaborerSpawn.transform.position.z + Random.Range(-bounds.z, bounds.z);
        Vector3 spawnPos = new Vector3(x, LaborerSpawn.transform.position.y, z);
        rmb.networkObject.SendRpc("makeIntoLabourer",Receivers.All, spawnPos);
    }
    /*
     * if (team == 1)
        {
            rmb.networkObject.SendRpc("setStarting", Receivers.AllBuffered, RedPositions[position].position, RedPositions[position].rotation, 1);
        }
        else
        {
            rmb.networkObject.SendRpc("setStarting", Receivers.AllBuffered, BluePositions[position].position, BluePositions[position].rotation, 2);
        }*/
    IEnumerator setPlayerPositions(RobotManagerBehavior rmb,int team,int position,NetworkingPlayer toGive, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (team == 1) {
            rmb.networkObject.SendRpc("setStarting", Receivers.All, RedPositions[position].position, RedPositions[position].rotation, team);
        }
        else
        {
            rmb.networkObject.SendRpc("setStarting", Receivers.All, BluePositions[position].position, BluePositions[position].rotation, team);
        }
        if (toGive != null)
        {
            rmb.networkObject.AssignOwnership(toGive);
        }
    }
    /*
    
    */
    void spawnPlayers()
    {
        room room1 = GlobalVariables.instance.foundGames[0].First;
        room room2 = GlobalVariables.instance.foundGames[0].Second;
        Debug.Log(GlobalVariables.instance.foundGames.Count);
        int counter = 0;
        foreach (GlobalVariables.PlayerDetails player in room1.Players)
        {
            Debug.Log("player " + player.Name + " in game with id " + player.Player.NetworkId);
            RobotManagerBehavior behavior = NetworkManager.Instance.InstantiateRobotManager();
            StartCoroutine(setPlayerPositions(behavior, 1, counter, player.Player, 0.1f));
        }
        counter = 0;
        foreach (GlobalVariables.PlayerDetails player in room2.Players)
        {
            Debug.Log("player " + player.Name + " in game with id " + player.Player.NetworkId);
            RobotManagerBehavior behavior = NetworkManager.Instance.InstantiateRobotManager();
            StartCoroutine(setPlayerPositions(behavior, 2, counter, player.Player, 0.1f));
        }
    }
        void Start () {
        DontDestroyOnLoad(gameObject);
        RobotManagerBehavior behavior;
        if (NetworkManager.Instance.IsServer)
        {
            for (int i = 0; i < 10; i++)
            {
                behavior = NetworkManager.Instance.InstantiateRobotManager();
                StartCoroutine(setLabourer(behavior, 0.1f));
            }
        }
        if (NetworkManager.Instance.IsServer)
        {
            Invoke("spawnPlayers", 0.2f);
        }
    }

    private void OnPlayerDisconnected(NetworkPlayer player)
    {
        Debug.Log("player disconnected");
    }

    // Update is called once per frame
    void Update () {
        if (NetworkManager.Instance.IsServer)
        { 
            if (blueTeamDead.Count > 0)
            {
                if (TimeMachine.blueTimeMachine.MAvalableLaboreres.Count > 0)
                {
                    Debug.Log("respawning player as blue");
                    NetworkingPlayer toGive = blueTeamDead.Dequeue();
                    GameObject obj = TimeMachine.blueTimeMachine.MAvalableLaboreres.Dequeue();
                    obj.GetComponent<RMManager>().networkObject.Destroy();
                    RobotManagerBehavior behavior = NetworkManager.Instance.InstantiateRobotManager();
                    StartCoroutine(setPlayerPositions(behavior, 2,2, toGive, 0.1f));
                    
                }
            }
            if (redTeamDead.Count > 0)
            {
                Debug.Log("this is right");
                if (TimeMachine.redTimeMachine.MAvalableLaboreres.Count > 0)
                {
                    Debug.Log("respawning player as red");
                    NetworkingPlayer toGive = redTeamDead.Dequeue();
                    GameObject obj = TimeMachine.redTimeMachine.MAvalableLaboreres.Dequeue();
                    obj.GetComponent<RMManager>().networkObject.Destroy();
                    RobotManagerBehavior behavior = NetworkManager.Instance.InstantiateRobotManager();
                    StartCoroutine(setPlayerPositions(behavior, 1,2, toGive, 0.1f));
                }
            }
        }
	}
}
