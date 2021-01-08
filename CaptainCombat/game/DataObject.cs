using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ECS.Domain;

namespace CaptainCombat.game
{
    class DataObject
    {
        string comp; 
        string client_id;
        int component_id;
        int entity_id;
        object data; 
        

        public DataObject(string comp, string client_id, int component_id, int entity_id, Object data)
        {
            this.comp = comp;
            this.client_id = client_id;
            this.component_id = component_id;
            this.entity_id = entity_id;
            this.data = data; 
        }

       
    }
}
