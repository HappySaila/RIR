using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking;

public class GlobalVariables : MonoBehaviour {

    public class Pair<T, U>
    {
        public Pair()
        {
        }

        public Pair(T first, U second)
        {
            this.First = first;
            this.Second = second;
        }

        public T First { get; set; }
        public U Second { get; set; }
    };

    public struct PlayerDetails
    {
        private string name;
        private int mmr;
        private NetworkingPlayer player;

        public PlayerDetails(string name, int mmr,NetworkingPlayer player)
        {
            this.mmr = mmr;
            this.name = name;
            this.player = player;
        }

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        public int Mmr
        {
            get
            {
                return mmr;
            }

            set
            {
                mmr = value;
            }
        }

        public NetworkingPlayer Player
        {
            get
            {
                return player;
            }

            set
            {
                player = value;
            }
        }
    }

    public static GlobalVariables instance;

    public List<room> existingRooms;
    public Dictionary<int, List<room>> searchingRooms;
    public List<Pair<room,room>> foundGames;
    public Dictionary<int, PlayerDetails> players;
    public PlayerDetails me = new PlayerDetails();
    public List<room> toRemove;
    public int messageGroupIncrement = 0;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        } else
        {
            instance = this;
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}
