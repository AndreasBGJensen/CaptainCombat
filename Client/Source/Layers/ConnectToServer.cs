//using CaptainCombat.Client.Scenes;
//using Microsoft.Xna.Framework;
//using System.Threading.Tasks;
//using CaptainCombat.Common;

//namespace CaptainCombat.Client.Layers
//{

//    class ConnectToServer : Layer
//    {
//        private Camera Camera;

//        private Game Game;
//        private State ParentState;

//        private Loader loader;

//        public ConnectToServer(Game game, State state)
//        {
//            ParentState = state; 
//            Game = game;

//            loader = new Loader("Connecting to server");

//            Task.Factory.StartNew(async () => {
//                await Task.Delay(3000); // TODO: REMOVE THIS
//                ParentState._context.TransitionTo(new JoinState(Game));
//            });
//        }

//        public override void update(GameTime gameTime)
//        {
//            loader.update();
//        }

//        public override void draw(GameTime gameTime)
//        {
//            loader.draw();
//        }

//    }
//}
