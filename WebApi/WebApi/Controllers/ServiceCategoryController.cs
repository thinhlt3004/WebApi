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
    public class ServiceCategoryController : ControllerBase
    {
        Project3Context _ctx;

        public ServiceCategoryController(Project3Context ctx)
        {
            _ctx = ctx;
        }

        [HttpGet]
        public async Task<ActionResult<List<ServiceCategory>>> GetAll()
        {
            return Ok(await _ctx.ServiceCategories.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceCategory>> GetById(int id)
        {
            return Ok(await _ctx.ServiceCategories.SingleOrDefaultAsync(i => i.Id == id));
        }

        [Authorize(Roles = "Employee, Admin")]
        [HttpPost]
        public async Task<ActionResult> PostSC(ServiceCategory sc)
        {
            _ctx.ServiceCategories.Add(sc);
            await _ctx.SaveChangesAsync();
            var current = await _ctx.ServiceCategories.SingleOrDefaultAsync(i => i.CaterogoryName == sc.CaterogoryName);
            return Ok(current);
        }

        [Authorize(Roles = "Employee, Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult> PutSC(int id,ServiceCategory sc)
        {
            if(id != sc.Id)
            {
                return BadRequest();
            }
            _ctx.Entry<ServiceCategory>(sc).State = EntityState.Modified;
            var result = await _ctx.SaveChangesAsync();
            if(result > 0)
            {
                var current = await _ctx.ServiceCategories.SingleOrDefaultAsync(i => i.Id == id);
                return Ok(current);
            }
            else
            {
                return NotFound();
            }
        }

        [Authorize(Roles = "Employee, Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteSC(int id)
        {
            var sc = await _ctx.ServiceCategories.SingleOrDefaultAsync(i => i.Id == id);
            if(sc != null)
            {
                _ctx.ServiceCategories.Remove(sc);
                await _ctx.SaveChangesAsync();
                return Ok();
            }
            return NotFound();
        }
    }
}
