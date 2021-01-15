using dotSpace.Objects.Network;

namespace CaptainCombat.singletons
{
    public sealed class Connection
    {

        private static readonly Connection instance = new Connection();
        private RemoteSpace space = null;
        private string user = null;
     
        // TODO: Change this to uint
        private int user_id; 

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

        public int User_id { get => user_id; set => user_id = value; }

    }
}
