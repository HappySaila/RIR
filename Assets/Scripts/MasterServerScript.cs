using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class MasterServerScript : masterServerBehavior
{

    public InputField roomName;
    public InputField roomSize;
    public Text matchingText;
    bool matching;
    int timer;
    room myRoom;
    // Use this for initialization
    void Start()
    {
        BMSLogger.Instance.Log("master server started");
        timer = 0;
        matching = false;
        GlobalVariables.instance.players = new Dictionary<int, GlobalVariables.PlayerDetails>();
        GlobalVariables.instance.existingRooms = new List<room>();
        GlobalVariables.instance.foundGames = new List<GlobalVariables.Pair<room,room>>();
        GlobalVariables.instance.toRemove = new List<room>();
        GlobalVariables.instance.searchingRooms = new Dictionary<int, List<room>>();
        networkObject.Networker.playerConnected += Networker_playerConnected;
        networkObject.Networker.playerDisconnected += Networker_playerDisconnected;
        GlobalVariables.instance.me = new GlobalVariables.PlayerDetails("hi",1200,networkObject.Networker.Me);
    }

    private void Networker_playerDisconnected(NetworkingPlayer player, NetWorker sender)
    {
        GlobalVariables.instance.players.Remove((int)player.NetworkId);
        GlobalVariables.instance.existingRooms.FindAll(room => room.Players.FindAll(pla => pla.Player.NetworkId == player.NetworkId).Count > 0).RemoveAll(i => true);
    }

    private void Networker_playerConnected(NetworkingPlayer player, NetWorker sender)
    {
        if (networkObject.IsServer)
        {
            BMSLogger.Instance.Log("player connected with ID " + player.NetworkId);
        }
    }



    public void createRoomButtonPressed()
    {
        BMSLogger.Instance.Log("connected");
        //@Grant add logic in here where you want to get the size and name of the room, I am leaving a default for now
        string roomN = roomName.text;
        int size = int.Parse(roomSize.text);
        //The global variables.me needs to be instantiated when the player logs on.
        networkObject.SendRpc(RPC_CREATE_ROOM, Receivers.AllBuffered, roomN, size, GlobalVariables.instance.me.Mmr, GlobalVariables.instance.me.Name);
    }

    public void joinRoomButtonPressed()
    {
        //@Grant add logic in here where you want to get the name of the room, I am leaving a default for now
        string roomN = roomName.text;
        networkObject.SendRpc(RPC_JOIN_ROOM, Receivers.AllBuffered, roomN, GlobalVariables.instance.me.Mmr, GlobalVariables.instance.me.Name);
    }
    // Update is called once per frame
    void Update()
    {
        if (networkObject.IsServer)
        {
            serverUpdateFunction();
        }
        else
        {
            clientUpdateFunction();
        }
    }
    #region serverCode

    public void serverUpdateFunction()
    {
        foreach(room rooms in GlobalVariables.instance.toRemove)
        {
            GlobalVariables.instance.searchingRooms[rooms.RoomSize].Remove(rooms);
        }
        GlobalVariables.instance.toRemove = new List<room>(); //redeclare the list as we have removed all the relevant rooms
        List<room> roomsWithSufficientPlayers = GlobalVariables.instance.existingRooms.FindAll(m => m.RoomSize == m.Players.Count);
        foreach(room foundRooms in roomsWithSufficientPlayers)
        {
            if (!GlobalVariables.instance.searchingRooms.ContainsKey(foundRooms.RoomSize))
            {
                GlobalVariables.instance.searchingRooms.Add(foundRooms.RoomSize, new List<room>());

            }
            GlobalVariables.instance.searchingRooms[foundRooms.RoomSize].Add(new room(foundRooms));
            GlobalVariables.instance.existingRooms.Remove(foundRooms);
            foreach(GlobalVariables.PlayerDetails player in foundRooms.Players)
            {
                networkObject.SendRpc(player.Player, RPC_START_MATCHING, foundRooms.RoomName);
            }
        }
        foreach (List<room> rooms in GlobalVariables.instance.searchingRooms.Values)
        {
            if(rooms.Count > 1)
            {
                //there are more than 2 teams currently searching for the same room size
                matchingTeams(rooms);
            }
        }


        stopSearching();
        
        //remove the found games so it doesn't try and start twice
        //GlobalVariables.instance.foundGames = new List<GlobalVariables.Pair<room, room>>();
    }

    public void NewServer()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        
    }

    #region dynamicServerSpawningLogic

   /* public void Connect()
    {
        
    }

    public void Host()
    {
        NetWorker server;

        server = new UDPServer(64);

        ((UDPServer)server).Connect(GlobalVariables.instance.IpAdress, (ushort)++GlobalVariables.instance.PortNumber);

        server.playerTimeout += (player, sender) =>
        {
            Debug.Log("Player " + player.NetworkId + " timed out");
        };
        Connected(server);
    }

    public void Connected(NetWorker networker)
    {
       
    }

    public void spawnObject()
    {

        
    }*/

    #endregion
    public void stopSearching()
    {
        foreach (GlobalVariables.Pair<room, room> pairs in GlobalVariables.instance.foundGames)
        {
            BMSLogger.Instance.Log("stopping the searching between 2 clients");
            //stop searching for the people who have found games
            foreach (GlobalVariables.PlayerDetails players in pairs.First.Players)
            {
                networkObject.SendRpc(players.Player, RPC_STOP_SEARCHING, pairs.First.RoomName, pairs.Second.RoomName);
            }

            foreach (GlobalVariables.PlayerDetails players in pairs.Second.Players)
            {
                networkObject.SendRpc(players.Player, RPC_STOP_SEARCHING, pairs.First.RoomName, pairs.Second.RoomName);
            }
            NewServer();
        }
    }

    public float getAverageMMR(room rooms)
    {
        float total = 0;
        foreach (GlobalVariables.PlayerDetails players in rooms.Players)
        {
            total += players.Mmr;
        }
        total /= rooms.Players.Count;
        return total;
    }

    public void matchingTeams(List<room> toBeMatched)
    {
        foreach(room room1 in toBeMatched)
        {
            float averageMMR = getAverageMMR(room1);
            foreach(room room2 in toBeMatched)
            {
                if(room1 != room2)
                {
                    float averageMMR2 = getAverageMMR(room2);
                    if ((averageMMR - room1.mmrLowerBound <= averageMMR2 && averageMMR + room1.mmrUpperBound >= averageMMR2) 
                        || (averageMMR2 - room2.mmrLowerBound <= averageMMR && averageMMR2 + room2.mmrUpperBound >= averageMMR))
                    {
                        GlobalVariables.instance.foundGames.Add(new GlobalVariables.Pair<room,room>(room1,room2));
                        BMSLogger.Instance.Log("pairing 2 games together");
                        GlobalVariables.instance.toRemove.Add(room1);
                        GlobalVariables.instance.toRemove.Add(room2);
                        //yay they should be matched
                    }
                }
            }
            room1.mmrUpperBound += 1;
            room1.mmrLowerBound -= 1;
            
            //make the bound slightly bigger

        }
    }
    public void serverJoinRoom(string roomName, int playerID)
    {
        BMSLogger.Instance.Log("player joined room " + roomName);
        GlobalVariables.instance.existingRooms.Find(i => i.RoomName == roomName).Players.Add(GlobalVariables.instance.players[playerID]);
    }

    public void serverCreateRoom(string roomName, int roomSize, int playerID)
    {
        BMSLogger.Instance.Log("player created room " + roomName);
        GlobalVariables.instance.existingRooms.Add(new room(GlobalVariables.instance.players[playerID], roomSize, roomName));
    }

    #endregion
    #region clientCode

        public void clientUpdateFunction()
        {
         if(matching == true)
            {
            matchingText.text = timer + "";
            }
            timer += 1;
        }

        public void clientCreateRoom(string roomName, int roomSize, int playerID)
        {
            GlobalVariables.instance.existingRooms.Add(new room(GlobalVariables.instance.players[playerID], roomSize, roomName));
            myRoom = new room(GlobalVariables.instance.players[playerID], roomSize,roomName);
        }

        public void clientJoinRoom(string roomName, int playerID)
        {
            myRoom = GlobalVariables.instance.existingRooms.Find(roomname => roomname.RoomName == roomName);
            myRoom.Players.Add(GlobalVariables.instance.players[playerID]);
        }
        #endregion
    #region RPC

    public override void connectToNewServer(RpcArgs args)
    {
        throw new System.NotImplementedException();
    }

    public override void createRoom(RpcArgs args)
    {
        string roomName = args.GetNext<string>();
        int roomSize= args.GetNext<int>();
        int mmr = args.GetNext<int>();
        string playerName = args.GetNext<string>();
        int playerID = (int)args.Info.SendingPlayer.NetworkId;
        if (!GlobalVariables.instance.players.ContainsKey(playerID))
        {
            GlobalVariables.instance.players[playerID] = new GlobalVariables.PlayerDetails(playerName, mmr,args.Info.SendingPlayer);
        }
        if (networkObject.IsServer)
        {
            BMSLogger.Instance.Log("room joined");
            serverCreateRoom(roomName, roomSize, playerID);
        }
        else
        {
            clientCreateRoom(roomName, roomSize, playerID);
        }
    }

    public override void joinRoom(RpcArgs args)
    {
        string roomName = args.GetNext<string>();
        int playerID = (int)args.Info.SendingPlayer.NetworkId;
        int mmr = args.GetNext<int>();
        string playerName = args.GetNext<string>();
        if (!GlobalVariables.instance.players.ContainsKey(playerID))
        {
            GlobalVariables.instance.players[playerID] = new GlobalVariables.PlayerDetails(playerName, mmr, args.Info.SendingPlayer);
        }
        if (networkObject.IsServer)
        {
            serverJoinRoom(roomName, playerID);
        }
        else
        {
            BMSLogger.Instance.Log("room joined");
            clientJoinRoom(roomName, playerID);
        }
    }

    public override void playerJoinRoom(RpcArgs args)
    {
        throw new System.NotImplementedException();
    }

    public override void readyUp(RpcArgs args)
    {
        throw new System.NotImplementedException();
    }

    public override void StartGame(RpcArgs args)
    {
        throw new System.NotImplementedException();
    }

    public override void startMatching(RpcArgs args)
    {
        BMSLogger.Instance.Log("you should start matching");
        matching = true;
        //@Grant, put your logic in here. this will display something on the clients side saying that they are matching
    }

    public override void stopSearching(RpcArgs args)
    {

        matching = false;
        matchingText.text = "match found!";
        Invoke("disableGameObject", 1f);
    }

    void disableGameObject()
    {
        this.gameObject.SetActive(false);
    }
#endregion
}
