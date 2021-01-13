using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace CaptainCombat.Source.Event {
    
    public class HelloEvent : Event {

        public string Msg { get; set; }

        // All events must have a default constructor
        public HelloEvent() {}

        public HelloEvent(string msg, uint receiver) : base(receiver) {
            Msg = msg;
        }
    }

}
