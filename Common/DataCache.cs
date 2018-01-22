using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Common
{
    public class DataCache
    {
        public static object GetCache(string key)
        {
            System.Web.Caching.Cache cache = System.Web.HttpRuntime.Cache;
            return cache[key];
        }

        public static void SetCache(string key, object obj)
        {
            System.Web.Caching.Cache cache = System.Web.HttpRuntime.Cache;
            cache.Insert(key,obj);
        }
    }
}
