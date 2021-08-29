using dotSpace.Interfaces.Space;
using dotSpace.Objects.Network;

namespace CaptainCombat.Server
{
    /// <summary>
    /// Contains global information about the server
    /// and its Tuple space
    /// </summary>
    static class Connection
    {
        public static SpaceRepository repository = null;
        public static ISpace Space = null;
    }
}
