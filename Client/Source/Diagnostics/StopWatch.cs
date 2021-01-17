using System;
using System.Diagnostics;

namespace CaptainCombat.Diagnostics
{
    class StopWatch
    {
        string outputMessage; 

        Stopwatch sw = new Stopwatch();
        private Stopwatch internalWatch = new Stopwatch();
        double sum = 0;
        int avereageOver;
        int counter;

        public StopWatch(string message, int avereageOver)
        {
            outputMessage = message;
            this.avereageOver = avereageOver;
            counter = 0;
        }

       public void Start()
        {
            counter++;
            //Console.WriteLine(String.Format("Counter: {0}", counter));
            sw.Start();
        }

        public void Stop()
        {
            sw.Stop();
            sum += sw.Elapsed.TotalMilliseconds;
            sw.Reset();
            
        }

        public void PrintTimer()
        {
            //internalWatch.Start();
            if (counter== this.avereageOver)
            {
                // Console.WriteLine(String.Format(outputMessage + ", avereage:{0} ", ((sum / avereageOver)- timerWatch_sum)));
                Console.WriteLine(String.Format(outputMessage + ", avereage:{0} /  {1}", (sum / avereageOver), avereageOver));
                counter = 0;
                sum = 0;
            }
            //internalWatch.Stop();
            //timerWatch_sum += internalWatch.Elapsed.TotalMilliseconds;
            //internalWatch.Reset();

        }
        
    }
}
