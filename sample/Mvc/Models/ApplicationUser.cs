using System;
using System.Collections.Generic;
using System.Security.Claims;
using Identity.Couchbase;

namespace Mvc.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IIdentityUser
    {
        public string Id { get; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string NormalizedUserName { get; set; }
        public string PasswordHash { get; set; }
        public ICollection<IIdentityRole> Roles { get; }
        public ICollection<IdentityUserLogin> Logins { get; }
        public string ConcurrencyStamp { get; set; }
        public ICollection<Claim> Claims { get; set; }
    }
}
