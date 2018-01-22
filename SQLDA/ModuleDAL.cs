using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Models;
using IDAL;
using System.Data.SqlClient;
using System.Data;

namespace SQLDAL
{
    public class ModuleDAL:IModule
    {
        public List<Module> GetModulesById(int id)
        {
            List<Module> list = new List<Module>();
            SqlParameter[] paras = new SqlParameter[] { 
               new SqlParameter("@moduleId1",id),
               
            };
            DataSet ds = SQLHelper.RunProcedure("pro_GetModulesById", paras, "modules");
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                Module model;
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    model = new Module();
                    if (!string.IsNullOrWhiteSpace(dr["Id"].ToString()))
                    {
                        model.Id = int.Parse(dr["Id"].ToString());
                    }
                    if (!string.IsNullOrWhiteSpace(dr["ParentId"].ToString()))
                    {
                        model.ParentId = int.Parse(dr["ParentId"].ToString());
                    }
                    if (!string.IsNullOrWhiteSpace(dr["ModuleName"].ToString()))
                    {
                        model.ModuleName = dr["ModuleName"].ToString();
                    }
                    if (!string.IsNullOrWhiteSpace(dr["OrderNum"].ToString()))
                    {
                        model.OrderNum = int.Parse(dr["OrderNum"].ToString());
                    }

                    if (!string.IsNullOrWhiteSpace(dr["Url"].ToString()))
                    {
                        model.Url = dr["Url"].ToString();
                    }
                    if (!string.IsNullOrWhiteSpace(dr["IsDelete"].ToString()))
                    {
                        model.IsDelete = bool.Parse(dr["IsDelete"].ToString());
                    }
                    if (!string.IsNullOrWhiteSpace(dr["ImgUrl"].ToString()))
                    {
                        model.ImgUrl = dr["ImgUrl"].ToString();
                    }
                    if (!string.IsNullOrWhiteSpace(dr["IsShowOnHome"].ToString()))
                    {
                        model.IsShowOnHome = bool.Parse(dr["IsShowOnHome"].ToString());
                    }
                    
                    list.Add(model);
                }
            }
            return list;
        }
    }
}
