using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HGS_LoginSystem.Models
{
    public class User
    {
        
            public string UserID { get; set; }
            public string UserName { get; set; }
            public DateTime DateOfBirth { get; set; }
            public string Email { get; set; }
            public string Gender { get; set; }
            public string Status { get; set; }
            public string PasswordHash { get; set; }
        
    }
}