using System;
using System.Threading;
using dotSpace.Interfaces.Space;
using dotSpace.Objects.Network;
using dotSpace.Objects.Space;
using RemoteServer.singletons;
using RemoteServer.threads;
using Tuple = dotSpace.Objects.Space.Tuple;

namespace RemoteServer
{
    class Program
    {
        
        static void Main(string[] args)
        {
            Serialization serializationProtocol = new Serialization();
            string uri = "tcp://127.0.0.1:123/space?CONN";

            SpaceRepository repository = new SpaceRepository();
            repository.AddGate(uri);
            SequentialSpace space = new SequentialSpace();
            repository.AddSpace("space", space);



            Connection.Instance.Space = space;
            //Thread serializationThread = new Thread(new ThreadStart(serializationProtocol.RunProtocol));
            serializationProtocol.RunProtocol();
        }
       
        /*
        static void Main(string[] args)
        {
            string uri = "tcp://127.0.0.1:5000?CONN";

            SpaceRepository repository = new SpaceRepository();
            repository.AddGate(uri);
            SequentialSpace space = new SequentialSpace();
            repository.AddSpace("space", space);
            Console.WriteLine("Start Gameserver");
            Connection.Instance.Space = space;

            NewUsers newUserProtocol = new NewUsers();
            Thread newUserThread = new Thread(new ThreadStart(newUserProtocol.RunProtocol));
            newUserThread.Start();

            Serialization serializationProtocol = new Serialization();
            Thread serializationThread = new Thread(new ThreadStart(serializationProtocol.RunProtocol));
            serializationThread.Start();

        }
        */
        
    }
}
