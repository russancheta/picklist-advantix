using System;
using System.Collections.Generic;

namespace AspNetAdvantix.ViewModel
{
    public class TransactionViewModel
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public string CardCode { get; set; }
        public string DocDate { get; set; }
        public string DocDueDate { get; set; }
        public int Series { get; set; }
        public string FromWarehouse { get; set; }
        public string ToWarehouse { get; set; }
        public string ITType { get; set; }
        public string Comments { get; set; }
        public List<TransactionContentViewModel> transactionContent { get; set; }
    }
}