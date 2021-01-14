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
        /// <summary>
        /// Creates an array to be insertet in a tuple.
        /// It converts datatypes into a format that can be consumed by dotSpaces.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="updateID"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Convert datatypes in to a format that is fit for Tuple spaces.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
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
