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
    public class ServiceCustomerController : ControllerBase
    {
        Project3Context _ctx;

        public ServiceCustomerController(Project3Context ctx)
        {
            _ctx = ctx;
        }

        [Authorize(Roles = "Admin, Employee")]
        [HttpGet]
        public async Task<ActionResult<List<ServiceCustomer>>> GetAll()
        {
            return Ok(await _ctx.ServiceCustomers.ToListAsync());
        }

        [Authorize]
        [HttpGet("{cusId}")]
        public async Task<ActionResult<List<ServiceCustomer>>> GetByCustomerID(int cusId)
        {
            return Ok(await _ctx.ServiceCustomers.Where(i => i.CustomerId == cusId).OrderByDescending(x => x.Id).ToListAsync());
        } 

        [Authorize(Roles = "User")]
        [HttpPost]
        public async Task<ActionResult> PostCreate(ServiceCustomer sc)
        {
            sc.EmployeeHandle = false;
            _ctx.ServiceCustomers.Add(sc);
            var result = await _ctx.SaveChangesAsync();
            if(result > 0)
            {
                return Ok();
            }
            return BadRequest();
        }

    }
}
