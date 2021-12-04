using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        Project3Context _ctx;
        IEmailSender _email;
        IConfiguration _config;
        public EmployeeController(Project3Context ctx, IEmailSender email, IConfiguration config)
        {
            _ctx = ctx;
            _email = email;
            _config = config;
        }

        [HttpGet]
        public async Task<ActionResult<List<Account>>> Get()
        {
            return Ok(await _ctx.Accounts.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<List<Account>>> Get(string id)
        {
            return Ok(await _ctx.Accounts.SingleOrDefaultAsync(i => i.EmployeeId == id));
        }


        //Demo
        //{
        //"employeeId": "E002",
        //"userName": "taile",
        //"fullName": "Le Van Tai",
        //"email": "tai@gmail.com",
        //"emailConfirm": false,
        //"roleId": 2,
        //"passwordHash": "tai123",
        //"phoneNumber": "0966949379",
        //"confirmToken": "dsnifunuisdbgfiubwe",
        //"department": "D02",
        //"image": ""
        //}
        [HttpPost("Create")]
        public async Task<ActionResult> PostCreate(Account c)
        {
            var hashPassword = BCrypt.Net.BCrypt.HashPassword(c.PasswordHash);
            string token = Guid.NewGuid().ToString();
            c.ConfirmToken = token;
            c.EmailConfirm = false;
            c.PasswordHash = hashPassword;
            _ctx.Accounts.Add(c);
            var result = await _ctx.SaveChangesAsync();
            if (result > 0)
            {
                var current = await _ctx.Accounts.SingleOrDefaultAsync(i => i.Email == c.Email);
                //Change redirect to website from email of employee HERE
                string url = @"http://localhost:3000/confirm/" + token;
                string template = $"<h1>Dear Mr/Mrs {current.FullName}</h1> " +
                                  $"<p>We registed your account. Please confirm it</p>" +
                                   $"<p>Have a nice day.</p>" +
                                  $"<a href='{HtmlEncoder.Default.Encode(url)}'>Click here to confirm your account</a>";
                await _email.SendEmailAsync(current.Email, "Confirm Account", template);
                return Ok();
            }
            return BadRequest();
        }

        [HttpPatch("{token}")]
        public async Task<ActionResult> ConfirmAccount(string token)
        {
            var acc = await _ctx.Accounts.SingleOrDefaultAsync(i => i.ConfirmToken == token);
            if (acc != null)
            {
                acc.EmailConfirm = true;
                await _ctx.SaveChangesAsync();
                return Ok();
            }
            return BadRequest();
        }

        [HttpPatch("update-password/{empId}/{password}")]
        public async Task<ActionResult> UpdatePassword(string empId, string password)
        {
            var emp = await _ctx.Accounts.SingleOrDefaultAsync(i => i.EmployeeId == empId);
            if(emp != null)
            {
                var hashPassword = BCrypt.Net.BCrypt.HashPassword(password);
                emp.PasswordHash = hashPassword;
                await _ctx.SaveChangesAsync();
                return Ok();
            }
            else
            {
                return NotFound();
            }

        }


        //{
        //  "employeeId": "E001",
        //  "userName": "thinh le truong",
        //  "fullName": "truong thinh",
        //  "email": "thinh.lt0496@gmail.com",
        //  "emailConfirm": true,
        //  "roleId": 1,
        //  "passwordHash": "$2a$11$GopIIgH5B0JNT5FkaFK2.uzObn1IEcIad7je0uxoyrtjsPfOS7t8S",
        //  "phoneNumber": "09669751874",
        //  "confirmToken": "ca0ac42b-6d53-477f-9f82-c6f85731a5a8",
        //  "department": "D02",
        //  "image": "thinh.jpg"
        // }
        //Password will handle private , not update Password in here
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAccount(string id, Account c)
        {
            if(id != c.EmployeeId)
            {
                return NotFound();
            }
            _ctx.Entry<Account>(c).State = EntityState.Modified;
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
    }
}
