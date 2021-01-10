﻿using dotSpace.Interfaces.Space;
using dotSpace.Objects.Space;
using Newtonsoft.Json;
using RemoteServer.Collector.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            IEnumerable<ITuple> results = mySpace.GetAll(typeof(string), typeof(string));
            foreach (Tuple x in results)
            {
               JArray jarray = JsonConvert.DeserializeObject<JArray>((string)x[1]);
               foreach (Newtonsoft.Json.Linq.JToken jToken in jarray)
               {
                   UpdatorJToken(jToken);
               };   
            }
        }

        private void UpdatorJToken(JToken serarchParam)
        {
            var array = creator.CreateArray(serarchParam);
            
            ITuple result = mySpace.GetP(array);
            mySpace.Put(array);

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
