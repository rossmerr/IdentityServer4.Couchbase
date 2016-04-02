using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Identity.Couchbase
{
    public interface IIdentityUser
    {
        string Id { get; }
        string Email { get; set; }
        string UserName { get; set; }
        string NormalizedUserName { get; set; }
        string PasswordHash { get; set; }
        ICollection<IIdentityRole> Roles { get; }
        ICollection<IdentityUserLogin> Logins { get; }
        string ConcurrencyStamp { get; set; }

        ICollection<Claim> Claims { get; set; }
    }
}
