using System;

namespace AspNetAdvantix.Model
{
    public partial class SalesOrderMonitoringDetails
    {
        public int DocNum { get; set; }
        public int PLNo { get; set; }
        public string ItemCode { get; set; }
        public decimal Qty { get; set; }
        public string Remarks { get; set; }
        public DateTime Date { get; set; }
        public string UserName { get; set; }
    }
}