//------------------------------------------------------------------------------
// <auto-generated>
//    此代码是根据模板生成的。
//
//    手动更改此文件可能会导致应用程序中发生异常行为。
//    如果重新生成代码，则将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class CustomerMaterial
    {
        public int Id { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public string RegionName { get; set; }
        public Nullable<int> RegionId { get; set; }
        public Nullable<int> ProvinceId { get; set; }
        public Nullable<int> CityId { get; set; }
        public Nullable<int> BigTypeId { get; set; }
        public Nullable<int> SmallTypeId { get; set; }
        public string CustomerMaterialName { get; set; }
        public Nullable<decimal> CostPrice { get; set; }
        public Nullable<decimal> SalePrice { get; set; }
        public Nullable<System.DateTime> AddDate { get; set; }
        public Nullable<int> AddUserId { get; set; }
        public Nullable<bool> IsDelete { get; set; }
    }
}
