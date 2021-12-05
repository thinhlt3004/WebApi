using System;
using System.Collections.Generic;

#nullable disable

namespace WebApi.Models
{
    public partial class Department
    {
        public Department()
        {
            Accounts = new HashSet<Account>();
        }

        public string Id { get; set; }
        public string DepartmentName { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Account> Accounts { get; set; }
    }
}
