using BeardedManStudios.Forge.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class room {
        public room(GlobalVariables.PlayerDetails partyLeader, int roomSize, string roomName, int messageGroup)
        {
            this.partyLeader = partyLeader;
            this.roomSize = roomSize;
            this.roomName = roomName;
            matching = false;
            mmrs = new List<int>();
            players = new List<GlobalVariables.PlayerDetails>();
            players.Add(partyLeader);
            messageGroupNumber = messageGroup;
        }

        public room(room toReplace)
        {
            this.partyLeader = toReplace.partyLeader;
            this.roomName = toReplace.roomName;
            this.roomSize = toReplace.roomSize;
            matching = true;
            players = toReplace.players;
            mmrs = toReplace.Mmrs;
        }
        List<int> mmrs;
        GlobalVariables.PlayerDetails partyLeader;
        List<GlobalVariables.PlayerDetails> players;
        bool matching;
        int roomSize;
        string roomName;
        public int mmrLowerBound = 0;
        public int mmrUpperBound = 0;
        public int messageGroupNumber;

        public GlobalVariables.PlayerDetails PartyLeader
        {
            get
            {
                return partyLeader;
            }

            set
            {
                partyLeader = value;
            }
        }

        public List<GlobalVariables.PlayerDetails> Players
        {
            get
            {
                return players;
            }

            set
            {
                players = value;
            }
        }

        public int RoomSize
        {
            get
            {
                return roomSize;
            }

            set
            {
                roomSize = value;
            }
        }

        public string RoomName
        {
            get
            {
                return roomName;
            }

            set
            {
                roomName = value;
            }
        }

        public bool Matching
        {
            get
            {
                return matching;
            }

            set
            {
                matching = value;
            }
        }

        public List<int> Mmrs
        {
            get
            {
                return mmrs;
            }

            set
            {
                mmrs = value;
            }
        }
    
}
