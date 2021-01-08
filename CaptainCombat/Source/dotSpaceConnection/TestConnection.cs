using dotSpace.Objects.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainCombat.dotSpaceConnection
{
    class TestConnection
    {

        public void Init()
        {
            try {
                string uri = "tcp://127.0.0.1:123/space?CONN";
                RemoteSpace space = new RemoteSpace(uri);

                space.Put("Hello", "World");

            } catch (Exception e) {
                Console.WriteLine(e.Message);
            }

        }
    }
}
