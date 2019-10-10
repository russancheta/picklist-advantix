using System;

namespace AspNetAdvantix
{
    public partial class ReleaseToPick
    {
        public int DocNum { get; set; }
        public string CardName { get; set; }
        public string ItemCode { get; set; }
        public decimal Quantity { get; set; }
        public decimal QuantityToPost { get; set; }
    }
}