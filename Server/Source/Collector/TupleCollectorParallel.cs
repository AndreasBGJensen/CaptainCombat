using dotSpace.Interfaces.Space;
using dotSpace.Objects.Space;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using CaptainCombat.Server.Collector.Helpers;


namespace CaptainCombat.Server.Collector
{
    class TupleCollectorParallel : CollectorClass, ICollector
    {
        private SequentialSpace mySpace;
        private ArrayCreator creator;

        public TupleCollectorParallel(ArrayCreator creator, SequentialSpace space)
        {
           mySpace = space;
           this.creator = creator;

           //Initializing a lock so that we get atomic transactions when we update the components.
           //Locks are not really nesseasery in this case because we at just updating and using pattern matching, 
           //but i thought i would be a good practice
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
                 JArray jarray = JsonConvert.DeserializeObject<JArray>((string)item[1]);
                 jarray.AsParallel().ForAll(jToken =>
                 {
                    UpdatorJToken(jToken);
                 });        
            });


        }

        //This is ofe user if we use JObject instead of JArray's
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
            var array = creator.CreateArray(serarchParam,null);
            
            //Updating Tuple
            mySpace.Get("lock");
            //TODO: How can I fix this so that it will not be depended og each element??
            ITuple result = mySpace.GetP(array[0], array[1], array[2], array[3], typeof(string));
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

    }
}
