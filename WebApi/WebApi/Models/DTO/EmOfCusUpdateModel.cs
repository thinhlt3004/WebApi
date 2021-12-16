using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models.DTO
{
    public class EmOfCusUpdateModel
    {
        public int ServiceOfCus { get; set; }
        public DateTime date { get; set; }
        public int count { get; set; }
        public double totalPrice { get; set; }
    }
}
