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
    
    public partial class Quota
    {
        public Quota()
        {
            this.QuotaRecord = new HashSet<QuotaRecord>();
        }
    
        public string Code { get; set; }
        public string Description { get; set; }
        public Nullable<int> Seq { get; set; }
        public Nullable<int> Value { get; set; }
        public string Standard { get; set; }
        public string IsNeed { get; set; }
        public string GroupCode { get; set; }
        public string ShowType { get; set; }
    
        public virtual QuotaGroup QuotaGroup { get; set; }
        public virtual ICollection<QuotaRecord> QuotaRecord { get; set; }
    }
}
