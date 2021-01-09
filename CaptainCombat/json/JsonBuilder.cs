using CaptainCombat.game;
using CaptainCombat.singletons;
using ECS;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static ECS.Domain;

namespace CaptainCombat.json
{
    class JsonBuilder
    {
        
        public string createJsonString()
        {
            List<DataObject> data = new List<DataObject>();


            DomainState.Instance.Domain.ForLocalComponents((component) => {
                data.Add(new DataObject(
                    component.GetTypeIdentifier(),
                    Connection.Instance.User_id,
                    (int)component.Id.objectId,
                    (int)component.Entity.Id.objectId,
                    component.getData()
                ));
            });

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            Console.WriteLine(json); 

             Newtonsoft.Json.Linq.JArray jarray = JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JArray>(json);
            Console.WriteLine(jarray);

            return json;
      }

   }
}
