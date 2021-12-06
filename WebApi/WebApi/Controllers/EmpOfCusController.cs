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
    public class EmpOfCusController : ControllerBase
    {
        Project3Context _ctx;

        public EmpOfCusController(Project3Context ctx)
        {
            _ctx = ctx;
        }
        //Get All Employee supporting Customer Service Paid
        [Authorize(Roles = "Admin, Employee")]
        [HttpGet("{serviceCusId}")]
        public async Task<ActionResult<List<EmpOfCustomer>>> Get(int serviceCusId)
        {
            var cus = await _ctx.EmpOfCustomers.Where(i => i.ServiceOfCus == serviceCusId).ToListAsync();
            return Ok(cus);
        }

        //Get Current Employee who is supporting customer service Paid
        [Authorize]
        [HttpGet("Customer/{serviceCusId}")]
        public async Task<ActionResult<List<EmpOfCustomer>>> GetForCus(int serviceCusId)
        {
            var cus = await _ctx.EmpOfCustomers.Where(i => i.ServiceOfCus == serviceCusId && i.Status == true).SingleOrDefaultAsync();
            return Ok(cus);
        }

        //Create new 
        [Authorize(Roles = "Admin, Employee")]
        [HttpPost]
        public async Task<ActionResult> Create(EmpOfCustomer en)
        {
            var serCus = await _ctx.ServiceCustomers.SingleOrDefaultAsync(i => i.Id == en.ServiceOfCus);
            serCus.EmployeeHandle = true;
            await _ctx.SaveChangesAsync();
            en.Status = true;
            _ctx.EmpOfCustomers.Add(en);
            await _ctx.SaveChangesAsync();
            return Ok();
        }


        [Authorize(Roles = "Admin, Employee")]
        [HttpPut]
        public async Task<ActionResult> Put(EmpOfCustomer en)
        {
            var empOfCus = await _ctx.EmpOfCustomers.SingleOrDefaultAsync(i => i.ServiceOfCus == en.ServiceOfCus && i.Status == true);
            empOfCus.Status = false;
            await _ctx.SaveChangesAsync();
            en.Status = true;
            _ctx.EmpOfCustomers.Add(en);
            await _ctx.SaveChangesAsync();
            return Ok();
        }



    }
}
