using System;
using System.Collections.Generic;

#nullable disable

namespace WebApi.Models
{
    public partial class Customer
    {
        public Customer()
        {
            ServiceCustomers = new HashSet<ServiceCustomer>();
        }

        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime Birthday { get; set; }
        public bool Gender { get; set; }
        public string PasswordHash { get; set; }

        public virtual ICollection<ServiceCustomer> ServiceCustomers { get; set; }
    }
}
