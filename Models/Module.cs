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
    
    public partial class Module
    {
        public int Id { get; set; }
        public Nullable<int> ParentId { get; set; }
        public string ModuleName { get; set; }
        public Nullable<int> OrderNum { get; set; }
        public string Url { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public Nullable<bool> IsLeaf { get; set; }
        public string ImgUrl { get; set; }
        public Nullable<bool> IsShowOnHome { get; set; }
        public Nullable<bool> IsShow { get; set; }
    }
}
