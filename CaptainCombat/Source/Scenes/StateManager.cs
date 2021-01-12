using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainCombat.Source.Scenes
{
    class StateManager 
    {
        //public State gameState = new GameState();
        //public State menuState = new MenuState();

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
                this._state.onExit();
            }
            this._state = state;
            this._state.SetContext(this);
            this._state.onEnter();
        }

    }


    abstract class State
    {
        protected StateManager _context;

        public void SetContext(StateManager context)
        {
            this._context = context;
        }

        public abstract void update(GameTime gameTime);

        public abstract void draw(GameTime gameTime);

        public abstract void onEnter();

        public abstract void onExit();
    }




}
