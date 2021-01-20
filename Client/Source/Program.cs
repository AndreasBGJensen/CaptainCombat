using System;
using System.Threading;

namespace CaptainCombat.Client
{
    public static class Program
    {
        [STAThread]
        static void Main(string[] args) {

            Console.WriteLine("Press any key to start client");
            Console.ReadKey();
            Console.WriteLine("Starting client\n");
                
            using (var game = new GameController())
                game.Run();
        }
    }
}
