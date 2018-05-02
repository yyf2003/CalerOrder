using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;


namespace Common
{
    public class HttpModelClass:IHttpModule
    {
        #region IHttpModule 成员

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        public void Init(HttpApplication context)
        {
            //context.BeginRequest += new EventHandler(context_BeginRequest);
            context.PreRequestHandlerExecute += new EventHandler(context_PreRequestHandlerExecute);
        }

        void context_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            HttpApplication application = sender as HttpApplication;
            HttpRequest request = application.Request;
            string url = request.FilePath;
            //string url = application.Context.Request.Url.ToString();
            if (url.Contains(".aspx"))
            {
                if (!url.Contains("/Statistics"))
                {
                    try
                    {
                        if (application.Context.Session["shopTypeSelected"] != null)
                            application.Context.Session["shopTypeSelected"] = null;
                        if (application.Context.Session["userSelected"] != null)
                            application.Context.Session["userSelected"] = null;
                        if (application.Context.Session["subjectCategorySelected"] != null)
                            application.Context.Session["subjectCategorySelected"] = null;
                        if (application.Context.Session["priceSubjectSelected"] != null)
                            application.Context.Session["priceSubjectSelected"] = null;
                        if (application.Context.Session["subjectSelected"] != null)
                            application.Context.Session["subjectSelected"] = null;
                        if (application.Context.Session["secondInstallSubjectSelected"] != null)
                            application.Context.Session["secondInstallSubjectSelected"] = null;
                        if (application.Context.Session["provinceSelected"] != null)
                            application.Context.Session["provinceSelected"] = null;
                        if (application.Context.Session["citySelected"] != null)
                            application.Context.Session["citySelected"] = null;
                        //if (application.Context.Session["orderDetailStatistics"] != null)
                            //application.Context.Session["orderDetailStatistics"] = null;
                        if (application.Context.Session["shopStatistics"] != null)
                            application.Context.Session["shopStatistics"] = null;
                        if (application.Context.Session["subjectStatistics"] != null)
                            application.Context.Session["subjectStatistics"] = null;
                        if (application.Context.Session["guidanceStatistics"] != null)
                            application.Context.Session["guidanceStatistics"] = null;


                        //if (application.Context.Session["installPriceDicStatistics"] != null)
                        //    application.Context.Session["installPriceDicStatistics"] = null;
                        //if (application.Context.Session["expressPriceDicStatistics"] != null)
                        //    application.Context.Session["expressPriceDicStatistics"] = null;
                        //if (application.Context.Session["materialPriceDicStatistics"] != null)
                        //    application.Context.Session["materialPriceDicStatistics"] = null;
                        //if (application.Context.Session["freightDicStatistics"] != null)
                        //    application.Context.Session["freightDicStatistics"] = null;

                        //if (application.Context.Session["secondInstallPriceDicStatistics"] != null)
                        //    application.Context.Session["secondInstallPriceDicStatistics"] = null;
                        //if (application.Context.Session["secondExperssDicStatistics"] != null)
                            //application.Context.Session["secondExperssDicStatistics"] = null;
                       
                    }
                    catch { }
                }
                if (!url.Contains("/Customer"))
                {
                    try
                    {
                        if (application.Context.Session["shopList"] != null)
                            application.Context.Session["shopList"] = null;
                    }
                    catch { }
                }
                if (!url.Contains("Subjects/OutsourceMaterialOrder/AddSubject.aspx"))
                {
                    try
                    {
                        if (application.Context.Session["OutsourceMaterial"] != null)
                            application.Context.Session["OutsourceMaterial"] = null;
                    }
                    catch { }
                }
                //if (!url.Contains("Subjects/InstallPrice/SubmitInstallPrice.aspx"))
                //{
                //    try
                //    {
                //        if (application.Context.Session["orderDetailInstallPrice"] != null)
                //            application.Context.Session["orderDetailInstallPrice"] = null;
                //        if (application.Context.Session["shopInstallPrice"] != null)
                //            application.Context.Session["shopInstallPrice"] = null;
                //        if (application.Context.Session["subjectInstallPrice"] != null)
                //            application.Context.Session["subjectInstallPrice"] = null;
                //        if (application.Context.Session["assginShopIdInstallPrice"] != null)
                //            application.Context.Session["assginShopIdInstallPrice"] = null;
                //    }
                //    catch { }
                //}
            }
            if (url.Contains(".ashx"))
            {
                if (!url.Contains("AssignOrder.ashx"))
                {
                    try
                    {
                        if (application.Context.Session["orderAssign"] != null)
                            application.Context.Session["orderAssign"] = null;
                        if (application.Context.Session["shopAssign"] != null)
                            application.Context.Session["shopAssign"] = null;
                    }
                    catch { }
                }
            }
        }

        

        #endregion

       
    }
}
