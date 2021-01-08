﻿using CaptainCombat.game;
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
    //class JsonBuilder
    //{
        
        public string createJsonString()
        {
            List<DataObject> data = new List<DataObject>();

            foreach (Component component in DomainState.Instance.Domain.getAllComponents())
            {
                data.Add(new DataObject(component.GetType().Name, Connection.Instance.User_id, (int)component.Id.objectId, (int)component.Entity.Id.objectId, component.getData())); 
                /*
                Console.WriteLine("comp: " + component.GetType().Name);
                Console.WriteLine("client_id: " + Connection.Instance.User);
                Console.WriteLine("component_id: " + component.Id.objectId);
                Console.WriteLine("entity_id: " + component.Entity.Id.objectId);
                Console.WriteLine(" data:"+component.getData());
                */
            }

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            Console.WriteLine(json); 

             //Newtonsoft.Json.Linq.JArray jarray = JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JArray>(json);
            //Console.WriteLine(jarray);

    //        //var test1 = (Newtonsoft.Json.Linq.JObject)JsonConvert.DeserializeObject(json);
    //        //Console.WriteLine(test1); 


    //        return json;
    //    }

    //}
}
