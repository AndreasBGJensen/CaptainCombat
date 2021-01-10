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
using RemoteServer.Collector.Helpers;

namespace RemoteServer.Collector
{
    class TupleCollectorParallel : CollectorClass, ICollector
    {
        //private string searchString = "components"; //Default
        private SequentialSpace mySpace;
        private ArrayCreator creator;

        public TupleCollectorParallel(ArrayCreator creator, SequentialSpace space)
        {
            mySpace = space;
            this.creator = creator;
           if (mySpace.QueryP("lock") == null)
            {
                mySpace.Put("lock");
            }
        }

        public void Collect()
        { 
            IEnumerable<ITuple> results = mySpace.GetAll(typeof(string), typeof(string));
            results.AsParallel().ForAll(item =>
            {
                 //Check if a component consist of a single JSON or if it consist of a multiple components
                 JArray jarray = JsonConvert.DeserializeObject<JArray>((string)item[1]);
                 jarray.AsParallel().ForAll(jToken =>
                 {
                    UpdatorJToken(jToken);
                 });        
            });


        }

      /*  private void UpdatorJObject(JObject serarchParam)
        {
            var comp = (string)serarchParam.SelectToken("comp");
            var client_id = (int)serarchParam.SelectToken("client_id");
            var component_id = (int)serarchParam.SelectToken("component_id");
            var entity_id = (int)serarchParam.SelectToken("entity_id");
            var data = serarchParam.SelectToken("data");
            var data_string = JsonConvert.SerializeObject(data);
            //Updating Tuple
            mySpace.Get("lock");
            ITuple result = mySpace.GetP(comp, client_id, component_id, entity_id, typeof(string));
            mySpace.Put(new Tuple(comp, client_id, component_id, entity_id, data_string));
            mySpace.Put("lock");
        }*/

        private void UpdatorJToken(JToken serarchParam)
        {
            var array = creator.CreateArray(serarchParam);
            
            //Updating Tuple
            mySpace.Get("lock");
            ITuple result = mySpace.GetP(array);
            mySpace.Put(array);
            mySpace.Put("lock");
        }

        public void TestPrintQueryAll()
        {
            IEnumerable<ITuple> results = mySpace.QueryAll(typeof(string), typeof(string));
            foreach (ITuple tuple in results)
            {
                Console.WriteLine(tuple[1]);
            }
        }

        public void PrintUpdateComponents()
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
