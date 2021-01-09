using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;

namespace PingTester
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
            try { 
                this.pingAddress = pingAdress;
            pinger = new Ping();
            reply = pinger.Send(this.pingAddress);
                 pingable = reply.Status == IPStatus.Success;
            if (!pingable)
            {
                throw new PingException(this.pingAddress);
            }
            }catch(Exception e)
            {
                throw new PingException(String.Format("Pinging the given adress: {0} failed: ", this.pingAddress));
            }
        }

            public long StartPinging()
            {



                try
                {
                    
                    reply = pinger.Send(pingAddress);
                    roundTripTime = reply.RoundtripTime;
                    
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
