using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        Project3Context _ctx;

        public CustomerController(Project3Context ctx)
        {
            _ctx = ctx;
        }

        [HttpGet]
        public async Task<List<Customer>> GetAll()
        {
            return await _ctx.Customers.ToListAsync();
        }
    }
}
