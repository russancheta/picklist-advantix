using System;
using System.Collections.Generic;

namespace AspNetAdvantix.ViewModel
{
    public class OPKLViewModel
    {
        public DateTime PickDate { get; set; }
        public string Remarks { get; set; }
        public List<OPKLContentViewModel> opklContent { get; set; }
    }
}