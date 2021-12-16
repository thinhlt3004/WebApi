using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Models;
using WebApi.Models.DTO;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        Project3Context _ctx;

        public ReportController(Project3Context ctx)
        {
            _ctx = ctx;
        }

        [Authorize()]
        [HttpPost]
        public async Task<ActionResult> PostCreate(Report r)
        {
            var current = await _ctx.Reports.SingleOrDefaultAsync(i => i.Date.Value == r.Date.Value && i.ServiceOfCus == r.ServiceOfCus);
            if(current != null)
            {
                return BadRequest("Today is had report");
            }
            _ctx.Reports.Add(r);           
            await _ctx.SaveChangesAsync();
            var cb = await _ctx.Reports.SingleOrDefaultAsync(i => i.Date.Value == r.Date.Value && i.ServiceOfCus == r.ServiceOfCus);
            return Ok(cb);
        }

        [Authorize]
        //Get By Month
        [HttpGet("get-by-month/{month}")]
        public async Task<ActionResult> GetByMonth(int month)
        {
            var result = await _ctx.Reports.Where(i => i.Date.Value.Month == month).ToListAsync();
            return Ok(result);
        }

        [Authorize]
        [HttpGet("get-current-month/{cusSerID}")]
        public async Task<ActionResult> GetCurrentMonth(int cusSerID)
        {
            int cMonth = DateTime.Now.Month;
            var result = await _ctx.Reports.Where(i => i.ServiceOfCus == cusSerID && i.Date.Value.Month == cMonth).ToListAsync();
            return Ok(result);
        }

        [Authorize]
        [HttpGet("get-last-month/{cusSerID}")]
        public async Task<ActionResult> GetLastMonth(int cusSerID)
        {
            int cMonth = DateTime.Now.Month - 1;
            var result = await _ctx.Reports.Where(i => i.ServiceOfCus == cusSerID && i.Date.Value.Month == cMonth).ToListAsync();
            return Ok(result);
        }

        [Authorize(Roles = "Admin, Employee")]
        [HttpGet("get-by-cus-month/{cusSerID}/{month}")]
        public async Task<ActionResult> GetbyCusMonth(int cusSerID, int month)
        {
            var result = await _ctx.Reports.Where(i => i.ServiceOfCus == cusSerID && i.Date.Value.Month == month).ToListAsync();
            return Ok(result);
        }

        [Authorize]
        [HttpGet("get-amount-by-id/{cusSerID}/{month}")]
        public async Task<ActionResult> GetAmountOfProductByServiceCusId(int cusSerID, int month)
        {
            return Ok(await _ctx.Reports.Where(i => i.ServiceOfCus == cusSerID && i.Date.Value.Month == month).Select(i => new DataChart
            { 
                totalPrice = i.TotalPrice.Value,
                date = i.Date.Value,
                amount = i.Count.Value
            }).ToListAsync());
        }

        [Authorize(Roles = "Admin, Employee")]
        [HttpPut("update-data")]
        public async Task<ActionResult> Update(EmOfCusUpdateModel m)
        {
            var currentReport = await _ctx.Reports.SingleOrDefaultAsync(i => i.ServiceOfCus == m.ServiceOfCus && i.Date.Value == m.date);
            if(currentReport != null)
            {
                currentReport.Count = m.count;
                currentReport.TotalPrice = m.totalPrice;
                await _ctx.SaveChangesAsync();
                return Ok(currentReport);
            }
            return NotFound();
        }
    }
}
