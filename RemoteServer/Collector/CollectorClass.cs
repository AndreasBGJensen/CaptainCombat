using dotSpace.Objects.Space;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteServer.Collector
{
    class CollectorClass
    {
        private ICollector collector;

        public void BeginCollect()
        {
            collector.Collect();
        }

       
        

        public void SetCollector(ICollector collector)
        {
            this.collector = collector;
        }

        public void PrintUpdateComponents()
        {
            collector.PrintUpdateComponents();
        }

    }
}
