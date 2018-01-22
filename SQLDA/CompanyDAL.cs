using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IDAL;
using Models;
using System.Data.SqlClient;
using System.Data;

namespace SQLDAL
{
    public class CompanyDAL:ICompany
    {
        public List<Company> GetSonCompalyList(int companyId)
        {
            List<Company> list = new List<Company>();
            SqlParameter[] paras = new SqlParameter[] { 
               new SqlParameter("@CompanyId",companyId),
               
            };
            DataSet ds = SQLHelper.RunProcedure("pro_GetMySonCompany", paras, "companys");
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                Company model;
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    model = new Company();
                    if (!string.IsNullOrWhiteSpace(dr["Id"].ToString()))
                    { 
                       model.Id=int.Parse(dr["Id"].ToString());
                    }
                    if (!string.IsNullOrWhiteSpace(dr["ParentId"].ToString()))
                    {
                        model.ParentId = int.Parse(dr["ParentId"].ToString());
                    }
                    if (!string.IsNullOrWhiteSpace(dr["CompanyName"].ToString()))
                    {
                        model.CompanyName = dr["CompanyName"].ToString();
                    }
                    if (!string.IsNullOrWhiteSpace(dr["BaseCode"].ToString()))
                    {
                        model.BaseCode = dr["BaseCode"].ToString();
                    }
                    list.Add(model);
                }
            }
            return list;
        }
    }
}
