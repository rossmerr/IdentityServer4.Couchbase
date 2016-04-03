using System;
using System.Collections.Generic;
using System.Security.Claims;
using Identity.Couchbase;

namespace Mvc.Models
{
    public class ApplicationRole : IRole
    {
        public string RoleId { get; set; }
        public string NormalizedName { get; set; }
        public string Name { get; set; }
        public ICollection<Claim> Claims { get; set; }
        public string ConcurrencyStamp { get; set; }
    }
}
