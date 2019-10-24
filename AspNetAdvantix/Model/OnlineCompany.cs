using System;
using SAPbobsCOM;

namespace AspNetAdvantix
{
    public partial class OnlineCompany
    {
        public Company SAPCompany { get; set; }
        public string Token { get; set; }
        public string Database { get; set; }
    }
}