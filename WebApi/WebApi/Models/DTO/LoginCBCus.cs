using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models.DTO
{
    public class LoginCBCus
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime Birthday { get; set; }
        public bool Gender { get; set; }
        public string PasswordHash { get; set; }
        public bool? EmailConfirm { get; set; }
        public string ConfirmToken { get; set; }
        public string AccessToken { get; set; }
    }
}
