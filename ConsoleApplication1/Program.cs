using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            List<int> a = new List<int>() { 2, 3,9 };
            List<int> b = new List<int>() { 2,3, 4, 5 };
            ////List<int> c = a.Where(s=>b.Exists(l=>l==s)).ToList();
            List<int> c=a.Union(b).ToList();
            ////List<int> c = a.FindAll(s => b.Exists(l => l == s)).ToList();
            //Console.Write(c.Count);
            
            c.ForEach(s =>
            {
                Console.Write(s);
                Console.Write("\n");
            });
            //List<string> list = new List<string>() { "123","ddd","eee"};
            //RedisHelper.Set("l1", list);
            //List<string> l1 = RedisHelper.Get<List<string>>("l1");
            //if (l1.Any())
            //{
            //    list.ForEach(s => {
            //        Console.Write(s);
            //    });
            //}
            Console.ReadKey();
        }

    }
}
