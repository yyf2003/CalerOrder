using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Models;

namespace IDAL
{
    public interface ICompany
    {
        List<Company> GetSonCompalyList(int companyId);

    }
}
