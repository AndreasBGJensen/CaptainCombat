using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dotSpace.Interfaces.Space;
using dotSpace.Objects.Network;
using dotSpace.Objects.Space;
using RemoteServer.singletons;
using Tuple = dotSpace.Objects.Space.Tuple;
using Newtonsoft.Json;
using RemoteServer.TestData;
using static RemoteServer.TestData.TestComponentClasse;
using RemoteServer.Collector;
using System.Diagnostics;
using Newtonsoft.Json.Linq;

namespace RemoteServer.threads
{
   class Serialization
    {
        TestComponentClasse test = new TestComponentClasse();
        CollectorClass collector = new CollectorClass();
        Stopwatch sw = new Stopwatch(); // For performance testing
        public void RunProtocol()
        {
            collector.SetSpace(Connection.Instance.Space);
            //collector.SetCollector(new TupleCollectorParallel("components",Connection.Instance.Space));
            collector.SetCollector(new TupleCollector("components", Connection.Instance.Space));
            Console.WriteLine("Running...");
           while (true)
           {
            try
                {

                collector.BeginCollect();
                //PrintUpdateComponents();
            }
            catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                }
            }
        }


        private void Collector()
        {
            //Collecting components
            const string searchString = "components";
            IEnumerable<ITuple> results = Connection.Instance.Space.GetAll(searchString, typeof(string));
                foreach (Tuple x in results)
                {
                    try { 
                    //Check if a component consist of a single JSON or if it consist of a multiple components
                    //If multiple components are uploaded it will be a JsonArray and the operation below will throw a Invalid Cast Exception and JsonArray will be unpaced in the catch block.
                    var test1 = (Newtonsoft.Json.Linq.JObject)JsonConvert.DeserializeObject((string)x[1]);
                    //Console.WriteLine(test1.Count);
                    UpdatorJObject((string)x[1], test1);

                    }
                    catch (InvalidCastException e)
                    {
                        Newtonsoft.Json.Linq.JArray jarray = JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JArray>((string)x[1]);
                        foreach (Newtonsoft.Json.Linq.JToken jToken in jarray)
                        {
                        //Console.WriteLine(jToken); 
                            UpdatorJToken(JsonConvert.SerializeObject(jToken), jToken);
                        };
                    }
                
                }
            
                
                //Running updates in parallel - Currently this wont work because we get multiple updates
               /* results.AsParallel().ForAll(item =>
                     {
                         try {
                             //Check if a component consist of a single JSON or if it consist of a multiple components
                             Newtonsoft.Json.Linq.JArray jarray = JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JArray>((string)item[1]);
                             jarray.AsParallel().ForAll(jToken =>
                             {
                                 UpdatorJToken(JsonConvert.SerializeObject(jToken), jToken);
                             });
                         }
                         catch(InvalidCastException e)
                         {
                                  //Er det Json stringen for componenten som skal uddateres?
                                  //Eller skal den indeholde en component?

                                  var test1 = (Newtonsoft.Json.Linq.JObject)JsonConvert.DeserializeObject((string)item[1]);
                                  UpdatorJObject((string)item[1], test1);
                         }
                     });*/

            } 

        private void UpdatorJObject(string stringComponentUpdate, JObject serarchParam)
        {
            var comp = (string)serarchParam.SelectToken("comp");
            var client_id = (int)serarchParam.SelectToken("client_id");
            var component_id = (int)serarchParam.SelectToken("component_id");
            var entity_id = (int)serarchParam.SelectToken("entity_id");
            var data = serarchParam.SelectToken("data");
            ITuple result = Connection.Instance.Space.GetP(comp,client_id,component_id,entity_id, typeof(string));

            Connection.Instance.Space.Put(new Tuple(comp, client_id, component_id, entity_id, data));
        }

        private void UpdatorJToken(string stringComponentUpdate, JToken serarchParam)
        {
            var comp = (string)serarchParam.SelectToken("comp");
            var client_id = (int)serarchParam.SelectToken("client_id");
            var component_id = (int)serarchParam.SelectToken("component_id");
            var entity_id = (int)serarchParam.SelectToken("entity_id");


            var data = serarchParam.SelectToken("data");

            var data_string = JsonConvert.SerializeObject(data);


            ITuple result = Connection.Instance.Space.GetP(comp, client_id, component_id, entity_id, typeof(string));
            Connection.Instance.Space.Put(new Tuple(comp, client_id, component_id, entity_id, data_string));
        }



        private void TestPrintQueryAll()
        {
            IEnumerable<ITuple> results = Connection.Instance.Space.QueryAll("components", typeof(string));
            foreach (ITuple tuple in results)
            {
                Console.WriteLine(tuple[1]);
            }
        }

        private void PrintUpdateComponents()
        {
           // Console.WriteLine("Printing test components");
            IEnumerable<ITuple> results3 = Connection.Instance.Space.QueryAll(typeof(string), typeof(int), typeof(int), typeof(int), typeof(string));
            foreach (ITuple tuple in results3)
            {
                Console.WriteLine(tuple);
            }
        }
    }
}
