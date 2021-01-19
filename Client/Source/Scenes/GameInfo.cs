
using CaptainCombat.Common.Singletons;
using System;
using System.Collections.Generic;

namespace CaptainCombat.Client.Scenes {


    public class GameInfo {

        public static GameInfo Current { get; set; }

        private readonly Dictionary<uint, Client> clients = new Dictionary<uint, Client>();


        public List<Client> Clients { get {
                var list = new List<Client>();
                foreach (var client in clients.Values)
                    list.Add(client);
                return list;
            }
        }


        public uint NumClients { get => (uint)clients.Count; }


        public Client AddClient(Client client) {
            if (clients.ContainsKey(client.Id))
                throw new ArgumentException($"Client with ID '{client.Id}' already exists");
            clients[client.Id] = client;
            return client;
        }
    }


    


    public class Client {
        public static Client Server { get; } = new Client();

        public uint Id { get; private set; }
        public string Name { get; private set; }

        public bool IsLocal { get => (uint)Connection.Instance.User_id == Id; }
    
        public Client(uint id, string name) {
            if (id == 0)
                throw new ArgumentException($"Client ID 0 is reserved for unknown client");
            if (id == 1)
                throw new ArgumentException($"Client ID 1 is reserved for server");
            
            Id = id;
            Name = name;
        }


        // Constructs the server client (without exceptions)
        private Client() {
            Id = 1;
            Name = "Server";
        }
    }





}
