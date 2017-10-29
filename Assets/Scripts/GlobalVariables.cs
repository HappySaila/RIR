using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking;
public class GlobalVariables : MonoBehaviour {

    public struct PlayerDetails
    {
        private string name;
        private int mmr;

        public PlayerDetails(string name, int mmr)
        {
            this.mmr = mmr;
            this.name = name;
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
    }

    public static GlobalVariables instance;

    
    public List<room> existingRooms;
    public Dictionary<int, List<room>> searchingRooms;
    public List<room> foundGames;
    public Dictionary<int, PlayerDetails> players;
    public PlayerDetails me = new PlayerDetails();
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
