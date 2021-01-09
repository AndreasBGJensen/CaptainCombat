using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingTester.Exceptions
{
    [Serializable]
    class PingException : Exception

    {

        public PingException(string message) : base(message)
        {
            
        }
    }
}
