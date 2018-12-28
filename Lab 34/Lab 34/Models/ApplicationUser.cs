using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Lab_34.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName {get;set;}
        public string MiddleName {get;set;}
        public string LastName {get;set;}
    }
}
