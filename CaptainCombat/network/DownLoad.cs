using CaptainCombat.singletons;
using CaptainCombat.Diagnostics;
using System.Collections.Generic;
using dotSpace.Interfaces.Space;

namespace CaptainCombat.network
{
    class DownLoad
    {

        public Dictionary<uint, ulong> updateIdMap = new Dictionary<uint, ulong>();

        StopWatch watch = new StopWatch("Downloading: ",50);
           
        public void RunProtocol() { 
            while (true) {
                watch.Start();
                
                var components = Connection.Instance.Space.QueryAll(typeof(string), typeof(int), typeof(int), typeof(int), typeof(string), typeof(string));

                List<ITuple> sortedComponents = new List<ITuple>();
                foreach(var component in components) {
                    var clientId = (uint)component[1];

                    if (clientId == Connection.Instance.User_id) continue;

                    var updateId = ulong.Parse((string)component[5]);

                    ulong currentUpdateId;
                    if( !updateIdMap.TryGetValue(clientId, out currentUpdateId) ){
                        currentUpdateId = 0;
                    }

                    if (updateId <= currentUpdateId) continue;

                    sortedComponents.Add(component);
                    updateIdMap[clientId] = updateId;
                }

                DomainState.Instance.Download = sortedComponents;
                


                /*
                    foreach (ITuple data in gameData)
                    {
                        Console.WriteLine(data);

                    }
                    */
                watch.Stop();
                watch.PrintTimer();
                //Console.WriteLine("Done");
            }
           
        }
    }
}
