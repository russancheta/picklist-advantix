using System;
using System.Collections.Generic;

namespace AspNetAdvantix.ViewModel
{
    public class TransactionContentViewModel
    {
        public string ItemCode { get; set; }
        public decimal Quantity { get; set; }
        public string WhsCode { get; set; }
    }
}