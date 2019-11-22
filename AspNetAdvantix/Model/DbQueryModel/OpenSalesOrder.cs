using System;

namespace AspNetAdvantix.Model
{
    public partial class OpenSalesOrder
    {
        public string Type { get; set; }
        public int DocNum { get; set; }
        public string PONo { get; set; }
        public DateTime DocDate { get; set; }
        public string SOType { get; set; }
        public string DelStat { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string BPName { get; set; }
        public string WhseBranch { get; set; }
        public string ItemCode { get; set; }
        public string Dscription { get; set; }
        public decimal OpenQty { get; set; }
        public decimal QtyToPost { get; set; }
        public decimal InStock { get; set; }
        public decimal Available { get; set; }
        public string WhsCode { get; set; }
        public string Branch { get; set; }
        public int DocEntry { get; set; }
        public int LineNum { get; set; }
        public string ObjType { get; set; }
        public string UseBaseUnits { get; set; }
    }
}