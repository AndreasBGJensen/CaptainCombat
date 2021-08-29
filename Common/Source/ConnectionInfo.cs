
namespace CaptainCombat.Common
{

    public static class ConnectionInfo {

        // Set to 'true' to activate localhost server
        public static readonly bool LOCAL_SERVER = true;

        public static readonly uint PORT = 5000;

        public static readonly string REMOTE_SERVER_IP = "49.12.75.251";

        public static readonly string SPACE_NAME = "space";
      
        public static readonly string SERVER_ADDRESS = (LOCAL_SERVER ? "127.0.0.1" : REMOTE_SERVER_IP) + ":" + PORT;
    }
}
