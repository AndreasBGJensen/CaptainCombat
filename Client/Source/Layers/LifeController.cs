
using CaptainCombat.Common.Singletons;
using dotSpace.Objects.Space;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace CaptainCombat.Client.Layers {

    class LifeController {

        private SequentialSpace localSpace = new SequentialSpace();
        private Thread lifeUpdateThread;
        private Thread gameFinishThread;

        private Dictionary<uint, int> currentLives = new Dictionary<uint, int>();


        public LifeController() {
           /* lifeUpdateThread = new Thread(UpdateLives);
            lifeUpdateThread.Priority = ThreadPriority.Highest; 
            lifeUpdateThread.Start();*/

            localSpace.Put("lock");

            /*gameFinishThread = new Thread(GameFinish);
            gameFinishThread.Priority = ThreadPriority.Highest;
            gameFinishThread.Start();*/
        }

        public void Stop() {
            //lifeUpdateThread.Abort();
            //gameFinishThread.Abort();
        }


        public uint GetWinner() {
            var tuple = localSpace.GetP("winner", typeof(int));
            if (tuple == null) return 0;
            return (uint)(int)tuple[1];
        }

        public void GameFinish() {                                                // Winner id
            var tuple = Connection.Instance.lobbySpace.Get("winner", typeof(int));
            localSpace.Put(tuple);
        }

        private void UpdateLives() {
            Console.Error.WriteLine("Thread is " + Thread.CurrentThread.Name);
            while (true) {
                Thread.Sleep(100);
                Console.WriteLine("Get life-lock");
                Connection.Instance.lobbySpace.Get("life-lock");

                var livesTuples = Connection.Instance.lobbySpace.QueryAll("lives", typeof(int), typeof(int));
                System.Console.WriteLine("Get lock");
                localSpace.Get("lock");


                // Update opponents' lives
                localSpace.GetAll("lives", typeof(int), typeof(int));
                var ownLives = 0;
                var clientsAlive = new Dictionary<uint, int>();
                foreach (var livesTuple in livesTuples)
                    if ((uint)(int)livesTuple[1] != Connection.Instance.User_id) {
                        localSpace.Put(livesTuple);
                        if ((int)livesTuple[2] > 1)
                            clientsAlive[(uint)(int)livesTuple[1]] = (int)livesTuple[1];
                    }
                    else {
                        ownLives = (int)livesTuple[2];
                    }

                // Update own lives
                var livesLost = 0;
                foreach (var t in localSpace.GetAll("decrement"))
                    livesLost++;

                uint winner = 0;
                if (ownLives > 0) {
                    ownLives -= livesLost;
                    if (ownLives <= 0) {
                        ownLives = 0;
                        if (clientsAlive.Count == 1) {
                            winner = clientsAlive.First().Key;
                        }
                    }
                }

                localSpace.Put("lives", Connection.Instance.User_id, ownLives);
                localSpace.Put("lock");

                if (livesLost > 0) {
                    // Update remote (only if lives were updated)
                    Connection.Instance.lobbySpace.Get("lives", Connection.Instance.User_id, typeof(int));
                    Connection.Instance.lobbySpace.Put("lives", Connection.Instance.User_id, ownLives);
                }

                if ( winner == 0 ) {
                    System.Console.WriteLine("Releasing lock");
                    // Don't unlock scores if a winner was found
                    Connection.Instance.lobbySpace.Put("life-lock");
                    System.Console.WriteLine("Released lock");
                }
                else {
                    Connection.Instance.lobbySpace.Put("winner", (int)winner);
                }

            }

        }


        public void DecrementLife() {
            localSpace.Put("decrement");
        }


        public int GetOwnLives() {
            // TODO: Revert this
            return 10;
            //var lives = GetLives();
            //return lives[(uint)Connection.Instance.User_id];    
        }

        public Dictionary<uint, int> GetLives() {
            localSpace.Get("lock");
            //System.Console.WriteLine("Got lock");
            var tuples = localSpace.QueryAll("lives", typeof(int), typeof(int));
            localSpace.Put("lock");

            currentLives.Clear();
            foreach (var tuple in tuples)
                currentLives[(uint)(int)tuple[1]] = (int)tuple[2];

            return currentLives;
        }


    }
}
