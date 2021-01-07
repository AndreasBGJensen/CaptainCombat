using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainCombat.game
{
    class DataObject
    {
        string information = null;

        public DataObject(string information)
        {
            this.Information = information;
        }

        public string Information { get => information; set => information = value; }
    }
}
