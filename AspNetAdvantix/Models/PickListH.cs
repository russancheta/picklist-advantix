using System;
using System.Collections.Generic;

namespace AspNetAdvantix.Models
{
    public partial class PickListH
    {
        public int DocEntry { get; set; }
        public int? DocNum { get; set; }
        public int? Period { get; set; }
        public short? Instance { get; set; }
        public int? Series { get; set; }
        public string Handwrtten { get; set; }
        public string Canceled { get; set; }
        public string Object { get; set; }
        public int? LogInst { get; set; }
        public int? UserSign { get; set; }
        public string Transfered { get; set; }
        public string Status { get; set; }
        public DateTime? CreateDate { get; set; }
        public short? CreateTime { get; set; }
        public DateTime? UpdateDate { get; set; }
        public short? UpdateTime { get; set; }
        public string DataSource { get; set; }
        public string RequestStatus { get; set; }
        public string Creator { get; set; }
        public string Remark { get; set; }
        public int? USoDocNum { get; set; }
        public int? USoPlno { get; set; }
        public string USoItemCode { get; set; }
        public decimal? USoQty { get; set; }
        public string USoRemarks { get; set; }
        public DateTime? USoDate { get; set; }
        public string USoUserName { get; set; }
        public string UObjType { get; set; }
    }
}
