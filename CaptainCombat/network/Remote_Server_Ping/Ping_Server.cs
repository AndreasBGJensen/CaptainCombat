using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;

namespace CaptainCombat.network.Remote_Server_Ping
{
    class Ping_Server
    {
        private string pingAddress;
        PingReply reply;
        private long roundTripTime;
        Ping pinger = null;
        bool pingable;

        public Ping_Server(string pingAdress)
        {
            this.pingAddress = pingAdress;
            reply = pinger.Send(this.pingAddress);
        }

        public long Ping(string nameOrAddress)
        {
            
            

            try
            {
                pinger = new Ping();
                reply = pinger.Send(nameOrAddress);
                roundTripTime = reply.RoundtripTime;
                pingable = reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {
                // Discard PingExceptions and return false;
            }
            finally
            {
                if (pinger != null)
                {
                    pinger.Dispose();
                }
            }

            return roundTripTime;
        }
    }
}
