using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AspNetAdvantix.Model
{
    public partial class AdvantixContext : DbContext
    {
        public AdvantixContext(DbContextOptions<AdvantixContext> options)
            :base(options)
            {

            }
        public virtual DbQuery<OpenSalesOrder> OpenSalesOrder { get; set; }
        public virtual DbQuery<PickedSalesOrder> PickedSalesOrder { get; set; }
        public virtual DbQuery<Picklist> Picklist { get; set; }
        public virtual DbQuery<PickListNo> PickListNo { get; set; }
        public virtual DbQuery<SalesOrderMonitoring> SalesOrderMonitoring { get; set; }
        public virtual DbQuery<SalesOrderMonitoringDetails> SalesOrderMonitoringDetails { get; set; }
    }
}