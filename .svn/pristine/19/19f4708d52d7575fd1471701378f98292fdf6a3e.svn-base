using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models
{
    [Serializable]
    public class LoginUser
    {
        public int UserId { get; set; }
        public string LoginName { get; set; }
        public string UserName { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string BaseCode { get; set; }
        //所负责客户ID
        public string CustomerIds { get; set; }
        //所负责客户名称
        public string CustomerNames { get; set; }

        public List<UserCustomer> Customers { get; set; }
        public int? UserLevelId { get; set; }
    }
    [Serializable]
    public class UserCustomer
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
    }
}
