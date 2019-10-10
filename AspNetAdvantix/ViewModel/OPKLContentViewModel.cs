using System;

namespace AspNetAdvantix.ViewModel
{
    public class OPKLContentViewModel
    {
        public int OrderEntry { get; set; }
        public int OrderLine { get; set; }
        public string BaseObjectType { get; set; }
        public decimal RelQtty { get; set; }
    }
}