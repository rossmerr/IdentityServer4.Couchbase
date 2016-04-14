using System;
using System.Collections.Generic;
using System.Security.Claims;
using Identity.Couchbase;

namespace IdSvrHost
{
    public class CouchbaseUser : IUser
    {
        public CouchbaseUser()
        {
            Claims = new List<Claim>();
            Roles = new List<IRole>();
            Logins = new List<UserLogin>();
        }

        public string Email { get; set; }
        public string Username { get; set; }
        public string NormalizedUserName { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string ConcurrencyStamp { get; set; }
        public ICollection<IRole> Roles { get; set; }
        public ICollection<UserLogin> Logins { get; set; }
        public ICollection<Claim> Claims { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public int AccessFailedCount { get; set; }
        public bool LockoutEnabled { get; set; }
    }
}
