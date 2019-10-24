using System;

namespace AspNetAdvantix.Model
{
    public partial class UpdateSalesOrder
    {
        public int DocEntry { get; set; }
        public int LineNum { get; set; }
        public string ItemCode { get; set; }
        public decimal PickedQty { get; set; }
        public int PLNo { get; set; }
        public string Remarks { get; set; }
        public DateTime Date { get; set; }

        public string ObjType { get; set; }
    }
}