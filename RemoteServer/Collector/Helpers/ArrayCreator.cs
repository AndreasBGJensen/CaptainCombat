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

        public object[] CreateArray(JToken token,string updateID)
        {

            JToken[] array = token.ToArray();
            var length = array.Length+1;


            object[] newArray = new object[length];
            for (int i = 0; i < length-1; i++)
            {
                newArray[i] = DefineDatatype(array[i].First);
            }

            //For converting the long to the same form as in the Tuple
            long type_updateID = long.Parse(updateID);
            newArray[length - 1] = type_updateID.ToString();
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
