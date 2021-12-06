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
        }

        public string Id { get; set; }
        public int? CustomerId { get; set; }
        public string ServiceId { get; set; }
        public DateTime? StartDate { get; set; }
        public double? CurrentPrice { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? EmployeeHandle { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Service Service { get; set; }
        public virtual ICollection<EmpOfCustomer> EmpOfCustomers { get; set; }
    }
}
