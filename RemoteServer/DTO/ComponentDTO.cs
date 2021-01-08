using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


//This name space is not in use and is to be deleted
namespace RemoteServer.DTO
{

    class ComponentDTO
    {
        public string comp = "name";
        public int client_id = 0;
        public int component_id = 13;
        public int entity_id = 24;
        public string data = null;


        public ComponentDTO()
        {
            data = @"{'x': '10', 'y': '12'}";
        }
    }
}
