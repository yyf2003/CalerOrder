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
    
    public partial class OrderChangeApplicationDetail
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }
        public int SubjectId { get; set; }
        public int ChangeType { get; set; }
        public string Remark { get; set; }
        public Nullable<System.DateTime> ActivityDate { get; set; }
        public Nullable<int> State { get; set; }
        public Nullable<int> FinishUserId { get; set; }
        public Nullable<System.DateTime> FinishDate { get; set; }
        public Nullable<bool> Running { get; set; }
    }
}
