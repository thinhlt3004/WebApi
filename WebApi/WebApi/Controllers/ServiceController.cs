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
            return await _ctx.Services.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Service>> GetByID(string id)
        {
            return Ok(await _ctx.Services.SingleOrDefaultAsync(i => i.Id == id));
        }
        //Get Service by service Category

        [HttpGet("get-service-by-cate/{cateId}")]
        public async Task<ActionResult<List<Service>>> GetListServices(int cateId)
        {
            return await _ctx.Services.Where(i => i.ServiceCategoryId == cateId).ToListAsync();
        }


        [Authorize(Roles = "Admin, Employee")]
        [HttpPost]
        public async Task<ActionResult> PostService(Service s)
        {
            _ctx.Services.Add(s);
            await _ctx.SaveChangesAsync();
            return Ok();
        }

        [Authorize(Roles = "Admin, Employee")]
        [HttpPut("{id}")]
        public async Task<ActionResult> PutService(string id, Service s)
        {
            if(id != s.Id)
            {
                return BadRequest("Id is not matched");
            }
            _ctx.Entry<Service>(s).State = EntityState.Modified;
            var result = await _ctx.SaveChangesAsync();
            if(result > 0)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }
        //Admin , Employee, User
        [Authorize(Roles = "Admin, Employee")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteService(string id)
        {
            var ser = await _ctx.Services.SingleOrDefaultAsync(i => i.Id == id);
            if(ser != null)
            {
                _ctx.Services.Remove(ser);
                await _ctx.SaveChangesAsync();
                return Ok();
            }
            return NotFound();
        }
    }
}
