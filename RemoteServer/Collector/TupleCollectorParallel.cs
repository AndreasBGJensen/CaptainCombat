using dotSpace.Interfaces.Space;
using dotSpace.Objects.Space;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuple = dotSpace.Objects.Space.Tuple;
using Newtonsoft.Json.Linq;

namespace RemoteServer.Collector
{
    class TupleCollectorParallel : CollectorClass, ICollector
    {
        private string searchString = "components"; //Default
        private SequentialSpace mySpace;

        public TupleCollectorParallel(string searchString, SequentialSpace space)
        {
            mySpace = space;
            this.searchString = searchString;
           if (mySpace.QueryP("lock") == null)
            {
                mySpace.Put("lock");
            }
        }

        public void Collect()
        {
            
            IEnumerable<ITuple> results = mySpace.GetAll(searchString, typeof(string));
            results.AsParallel().ForAll(item =>
            {
                try
                {
                    //Check if a component consist of a single JSON or if it consist of a multiple components
                    JArray jarray = JsonConvert.DeserializeObject<JArray>((string)item[1]);
                    jarray.AsParallel().ForAll(jToken =>
                    {
                        UpdatorJToken(JsonConvert.SerializeObject(jToken), jToken);
                    });
                }
                catch (InvalidCastException e)
                {
                    //Er det Json stringen for componenten som skal uddateres?
                    //Eller skal den indeholde en component?

                    var test1 = (JObject)JsonConvert.DeserializeObject((string)item[1]);
                    UpdatorJObject((string)item[1], test1);
                }
            });


        }

        private void UpdatorJObject(string stringComponentUpdate, JObject serarchParam)
        {
            var comp = (string)serarchParam.SelectToken("comp");
            var client_id = (int)serarchParam.SelectToken("client_id");
            var component_id = (int)serarchParam.SelectToken("component_id");
            var entity_id = (int)serarchParam.SelectToken("entity_id");
            var data = serarchParam.SelectToken("data");

            //Updating Tuple
            mySpace.Get("lock");
            ITuple result = mySpace.GetP(comp, client_id, component_id, entity_id, typeof(string));
            mySpace.Put(new Tuple(comp, client_id, component_id, entity_id, data));
            mySpace.Put("lock");
        }

        private void UpdatorJToken(string stringComponentUpdate, JToken serarchParam)
        {
            var comp = (string)serarchParam.SelectToken("comp");
            var client_id = (int)serarchParam.SelectToken("client_id");
            var component_id = (int)serarchParam.SelectToken("component_id");
            var entity_id = (int)serarchParam.SelectToken("entity_id");
            var data = serarchParam.SelectToken("data");


            //Updating Tuple
            mySpace.Get("lock");
            ITuple result = mySpace.GetP(comp, client_id, component_id, entity_id, typeof(string));
            mySpace.Put(new Tuple(comp, client_id, component_id, entity_id, data));
            mySpace.Put("lock");
        }



        private void TestPrintQueryAll()
        {
            IEnumerable<ITuple> results = mySpace.QueryAll("components", typeof(string));
            foreach (ITuple tuple in results)
            {
                Console.WriteLine(tuple[1]);
            }
        }

        private void PrintUpdateComponents()
        {
            Console.WriteLine("Printing test components");
            IEnumerable<ITuple> results3 = mySpace.QueryAll(typeof(string), typeof(int), typeof(int), typeof(int), typeof(string));
            foreach (ITuple tuple in results3)
            {
                Console.WriteLine(tuple);
            }
        }
    }
}
