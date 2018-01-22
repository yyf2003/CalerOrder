using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Common
{
    /// <summary>
    /// 序列化对象类
    /// </summary>
    public static class SerializeObject
    {
        /// <summary>
        /// 二进制序列化为字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string BinaryToString(object obj)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            formatter.Serialize(ms, obj);
            byte[] buffer = new byte[ms.Length];
            buffer = ms.ToArray();
            string result = Convert.ToBase64String(buffer);
            ms.Flush();
            ms.Close();
            return result;
        }

        public static object BinaryToObj(string str)
        {
            byte[] buffer = Convert.FromBase64String(str);
            MemoryStream ms = new MemoryStream(buffer, 0, buffer.Length);
            BinaryFormatter formatter = new BinaryFormatter();
            object obj = formatter.Deserialize(ms);
            ms.Flush();
            ms.Close();
            return obj;
        }
    }
}
