using System;

namespace CaptainCombat
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {

            Domain domain = new Domain();

            Entity entity = new Entity(domain);
            var c = entity.AddComponent<Transform>("Hello world");

            var entity2 = new Entity(domain);
            entity2.AddComponent<Transform>("Entity 2s");
            entity2.AddComponent<TestComponent2>().s = "Entity 2";

            // TypeList.add<TestComponent>().add<TestComponent>() 
            //domain.forMatchingEntities(new Type[]{ typeof(TestComponent)});

            domain.Clean();

            Console.WriteLine();

            domain.ForMatchingEntities<Transform>((e) => {
                Console.WriteLine(e.GetComponent<Transform>().s);
                e.Delete();
            });

            Console.WriteLine();

            Console.WriteLine("Final: ");
            //domain.clean();
            domain.ForMatchingEntities<Transform>((e) => {
                Console.WriteLine(e.GetComponent<Transform>().s);
            });

            Console.WriteLine("\nDone!");

            using (var game = new Game1())
                game.Run();
        }
    }
}
