using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace CaptainCombat.Server.Collector.Helpers
{
    static class ArrayCreator
    {
        /// <summary>
        /// Creates an array to be insertet in a tuple.
        /// It converts datatypes into a format that can be consumed by dotSpaces.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="updateID"></param>
        /// <returns></returns>
        public static object[] CreateArray(JToken token,string updateID)
        {

            JToken[] array = token.ToArray();
            var length = array.Length+1;


            object[] newArray = new object[length];
            for (int i = 0; i < length-1; i++)
            {
                newArray[i] = GetDataType(array[i].First);
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
        private static dynamic GetDataType(JToken data)
        {
            switch (data.Type)
            {
                case JTokenType.Object: return JsonConvert.SerializeObject(data);

                case JTokenType.Integer: return (int)data;

                case JTokenType.String: return (string)data;

                case JTokenType.Float: return (double)data;

                default: return JsonConvert.SerializeObject(data);
            }
        }
    }
}
