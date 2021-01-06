using dotSpace.Interfaces.Space;
using dotSpace.Objects.Network;
using dotSpace.Objects.Space;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteServer
{
    class Program
    {
        static void Main(string[] args)
        {
            string uri = "tcp://127.0.0.1:123/space?CONN";

            SpaceRepository repository = new SpaceRepository();
            repository.AddGate(uri);
            SequentialSpace space = new SequentialSpace();
            repository.AddSpace("space", space);

            ITuple message = space.Get(typeof(string), typeof(string));

            Console.WriteLine(message[0] + ":" + message[1]);
        }
    }
}
