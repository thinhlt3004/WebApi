using System;
using System.Collections.Generic;

#nullable disable

namespace WebApi.Models
{
    public partial class Service
    {
        public Service()
        {
            RequestCustomers = new HashSet<RequestCustomer>();
            ServiceCustomers = new HashSet<ServiceCustomer>();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int? ServiceCategoryId { get; set; }
        public string Image { get; set; }

        public virtual ServiceCategory ServiceCategory { get; set; }
        public virtual ICollection<RequestCustomer> RequestCustomers { get; set; }
        public virtual ICollection<ServiceCustomer> ServiceCustomers { get; set; }
    }
}
