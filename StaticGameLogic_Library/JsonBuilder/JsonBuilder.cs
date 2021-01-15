using Newtonsoft.Json;
using StaticGameLogic_Library.Game;
using StaticGameLogic_Library.Singletons;
using System.Collections.Generic;
using static StaticGameLogic_Library.Source.ECS.Domain;

namespace StaticGameLogic_Library.JsonBuilder
{

    public static class JsonBuilder {

        public static string createJsonString() {

            List<DataObject> data = new List<DataObject>();

            DomainState.Instance.Domain.ForLocalComponents((component) => {
                if (component.SyncMode != Component.SynchronizationMode.NONE)
                    data.Add(new DataObject(
                        component.GetTypeIdentifier(),
                        (uint)Connection.Instance.User_id,
                        component.Id.objectId,
                        component.Entity.Id.objectId,
                        component
                    ));
            });

            string json = JsonConvert.SerializeObject(data);

            return json;
        }

    }
}
