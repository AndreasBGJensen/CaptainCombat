using Newtonsoft.Json;
using System.Collections.Generic;
using static CaptainCombat.Common.Domain;

namespace CaptainCombat.Common.JsonBuilder
{

    public static class JsonBuilder {

        public static string BuildComponentsJSONString(Domain domain) {
            List<ComponentData> data = new List<ComponentData>();

            domain.ForLocalComponents((component) => {
                if (component.SyncMode != Component.SynchronizationMode.NONE)

                    data.Add(new ComponentData(
                        component.GetTypeIdentifier(),
                        component.ClientId,
                        component.Id.objectId,
                        component.Entity.Id.objectId,
                        component
                    ));
            });

            string json = JsonConvert.SerializeObject(data);

            return json;
        }
    }


    public class ComponentData
    {
        public string comp;
        public uint client_id;
        public uint component_id;
        public uint entity_id;
        public Component data;

        public ComponentData(string comp, uint client_id, uint component_id, uint entity_id, Component data)
        {
            this.comp = comp;
            this.client_id = client_id;
            this.component_id = component_id;
            this.entity_id = entity_id;
            this.data = data;
        }

    }
}
