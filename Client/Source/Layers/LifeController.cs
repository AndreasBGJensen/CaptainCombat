
using CaptainCombat.Common.Singletons;
using dotSpace.Objects.Space;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace CaptainCombat.Client.Source.Layers {

    class LifeController {

        public delegate void OnGameFinishCallback(uint winnerId);
        public OnGameFinishCallback OnGameFinish { get; set; } = null;

        private SequentialSpace localLives = new SequentialSpace();
        private Thread lifeUpdateThread;
        private Thread gameFinishThread;

        private Dictionary<uint, int> currentLives = new Dictionary<uint, int>();


        public LifeController() {
            lifeUpdateThread = new Thread(UpdateLives);
            lifeUpdateThread.Priority = ThreadPriority.Highest; 
            lifeUpdateThread.Start();

            localLives.Put("lock");

            gameFinishThread = new Thread(GameFinish);
            gameFinishThread.Priority = ThreadPriority.Highest;
            gameFinishThread.Start();
        }

        public void Stop() {
            lifeUpdateThread.Abort();
            gameFinishThread.Abort();
        }


        public void GameFinish() {                                                // Winner id
            var tuple = Connection.Instance.Space.Get("winner", typeof(int));
            OnGameFinish?.Invoke((uint)(int)tuple[1]);
        }

        private void UpdateLives() {
            Console.Error.WriteLine("Thread is " + Thread.CurrentThread.Name);
            while (true) {
                Console.WriteLine("Get life-lock");
                Connection.Instance.Space.Get("life-lock");

                var livesTuples = Connection.Instance.Space.QueryAll("lives", typeof(int), typeof(int));
                System.Console.WriteLine("Get lock");
                localLives.Get("lock");


                // Update opponents' lives
                localLives.GetAll("lives", typeof(int), typeof(int));
                var ownLives = 0;
                var clientsAlive = new Dictionary<uint, int>();
                foreach (var livesTuple in livesTuples)
                    if ((uint)(int)livesTuple[1] != Connection.Instance.User_id) {
                        localLives.Put(livesTuple);
                        if ((int)livesTuple[2] > 1)
                            clientsAlive[(uint)(int)livesTuple[1]] = (int)livesTuple[1];
                    }
                    else {
                        ownLives = (int)livesTuple[2];
                    }

                // Update own lives
                var livesLost = 0;
                foreach (var t in localLives.GetAll("decrement"))
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

                localLives.Put("lives", Connection.Instance.User_id, ownLives);
                localLives.Put("lock");

                if (livesLost > 0) {
                    // Update remote (only if lives were updated)
                    Connection.Instance.Space.Get("lives", Connection.Instance.User_id, typeof(int));
                    Connection.Instance.Space.Put("lives", Connection.Instance.User_id, ownLives);
                }

                if ( winner == 0 ) {
                    System.Console.WriteLine("Releasing lock");
                    // Don't unlock scores if a winner was found
                    Connection.Instance.Space.Put("life-lock");
                    System.Console.WriteLine("Released lock");
                }
                else {
                    Connection.Instance.Space.Put("winner", (int)winner);
                }

            }

        }


        public void DecrementLife() {
            localLives.Put("decrement");
        }


        public int GetOwnLives() {
            var lives = GetLives();
            return lives[(uint)Connection.Instance.User_id];    
        }

        public Dictionary<uint, int> GetLives() {
            localLives.Get("lock");
            //System.Console.WriteLine("Got lock");
            var tuples = localLives.QueryAll("lives", typeof(int), typeof(int));
            localLives.Put("lock");

            currentLives.Clear();
            foreach (var tuple in tuples)
                currentLives[(uint)(int)tuple[1]] = (int)tuple[2];

            return currentLives;
        }


    }
}
