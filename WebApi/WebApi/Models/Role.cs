using System;
using System.Collections.Generic;

#nullable disable

namespace WebApi.Models
{
    public partial class Role
    {
        public Role()
        {
            Accounts = new HashSet<Account>();
        }

        public int Id { get; set; }
        public string Role1 { get; set; }

        public virtual ICollection<Account> Accounts { get; set; }
    }
}
