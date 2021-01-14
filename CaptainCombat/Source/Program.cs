using CaptainCombat.states;
using System;

namespace CaptainCombat
{
    public static class Program
    {

        //[STAThread]
        //static void Main() {
        //    using (var game = new GameController())
        //        game.Run();
        //}

        [STAThread]
        static void Main(string[] args) {
            Context context = new Context(new Connect());
        }

    }
}
