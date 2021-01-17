using System;

namespace CaptainCombat.Client
{
    public static class Program
    {
        [STAThread]
        static void Main(string[] args) {

            Console.ReadLine(); 

            using (var game = new GameController())
                game.Run();
        }
    }
}
