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
        private SequentialSpace mySpace = null;
        private ICollector collector;

        public void BeginCollect()
        {
            collector.Collect();
        }

       
        public void SetSpace(SequentialSpace space)
        {
            if (mySpace == null)
            {
                mySpace = space;
            }
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
