using System;
using System.Collections.Generic;

#nullable disable

namespace WebApi.Models
{
    public partial class EmpOfCustomer
    {
        public int Id { get; set; }
        public string EmpId { get; set; }
        public int ServiceOfCus { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public virtual ServiceCustomer ServiceOfCus1 { get; set; }
        public virtual Account ServiceOfCusNavigation { get; set; }
    }
}
