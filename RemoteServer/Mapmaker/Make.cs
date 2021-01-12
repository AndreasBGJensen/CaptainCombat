using dotSpace.Objects.Space;
using Source.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteServer.Mapmaker
{
    public abstract class Make
    {
        private SequentialSpace space;
        private Domain domain;

        public abstract void MakeEntity();

        public void SetSpaceAndDomain(SequentialSpace space, Domain domain)
        {
            this.space = space;
            this.domain = domain;
        }  

    }
}
