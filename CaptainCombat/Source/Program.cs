using CaptainCombat.Source;
using CaptainCombat.states;
using System;

namespace CaptainCombat
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
