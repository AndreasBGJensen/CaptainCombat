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
        internal SequentialSpace space = null;
        private ICollector collector;

        public void BeginCollect()
        {
            collector.Collect();
        }

        public void InitCollector(SequentialSpace space, ICollector collector)
        {
            if(this.space == null)
            {
                this.space = space; 
            }
            this.collector = collector;
        }




    }
}
