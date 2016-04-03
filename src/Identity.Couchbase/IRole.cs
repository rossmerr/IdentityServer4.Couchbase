using System.Collections.Generic;
using System.Security.Claims;

namespace Identity.Couchbase
{
    public abstract class IRole
    {
        internal const string Type = "role";

        public string RoleId { get; set; }
        public string NormalizedName { get; set; }
        public string Name { get; set; }
        public string ConcurrencyStamp { get; set; }
        public ICollection<Claim> Claims { get; set; }
        public string Discriminator => Type;
    }
}