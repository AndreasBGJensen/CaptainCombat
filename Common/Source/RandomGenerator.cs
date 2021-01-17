using MersenneTwister;

namespace CaptainCombat.Common {

    /// <summary>
    /// Random number generator using Mersenne-Twister
    /// </summary>
    public static class RandomGenerator
    {
        
        public static double Double(double min, double max)
        {
            return Randoms.NextDouble() * (max - min) + min;
        }
    }
}
