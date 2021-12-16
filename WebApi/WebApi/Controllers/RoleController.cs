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
    public class RoleController : ControllerBase
    {
        Project3Context _ctx;

        public RoleController(Project3Context ctx)
        {
            _ctx = ctx;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<List<Role>>> GetAll()
        {
            return Ok(await _ctx.Roles.ToListAsync());
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Role>> GetByID(int id)
        {
            return Ok(await _ctx.Roles.SingleOrDefaultAsync(i => i.Id == id));
        }
    }
}
