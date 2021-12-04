using System;
using System.Collections.Generic;

#nullable disable

namespace WebApi.Models
{
    public partial class RequestCustomer
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public string ServiceId { get; set; }
        public int? TotalDate { get; set; }
        public double? TotalPrice { get; set; }
        public DateTime? Date { get; set; }

        public virtual Service Service { get; set; }
    }
}
