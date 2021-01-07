using dotSpace.Objects.Space;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteServer.singletons
{
    public sealed class Connection
    {
        private static readonly Connection instance = new Connection();
        private SequentialSpace space = null;


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

        public SequentialSpace Space { get => space; set => space = value; }
    }
}
