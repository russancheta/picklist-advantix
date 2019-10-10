using System;
using System.Collections.Generic;

namespace AspNetAdvantix.ViewModel
{
    public class OWTRViewModel
    {
        public string CardCode { get; set; }
        public int Series { get; set; }
        public string FromWarehouse { get; set; }
        public string ToWarehouse { get; set; }
        public string ITType { get; set; }
        public string Comments { get; set; }
        public List <OWTRContentViewModel> owtrContent { get; set; }
    }
}