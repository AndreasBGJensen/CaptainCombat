using RemoteServer.Mapmaker.Util;
using Source.ECS;
using Source.EntityUtility;
using StaticGameLogic_Library.JsonBuilder;
using StaticGameLogic_Library.Singletons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteServer.Mapmaker.EntityToAdd
{
    class Rocks : IEntity
    {
        private int numberOfIslands;

        public Rocks(int numberOfIslands)
        {
            this.numberOfIslands = numberOfIslands;
        }

        public void OnComputerInit(object source, EventArgs e)
        {

            Domain domain = DomainState.Instance.Domain;
            RockElement rockElementContainer = new RockElement();

            int tries;

           /* for(int i = 0; i < numberOfIslands; i++)
            {
                tries = 5;
                RockElement rock = new RockElement();
                do
                {
                    tries--;
                    double x = RandomFloatGenerator.GiveRandomeDouble(-10000, 10000.0);
                    double y = RandomFloatGenerator.GiveRandomeDouble(-10000, 10000.0);
                    double scale = RandomFloatGenerator.GiveRandomeDouble(0.1, 10);
                    double rotation = RandomFloatGenerator.GiveRandomeDouble(0, 360);
                    rock.SetAttributes(x, y, scale, rotation);

                    if (tries == 0) break;
                } while (rock.CompareTo(rock) == 1);

                if (tries != 0) { 
                    rock.Add(rock);
               }
            }*/

           for (int i = 0; i < numberOfIslands; i++)
            {
                
                RockElement rock = new RockElement();
                double x = RandomFloatGenerator.GiveRandomeDouble(-10000, 10000.0);
                double y = RandomFloatGenerator.GiveRandomeDouble(-10000, 10000.0);
                double scale = RandomFloatGenerator.GiveRandomeDouble(0.1, 10);
                double rotation = RandomFloatGenerator.GiveRandomeDouble(0, 360);
                do
                {
                    x += 10;
                    y += 10;
                    
                    rock.SetAttributes(x, y, scale, rotation);

                    
                } while (rock.CompareTo(rock) == 1);
        
                    rock.Add(rock);
            }



            foreach (RockElement rockElement in RockElement.rockElements)
            {
                EntityUtility.CreateRock(domain, rockElement.x, rockElement.y, rockElement.scale, rockElement.rotation);
            }
            Console.WriteLine(String.Format("Number of Rocks: {0}", RockElement.rockElements.Count));
            /*EntityUtility.CreateRock(domain, 50, 100, 0.7, 120);
            EntityUtility.CreateRock(domain, 50, -200, 1.0, 40);
            EntityUtility.CreateRock(domain, 50, 50, 1.2, 300);
            EntityUtility.CreateRock(domain, -300, 75, 1.4, 170);
            EntityUtility.CreateRock(domain, -100, -200, 1.2, 30);*/


            domain.Clean();
            DomainState.Instance.Upload = JsonBuilder.createJsonString();
            Connection.Instance.Space.Put("components", DomainState.Instance.Upload);
        }


        class RockElement : IComparable<RockElement>
        {
            public static List<RockElement> rockElements = new List<RockElement>();
            public double x { get; set; }
            public double y { get; set; }
            public double scale { get; set; }
            public double rotation { get; set; }

            private int margin = 30;

            public RockElement()
            {
            }

            public void SetAttributes(double x, double y, double scale, double rotation)
            {
                this.x = x;
                this.y = y;
                this.scale = scale;
                this.rotation = rotation;
            }

             public int CompareTo(RockElement other)
             {
                 foreach (RockElement rock in rockElements)
                 {

                     if(other.x< rock.x) { 
                         if ((other.x + (other.scale * other.margin)) > (rock.x - (rock.scale * rock.margin)))
                         {
                             if (((other.y + (other.scale * other.margin)) > (rock.y - (rock.scale * rock.margin))) ||
                             ((other.y - (other.scale * other.margin)) < (rock.y + (rock.scale * rock.margin))))
                             {
                                 return 1;
                             }


                         }
                     }

                     if (other.x> rock.x) { 
                         if ((other.x - (other.scale * other.margin)) < (rock.x + (rock.scale * rock.margin)))
                         {
                             if (((other.y + (other.scale * other.margin)) > (rock.y - (rock.scale * rock.margin))) ||
                             ((other.y - (other.scale * other.margin)) < (rock.y + (rock.scale * rock.margin))))
                             {
                                 return 1;
                             }
                         }
                     }
                 }
                 return 0;
             }

            /*public int CompareTo(RockElement other)
            {
                
                Parallel.ForEach(rockElements, (rock) =>
                {
                   
                    if (other.x < rock.x)
                    {
                        if ((other.x + (other.scale * other.margin)) > (rock.x - (rock.scale * rock.margin)))
                        {
                            if (((other.y + (other.scale * other.margin)) > (rock.y - (rock.scale * rock.margin))) ||
                            ((other.y - (other.scale * other.margin)) < (rock.y + (rock.scale * rock.margin))))
                            {
                                result = 1;
                            }


                        }
                    }
                    else
                    {
                        result = 0;
                    }

                    if (other.x > rock.x)
                    {
                        if ((other.x - (other.scale * other.margin)) < (rock.x + (rock.scale * rock.margin)))
                        {
                            if (((other.y + (other.scale * other.margin)) > (rock.y - (rock.scale * rock.margin))) ||
                            ((other.y - (other.scale * other.margin)) < (rock.y + (rock.scale * rock.margin))))
                            {
                                result = 1;
                            }
                        }
                    }
                    else
                    {
                        result = 0;
                    }
                });
                return result;
            }*/

           

            public void Add(RockElement rock)
            {
                rockElements.Add(rock);
            }

            
        }
    }
}
