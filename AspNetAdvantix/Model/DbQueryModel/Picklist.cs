using System;

namespace AspNetAdvantix.Model
{
    public partial class Picklist
    {
        public int DocNum { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public DateTime ShipDate { get; set; }
        public string ItemCode { get; set; }
        public string Dscription  { get; set; }
    }
}