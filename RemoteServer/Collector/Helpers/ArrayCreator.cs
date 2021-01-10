using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteServer.Collector.Helpers
{
    class ArrayCreator
    {

        public object[] CreateArray(JToken token)
        {

            JToken[] array = token.ToArray();
            var length = array.Length;


            object[] newArray = new object[length];
            for (int i = 0; i < length; i++)
            {
                newArray[i] = DefineDatatype(array[i].First);

                /*  if (array[i].Path.Contains("data"))
                  {
                      newArray[i] = JsonConvert.SerializeObject(array[i]);

                  }*/

            }

            return newArray;
        }

        private dynamic DefineDatatype(JToken data)
        {
            switch (data.Type)
            {
                case (JTokenType)1: return JsonConvert.SerializeObject(data);

                case (JTokenType)6: return (int)data;

                case (JTokenType)8: return (string)data;

                default: return JsonConvert.SerializeObject(data);
            }
        }
    }
}
