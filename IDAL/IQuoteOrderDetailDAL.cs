using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IDAL
{
    public interface IQuoteOrderDetailDAL
    {
        void UpdateRate(string subjectId,decimal rate);
    }
}
