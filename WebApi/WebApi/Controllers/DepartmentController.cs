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
    public class DepartmentController : ControllerBase
    {
        Project3Context _ctx;

        public DepartmentController(Project3Context ctx)
        {
            _ctx = ctx;
        }

        [HttpGet]
        public async Task<ActionResult<List<Department>>> GetAll()
        {
            return await _ctx.Departments.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Department>> GetAll(string id)
        {
            return await _ctx.Departments.SingleOrDefaultAsync(i => i.Id == id);
        }

        [Authorize(Roles = "Employee, Admin")]
        [HttpPost]
        public async Task<ActionResult> PostDepartment(Department d)
        {
            var current = await _ctx.Departments.SingleOrDefaultAsync(i => i.Id == d.Id);
            if(current == null)
            {
                _ctx.Departments.Add(d);
                await _ctx.SaveChangesAsync();
                return Ok();
            }
            return BadRequest("Id is existed !");
        }

        [Authorize(Roles = "Employee, Admin")]
        [HttpDelete("{departmentId}")]
        public async Task<ActionResult> DeleteDepartment(string departmentId)
        {
            var current = await _ctx.Departments.SingleOrDefaultAsync(i => i.Id == departmentId);
            if(current != null)
            {
                _ctx.Departments.Remove(current);
                await _ctx.SaveChangesAsync();
                return Ok();
            }
            return NotFound();
        }

        [Authorize(Roles = "Employee, Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult> PutDepartment(string id,Department d)
        {
            if(id != d.Id)
            {
                return BadRequest();
            }
            _ctx.Entry<Department>(d).State = EntityState.Modified;
            await _ctx.SaveChangesAsync();
            return Ok();
        }

    }
}
