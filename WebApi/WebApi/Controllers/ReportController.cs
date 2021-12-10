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
            var current = await _ctx.Reports.SingleOrDefaultAsync(i => i.Date.Value == r.Date.Value);
            if(current != null)
            {
                return BadRequest("Today is had report");
            }
            _ctx.Reports.Add(r);           
            await _ctx.SaveChangesAsync();
            return Ok();
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
        [HttpGet("get-current-month")]
        public async Task<ActionResult> GetCurrentMonth()
        {
            int cMonth = DateTime.Now.Month;
            var result = await _ctx.Reports.Where(i => i.Date.Value.Month == cMonth).ToListAsync();
            return Ok(result);
        }

        [Authorize]
        [HttpGet("get-last-month")]
        public async Task<ActionResult> GetLastMonth()
        {
            int cMonth = DateTime.Now.Month - 1;
            var result = await _ctx.Reports.Where(i => i.Date.Value.Month == cMonth).ToListAsync();
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
    }
}
