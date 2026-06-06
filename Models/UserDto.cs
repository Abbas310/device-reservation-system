using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vanrise_Web.Models
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
    }
}