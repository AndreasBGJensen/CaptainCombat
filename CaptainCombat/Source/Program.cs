using CaptainCombat.states;
using System;
using System.Collections.Generic;
using System.Linq;


namespace CaptainCombat
{
    public static class Program
    {
        /*
        [STAThread]
        static void Main()
        {
            using (var game = new GameController())
                game.Run();
        }
        */
        [STAThread]
        static void Main(string[] args)
        {
            Context context = new Context(new Connect());
        }

    }
}
