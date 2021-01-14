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
            IEnumerable<ITuple> results = mySpace.GetAll("components", typeof(string));
            foreach (Tuple x in results)
            {
               JArray jarray = JsonConvert.DeserializeObject<JArray>((string)x[1]);
               var id=  (int)jarray.First.SelectToken("client_id");
                RemoveExistingClientTuples(id);
               foreach (Newtonsoft.Json.Linq.JToken jToken in jarray)
               {
                    UpdatorJToken(jToken);
                    //UpdateTupleSpace(jToken);
                };   
            }
        }

        private void UpdatorJToken(JToken serarchParam)
        {
            var array = creator.CreateArray(serarchParam);
            //TODO: How can I fix this so that it will not be depended og each element??
            ITuple result = mySpace.GetP(array[0], array[1], array[2], array[3], typeof(string));
            mySpace.Put(array);

        }

        private void UpdateTupleSpace(JToken serarchParam)
        {
            var array = creator.CreateArray(serarchParam);
            //TODO: How can I fix this so that it will not be depended og each element??
            //ITuple result = mySpace.GetP(array[0], array[1], array[2], array[3], typeof(string));
            //RemoveExistingClientTuples((int)array[1]);
            mySpace.Put(array);
        }

        private void RemoveExistingClientTuples(int ClientID)
        {
            IEnumerable<ITuple> result = mySpace.GetAll(typeof(string), ClientID, typeof(int), typeof(int), typeof(string));
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
