
using dotSpace.Interfaces.Space;
using System.Threading;

namespace CaptainCombat.Server
{

    // Checks that the game has finished
    class FinishListener
    {
        private ISpace space;
        private Thread thread;

        public delegate void GameFinishedCallback();
        private GameFinishedCallback callback;

        public FinishListener(ISpace space, GameFinishedCallback callback)
        {
            this.space = space;
            this.callback = callback;
        }

        public void Start()
        {
            thread = new Thread(CheckForFinish);
            thread.Start();
        }

        private void CheckForFinish()
        {
            // Id (2nd element) is unused here
            space.Query("winner", typeof(int));
            callback();
        }
    }
}
