using dotSpace.Interfaces.Space;
using dotSpace.Objects.Space;
using Newtonsoft.Json;
using RemoteServer.Collector.Helpers;
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Tuple = dotSpace.Objects.Space.Tuple;

namespace RemoteServer.Collector
{
     class TupleCollector : CollectorClass, ICollector
    {
        private SequentialSpace mySpace;
        private ArrayCreator creator;

        public TupleCollector(ArrayCreator creator, SequentialSpace space)
        {
            this.creator = creator;
            mySpace = space;

        }

        void ICollector.Collect()
        {
            //Collecting components
            IEnumerable<ITuple> results = mySpace.GetAll("components",typeof(string), typeof(string));

           
            foreach (Tuple x in results)
            {
                string update_id = (string)x[1];
                JArray jarray = JsonConvert.DeserializeObject<JArray>((string)x[2]);
               //Fetch client i order to remove all id components from the space
               var id=  (int)jarray.First.SelectToken("client_id");
                
                
               foreach (JToken jToken in jarray)
               {
                    UpdatorJToken(jToken, update_id);
                    //UpdateTupleSpace(jToken);
                };

                RemoveExistingClientTuples(id, update_id);
            }
        }

        private void UpdatorJToken(JToken serarchParam,string UpdateID)
        {
            var array = creator.CreateArray(serarchParam, UpdateID);
            //TODO: How can I fix this so that it will not be depended og each element??
            //ITuple result = mySpace.GetP(array[0], array[1], array[2], array[3], typeof(string));
            mySpace.Put(array);

        }

        private void RemoveExistingClientTuples(int ClientID,string update_id)
        {
            //Decrementing
            long update_id_long = long.Parse(update_id);
            update_id_long--;
            string decremented_update_id = update_id_long.ToString();
            IEnumerable<ITuple> result = mySpace.GetAll(typeof(string), ClientID, typeof(int), typeof(int), typeof(string), decremented_update_id);
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

        public void PrintAllComponents()
        {
            //Console.WriteLine("Printing test components");
            IEnumerable<ITuple> results3 = mySpace.QueryAll(typeof(string), typeof(int), typeof(int), typeof(int), typeof(string));
            foreach (ITuple tuple in results3)
            {
            Console.WriteLine(tuple);
            }
        }
    }
}
