using CaptainCombat.Common.Singletons;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CaptainCombat.Server.Source.LobbyContent
{
    public static class LobbyBookKeeping
    {
        public static ConcurrentDictionary<string, Lobby> lobbys { get; set; } = new ConcurrentDictionary<string, Lobby>();

        
        public static bool AddLobby(Lobby lobbyToadd)
        {
            if (lobbys.TryAdd(lobbyToadd.lobbyUrl, lobbyToadd))
                {
                Connection.Instance.Space.Put("existingLobby", lobbyToadd.spaceID, lobbyToadd.creator, lobbyToadd.lobbyUrl);
                Console.WriteLine(String.Format("Lobby created by: {0} is now open", lobbyToadd.creator));
                 return true;   
                }
            return false;
        }
    }
    
   
}
