using System;

namespace AspNetAdvantix.Model
{
    public partial class PickedSalesOrder
    {
        public string Type { get; set; }
        public int DocNum { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string BPName { get; set; }
        public string SOType { get; set; }
        public string WhseBranch { get; set; }
        public string ItemCode { get; set; }
        public decimal PickedQty { get; set; }
        public decimal QtyToPost { get; set; }
        public int PLNo { get; set; }
        public DateTime? PLDate { get; set; }
        public string PLRemarks { get; set; }
        public string PLUser { get; set; }
        public string Dscription { get; set; }
        public decimal InStock { get; set; }
        public string WhsCode { get; set; }
        public int DocEntry { get; set; }
        public int LineNum { get; set; }
        public string ObjType { get; set; }
    }
}