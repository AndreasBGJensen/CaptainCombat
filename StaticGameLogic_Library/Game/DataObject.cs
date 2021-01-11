using System;

namespace StaticGameLogic_Library.Game
{
    class DataObject
    {
        public string comp;
        public int client_id;
        public int component_id;
        public int entity_id;
        public object data; 
        

        public DataObject(string comp, int client_id, int component_id, int entity_id, Object data)
        {
            this.comp = comp;
            this.client_id = client_id;
            this.component_id = component_id;
            this.entity_id = entity_id;
            this.data = data; 
        }

    }
}
