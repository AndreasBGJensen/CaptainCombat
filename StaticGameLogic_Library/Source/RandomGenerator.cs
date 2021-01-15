using MersenneTwister;

namespace StaticGameLogic_Library.Source
{

    public static class RandomGenerator
    {
        
        public static double Double(double min, double max)
        {
            return Randoms.NextDouble() * (max - min) + min;
        }
    }
}
