using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Models;
using System.Runtime.Remoting.Messaging;

namespace DAL
{
    public class CurrentContext
    {
        public static KalerOrderDBEntities DbContext
        {
            get
            {
                KalerOrderDBEntities dbcontext = CallContext.GetData("dbContext") as KalerOrderDBEntities;
                if (dbcontext == null)
                {
                    dbcontext = new KalerOrderDBEntities();
                    CallContext.SetData("dbContext", dbcontext);
                }
                return dbcontext;
            }
        }
    }
}
