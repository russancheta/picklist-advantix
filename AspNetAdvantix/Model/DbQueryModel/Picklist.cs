using System;

namespace AspNetAdvantix.Model
{
    public partial class Picklist
    {
        public int AbsEntry { get; set; }
        public string CardName { get; set; }
        public DateTime ShipDate { get; set; }
        public string ItemCode  { get; set; }
        public string Dscription { get; set; }
        public decimal RelQtty { get; set; }
        public decimal PickQtty { get; set; }
        public decimal OrderedQty { get; set; }
        public string Remarks { get; set; }
        public string SOType { get; set; }
        public string Branch { get; set; }
    }
}