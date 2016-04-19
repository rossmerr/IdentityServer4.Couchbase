using System;
using System.Collections.Generic;
using System.Security.Claims;
using Newtonsoft.Json;

namespace Identity.Couchbase
{
    public abstract class IUser
    {
        protected IUser()
        {
            Claims = new List<Claim>();
            Roles = new List<IRole>();
            Logins = new List<UserLogin>();
            SubjectId = Guid.NewGuid().ToString();
        }

        public string SubjectId { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string NormalizedUserName { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string ConcurrencyStamp { get; set; }
        public ICollection<IRole> Roles { get; set; }
        public ICollection<UserLogin> Logins { get; set; }

        [JsonConverter(typeof(ClaimConverter))]
        public ICollection<Claim> Claims { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public int AccessFailedCount { get; set; }
        public bool LockoutEnabled { get; set; }
    }
}
