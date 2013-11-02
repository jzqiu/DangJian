using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DangJian.ViewModels
{
    public class QuotaRecordVM
    {
        public string QuotaCode { get; set; }
        public string Description { get; set; }
        public string FillInfo { get; set; }
        public int? Value { get; set; }
        public bool IsNeed { get; set; }
    }
}