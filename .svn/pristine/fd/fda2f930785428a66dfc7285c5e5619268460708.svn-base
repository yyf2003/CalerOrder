using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DAL;
using Models;

namespace BLL
{
    public class UserBLL : BaseDAL<UserInfo>
    {
        /// <summary>
        /// 检查重复
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="userId"></param>
        /// <returns>True:存在重复，false:不重复</returns>
        public bool CheckExist(string userName, int userId)
        {
            var list = GetList(s=>s.UserName==userName);
            if (userId > 0)
            {
                list = list.Where(s=>s.UserId!=userId).ToList();
            }
            return list.Any();
        }
    }
}
