using Common;
using System;
using System.Collections.Generic;
using System.Text;
using StackExchange.Redis;
using Newtonsoft.Json;

namespace DataAccess
{
    public class CacheDataAccess
    {
        private IDatabase myDb;
        public CacheDataAccess(string connectionString)
        {
            ConnectionMultiplexer cm = ConnectionMultiplexer.Connect(connectionString);
            myDb  = cm.GetDatabase();

        }

        public List<MenuItem> GetMenuItems()
        {
            var jsonString = myDb.StringGet("mainmenu");
            if (jsonString.IsNullOrEmpty)
                return new List<MenuItem>();
            else
                return JsonConvert.DeserializeObject<List<MenuItem>>(jsonString);
        }

        public void AddMenuItem(MenuItem m)
        {
            var list = GetMenuItems();
            list.Add(m);
            string jsonString = JsonConvert.SerializeObject(list);
            myDb.StringSet("mainmenu", jsonString);
        }
    }
}
