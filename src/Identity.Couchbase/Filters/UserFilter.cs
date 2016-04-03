using System;
using System.Linq;
using Couchbase.Linq.Filters;

namespace Identity.Couchbase.Filters
{
    public class UserFilter<TUser> : IDocumentFilter<TUser> where TUser : IUser, new()
    {
        public int Priority { get; set; }

        public IQueryable<TUser> ApplyFilter(IQueryable<TUser> source)
        {
            return source.Where(p => p.Discriminator == nameof(IUser.Type));
        }
    }
}
