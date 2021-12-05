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
    public class ServiceController : ControllerBase
    {
        Project3Context _ctx;

        public ServiceController(Project3Context ctx)
        {
            _ctx = ctx;
        }

        [HttpGet]
        public async Task<ActionResult<List<Service>>> GetAll()
        {
            return Ok(await _ctx.Services.ToListAsync());
        }

        [HttpGet("{serviceId}")]
        public async Task<ActionResult<Service>> GetByID(string serviceId)
        {
            return Ok(await _ctx.Services.SingleOrDefaultAsync(i => i.Id == serviceId));
        }

    }
}
