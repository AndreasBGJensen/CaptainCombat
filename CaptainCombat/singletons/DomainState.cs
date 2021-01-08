using ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainCombat.singletons
{
    public sealed class DomainState
    {
        private static readonly DomainState instance = new DomainState();
        private Domain domain = null; 
        private string upload = null;
        private string download = null;

        private DomainState()
        {
        }
        public static DomainState Instance
        {
            get
            {
                return instance;
            }
        }

        public string Upload { get => upload; set => upload = value; }
        public string Download { get => download; set => download = value; }

        public Domain Domain { get => domain; set => domain = value; }
    }
}
