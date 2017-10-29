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
    // Use this for initialization
    IEnumerator setLabourer(RobotManagerBehavior rmb, float delay){

        yield return new WaitForSeconds(delay);
        Vector3 bounds = LaborerSpawn.size;
        float x = LaborerSpawn.transform.position.x + Random.Range(-bounds.x, bounds.x);
        float z = LaborerSpawn.transform.position.z + Random.Range(-bounds.z, bounds.z);
        Vector3 spawnPos = new Vector3(x, LaborerSpawn.transform.position.y, z);
        rmb.networkObject.SendRpc("makeIntoLabourer",Receivers.All, spawnPos);
    }

    void spawnPlayers()
    {
        RobotManagerBehavior behavior = NetworkManager.Instance.InstantiateRobotManager();
    }
        void Start () {
        RobotManagerBehavior behavior;
        if (NetworkManager.Instance.IsServer)
        {
            for (int i = 0; i < 10; i++)
            {
                behavior = NetworkManager.Instance.InstantiateRobotManager();
                StartCoroutine(setLabourer(behavior, 0.1f));
                Debug.Log("created server Robot");
            }
        }
        Invoke("spawnPlayers", 0.3f);
        
        /*if (NetworkManager.Instance.IsServer)
        {
                room room1 = GlobalVariables.instance.foundGames[0];
                room room2 = GlobalVariables.instance.foundGames[1];
                Debug.Log("room " + room1.RoomName);
                int counter = 0;
                foreach (NetworkingPlayer player in room1.Players)
                {
                    Debug.Log("player " + player.Name + " in game with id " + player.NetworkId);
                    RobotManagerBehavior behavior = NetworkManager.Instance.InstantiateRobotManager();
                    behavior.networkObject.AssignOwnership(player);
                    // Debug.Log("player put " + RedPositions[counter].position.x + " " + RedPositions[counter].position.y + " " + RedPositions[counter].position.z);
                    behavior.networkObject.SendRpc("setStarting", Receivers.All, RedPositions[counter].position, RedPositions[counter].rotation, 1);
                    counter++;
            }
          /*  counter = 0;
            foreach (NetworkingPlayer player in room2.Players)
            {
                Debug.Log("player " + GlobalVariables.instance.players[(int)player.NetworkId] + " in game with id " + player.NetworkId);
                RobotManagerBehavior behavior = NetworkManager.Instance.InstantiateRobotManager();
                behavior.networkObject.AssignOwnership(player);
                behavior.networkObject.SendRpc("setStarting", Receivers.All, BluePositions[counter++].position, BluePositions[counter].rotation, 2);
                counter++;
            }
        }
        else
        {
            RobotManagerBehavior behavior = NetworkManager.Instance.InstantiateRobotManager();
            Debug.Log("I am not the server");
        }*/
        // behavior = NetworkManager.Instance.InstantiateRobot(position: new Vector3(-1000, 0, 0));
    }

    private void OnPlayerDisconnected(NetworkPlayer player)
    {
        Debug.Log("player disconnected");
    }

    // Update is called once per frame
    void Update () {
		
	}
}
