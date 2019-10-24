using System;

namespace AspNetAdvantix.Model
{
    public partial class SalesOrderMonitoring
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public DateTime DocDate { get; set; }
        public int DaysDue { get; set; }
        public string SOType { get; set; }
        public string BPName { get; set; }
    }
}