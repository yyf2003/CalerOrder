using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp
{
    public class MyHttpModel:IHttpModule
    {
        #region IHttpModule 成员

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(context_BeginRequest);
            context.EndRequest += new EventHandler(context_EndRequest);
        }

        

        void context_BeginRequest(object sender, EventArgs e)
        {
            HttpApplication application = sender as HttpApplication;
            HttpRequest request = application.Request;
            string url = request.FilePath;
        }

        void context_EndRequest(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}