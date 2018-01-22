using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DAL;
using Models;

namespace BLL
{
    public class InstallBLL : BaseDAL<Install>
    {
        public decimal GetOtherInstallPrice(int guidanceId, int shopId)
        {
            decimal price = 0;
            var model = GetList(s => s.GuidanceId == guidanceId && s.ShopId == shopId).FirstOrDefault();
            if (model != null)
            {
                price = model.OtherPrice ?? 0;
            }
            return price;
        }
    }
}
