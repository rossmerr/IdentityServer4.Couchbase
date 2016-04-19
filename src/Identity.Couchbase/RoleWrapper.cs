using System;
using Couchbase.Linq.Filters;

namespace Identity.Couchbase
{
    [DocumentTypeFilter("role")]
    class RoleWrapper<TRole> where TRole : IRole
    {
        public RoleWrapper()
        {
            Type = "role";
        }

        public RoleWrapper(TRole role) : this()
        {
            Role = role;
        }

        public TRole Role { get; set; }
        public string Type { get; set; }
        public Guid Subject { get; set; }
    }
}