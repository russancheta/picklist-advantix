using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetAdvantix.Model
{
    public class ResultResponse
    {
        public string Result { get; set; }
        public string Message { get; set; }
        public object ResultData { get; set; }
    }
}