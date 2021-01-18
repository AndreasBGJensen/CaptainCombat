
using CaptainCombat.Common.Singletons;
using dotSpace.Objects.Space;
using System;
using System.Collections.Generic;
using System.Threading;

namespace CaptainCombat.Client.Source.Layers {

    public class LifeController {
        private SequentialSpace localLives = new SequentialSpace();
        private Thread thread;

        private Dictionary<uint, int> currentLives = new Dictionary<uint, int>();


        public LifeController() {
            thread = new Thread(UpdateLives);
            thread.Start();
            localLives.Put("lock");
        }

        public void Stop() {
            thread.Abort();
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

                var winnerFound = false;
                if (ownLives > 0) {
                    ownLives -= livesLost;
                    if (ownLives <= 0) {
                        ownLives = 0;
                        if (clientsAlive.Count == 1) {
                            winnerFound = true;

                            uint winner = 0;
                            foreach (var client in clientsAlive.Keys) {
                                if (client != Connection.Instance.User_id) {
                                    winner = client;
                                    break;
                                }
                            }

                            System.Console.WriteLine("Found winner: " + winner);
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

                if (!winnerFound) {
                    System.Console.WriteLine("Releasing lock");
                    // Don't unlock scores if a winner was found
                    Connection.Instance.Space.Put("life-lock");
                    System.Console.WriteLine("Released lock");
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
