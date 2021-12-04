using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace WebApi.Models
{
    [Table("Account")]
    public partial class Account
    {      
        public Account()
        {
            EmpOfCustomers = new HashSet<EmpOfCustomer>();
        }
        [Key]
        public string EmployeeId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public bool? EmailConfirm { get; set; }
        [ForeignKey("RoleId")]
        public int RoleId { get; set; }
        public string PasswordHash { get; set; }
        public string PhoneNumber { get; set; }
        public string ConfirmToken { get; set; }
        [ForeignKey("Department")]
        public string Department { get; set; }
        public string Image { get; set; }

        public virtual Department DepartmentNavigation { get; set; }
        public virtual Role Role { get; set; }
        public virtual ICollection<EmpOfCustomer> EmpOfCustomers { get; set; }
    }
}
