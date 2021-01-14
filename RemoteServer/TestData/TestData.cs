using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace RemoteServer.TestData
{
    //These classes are used for testing 
    public class TestData
    {

        public string randomJsonString = @"{
			'comp': 'Move',
			'client_id': 0,
			'component_id': 3,
			'entity_id': 24,
			'data': {
				'x': 12,
				'y': 12
			}
            }";
        public string randomJsonString2 = @"{
			'comp': 'Move',
			'client_id': 0,
			'component_id': 3,
			'entity_id': 24,
			'data': {
				'x': 12,
				'y': 12
			}
            }";

        public string randomJsonArray2 = @"[{
			'comp': 'Move',
			'client_id': 0,
			'component_id': 3,
			'entity_id': 24,
			'data': {
				'x': 12,
				'y': 12
			}
            },
{'comp': 'Draw',
			'client_id': 0,
			'component_id': 3,
			'entity_id': 24,
			'data': {
				'x': 12,
				'y': 12
			}}]";


        public string randomJsonArray3 = @"[{
			'comp': 'Move',
			'client_id': 0,
			'component_id': 3,
			'entity_id': 24,
			'data': {
				'x': 1200,
				'y': 1200
			}
            },
{'comp': 'Draw',
			'client_id': 0,
			'component_id': 3,
			'entity_id': 24,
			'data': {
				'x': 1000,
				'y': 100000
			}}]";

        public string randomJsonArray4 = @"[{
			'comp': 'Drag',
			'client_id': 0,
			'component_id': 3,
			'entity_id': 24,
			'data': {
				'x': 1200,
				'y': 1200
			}
            },
{'comp': 'Move2',
			'client_id': 0,
			'component_id': 3,
			'entity_id': 24,
			'data': {
				'x': 1000,
				'y': 100000
			}}]";

        public string randomJsonString3 = @"{'comp':'name','client_id':0,'component_id':13,'entity_id':24,'data':[{'x':10,'y':12},{'Note':Bla,bla,'y':12}}";

        public TestComponentClasse GetTestData()
        {

            return new TestComponentClasse();
        }

        public string GetTestJsonString()
        {
            var jsonString =  new TestComponentClasse();
            return jsonString.GetJsonString();
        }

        


    }

    public class TestComponentClasse : TestData { 
        public string comp = "name";
        public int client_id = 0;
        public int component_id = 13;
        public int entity_id = 24;
        public Data data = null;
        
        

        public TestComponentClasse()
        {
            //data = Data.GetInstance();
            data = Data.GetInstance();
        }

        public TestComponentClasse(string comp, int client_id, int component_id, int entity_id)
        {
            this.comp = comp;
            this.client_id = client_id;
            this.component_id = component_id;
            this.entity_id = entity_id;
            data = Data.GetInstance();
            //data = @"{'x': '10', 'y': '12'}";
        }

        public string GetRandomString()
        {
            return randomJsonString;
        }

        public string GetRandomString2()
        {
            return randomJsonString2;
        }

        public string GetRandomJsonArray2()
        {
            return randomJsonArray2;
        }

        public string GetRandomJsonArray3()
        {
            return randomJsonArray3;
        }

        public string GetRandomJsonArray4()
        {
            return randomJsonArray4;
        }

        public class Data
        {
            public int x=10;
            public int y = 12;



            Data() { }
            

            public static Data GetInstance()
            {
                return new Data();
            }
        }
        
        public string GetJsonString()
        {
            
            return JsonConvert.SerializeObject(this);
        }

        public TestComponentClasse GetClassObject(string JsonString)
        {
            TestComponentClasse deserializedProduct = JsonConvert.DeserializeObject<TestComponentClasse>(JsonString);

            return deserializedProduct;
        }


        public class TestComponentClasse2
        {
            public string comp = "name";
            public int client_id = 0;
            public int component_id = 13;
            public int entity_id = 24;
            public string data;
            //puvlic string data = 

            public TestComponentClasse2()
            {
                
            }
           
            
        }
    }
}
/*

                for (int i = 0; i<1000; i++)
                {
                    Connection.Instance.Space.Put("components", test.GetRandomJsonArray2());
                    Connection.Instance.Space.Put("components", test.GetRandomJsonArray3());
                    Connection.Instance.Space.Put("components", test.GetRandomString());
                    Connection.Instance.Space.Put("components", test.GetRandomString());
                    Connection.Instance.Space.Put("components", test.GetRandomString());
                    Connection.Instance.Space.Put("components", test.GetRandomString());
                }





                // ...


                Console.WriteLine("Sequentila Testing");


                sw.Start();
                //collector.BeginCollect();
                Collector();
                //PrintUpdateComponents();
                sw.Stop();
                Console.WriteLine("Sequential Elapsed={0}", sw.Elapsed.Milliseconds);


                Console.WriteLine("Parallel Testing");
                for (int i = 0; i < 1000; i++)
                {
                    Connection.Instance.Space.Put("components", test.GetRandomJsonArray2());
                    Connection.Instance.Space.Put("components", test.GetRandomJsonArray3());
                    Connection.Instance.Space.Put("components", test.GetRandomString());
                    Connection.Instance.Space.Put("components", test.GetRandomString());
                    Connection.Instance.Space.Put("components", test.GetRandomString());
                    Connection.Instance.Space.Put("components", test.GetRandomString());
                }

                sw.Start();
                collector.BeginCollect();
                //Collector();
                //PrintUpdateComponents();
                sw.Stop();
                Console.WriteLine("Parallel Elapsed={0}", sw.Elapsed.Milliseconds);

                Console.WriteLine("");
                Console.WriteLine("RUNNING 100 instances");
                Console.WriteLine("");
                for (int i = 0; i < 100; i++)
                {
                    Connection.Instance.Space.Put("components", test.GetRandomJsonArray2());
                    Connection.Instance.Space.Put("components", test.GetRandomJsonArray3());
                    Connection.Instance.Space.Put("components", test.GetRandomString());
                    Connection.Instance.Space.Put("components", test.GetRandomString());
                    Connection.Instance.Space.Put("components", test.GetRandomString());
                    Connection.Instance.Space.Put("components", test.GetRandomString());
                }
                sw.Start();
                collector.BeginCollect();
                //Collector();
                //PrintUpdateComponents();
                sw.Stop();
                Console.WriteLine("Parallel Elapsed={0}", sw.Elapsed.Milliseconds);

                for (int i = 0; i < 1000; i++)
                {
                    Connection.Instance.Space.Put("components", test.GetRandomJsonArray2());
                    Connection.Instance.Space.Put("components", test.GetRandomJsonArray3());
                    Connection.Instance.Space.Put("components", test.GetRandomString());
                    Connection.Instance.Space.Put("components", test.GetRandomString());
                    Connection.Instance.Space.Put("components", test.GetRandomString());
                    Connection.Instance.Space.Put("components", test.GetRandomString());
                }





                // ...


                Console.WriteLine("Sequentila Testing");


                sw.Start();
                //collector.BeginCollect();
                Collector();
                //PrintUpdateComponents();
                sw.Stop();
                Console.WriteLine("Sequential Elapsed={0}", sw.Elapsed.Milliseconds);
                */
