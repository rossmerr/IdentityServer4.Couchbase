using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Identity.Couchbase
{
    public interface IUser
    {
        string Email { get; set; }
        string Username { get; set; }
        string NormalizedUserName { get; set; }
        string PasswordHash { get; set; }
        string SecurityStamp { get; set; }
        string ConcurrencyStamp { get; set; }
        ICollection<IRole> Roles { get; set; }
        ICollection<UserLogin> Logins { get; set; }
        ICollection<Claim> Claims { get; set; }
        DateTimeOffset? LockoutEnd { get; set; }
        int AccessFailedCount { get; set; }
        bool LockoutEnabled { get; set; }
    }
}
