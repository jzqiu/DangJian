//------------------------------------------------------------------------------
// <auto-generated>
//    此代码是根据模板生成的。
//
//    手动更改此文件可能会导致应用程序中发生异常行为。
//    如果重新生成代码，则将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace DangJian.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class QuotaRecord
    {
        public string GUID { get; set; }
        public string QuotaCode { get; set; }
        public string Info { get; set; }
        public string Remark { get; set; }
        public string CreateUser { get; set; }
        public Nullable<int> CreateYear { get; set; }
        public string CreateDate { get; set; }
        public string DepartmentCode { get; set; }
    
        public virtual Quota Quota { get; set; }
    }
}
