using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetAdvantix.ViewModel
{
    using FluentValidation.AspNetCore;
    using AspNetAdvantix.ViewModel.Validations;

    public class CredentialsViewModel
    {
        public string Address { get; set; }
        public string DBName { get; set; }
        public string UserName { get; set; }
        public string Password {get; set; }
    }
}