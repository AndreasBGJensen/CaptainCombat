﻿using CaptainCombat.game;
using CaptainCombat.singletons;
using Newtonsoft.Json;
using System.Collections.Generic;


namespace CaptainCombat.json
{
    public static class JsonBuilder
    {
        
        public static string createJsonString() {

            List<DataObject> data = new List<DataObject>();

            DomainState.Instance.Domain.ForLocalComponents((component) => {
                if( component.SyncMode != Component.SynchronizationMode.NONE )
                data.Add(new DataObject(
                    component.GetTypeIdentifier(),
                    (uint)Connection.Instance.User_id,
                    component.Id.objectId,
                    component.Entity.Id.objectId,
                    component
                ));
            });

            string json = JsonConvert.SerializeObject(data);

            return json;
      }

   }
}
