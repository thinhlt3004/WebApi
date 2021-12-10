using System;
using System.Collections.Generic;

#nullable disable

namespace WebApi.Models
{
    public partial class Report
    {
        public int Id { get; set; }
        public int? ServiceOfCus { get; set; }
        public DateTime? Date { get; set; }
        public int? Count { get; set; }
        public double? TotalPrice { get; set; }

        public virtual ServiceCustomer ServiceOfCusNavigation { get; set; }
    }
}
