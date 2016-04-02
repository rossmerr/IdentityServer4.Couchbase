using System.Collections.Generic;
using System.Security.Claims;

namespace Identity.Couchbase
{
    public interface IIdentityRole
    {
        string RoleId { get; set; }

        string NormalizedName { get; set; }
        string Name { get; set; }

        ICollection<Claim> Claims { get; set; }
        string ConcurrencyStamp { get; set; }
    }
}