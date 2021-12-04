using System;
using System.Collections.Generic;

#nullable disable

namespace WebApi.Models
{
    public partial class ServiceCategory
    {
        public ServiceCategory()
        {
            Services = new HashSet<Service>();
        }

        public int Id { get; set; }
        public string CaterogoryName { get; set; }

        public virtual ICollection<Service> Services { get; set; }
    }
}
