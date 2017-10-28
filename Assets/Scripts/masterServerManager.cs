using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking;
using UnityEngine.UI;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine.SceneManagement;

public class masterServerManager : masterServerBehavior {

    public InputField input;
    public InputField roomSize;
    public Text one;
    public Text two;
    public Text three;
    public Text four;
    public Text matchMaking;
    bool matching;
    int currentUsedPort = 15999;
    public string ip = "127.0.0.1";
    int currentRoomSize = 0;
    public GameObject networkManager = null;
    private NetworkManager mgr = null;
    public GameObject panel;
    List<float> differences = new List<float>();
    double timer = 0;

    private room first;
    private room second;

    void Start()
    {
        GlobalVariables.instance.existingRooms = new List<room>();
        GlobalVariables.instance.foundGames = new List<room>();
        GlobalVariables.instance.searchingRooms = new Dictionary<int, List<room>>();
        GlobalVariables.instance.players = new Dictionary<int, GlobalVariables.PlayerDetails>();
    }


    // Update is called once per frame
    void Update()
    {
        if (matching == true)
        {
            timer += 1;
            matchMaking.text = "matching for " + timer;
        }
        if (GlobalVariables.instance.existingRooms == null)
        {
            return;
        }
        List<room> rooms = GlobalVariables.instance.existingRooms.FindAll(m => m.RoomSize == m.Players.Count);
        foreach (room foundRooms in rooms)
        {
            if (!GlobalVariables.instance.searchingRooms.ContainsKey(foundRooms.RoomSize))
            {
                GlobalVariables.instance.searchingRooms.Add(foundRooms.RoomSize,new List<room>());
                
            }
            GlobalVariables.instance.searchingRooms[foundRooms.RoomSize].Add(new room(foundRooms));
            Debug.Log(GlobalVariables.instance.searchingRooms[foundRooms.RoomSize].Count);
            GlobalVariables.instance.existingRooms.Remove(foundRooms);
            differences.Add(0);
            foreach (NetworkingPlayer player in foundRooms.Players)
            {
                networkObject.SendRpc(player, RPC_START_MATCHING, foundRooms.RoomName);
            }

            Debug.Log("room " + foundRooms.RoomName + " has enough players");
        }
        foreach (List<room> theRooms in GlobalVariables.instance.searchingRooms.Values)
        {
            if (theRooms.Count > 1)
            {
                bool found = searchForGame();
                if (found)
                {
                    differences.Remove(GlobalVariables.instance.searchingRooms[first.RoomSize].IndexOf(first));
                    differences.Remove(GlobalVariables.instance.searchingRooms[first.RoomSize].IndexOf(second));
                    GlobalVariables.instance.searchingRooms[first.RoomSize].Remove(first);
                    GlobalVariables.instance.searchingRooms[second.RoomSize].Remove(second);
                }
            }
        }
    }

    #region spawningNewInstance
    public void Host()
    {
        NetWorker server;
        server = new UDPServer(64);

        server.playerTimeout += (player, sender) =>
        {
            Debug.Log("Player " + player.NetworkId + " timed out");
        };
        ((UDPServer)server).Connect(ip, ushort.Parse(currentUsedPort.ToString()));
        Debug.Log("hosted new server on port " + currentUsedPort++);
        
        Connected(server);
    }

    public void Connected(NetWorker networker)
    {
        if (!networker.IsBound)
        {
            Debug.LogError("NetWorker failed to bind");
            return;
        }
        if (mgr == null && networkManager == null)
        {
            Debug.LogWarning("A network manager was not provided, generating a new one instead");
            networkManager = new GameObject("Network Manager");
            mgr = networkManager.AddComponent<NetworkManager>();
        }
        else if (mgr == null)
            mgr = Instantiate(networkManager).GetComponent<NetworkManager>();
        // networkObject.SendRpc()
        if (networker is IServer)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    public void Connect(ushort port)
    {
        NetWorker client;
        client = new TCPClient();
        ((TCPClient)client).Connect(ip, port);
        Connected(client);
    }
#endregion
    
    #region matchmaking
    public float getAverageMMR(room rooms){
        float total = 0;
        foreach(int mmr in rooms.Mmrs)
            {
                total += mmr;
            }
        total /= rooms.Mmrs.Count;
        return total;
    }
    
    bool searchForGame()
    {
        foreach(List<room> rooms in GlobalVariables.instance.searchingRooms.Values)
        {
            foreach(room room in rooms) { 
                float room1mmr = differences[rooms.IndexOf(room)];
                float lowerBound = getAverageMMR(room) - room1mmr;
                float upperBound = getAverageMMR(room) + room1mmr;
                differences[rooms.IndexOf(room)] += 10f;
                foreach (room toFight in rooms)
                {
                        if (!toFight.Equals(rooms))
                        {
                            float room2mmr = differences[rooms.IndexOf(room)];
                            float lowerBound2 = getAverageMMR(toFight) - room2mmr;
                            float upperBound2 = getAverageMMR(toFight) + room2mmr;
                            Debug.Log(getAverageMMR(room) - lowerBound2);
                            if (getAverageMMR(room) > lowerBound2 && getAverageMMR(room) < upperBound2)
                            {
                                //party 2 matches with party 1
                                Debug.Log("match");
                                first = room;
                                second = toFight;
                                GlobalVariables.instance.foundGames.Add(room);
                                GlobalVariables.instance.foundGames.Add(toFight);
                                networkObject.SendRpc(RPC_START_GAME, Receivers.Server, toFight.RoomName, room.RoomName);
                                return true;
                            }
                            if (getAverageMMR(toFight) > lowerBound && getAverageMMR(toFight) < upperBound)
                            {
                                Debug.Log("match");
                                //party 1 matches with party 2
                                first = room;
                                second = toFight;
                                GlobalVariables.instance.foundGames.Add(room);
                                GlobalVariables.instance.foundGames.Add(toFight);
                                networkObject.SendRpc(RPC_START_GAME, Receivers.Server, toFight.RoomName, room.RoomName);
                                return true;
                            }
                        }
                }
            }
        }
        return false;
    }
#endregion
    
    #region buttonPresses
    public void createRoom()
    {
        networkObject.Networker.Me.Name = "steven";
        if (string.IsNullOrEmpty(input.text))
        {
            return;
        }
        panel.SetActive(false);
        networkObject.SendRpc(RPC_CREATE_ROOM, Receivers.Server, input.text, int.Parse(roomSize.text), Random.RandomRange(500, 2000), networkObject.Networker.Me.Name);
    }

    public void joinRoom()
    {
        networkObject.Networker.Me.Name = "Jacob";
        if (string.IsNullOrEmpty(input.text))
        {
            return;
        }
        panel.SetActive(false);
        networkObject.SendRpc(RPC_JOIN_ROOM, Receivers.Server, input.text, Random.Range(500, 2000), networkObject.Networker.Me.Name);
    }
    #endregion
    #region RPCS

    public override void startMatching(RpcArgs args)
    {
        matching = true;
    }

    public override void joinRoom(RpcArgs args)
    {
        string name = args.GetNext<string>();
        int mmr = args.GetNext<int>();
        NetworkingPlayer pla = networkObject.Networker.GetPlayerById(args.Info.SendingPlayer.NetworkId);
        string playerName = args.GetNext<string>();
        GlobalVariables.instance.players[(int)args.Info.SendingPlayer.NetworkId] = new GlobalVariables.PlayerDetails(playerName,mmr);
        Debug.Log("found player with " + GlobalVariables.instance.players[(int)args.Info.SendingPlayer.NetworkId].Name);
        foreach (room rooms in GlobalVariables.instance.existingRooms)
        {
            if (rooms.RoomName == name)
            {
                foreach (NetworkingPlayer player in rooms.Players)
                {
                    //sends to each player in the room that you have joined
                    networkObject.SendRpc(player, RPC_PLAYER_JOIN_ROOM, GlobalVariables.instance.players[(int)args.Info.SendingPlayer.NetworkId].Name);

                }
                rooms.Players.Add(pla);
                rooms.Mmrs.Add(mmr);
                int counter = 0;
                foreach (NetworkingPlayer player in rooms.Players)
                {
                    Debug.Log(GlobalVariables.instance.players[(int)player.NetworkId].Name);
                    addToRoom(GlobalVariables.instance.players[(int)player.NetworkId].Name, counter++);
                }
                Debug.Log("added player to room " + name);
            }
        }
    }

    public override void StartGame(RpcArgs args)
    {
        string team1 = args.GetNext<string>();
        string team2 = args.GetNext<string>();
        Debug.Log("game should be started between " + team1 + " and " + team2);
        room room1 = GlobalVariables.instance.foundGames.Find(i => i.RoomName.Equals(team1));
        room room2 = GlobalVariables.instance.foundGames.Find(i => i.RoomName.Equals(team2));
        currentUsedPort++;
        string myport = currentUsedPort.ToString();
        NetWorker.PingForFirewall(ushort.Parse(myport));
        //GlobalVariables.instance.foundGames.Remove(room1);
        //GlobalVariables.instance.foundGames.Remove(room2);
        foreach (NetworkingPlayer player in room1.Players)
        {
            networkObject.SendRpc(player, RPC_STOP_SEARCHING, team1, team2);
        }
        foreach (NetworkingPlayer player in room2.Players)
        {
            networkObject.SendRpc(player, RPC_STOP_SEARCHING, team1, team2);
        }
        Host();
        foreach (NetworkingPlayer player in room1.Players)
        {
            networkObject.SendRpc(player, RPC_CONNECT_TO_NEW_SERVER, (ushort)(currentUsedPort - 1));
        }
        foreach (NetworkingPlayer player in room2.Players)
        {
            networkObject.SendRpc(player, RPC_CONNECT_TO_NEW_SERVER, (ushort)(currentUsedPort - 1));
        }
        //now I need to create a newServer with this being the host
    }

    //this doesn't do anything
    public override void readyUp(RpcArgs args)
    {
        string roomName = args.GetNext<string>();
        NetworkingPlayer pla = networkObject.Networker.GetPlayerById(args.Info.SendingPlayer.NetworkId);
    }
    public void addToRoom(string playerName,int counter)
    { 
        if (counter == 0)
        {
            one.text = playerName + " 0";
        }
        if (counter == 1)
        {
            two.text = playerName + " 1";
        }
        if (counter == 2)
        {
            three.text = playerName + " 2";
        }
        if (counter == 3)
        {
            four.text = playerName + " 3";
        }
        currentRoomSize++;
    }
    public override void playerJoinRoom(RpcArgs args)
    {
        string playerName = args.GetNext<string>();
        addToRoom(playerName,currentRoomSize);
    }

    public override void stopSearching(RpcArgs args)
    {
        string team1 = args.GetNext<string>();
        string team2 = args.GetNext<string>();
        matching = false;
        matchMaking.text = "found game!";
    }

    public override void connectToNewServer(RpcArgs args)
    {
        ushort port = args.GetNext<ushort>();
        Debug.Log("connecting to " + port);
        Connect(port);
        Debug.Log("connected");
    }

    public override void createRoom(RpcArgs args)
    {
        NetworkingPlayer pla = networkObject.Networker.GetPlayerById(args.Info.SendingPlayer.NetworkId);
        Debug.Log("sending player " + pla.NetworkId);
        string name = args.GetNext<string>();
        int roomSize = args.GetNext<int>();
        Debug.Log("sending player " + pla.Name);
        Debug.Log("in global storage");
        int mmr = args.GetNext<int>();
        string playerName = args.GetNext<string>();
        GlobalVariables.instance.players[(int)args.Info.SendingPlayer.NetworkId] = new GlobalVariables.PlayerDetails(playerName, mmr);
        Debug.Log("found player with " + GlobalVariables.instance.players[(int)args.Info.SendingPlayer.NetworkId].Name);
        room rooms = new room(pla, roomSize, name);
        rooms.Mmrs.Add(mmr);
        Debug.Log(networkObject);
        GlobalVariables.instance.existingRooms.Add(rooms);
        networkObject.SendRpc(pla, RPC_PLAYER_JOIN_ROOM, GlobalVariables.instance.players[(int)args.Info.SendingPlayer.NetworkId].Name);
        Debug.Log("created room");
    }
#endregion
}
