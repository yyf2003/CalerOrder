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
    
    public partial class Quotations
    {
        public int Id { get; set; }
        public Nullable<int> SubjectId { get; set; }
        public string CRNumber { get; set; }
        public string AdidasContact { get; set; }
        public string ProjectContact { get; set; }
        public string Category { get; set; }
        public string Blongs { get; set; }
        public string Classification { get; set; }
        public string TaxRate { get; set; }
        public Nullable<decimal> OfferPrice { get; set; }
        public string Account { get; set; }
        public string Remark { get; set; }
        public Nullable<System.DateTime> AccountCheckDate { get; set; }
        public Nullable<System.DateTime> QuotationsDate { get; set; }
        public string Invoices { get; set; }
        public Nullable<int> AddUserId { get; set; }
        public Nullable<System.DateTime> AddDate { get; set; }
        public Nullable<decimal> OtherPrice { get; set; }
        public string OtherPriceRemark { get; set; }
    }
}
