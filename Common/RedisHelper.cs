using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.Redis;
using ServiceStack.Redis.Generic;

namespace Common
{
    public class RedisHelper
    {
        static string host = "127.0.0.1";
        static string password = "caler123456";
        static RedisClient rc;
        static RedisClient RC {
            get {
                rc = new RedisClient(host, 6379);
                rc.Password = password;
                return rc;
            }
        }

        public static void Set<T>(string key, T obj) where T : class, new()
        {
            RC.Set<T>(key, obj);
        }

        public static T Get<T>(string key) where T:class,new()
        {
           return RC.Get<T>(key);
        }
    }
}
