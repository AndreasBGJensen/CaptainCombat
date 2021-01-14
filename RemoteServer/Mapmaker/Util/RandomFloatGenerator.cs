using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MersenneTwister;

namespace RemoteServer.Mapmaker.Util
{
    public static class RandomFloatGenerator
    {
        static Random random = new Random();
        

        public static double GiveRandomeDouble(double min, double max)
        {
            
            

            double random_value = Randoms.NextDouble() * (max - min) + min;

            return random_value;
        }
    }
}
