using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiServices.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }  
        public string Password { get; set; }
        public string Fname { get; set; }
        public string Lname { get; set; }
        //public string Profile { get; set; }
        public string Token { get; set; }
        public string Message { get; set; } 
    }
}
