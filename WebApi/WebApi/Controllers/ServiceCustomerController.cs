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

        [Authorize]
        [HttpGet("get-current-service-by-id/{serviceCusId}")]
        public async Task<ActionResult<ServiceCustomer>> GetByServiceCusId(int serviceCusId)
        {
            return Ok(await _ctx.ServiceCustomers.SingleOrDefaultAsync(i => i.Id == serviceCusId));
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
       

        [Authorize(Roles = "Admin, Employee")]
        [HttpGet("get-report-profit")]
        public async Task<ActionResult> GetProfit()
        {
            int cYear = DateTime.Now.Year - 1;
            var report = await _ctx
                .ServiceCustomers
                .Where(i => i.StartDate.Value.Year == cYear)
                .GroupBy(i => i.StartDate.Value.Month)
                .Select(i => new { month = i.Key, totalProfit = i.Sum(v => v.CurrentPrice) })
                .ToListAsync();
            return Ok(report);
        }
    }
}
