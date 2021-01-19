
using CaptainCombat.Client.NetworkEvent;
using CaptainCombat.Client.Scenes;
using CaptainCombat.Client.Source.Layers.Events;
using CaptainCombat.Common;
using CaptainCombat.Common.Singletons;
using System.Collections.Generic;
using System.Threading;

namespace CaptainCombat.Client.Layers {


    class LifeController {

        private Dictionary<uint, int> lives = new Dictionary<uint, int>();

        public Dictionary<uint, int> Lives { get {
                var copy = new Dictionary<uint, int>();
                foreach (var pair in lives) copy.Add(pair.Key, pair.Value);
                return copy;
            }
        }

        private bool settingWinner = false;
        
        public uint Winner { get; set; } = 0;
        public bool WinnerFound { get => Winner != 0; }

        public LifeController() {

            foreach (var client in GameInfo.Current.Clients)
                lives[client.Id] = Settings.LIVES;

            EventController.AddListener<LifeLostEvent>((e) => {
                lives[e.Sender]--;
                
                // Check if client is winner
                var opponentsAlive = 0;
                foreach (var pair in lives)
                    if (pair.Key != Connection.Instance.User_id)
                        if (pair.Value > 0)
                            opponentsAlive++;

                if (opponentsAlive == 0)
                    UploadWinner((uint)Connection.Instance.User_id);

                return true;
            });
            
            // TODO: Revert this
            //lifeUpdateThread = new Thread(UpdateLives);
            //lifeUpdateThread.Priority = ThreadPriority.Highest; 
            //lifeUpdateThread.Start();

            //localSpace.Put("lock");

            ListenForWinner();
        }



        private void UploadWinner(uint winnerId) {
            // To ensure that the client isn't attempting to set
            // 2 winners at the same time
            if (settingWinner) return;
            settingWinner = true;

            var t = new Thread((e) => {
                Connection.Instance.lobbySpace.Get("winner-lock");
                Connection.Instance.lobbySpace.Put("winner", (int)winnerId);
            });
            t.Priority = ThreadPriority.Highest;
            t.Start();
        }


        private void ListenForWinner() {
            var t = new Thread((e) => {
                var tuple = Connection.Instance.lobbySpace.Query("winner", typeof(int));
                Winner = (uint)(int)tuple[1];
            });
            t.Priority = ThreadPriority.Highest;
            t.Start();
        }


        /// <summary>
        /// Decrements the local client's lives
        /// </summary>
        public int DecrementLocalLives() {
            var clientId = (uint)Connection.Instance.User_id;
            if (lives[clientId] == 0) return 0;
            lives[clientId]--;

            // Send life lost event
            foreach (var client in GameInfo.Current.Clients)
                if (!client.IsLocal)
                    EventController.Send(new LifeLostEvent(client.Id));

            return lives[clientId];
        }


        public int GetClientLives(uint clientId) {
            return lives[clientId];
        }


        public int GetLocalClientLives() {
            return GetClientLives((uint)Connection.Instance.User_id);
        }


        //private void UpdateLives() {
        //    Console.Error.WriteLine("Thread is " + Thread.CurrentThread.Name);
        //    while (true) {
        //        Thread.Sleep(100);
        //        Console.WriteLine("Get life-lock");
        //        Connection.Instance.lobbySpace.Get("life-lock");

        //        var livesTuples = Connection.Instance.lobbySpace.QueryAll("lives", typeof(int), typeof(int));
        //        System.Console.WriteLine("Get lock");
        //        localSpace.Get("lock");


        //        // Update opponents' lives
        //        localSpace.GetAll("lives", typeof(int), typeof(int));
        //        var ownLives = 0;
        //        var clientsAlive = new Dictionary<uint, int>();
        //        foreach (var livesTuple in livesTuples)
        //            if ((uint)(int)livesTuple[1] != Connection.Instance.User_id) {
        //                localSpace.Put(livesTuple);
        //                if ((int)livesTuple[2] > 1)
        //                    clientsAlive[(uint)(int)livesTuple[1]] = (int)livesTuple[1];
        //            }
        //            else {
        //                ownLives = (int)livesTuple[2];
        //            }

        //        // Update own lives
        //        var livesLost = 0;
        //        foreach (var t in localSpace.GetAll("decrement"))
        //            livesLost++;

        //        uint winner = 0;
        //        if (ownLives > 0) {
        //            ownLives -= livesLost;
        //            if (ownLives <= 0) {
        //                ownLives = 0;
        //                if (clientsAlive.Count == 1) {
        //                    winner = clientsAlive.First().Key;
        //                }
        //            }
        //        }

        //        localSpace.Put("lives", Connection.Instance.User_id, ownLives);
        //        localSpace.Put("lock");

        //        if (livesLost > 0) {
        //            // Update remote (only if lives were updated)
        //            Connection.Instance.lobbySpace.Get("lives", Connection.Instance.User_id, typeof(int));
        //            Connection.Instance.lobbySpace.Put("lives", Connection.Instance.User_id, ownLives);
        //        }

        //        if ( winner == 0 ) {
        //            System.Console.WriteLine("Releasing lock");
        //            // Don't unlock scores if a winner was found
        //            Connection.Instance.lobbySpace.Put("life-lock");
        //            System.Console.WriteLine("Released lock");
        //        }
        //        else {
        //            Connection.Instance.lobbySpace.Put("winner", (int)winner);
        //        }

        //    }

        //}


        //public void DecrementLife() {
        //    localSpace.Put("decrement");
        //}


        //public int GetOwnLives() {
        //    // TODO: Revert this
        //    return 10;
        //    //var lives = GetLives();
        //    //return lives[(uint)Connection.Instance.User_id];    
        //}

        //public Dictionary<uint, int> GetLives() {



        //    localSpace.Get("lock");
        //    //System.Console.WriteLine("Got lock");
        //    var tuples = localSpace.QueryAll("lives", typeof(int), typeof(int));
        //    localSpace.Put("lock");

        //    currentLives.Clear();
        //    foreach (var tuple in tuples)
        //        currentLives[(uint)(int)tuple[1]] = (int)tuple[2];

        //    return currentLives;
        //}


    }
}
