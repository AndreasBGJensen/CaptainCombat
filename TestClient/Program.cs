using dotSpace.Objects.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestClient
{
    class Program
    {
        static void Main(string[] args)
        {

			string randomJsonArray3 = @"[{
			'comp': 'Move',
			'client_id': 0,
			'component_id': 3,
			'entity_id': 24,
			'data': {
				'x': 1200,
				'y': 1200
			}
            },
{'comp': 'Draw',
			'client_id': 0,
			'component_id': 3,
			'entity_id': 24,
			'data': {
				'x': 1000,
				'y': 100000
			}}]";


		string uri = "tcp://49.12.75.251:9182/space?CONN";
            RemoteSpace space = new RemoteSpace(uri);

            space.Put("components", randomJsonArray3);

			while(true)
			{ 
			//space.Get(typeof(string), typeof(int), typeof(int), typeof(int), typeof(string));
			Console.WriteLine(space.GetP(typeof(string), typeof(int), typeof(int), typeof(int), typeof(string)));
			
			Console.WriteLine(space.GetP("components", typeof(string)));
			
			}



		}
    }
}
