using System;
using static ECS.Domain;

namespace CaptainCombat.game
{
    class DataObject
    {
        public string comp;
        public uint client_id;
        public uint component_id;
        public uint entity_id;
        public Component data;

        public DataObject(string comp, uint client_id, uint component_id, uint entity_id, Component data)
        {
            this.comp = comp;
            this.client_id = client_id;
            this.component_id = component_id;
            this.entity_id = entity_id;
            this.data = data; 
        }

    }
}
