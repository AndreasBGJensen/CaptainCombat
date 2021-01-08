using dotSpace.Interfaces.Space;
using dotSpace.Objects.Space;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuple = dotSpace.Objects.Space.Tuple;

namespace RemoteServer.Collector
{
     class TupleCollector : CollectorClass, ICollector
    {
        private string searchString = "components";//Default
        private SequentialSpace mySpace;

        public TupleCollector(string searchString, SequentialSpace space)
        {
            this.searchString = searchString;
            mySpace = space;
        }

        void ICollector.Collect()
        {
         

        //Collecting components
        IEnumerable<ITuple> results = mySpace.GetAll(searchString, typeof(string));
                foreach (Tuple x in results)
                {
                    try
                    {
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
                            UpdatorJToken(JsonConvert.SerializeObject(jToken), jToken);
                        };
                    }

                }

        }
        private void UpdatorJObject(string stringComponentUpdate, Newtonsoft.Json.Linq.JObject serarchParam)
        {
            var comp = (string)serarchParam.SelectToken("comp");
            var client_id = (int)serarchParam.SelectToken("client_id");
            var component_id = (int)serarchParam.SelectToken("component_id");
            var entity_id = (int)serarchParam.SelectToken("entity_id");
            var data = serarchParam.SelectToken("data");
            var data_string = JsonConvert.SerializeObject(data.Parent);
            ITuple result = mySpace.GetP(comp, client_id, component_id, entity_id, typeof(string));

            mySpace.Put(new Tuple(comp, client_id, component_id, entity_id, data_string));
        }

        private void UpdatorJToken(string stringComponentUpdate, Newtonsoft.Json.Linq.JToken serarchParam)
        {
            var comp = (string)serarchParam.SelectToken("comp");
            var client_id = (int)serarchParam.SelectToken("client_id");
            var component_id = (int)serarchParam.SelectToken("component_id");
            var entity_id = (int)serarchParam.SelectToken("entity_id");
            var data = serarchParam.SelectToken("data");
            var data_string = JsonConvert.SerializeObject(data.Parent);

            ITuple result = mySpace.GetP(comp, client_id, component_id, entity_id, typeof(string));


            mySpace.Put(new Tuple(comp, client_id, component_id, entity_id, data_string));
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
