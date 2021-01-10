using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dotSpace.Interfaces.Space;
using dotSpace.Objects.Network;
using dotSpace.Objects.Space;
using RemoteServer.singletons;
using Tuple = dotSpace.Objects.Space.Tuple;
using Newtonsoft.Json;
using RemoteServer.TestData;
using static RemoteServer.TestData.TestComponentClasse;
using RemoteServer.Collector;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using RemoteServer.Collector.Helpers;

namespace RemoteServer.threads
{
   class Serialization
    {
        TestComponentClasse test = new TestComponentClasse();
        
        public void RunProtocol()
        {
           CollectorClass collector = new CollectorClass();
           ArrayCreator creator = new ArrayCreator();
           collector.SetSpace(Connection.Instance.Space);
           //collector.SetCollector(new TupleCollectorParallel(creator,Connection.Instance.Space));
           collector.SetCollector(new TupleCollector(creator, Connection.Instance.Space));
           Console.WriteLine("Running...");
           //while (true)
           //{
                try
                {
                Connection.Instance.Space.Put("components",test.GetRandomJsonArray2());
                Connection.Instance.Space.Put("components", test.GetRandomJsonArray3());

                collector.BeginCollect();
                    
                //Collector();
                PrintUpdateComponents();

            }
            catch (Exception e)
                {
                        Console.WriteLine(e.Message);
                        Console.WriteLine(e.StackTrace);
                }
            //}
        }


        private void Collector()
        {
            //Collecting components
            //const string searchString = "components";
            IEnumerable<ITuple> results = Connection.Instance.Space.GetAll(typeof(string), typeof(string));
                foreach (Tuple x in results)
                {
                    try { 
                    //Check if a component consist of a single JSON or if it consist of a multiple components
                    //If multiple components are uploaded it will be a JsonArray and the operation below will throw a Invalid Cast Exception and JsonArray will be unpaced in the catch block.
                    var test1 = (Newtonsoft.Json.Linq.JObject)JsonConvert.DeserializeObject((string)x[1]);
                    //Console.WriteLine(test1.Count);
                    UpdatorJObject((string)x[1], test1);

                    }
                    catch (InvalidCastException e)
                    {
                        JArray jarray = JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JArray>((string)x[1]);
                        foreach (Newtonsoft.Json.Linq.JToken jToken in jarray)
                        {
                        //Console.WriteLine(jToken); 
                         //   var hel = ArrayCreator(jToken);
                        //Connection.Instance.Space.Put(hel);

                        UpdatorJToken(JsonConvert.SerializeObject(jToken), jToken);
                    };
                    }
                
                }
            
                
                //Running updates in parallel - Currently this wont work because we get multiple updates
               /* results.AsParallel().ForAll(item =>
                     {
                         try {
                             //Check if a component consist of a single JSON or if it consist of a multiple components
                             Newtonsoft.Json.Linq.JArray jarray = JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JArray>((string)item[1]);
                             jarray.AsParallel().ForAll(jToken =>
                             {
                                 UpdatorJToken(JsonConvert.SerializeObject(jToken), jToken);
                             });
                         }
                         catch(InvalidCastException e)
                         {
                                  //Er det Json stringen for componenten som skal uddateres?
                                  //Eller skal den indeholde en component?

                                  var test1 = (Newtonsoft.Json.Linq.JObject)JsonConvert.DeserializeObject((string)item[1]);
                                  UpdatorJObject((string)item[1], test1);
                         }
                     });*/

            } 

        private void UpdatorJObject(string stringComponentUpdate, JObject serarchParam)
        {
            var comp = (string)serarchParam.SelectToken("comp");
            var client_id = (int)serarchParam.SelectToken("client_id");
            var component_id = (int)serarchParam.SelectToken("component_id");
            var entity_id = (int)serarchParam.SelectToken("entity_id");
            var data = serarchParam.SelectToken("data");
            ITuple result = Connection.Instance.Space.GetP(comp,client_id,component_id,entity_id, typeof(string));

            Connection.Instance.Space.Put(new Tuple(comp, client_id, component_id, entity_id, data));
        }

        private void UpdatorJToken(string stringComponentUpdate, JToken serarchParam)
        {

            var array = ArrayCreator(serarchParam);
            var comp = (string)serarchParam.SelectToken("comp");
            var client_id = (int)serarchParam.SelectToken("client_id");
            var component_id = (int)serarchParam.SelectToken("component_id");
            var entity_id = (int)serarchParam.SelectToken("entity_id");
            

            var data = serarchParam.SelectToken("data");
            var data_Type = data.Type;
            var data_string = JsonConvert.SerializeObject(data);

           ITuple result = Connection.Instance.Space.GetP(array[0], array[1], array[2], array[3], typeof(string));
           Connection.Instance.Space.Put(array);

            //ITuple result = Connection.Instance.Space.GetP(comp, client_id, component_id, entity_id, typeof(string));
            //Connection.Instance.Space.Put(new Tuple(comp, client_id, component_id, entity_id, data_string));
        }



        private void TestPrintQueryAll()
        {
            IEnumerable<ITuple> results = Connection.Instance.Space.QueryAll("components", typeof(string));
            foreach (ITuple tuple in results)
            {
                Console.WriteLine(tuple[1]);
            }
        }

        private void PrintUpdateComponents()
        {
            // Console.WriteLine("Printing test components");
            IEnumerable<ITuple> results3 = Connection.Instance.Space.QueryAll(typeof(string), typeof(int), typeof(int), typeof(int), typeof(string));
            //IEnumerable<ITuple> results3 = Connection.Instance.Space.QueryAll();

            foreach (ITuple tuple in results3)
            {
                Console.WriteLine(tuple);
            }
        }

        private object[] ArrayCreator(JToken token)
        {
            
            JToken[] array = token.ToArray();
            var length = array.Length;
           

            object[] newArray = new object[length];
            for(int i = 0; i< length; i++)
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
