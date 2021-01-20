using dotSpace.Interfaces.Space;
using Newtonsoft.Json;
using CaptainCombat.Server.Collector.Helpers;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Tuple = dotSpace.Objects.Space.Tuple;

namespace CaptainCombat.Server.Collector
{
     class TupleCollector : CollectorClass, ICollector
    {
        private ISpace mySpace;
        private ArrayCreator creator;

        public TupleCollector(ArrayCreator creator, ISpace space)
        {
            this.creator = creator;
            mySpace = space;

        }

        /// <summary>
        /// Collects Tuples matching {<"components", typeof(string), typeof(string)">}
        /// This method will upload the components and earlier versions of the same upload based on the second tuple element. This elements (Tuple[1])
        /// is expected to be a version number convertet to a string. 
        /// </summary>
        void ICollector.Collect()
        {
            //Collecting components uploaded from clients
            var tuple = mySpace.Get("components", typeof(string), typeof(string));
           
            string updateId = (string)tuple[1];
            string jsonData = (string)tuple[2]; 
            JArray jarray = JsonConvert.DeserializeObject<JArray>(jsonData);

            // Fetch client in order to remove all id components from the space
            var id = (int)jarray.First.SelectToken("client_id"); 
                
            foreach (JToken jToken in jarray)
                UpdatorJToken(jToken, updateId);

            RemoveExistingClientTuples(id, updateId);
        }

        /// <summary>
        /// Creates and uploads a tuple based on a JToken.
        /// </summary>
        /// <param name="serarchParam"></param>
        /// <param name="UpdateID"></param>
        private void UpdatorJToken(JToken serarchParam,string UpdateID)
        {
            var array = creator.CreateArray(serarchParam, UpdateID);
            mySpace.Put(array);

        }

        /// <summary>
        /// Remove earlier components uploaded from ClientID will be romeved. The update_id will be decremented by 1. 
        /// and all component tuples matching clientID and the decremented update_id will be removed.
        /// </summary>
        /// <param name="ClientID"></param>
        /// <param name="update_id"></param>
        private void RemoveExistingClientTuples(int ClientID,string update_id)
        {
            //Decrementing the update_id to remove earlier components uploaded from ClientID
            long update_id_long = long.Parse(update_id);
            update_id_long--;
            string decremented_update_id = update_id_long.ToString();
             IEnumerable<ITuple> result = mySpace.GetAll(typeof(string), ClientID, typeof(int), typeof(int), typeof(string), decremented_update_id);
        }

    }
}
