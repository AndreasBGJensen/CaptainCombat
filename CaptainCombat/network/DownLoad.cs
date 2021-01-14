﻿using CaptainCombat.singletons;
using CaptainCombat.Diagnostics;
using System.Collections.Generic;
using dotSpace.Interfaces.Space;

namespace CaptainCombat.network
{
    class DownLoad
    {


        StopWatch watch = new StopWatch("Downloading: ",50);
           
        public void RunProtocol() { 
            while (true) {
                watch.Start();
                
                DomainState.Instance.Download = Connection.Instance.Space.QueryAll(typeof(string), typeof(int), typeof(int), typeof(int), typeof(string), typeof(string));



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
