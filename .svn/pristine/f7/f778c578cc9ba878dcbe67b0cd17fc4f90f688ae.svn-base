using System.Collections.Generic;
using DAL;
using FactoryClass;
using IDAL;
using Models;

namespace BLL
{
    public class CompanyBLL : BaseDAL<Company>
    {
        private readonly ICompany dal = DataAccess.CreateCompany();
        public List<Company> GetSonCompalyList(int companyId)
        {
            return dal.GetSonCompalyList(companyId);
        }
    }
}
