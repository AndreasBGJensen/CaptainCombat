using dotSpace.Interfaces.Space;
using System.Collections.Generic;


namespace CaptainCombat.Common.Singletons
{
    public sealed class DomainState
    {
        private static readonly DomainState instance = new DomainState();
        private Domain domain = null; 
        private string upload = null;
        private IEnumerable<ITuple> messages = null;
        private IEnumerable<ITuple> clients = null;
        private IEnumerable<ITuple> scores = null;

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

        public IEnumerable<ITuple> Messages { get => messages; set => messages = value; }
        public IEnumerable<ITuple> Clients { get => clients; set => clients = value; }
        public IEnumerable<ITuple> ClientScores { get => scores; set => scores = value; }

        public string Upload { get => upload; set => upload = value; }
        public IEnumerable<ITuple> Download { get; set; }

        public Domain Domain { get => domain; set => domain = value; }
    }
}
