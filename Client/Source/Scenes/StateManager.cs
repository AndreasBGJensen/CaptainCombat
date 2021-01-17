using Microsoft.Xna.Framework;

namespace CaptainCombat.Client.Scenes
{
    class StateManager 
    {
        // A reference to the current state of the Context.
        public State _state = null; 
        public StateManager(State gameState)
        {
            TransitionTo(gameState); 
        }

        // The Context allows changing the State object at runtime.
        public void TransitionTo(State state)
        {
            if(_state != null)
            {
                _state.onExit();
            }
            _state = state;
            _state.SetContext(this);
            _state.onEnter();
        }

    }


    abstract class State
    {
        public StateManager _context;

        public void SetContext(StateManager context)
        {
            _context = context;
        }

        public abstract void update(GameTime gameTime);

        public abstract void draw(GameTime gameTime);

        public abstract void onEnter();

        public abstract void onExit();
    }




}
