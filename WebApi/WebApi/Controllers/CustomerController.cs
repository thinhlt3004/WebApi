using Microsoft.AspNetCore.Authorization;
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

        //[Consumes("application/x-www-form-urlencoded")]
        [Authorize(Roles = "Admin, Employee")]
        [HttpGet]
        public async Task<ActionResult<List<Customer>>> GetAll()
        {
            return Ok(await _ctx.Customers.ToListAsync());
        }

        [Authorize(Roles = "Admin, Employee")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetByID(int id)
        {
            return Ok(await _ctx.Customers.SingleOrDefaultAsync(i => i.Id == id));
        }
    }
}
