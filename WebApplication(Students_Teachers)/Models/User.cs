using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using WebApplication_Students_Teachers_.Infrastructure.Identity;

namespace WebApplication_Students_Teachers_.Models
{
    public enum Role
    {
        Student = 0,
        Teacher
    }
    public class User : IdentityUser
    {
        public string Name { get; set; }
        public ICollection<TimeTable> TimeTables { get; set; }

        internal async Task<ClaimsIdentity> GenerateUserIdentityAsync(ApplicationUserManager userManager)
        {
            var userIdentity = await userManager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            return userIdentity;
        }
    }
}