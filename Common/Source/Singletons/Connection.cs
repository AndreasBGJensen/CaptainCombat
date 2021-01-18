using dotSpace.Interfaces.Space;

namespace CaptainCombat.Common.Singletons {

    public sealed class Connection
    {

        private string user = null;
     
        // TODO: Change this to uint
        private int user_id;
        private bool spaceOwner; 

        private Connection()
        {
        }
        public static Connection Instance { get; } = new Connection();

        public ISpace Space { get; set; }

        public ISpace lobbySpace { get; set; }

        public string User { get => user; set => user = value; }

        public int User_id { get => user_id; set => user_id = value; }

        public bool Space_owner { get => spaceOwner; set => spaceOwner = value; }

    }
}
