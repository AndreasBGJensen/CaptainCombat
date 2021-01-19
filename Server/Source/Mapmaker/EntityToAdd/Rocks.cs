using CaptainCombat.Common.JsonBuilder;
using CaptainCombat.Common.Singletons;
using CaptainCombat.Common;
using System;
using System.Collections.Generic;
using dotSpace.Objects.Space;

namespace CaptainCombat.Server.Mapmaker.EntityToAdd
{
    class Rocks : IEntity
    {
        private int numRocks;
        private SequentialSpace lobbySpace;

        public Rocks(int numRocks, SequentialSpace space)
        {
            this.numRocks = numRocks;
            this.lobbySpace = space;
        }

        public void OnComputerInit()
        {
            
            for (int i = 0; i < numRocks; i++)
            {   
                RockElement rock = new RockElement();
                double x        = RandomGenerator.Double(-750, 750.0);
                double y        = RandomGenerator.Double(-750, 750.0);
                double scale    = RandomGenerator.Double(0.25, 2.5);
                double rotation = RandomGenerator.Double(0, 360);

                do
                {
                    // TODO: Improve this distribution algorithm
                    x += 10;
                    y += 10;
                    rock.SetAttributes(x, y, scale, rotation);
                } while (!rock.CheckDistance());

                RockElement.all.Add(rock);
            }



            Domain domain = DomainState.Instance.Domain;

            foreach (RockElement rockElement in RockElement.all)
            {
                EntityUtility.CreateRock(domain, rockElement.x, rockElement.y, rockElement.scale, rockElement.rotation);
            }
            Console.WriteLine(String.Format("Number of Rocks: {0}", RockElement.all.Count));

            domain.Clean();
            DomainState.Instance.Upload = JsonBuilder.createJsonString();
            lobbySpace.Put("components", "1", DomainState.Instance.Upload);
        }


        class RockElement
        {
            public static List<RockElement> all = new List<RockElement>();
            public double x { get; set; }
            public double y { get; set; }
            public double scale { get; set; }
            public double rotation { get; set; }

            private int margin = 30;


            public void SetAttributes(double x, double y, double scale, double rotation)
            {
                this.x = x;
                this.y = y;
                this.scale = scale;
                this.rotation = rotation;
            }


            public bool CheckDistance ()
            {
                foreach (RockElement rock in all)
                {

                    if(x< rock.x) { 
                        if ((x + (scale * margin)) > (rock.x - (rock.scale * rock.margin)))
                        {
                            if (((y + (scale * margin)) > (rock.y - (rock.scale * rock.margin))) ||
                            ((y - (scale * margin)) < (rock.y + (rock.scale * rock.margin))))
                            {
                            return false;
                            }


                        }
                    }

                    if (x > rock.x) { 
                        if ((x - (scale * margin)) < (rock.x + (rock.scale * rock.margin)))
                        {
                            if (((y + (scale * margin)) > (rock.y - (rock.scale * rock.margin))) ||
                            ((y - (scale * margin)) < (rock.y + (rock.scale * rock.margin))))
                            {
                                return false;
                            }
                        }
                    }
                }
                 
                return true;
            }
           
        }
    }
}
