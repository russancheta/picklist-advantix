using System;
using System.Collections.Generic;

namespace AspNetAdvantix.ViewModel
{
    public class OWTRContentViewModel
    {
        public string ItemCode { get; set; }
        public double Quantity { get; set; }
        //public string WhsCode { get; set; }
        public int BaseEntry { get; set; }
        public int BaseLine { get; set; }
        public int BaseType { get; set; }
    }
}