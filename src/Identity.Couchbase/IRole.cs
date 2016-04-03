using System.Collections.Generic;
using System.Security.Claims;
using Couchbase.Linq.Filters;

namespace Identity.Couchbase
{
    [DocumentTypeFilter("role")]
    public abstract class IRole
    {
        public string RoleId { get; set; }
        public string NormalizedName { get; set; }
        public string Name { get; set; }
        public string ConcurrencyStamp { get; set; }
        public ICollection<Claim> Claims { get; set; }
    }
}