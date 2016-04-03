using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Identity.Couchbase
{
    public abstract class IUser
    {
        internal const string Type = "user";

        public string Id { get; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string NormalizedUserName { get; set; }
        public string PasswordHash { get; set; }
        public string ConcurrencyStamp { get; set; }
        public ICollection<IRole> Roles { get; set; }
        public ICollection<UserLogin> Logins { get; set; }
        public ICollection<Claim> Claims { get; set; }
        public string Discriminator => Type;
    }
}
