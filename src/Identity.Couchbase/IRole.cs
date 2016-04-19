using System.Collections.Generic;
using System.Security.Claims;
using Newtonsoft.Json;

namespace Identity.Couchbase
{
    public abstract class IRole
    {
        protected IRole()
        {
            Claims = new List<Claim>();
        }

        public string RoleId { get; set; }
        public string NormalizedName { get; set; }
        public string Name { get; set; }
        public string ConcurrencyStamp { get; set; }

        [JsonConverter(typeof(ClaimConverter))]
        public ICollection<Claim> Claims { get; set; }
    }
}