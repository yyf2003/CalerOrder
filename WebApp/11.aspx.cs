using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp
{
    public partial class _11 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            List<int> list1 = new List<int>() {123,345,1234,555 };
            List<int> list2 = new List<int>() {345,1234,555 };
            list1.AddRange(list2);
            List<int> list3 = list1.Union(list2).ToList();
            int a = 0;
        }
    }
}