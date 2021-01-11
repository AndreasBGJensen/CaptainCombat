using StaticGameLogic_Library.Game;
using StaticGameLogic_Library.Singletons;
using System.Collections.Generic;


namespace StaticGameLogic_Library.JsonBuilder
{
    public static class JsonBuilder
    {
        
        public static string createJsonString() {

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
            //Console.WriteLine(json); 

            //Newtonsoft.Json.Linq.JArray jarray = JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JArray>(json);
            //Console.WriteLine(jarray);

            return json;
      }

   }
}
