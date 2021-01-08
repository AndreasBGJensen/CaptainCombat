using dotSpace.Interfaces.Space;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuple = dotSpace.Objects.Space.Tuple;

namespace RemoteServer.Collector
{
    class TupleCollectorParallel : CollectorClass, ICollector
    {
        private string searchString = "components"; //Default

        public TupleCollectorParallel(string searchString)
        {
            this.searchString = searchString;
            if (space.QueryP("lock") == null)
            {
                space.Put("lock");
            }
        }

        public void Collect()
        {
            IEnumerable<ITuple> results = space.GetAll(searchString, typeof(string));
            results.AsParallel().ForAll(item =>
            {
                try
                {
                    //Check if a component consist of a single JSON or if it consist of a multiple components
                    Newtonsoft.Json.Linq.JArray jarray = JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JArray>((string)item[1]);
                    jarray.AsParallel().ForAll(jToken =>
                    {
                        UpdatorJToken(JsonConvert.SerializeObject(jToken), jToken);
                    });
                }
                catch (InvalidCastException e)
                {
                    //Er det Json stringen for componenten som skal uddateres?
                    //Eller skal den indeholde en component?

                    var test1 = (Newtonsoft.Json.Linq.JObject)JsonConvert.DeserializeObject((string)item[1]);
                    UpdatorJObject((string)item[1], test1);
                }
            });


        }

        private void UpdatorJObject(string stringComponentUpdate, Newtonsoft.Json.Linq.JObject serarchParam)
        {
            var comp = (string)serarchParam.SelectToken("comp");
            var client_id = (int)serarchParam.SelectToken("client_id");
            var component_id = (int)serarchParam.SelectToken("component_id");
            var entity_id = (int)serarchParam.SelectToken("entity_id");
            var data = serarchParam.SelectToken("data");
            var data_string = JsonConvert.SerializeObject(data.Parent);

            //Updating Tuple
            space.Get("lock");
            ITuple result = space.GetP(comp, client_id, component_id, entity_id, typeof(string));
            space.Put(new Tuple(comp, client_id, component_id, entity_id, data_string));
            space.Put("lock");
        }

        private void UpdatorJToken(string stringComponentUpdate, Newtonsoft.Json.Linq.JToken serarchParam)
        {
            var comp = (string)serarchParam.SelectToken("comp");
            var client_id = (int)serarchParam.SelectToken("client_id");
            var component_id = (int)serarchParam.SelectToken("component_id");
            var entity_id = (int)serarchParam.SelectToken("entity_id");
            var data = serarchParam.SelectToken("data");
            var data_string = JsonConvert.SerializeObject(data.Parent);


            //Updating Tuple
            space.Get("lock");
            ITuple result = space.GetP(comp, client_id, component_id, entity_id, typeof(string));
            space.Put(new Tuple(comp, client_id, component_id, entity_id, data_string));
            space.Put("lock");
        }



        private void TestPrintQueryAll()
        {
            IEnumerable<ITuple> results = space.QueryAll("components", typeof(string));
            foreach (ITuple tuple in results)
            {
                Console.WriteLine(tuple[1]);
            }
        }

        private void PrintUpdateComponents()
        {
            Console.WriteLine("Printing test components");
            IEnumerable<ITuple> results3 = space.QueryAll(typeof(string), typeof(int), typeof(int), typeof(int), typeof(string));
            foreach (ITuple tuple in results3)
            {
                Console.WriteLine(tuple);
            }
        }
    }
}
