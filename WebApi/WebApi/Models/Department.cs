using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace WebApi.Models
{
    [Table("Departments")]
    public partial class Department
    {
        public Department()
        {
            Accounts = new HashSet<Account>();
        }
        [Key]
        public string Id { get; set; }
        public string DepartmentName { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Account> Accounts { get; set; }
    }
}
