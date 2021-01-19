using CaptainCombat.Common.JsonBuilder;
using CaptainCombat.Common.Singletons;
using CaptainCombat.Common;
using System;
using System.Collections.Generic;

namespace CaptainCombat.Server.Mapmaker.EntityToAdd
{
    class Rocks : IEntity
    {
        private uint numRocks;

        public Rocks(uint numRocks)
        {
            this.numRocks = numRocks;
        }

        public void OnComputerInit()
        {

            var mapSize = Settings.MAP_SIZE;
            var boundarySize = 800;
            var boundaryPosition = mapSize*0.5 + boundarySize * 0.5;

            Domain domain = DomainState.Instance.Domain;
            {
                EntityUtility.CreateBoundary(domain, -boundaryPosition, 0,  boundarySize, mapSize*2);
                EntityUtility.CreateBoundary(domain,  boundaryPosition, 0,  boundarySize, mapSize*2);
                EntityUtility.CreateBoundary(domain,  0, -boundaryPosition, mapSize*2, boundarySize);
                EntityUtility.CreateBoundary(domain,  0,  boundaryPosition, mapSize*2, boundarySize);
            }


            var rockMargin = 0.1;
            var rockSpawnBoundary = mapSize * (0.5-rockMargin);
            
            for (uint i = 0; i < numRocks; i++)
            {   
                RockElement rock = new RockElement();
                double x = RandomGenerator.Double(-rockSpawnBoundary, rockSpawnBoundary);
                double y = RandomGenerator.Double(-rockSpawnBoundary, rockSpawnBoundary);
                double scale = RandomGenerator.Double(0.5, 1.5);
                double rotation = RandomGenerator.Double(0, 360);
                rock.SetAttributes(x, y, scale, rotation);

                while (!rock.CheckDistance()) {
                    x += 10;
                    y += 10;
                    rock.SetAttributes(x, y, scale, rotation);
                }

                RockElement.all.Add(rock);
            }



            foreach (RockElement rockElement in RockElement.all)
            {
                EntityUtility.CreateRock(domain, rockElement.x, rockElement.y, rockElement.scale, rockElement.rotation);
            }
            Console.WriteLine($"Number of Rocks: {RockElement.all.Count}");

            domain.Clean();
            DomainState.Instance.Upload = JsonBuilder.createJsonString();

            // TODO: Fix this uploading from server side
            Connection.Instance.Space.Put("components", "1", DomainState.Instance.Upload);
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
