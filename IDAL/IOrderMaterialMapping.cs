﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace IDAL
{
    public interface IOrderMaterialMapping
    {
        DataSet GetDataList(string whereStr);
    }
}
