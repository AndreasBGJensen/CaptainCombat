using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingTester
{
    class Program
    {
        static void Main(string[] args)
        {
            try { 
            Ping_Server pingRunner = new Ping_Server("www.google.com");


            var runtime = pingRunner.StartPinging();
                Console.WriteLine(runtime);
                Console.ReadLine();

            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }
        }
    }
}
