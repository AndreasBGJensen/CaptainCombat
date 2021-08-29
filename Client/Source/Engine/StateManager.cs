using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace CaptainCombat.Client.Scenes
{
    class StateManager 
    {
        // A reference to the current state of the Context.
        public State _state = null;

        private State stateToTransitionTo = null;

        public StateManager(State gameState)
        {
            TransitionTo(gameState); 
        }

        // The Context allows changing the State object at runtime.
        public void TransitionTo(State state)
        {
            stateToTransitionTo = state;
        }

        public void HandleTransition()
        {
            if (stateToTransitionTo == null) return;

            if (_state != null)
                _state.onExit();
            
            _state = stateToTransitionTo;
            stateToTransitionTo = null;
            
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

        public virtual void update(GameTime gameTime) { }

        public virtual void draw(GameTime gameTime) { }

        public virtual void onEnter() { }

        public virtual void onExit() { }

        public virtual void OnKeyDown(Keys key) { }
    }




}
