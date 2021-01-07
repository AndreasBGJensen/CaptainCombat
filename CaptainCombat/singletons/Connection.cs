using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dotSpace.Objects.Network;
using dotSpace.Objects.Space;
using Tuple = dotSpace.Objects.Space.Tuple;


namespace CaptainCombat.singletons
{
    public sealed class Connection
    {
        private static readonly Connection instance = new Connection();
        private RemoteSpace space = null;
        private string user = null;


        private Connection()
        {
        }
        public static Connection Instance
        {
            get
            {
                return instance;
            }
        }

        public RemoteSpace Space { get => space; set => space = value; }
        public string User { get => user; set => user = value; }
    }
}
