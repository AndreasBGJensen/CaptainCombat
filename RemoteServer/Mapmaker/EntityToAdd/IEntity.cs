using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteServer.Mapmaker.EntityToAdd
{
    interface IEntity
    {
        void OnComputerInit(object source, EventArgs e);
    }
}
