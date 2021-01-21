using System;
using System.Threading;

namespace CaptainCombat.Client
{
    public static class Start
    {
        [STAThread]
        static void Main(string[] args) {

            Console.WriteLine("Press any key to start client");
            Console.ReadKey();
            Console.WriteLine("Starting client\n");
                
            using (var game = new ApplicationController())
                game.Run();
        }
    }
}
