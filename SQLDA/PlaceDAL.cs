using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using IDAL;
using Models;



namespace SQLDAL
{
    public class PlaceDAL:IPlace
    {
        #region IPlace 成员

        public int Insert(Place model)
        {
            string sql = "insert into Place(ID,PlaceName,ParentID,AreaSort,AreaDeep,AreaRegion) values(@ID,@PlaceName,@ParentID,@AreaSort,@AreaDeep,@AreaRegion);select @@identity;";
            SqlParameter[] para = new SqlParameter[] { 
               new SqlParameter("@ID",model.ID),
               new SqlParameter("@PlaceName",model.PlaceName),
               new SqlParameter("@ParentID",model.ParentID),
               new SqlParameter("@AreaSort",model.AreaSort),
               new SqlParameter("@AreaDeep",model.AreaDeep),
               new SqlParameter("@AreaRegion",model.AreaRegion)
            };
            return SQLHelper.ExecuteSql(sql, para);
        }

        #endregion
    }
}
