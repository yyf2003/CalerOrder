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
    
    public partial class Attachment
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public Nullable<int> SubjectId { get; set; }
        public Nullable<int> SecItemId { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; }
        public string FileCode { get; set; }
        public string SmallImgUrl { get; set; }
        public Nullable<int> AddUserId { get; set; }
        public Nullable<System.DateTime> AddDate { get; set; }
        public Nullable<bool> IsDelete { get; set; }
    }
}
