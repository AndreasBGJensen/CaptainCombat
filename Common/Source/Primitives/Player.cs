
namespace CaptainCombat.Common
{

    public struct Player
    {
        public static Player Unknown { get; } = new Player(0, "Unknown");
        public static Player Server { get; } = new Player(1, "Server");

        public uint Id { get; set; }
        public string Name { get; set; }

        public bool IsUnknown { get => this == Player.Unknown; }

        public bool IsServer { get => this == Player.Server; }

        public Player(uint id, string name)
        {
            Id = id;
            Name = name;
        }



        // - - - - - -  - - -  - - -  - - -  - - -  - - - 
        // Equality overload

        public static bool operator ==(Player p1, Player p2)
        {
            if ((object)p1 == null)
                return (object)p2 == null;
            return p1.Equals(p2);
        }

        public static bool operator !=(Player p1, Player p2)
        {
            return !(p1 == p2);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            var p1 = (Player)obj;
            return Id == p1.Id && p1.Name == p1.Name;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode() ^ Name.GetHashCode();
        }
    }
}
