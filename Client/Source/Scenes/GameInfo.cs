
using CaptainCombat.Common.Singletons;
using System;
using System.Collections.Generic;

namespace CaptainCombat.Client.Scenes {


    public class GameInfo {

        public static GameInfo Current { get; set; }

        private readonly Dictionary<uint, Player> clients = new Dictionary<uint, Player>();


        public List<Player> Clients { get {
                var list = new List<Player>();
                foreach (var client in clients.Values)
                    list.Add(client);
                return list;
            }
        }

        public Player GetPlayer(uint playerId) {
            return clients[playerId];
        }

        public uint NumClients { get => (uint)clients.Count; }


        public Player AddClient(Player client) {
            if (clients.ContainsKey(client.Id))
                throw new ArgumentException($"Player with ID '{client.Id}' already exists");
            clients[client.Id] = client;
            return client;
        }
    }



    public class Player {
        public static Player Server { get; } = new Player();

        public uint Id { get; private set; }
        public string Name { get; private set; }

        public bool IsLocal { get => (uint)Connection.Instance.User_id == Id; }
    
        public Player(uint id, string name) {
            if (id == 0)
                throw new ArgumentException($"Player ID 0 is reserved for unknown client");
            if (id == 1)
                throw new ArgumentException($"Player ID 1 is reserved for server");
            
            Id = id;
            Name = name;
        }


        // Constructs the server client (without exceptions)
        private Player() {
            Id = 1;
            Name = "Server";
        }
    }





}
