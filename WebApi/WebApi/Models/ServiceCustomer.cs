using System;
using System.Collections.Generic;

#nullable disable

namespace WebApi.Models
{
    public partial class ServiceCustomer
    {
        public ServiceCustomer()
        {
            EmpOfCustomers = new HashSet<EmpOfCustomer>();
            Reports = new HashSet<Report>();
        }

        public int Id { get; set; }
        public int? CustomerId { get; set; }
        public string ServiceId { get; set; }
        public DateTime? StartDate { get; set; }
        public double? CurrentPrice { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? EmployeeHandle { get; set; }
        public string Product { get; set; }
        public double? ProductPrice { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Service Service { get; set; }
        public virtual ICollection<EmpOfCustomer> EmpOfCustomers { get; set; }
        public virtual ICollection<Report> Reports { get; set; }
    }
}
