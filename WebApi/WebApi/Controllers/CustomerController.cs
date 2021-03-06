
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
    public class CustomerController : ControllerBase
    {
        Project3Context _ctx;
        IEmailSender _email;
        IConfiguration _config;

        public CustomerController(Project3Context ctx, IEmailSender email, IConfiguration config)
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



        //[Consumes("application/x-www-form-urlencoded")]
        [Authorize(Roles = "Admin, Employee")]
        [HttpGet]
        public async Task<ActionResult<List<Customer>>> GetAll()
        {
            return Ok(await _ctx.Customers.ToListAsync());
        }

        [Authorize(Roles = "Admin, Employee")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetByID(int id)
        {
            return Ok(await _ctx.Customers.SingleOrDefaultAsync(i => i.Id == id));
        }

        //[Authorize(Roles = "Admin, Employee")]
        [HttpPost]
        public async Task<ActionResult> PostCustomer(Customer c)
        {
            var hashPassword = BCrypt.Net.BCrypt.HashPassword(c.PasswordHash);
            string token = Guid.NewGuid().ToString();

            c.ConfirmToken = token;
            c.EmailConfirm = false;
            c.PasswordHash = hashPassword;
            _ctx.Customers.Add(c);
            var result = await _ctx.SaveChangesAsync();
            if (result > 0)
            {
                var current = await _ctx.Customers.SingleOrDefaultAsync(i => i.Email == c.Email);
                string url = @"http://localhost:3000/confirm/" + token;
                string template = $"<h1>Dear Mr/Mrs {current.FullName}</h1> " +
                                  $"<p>Thanks for using our services. Your account is below :</p>" +
                                  $"<p>Your Email : {current.Email}</p>" +
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
            var current = await _ctx.Customers.SingleOrDefaultAsync(i => i.ConfirmToken.Contains(token));
            if(current != null)
            {
                current.EmailConfirm = true;
                await _ctx.SaveChangesAsync();
                return Ok();
            }
            return NotFound("Cant find customer");
        }

        [Authorize(Roles = "User")]
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
                if (ms < expTime)
                {
                    var id = claims.FirstOrDefault(c => c.Type == "Id").Value;
                    var user = await _ctx.Customers.SingleOrDefaultAsync(c => c.Id == int.Parse(id));
                    return Ok(user);
                }
                else
                {
                    return BadRequest("Expire Time !");
                }
            }
            return BadRequest("You are not authenticated");
        }


        [HttpPost("Login")]
        public async Task<ActionResult> PostLogin(LoginModel u)
        {
            var cus = await _ctx.Customers.SingleOrDefaultAsync(i => i.Email == u.Email);
            if(cus != null && BCrypt.Net.BCrypt.Verify(u.password, cus.PasswordHash))
            {
                string accessToken = GetToken(cus.Id.ToString(), cus.Email, "User");
                LoginCBCus loginResult = new LoginCBCus
                {
                    Id = cus.Id,
                    FullName = cus.FullName,
                    Email = cus.Email,
                    PhoneNumber = cus.PhoneNumber,
                    Birthday = cus.Birthday,
                    Gender = cus.Gender,
                    EmailConfirm = cus.EmailConfirm,
                    ConfirmToken = cus.ConfirmToken,
                    PasswordHash = cus.PasswordHash,
                    AccessToken = accessToken
                };
                return Ok(loginResult);
            }
            return BadRequest("Email or password is not valid !");
        }


        [Authorize(Roles = "Admin")]
        [HttpDelete("{cusId}")]
        public async Task<ActionResult> DeleteCustomer(int cusId)
        {
            var cus = await _ctx.Customers.SingleOrDefaultAsync(i => i.Id == cusId);
            if(cus != null)
            {
                _ctx.Customers.Remove(cus);
                await _ctx.SaveChangesAsync();
                return Ok();
            }
            return NotFound();
        }

        [Authorize(Roles = "Employee, Admin, User")]
        [HttpPatch("update-password/{cusId}/{password}")]
        public async Task<ActionResult> UpdatePassword(int cusId, string password)
        {
            var cus = await _ctx.Customers.SingleOrDefaultAsync(i => i.Id == cusId);
            if(cus != null)
            {
                var hashPassword = BCrypt.Net.BCrypt.HashPassword(password);
                cus.PasswordHash = hashPassword;
                await _ctx.SaveChangesAsync();
                return Ok(cus);
            }
            return NotFound();
        }

        [Authorize(Roles = "Employee, Admin, User")]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAccount(int id, Customer c)
        {
            if (id != c.Id)
            {
                return BadRequest("Id is not matched !");
            }
             var cusCheck = await _ctx.Customers.Where(x => x.Email == c.Email).SingleOrDefaultAsync();
            if (cusCheck != null)
            {
                if(cusCheck.Id != id)
                {

                return BadRequest("Email is existed !");
                }
            }
            var current = await _ctx.Customers.SingleOrDefaultAsync(i => i.Id == id);
            current.FullName = c.FullName;
            current.Gender = c.Gender;
            current.Email = c.Email;
            current.PhoneNumber = c.PhoneNumber;
            current.Birthday = c.Birthday;
            var result = await _ctx.SaveChangesAsync();
            if(result > 0)
            {
                var cus = await _ctx.Customers.SingleOrDefaultAsync(i => i.Id == id);
                return Ok(cus);
            }
            return NotFound();
        }
    }
}
