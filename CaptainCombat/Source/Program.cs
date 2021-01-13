using CaptainCombat.states;
using System;
using System.Collections.Generic;
using System.Linq;


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
