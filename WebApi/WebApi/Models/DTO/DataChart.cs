using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models.DTO
{
    public class DataChart
    {
        public double totalPrice { get; set; }
        public DateTime date { get; set; }
        public int amount { get; set; }
    }
}
