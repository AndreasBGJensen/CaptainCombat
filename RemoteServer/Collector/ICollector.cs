using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteServer.Collector
{
    interface ICollector
    {
        void Collect();

        void PrintUpdateComponents();
    }
}
