using dotSpace.Interfaces.Space;
using CaptainCombat.Server.Mapmaker.EntityToAdd;
using System;
using System.Collections.Generic;
using CaptainCombat.Common;
using CaptainCombat.Common.Singletons;
using dotSpace.Objects.Space;

namespace CaptainCombat.Server.Mapmaker
{

    class Game
    {

        Domain domain = new Domain();
        public delegate void InitCmponent(object source, EventArgs e);
        public event InitCmponent ComputerInit;


        public void Init(ISpace lobbySpace)
        {

            DomainState.Instance.Domain = domain;
            //Join();
            
            IEntity entity = new Rocks(Settings.NUM_ROCKS, lobbySpace);
            entity.OnComputerInit();
            //game.ComputerInit += entity.OnComputerInit;

        }

        public void Join()
        {
            string username = "Server";

            ISpace space = Connection.Instance.Space;

            Console.Write("User in server: ");

            IEnumerable<ITuple> users = space.QueryAll("users", typeof(string));

            foreach (ITuple user in users)
            {
                Console.Write(user[1]);
            }            

            space.Put("user", username);

            ITuple results = space.Get("connected", typeof(int), typeof(string));
            Console.WriteLine(results[2]);
            Connection.Instance.User = username;
            Connection.Instance.User_id = (int)results[1];

        }

        protected virtual void OnComputerInit()
        {
            if(ComputerInit != null)
            {
                ComputerInit(this, EventArgs.Empty);
            }
        }
    }
}
