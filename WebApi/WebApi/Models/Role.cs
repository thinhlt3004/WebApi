using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace WebApi.Models
{
    [Table("Roles")]
    public partial class Role
    {
        public Role()
        {
            Accounts = new HashSet<Account>();
        }
        [Key]
        public int Id { get; set; }
        public string Role1 { get; set; }

        public virtual ICollection<Account> Accounts { get; set; }
    }
}
