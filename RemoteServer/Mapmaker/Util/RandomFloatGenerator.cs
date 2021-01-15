using System;
using MersenneTwister;

namespace RemoteServer.Mapmaker.Util
{
    public static class RandomFloatGenerator
    {
        

        public static double GiveRandomeDouble(double min, double max)
        {
            
            

            double random_value = Randoms.NextDouble() * (max - min) + min;

            return random_value;
        }
    }
}
