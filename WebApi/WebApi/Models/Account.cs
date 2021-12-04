using System;
using System.Collections.Generic;

#nullable disable

namespace WebApi.Models
{
    public partial class Account
    {
        public Account()
        {
            EmpOfCustomers = new HashSet<EmpOfCustomer>();
        }

        public int EmployeeId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public bool EmailConfirm { get; set; }
        public int RoleId { get; set; }
        public string PasswordHash { get; set; }
        public string PhoneNumber { get; set; }
        public string ConfirmToken { get; set; }
        public string Department { get; set; }
        public string Image { get; set; }

        public virtual Department DepartmentNavigation { get; set; }
        public virtual Role Role { get; set; }
        public virtual ICollection<EmpOfCustomer> EmpOfCustomers { get; set; }
    }
}
