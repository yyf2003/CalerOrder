using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DAL;
using Models;
using FactoryClass;
using IDAL;

namespace BLL
{
    public class PlaceBLL : BaseDAL<Place>
    {
        private readonly IPlace dal = DataAccess.CreatePlace();
        /// <summary>
        /// 检查城市是否正确，正确的返回城市Id和省份ID
        /// </summary>
        /// <param name="cityName"></param>
        /// <param name="cityId"></param>
        /// <param name="provinceId"></param>
        /// <returns></returns>
        public bool CheckCity(string cityName, ref List<Place> placeList, out int cityId, out int provinceId)
        {
            cityId = 0;
            provinceId = 0;
            bool flag = false;
            if (!placeList.Any())
            {
                placeList = GetList(s => s.ID > 0);
            }
            if (placeList.Any())
            {
                var  placeList1 = placeList.Where(s => s.PlaceName == cityName).ToList();
                if (placeList1.Any())
                {
                    cityId = placeList1.First().ID;
                    provinceId = placeList1.First().ParentID ?? 0;
                    flag = true;
                }
            }
            return flag;
        }

        public int Insert(Place model)
        {
            return dal.Insert(model);
        }
    }
}
