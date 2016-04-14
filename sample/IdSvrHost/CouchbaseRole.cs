using System.Collections.Generic;
using System.Security.Claims;
using Identity.Couchbase;

namespace IdSvrHost
{
    public class CouchbaseRole : IRole
    {
        public CouchbaseRole()
        {
            Claims = new List<Claim>();
        }

        public string RoleId { get; set; }
        public string NormalizedName { get; set; }
        public string Name { get; set; }
        public string ConcurrencyStamp { get; set; }
        public ICollection<Claim> Claims { get; set; }
    }
}
