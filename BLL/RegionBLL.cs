using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DAL;
using Models;

namespace BLL
{
    public class RegionBLL : BaseDAL<Region>
    {
        /// <summary>
        /// 检查区域，如果不存在就添加
        /// </summary>
        /// <param name="regionName"></param>
        public void CheckRegion(string regionName)
        {
            if (!string.IsNullOrWhiteSpace(regionName))
            {
                bool canAdd = true;
                var list = GetList(s => s.Id > 0);
                if (list.Any())
                {
                    list.ForEach(s =>
                    {
                        if (s.RegionName.ToLower() == regionName.ToLower())
                        {
                            canAdd = false;
                        }
                    });
                }
                if (canAdd)
                {
                    Region model = new Region();
                    model.RegionName = regionName;
                    Add(model);
                }
            }
            
        }
    }
}
