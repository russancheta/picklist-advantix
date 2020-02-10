using System;

namespace AspNetAdvantix.Model
{
    public partial class SalesOrderMonitoring
    {
        public string Type { get; set; }
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public DateTime DocDate { get; set; }
        public int DaysDue { get; set; }
        public string SOType { get; set; }
        public string BPName { get; set; }
        public string Remarks { get; set; }
    }
}