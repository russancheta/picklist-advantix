using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AspNetAdvantix.Model
{
    public partial class PicklistContext : DbContext
    {
        public PicklistContext(DbContextOptions<PicklistContext> options)
            :base(options)
            {

            }
    }
}