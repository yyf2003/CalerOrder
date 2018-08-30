using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Common;
using IDAL;
using System.Reflection;

namespace FactoryClass
{
    public class DataAccess
    {
        private static readonly string assemlyPath = ConfigurationManager.AppSettings["SQLDAL"];

        public static object CreateObject(string assemblyPath1, string className)
        {
            object obj = DataCache.GetCache(className);
            if (obj == null)
            {
                try
                {
                    obj = Assembly.Load(assemblyPath1).CreateInstance(className);
                    DataCache.SetCache(className, obj);
                }
                catch(Exception ex) { }
            }
            return obj;
        }

        public static IMergeOriginalOrder CreateMergeOriginalOrder()
        {
            string className = assemlyPath + ".MergeOriginalOrderDAL";
            return (IMergeOriginalOrder)CreateObject(assemlyPath,className);
        }

        public static IFinalOrderDetailTemp CreateFinalOrderDetailTemp()
        {
            string className = assemlyPath + ".FinalOrderDetailTempDAL";
            return (IFinalOrderDetailTemp)CreateObject(assemlyPath, className);
        }

        public static ICompany CreateCompany()
        {
            string className = assemlyPath + ".CompanyDAL";
            return (ICompany)CreateObject(assemlyPath, className);
        }

        public static IModule CreateModule()
        {
            string className = assemlyPath + ".ModuleDAL";
            return (IModule)CreateObject(assemlyPath, className);
        }

        public static ICheckOrderResult CreateCheckOrderResult()
        {
            string className = assemlyPath + ".CheckOrderResultDAL";
            return (ICheckOrderResult)CreateObject(assemlyPath, className);
        }

        public static IShop CreateShop()
        {
            string className = assemlyPath + ".ShopDAL";
            return (IShop)CreateObject(assemlyPath, className);
        }

        public static IListOrder CreateListOrder()
        {
            string className = assemlyPath + ".ListOrderDAL";
            return (IListOrder)CreateObject(assemlyPath, className);
        }

        public static IPOPOrder CreatePOPOrder()
        {
            string className = assemlyPath + ".POPOrderDAL";
            return (IPOPOrder)CreateObject(assemlyPath, className);
        }

        public static ISupplementOrder CreateSupplementOrder()
        {
            string className = assemlyPath + ".SupplementOrderDAL";
            return (ISupplementOrder)CreateObject(assemlyPath, className);
        }

        public static IShopMachineFrame CreateShopMachineFrame()
        {
            string className = assemlyPath + ".ShopMachineFrameDAL";
            return (IShopMachineFrame)CreateObject(assemlyPath, className);
        }

        public static IPlace CreatePlace()
        {
            string className = assemlyPath + ".PlaceDAL";
            return (IPlace)CreateObject(assemlyPath, className);
        }

        public static IBasicMaterialDAL CreateBasicMaterial()
        {
            string className = assemlyPath + ".BasicMaterialDAL";
            return (IBasicMaterialDAL)CreateObject(assemlyPath, className);
        }

        public static ICustomerMaterialInfoDAL CreateCustomerMaterialInfo()
        {
            string className = assemlyPath + ".CustomerMaterialInfoDAL";
            return (ICustomerMaterialInfoDAL)CreateObject(assemlyPath, className);
        }

        public static IOrderMaterialMapping CreateOrderMaterialMapping()
        {
            string className = assemlyPath + ".OrderMaterialMappingDAL";
            return (IOrderMaterialMapping)CreateObject(assemlyPath, className);
        }

        public static IQuoteMaterialDAL CreateQuoteMaterial()
        {
            string className = assemlyPath + ".QuoteMaterialDAL";
            return (IQuoteMaterialDAL)CreateObject(assemlyPath, className);
        }

        public static IBasicMachineFrameDAL CreateBasicMachineFrame()
        {
            string className = assemlyPath + ".BasicMachineFrameDAL";
            return (IBasicMachineFrameDAL)CreateObject(assemlyPath, className);
        }

        public static IQuoteOrderDetailDAL CreateQuoteOrderDetail()
        {
            string className = assemlyPath + ".QuoteOrderDetailDAL";
            return (IQuoteOrderDetailDAL)CreateObject(assemlyPath, className);
        }

        public static IOutsourceOrderDetailDAL CreateOutsourceOrderDetail()
        {
            string className = assemlyPath + ".OutsourceOrderDetailDAL";
            return (IOutsourceOrderDetailDAL)CreateObject(assemlyPath, className);
        }
    }
}
