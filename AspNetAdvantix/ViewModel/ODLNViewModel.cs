using System;
using System.Collections.Generic;

namespace AspNetAdvantix.ViewModel
{
    public class ODLNViewModel
    {
        public string CardCode { get; set; }
        public string Comments { get; set; }
        public List<ODLNContentViewModel> odlnContent { get; set; }
    }
}