
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

        [Authorize(Roles = "Admin, Employee")]
        [HttpPost]
        public async Task<ActionResult> PostCustomer(Customer c)
        {
            var hashPassword = BCrypt.Net.BCrypt.HashPassword("123456");
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
                                  $"<p>Your Password : 123456</p>" +
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

    }
}
