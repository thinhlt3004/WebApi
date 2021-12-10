using Microsoft.AspNetCore.Authorization;
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
using WebApi.Models.DTO;

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
        [NonAction]
        private string GetToken(string id, string email, string role)
        {
            var claims = new[]
                   {
                        new Claim(JwtRegisteredClaimNames.Sub, _config["Jwt:Subject"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToString()),
                        new Claim("Id", id),
                        new Claim("roles", role),
                        new Claim("Email", email)
                    };
            //Get Key of Jwt in appsetings.json and bytes it
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            //Create token
            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"]
                , _config["Jwt:Audience"]
                , claims
                , expires: DateTime.Now.AddDays(3)
                , signingCredentials: signIn
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        [Authorize(Roles = "Employee, Admin")]
        [HttpGet]
        public async Task<ActionResult<List<Account>>> Get()
        {
            return Ok(await _ctx.Accounts.ToListAsync());
        }

        [Authorize(Roles = "Employee, Admin, User")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Account>> Get(string id)
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

        //Comment line below to generate new account , change role to admin 
        [Authorize(Roles = "Admin")]
        [HttpPost("Create")]
        public async Task<ActionResult> PostCreate(Account c)
        {
            
            var hashPassword = BCrypt.Net.BCrypt.HashPassword("111111");
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
                                  $"<p>Your Email : {current.Email}</p>" +
                                  $"<p>Your Password : 111111</p>" +
                                  $"<p>Have a nice day.</p>" +
                                  $"<a href='{HtmlEncoder.Default.Encode(url)}'>Click here to confirm your account</a>";
                await _email.SendEmailAsync(current.Email, "Confirm Account", template);
                return Ok();
            }
            return BadRequest();
        }

        [HttpPost("Login")]
        public async Task<ActionResult> PostLogin(LoginModel u)
        {
            var current = await _ctx.Accounts.SingleOrDefaultAsync(i => i.Email == u.Email);
            if(current != null && BCrypt.Net.BCrypt.Verify(u.password, current.PasswordHash))
            {
                var role = await _ctx.Roles.SingleOrDefaultAsync(i => i.Id == current.RoleId);
                string accessToken = GetToken(current.EmployeeId, current.Email, role.Role1);
                var loginResult = new LoginCB
                {
                    EmployeeId = current.EmployeeId,
                    UserName = current.UserName,
                    FullName = current.FullName,
                    Email = current.Email,
                    EmailConfirm = current.EmailConfirm,
                    RoleId = current.RoleId,
                    PasswordHash = current.PasswordHash,
                    PhoneNumber = current.PhoneNumber,
                    ConfirmToken = current.ConfirmToken,
                    Department = current.Department,
                    Image = current.Image,
                    AccessToken = accessToken
                };
                return Ok(loginResult);
            }
            return BadRequest("Email or password is not valid !");
        }
        //It will get all informations of token
        [Authorize(Roles = "Employee, Admin")]
        [HttpGet("GetUserByToken")]
        public async Task<ActionResult> GetUserByToken()
        {
            var claimsIdentity = HttpContext.User.Identity as ClaimsIdentity;
            if (claimsIdentity.Claims.Count() != 0)
            {
                IEnumerable<Claim> claims = claimsIdentity.Claims;
                var expTime = long.Parse(claims.FirstOrDefault(c => c.Type == "exp").Value) * 1000;
                DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                long ms = (long)(DateTime.UtcNow - epoch).TotalMilliseconds;
                //var expTime = claims.FirstOrDefault(c => c.Type == "exp");
                if(ms < expTime)
                {
                    var id = claims.FirstOrDefault(c => c.Type == "Id").Value;
                    var user = await _ctx.Accounts.SingleOrDefaultAsync(c => c.EmployeeId.Equals(id));
                    return Ok(user);
                }
                else
                {
                    return BadRequest("Expire Time !");
                }
            }
            return BadRequest("You are not authenticated");
        }


        [HttpPatch("{token}")]
        public async Task<ActionResult> ConfirmAccount(string token)
        {
            var acc = await _ctx.Accounts.SingleOrDefaultAsync(i => i.ConfirmToken == token);
            if (acc != null)
            {
                acc.EmailConfirm = true;
                await _ctx.SaveChangesAsync();
                var updatedAcc = await _ctx.Accounts.SingleOrDefaultAsync(i => i.ConfirmToken == token);
                return Ok(updatedAcc);
            }
            return BadRequest();
        }

        [Authorize(Roles = "Employee, Admin")]
        [HttpPatch("update-password/{empId}/{password}")]
        public async Task<ActionResult> UpdatePassword(string empId, string password)
        {
            var emp = await _ctx.Accounts.SingleOrDefaultAsync(i => i.EmployeeId == empId);
            if(emp != null)
            {
                var hashPassword = BCrypt.Net.BCrypt.HashPassword(password);
                emp.PasswordHash = hashPassword;
                await _ctx.SaveChangesAsync();
                return Ok(emp);
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
        [Authorize(Roles = "Admin, Employee")]
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
        //If not right role => will return statusCode =  403
        [Authorize(Roles = "Admin")]
        [HttpDelete("{empId}")]
        public async Task<ActionResult> DeleteAccount(string empId)
        {
            var emp = await _ctx.Accounts.SingleOrDefaultAsync(i => i.EmployeeId == empId);
            if(emp != null)
            {
                _ctx.Accounts.Remove(emp);
                await _ctx.SaveChangesAsync();
                return Ok();
            }
            return NotFound();
        }
    }
}
