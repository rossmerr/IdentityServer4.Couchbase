using System;
using System.Linq;
using Couchbase.Linq.Filters;

namespace Identity.Couchbase.Filters
{
    public class RoleFilter<TRole> : IDocumentFilter<TRole> where TRole : IRole, new()
    {
        public int Priority { get; set; }

        public IQueryable<TRole> ApplyFilter(IQueryable<TRole> source)
        {
            return source.Where(p => p.Discriminator == nameof(IRole.Type));
        }
    }
}
