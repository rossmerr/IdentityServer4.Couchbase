using System;
using System.Linq;
using Couchbase.Linq.Filters;
using IdentityServer4.Core.Models;
using IdentityServer4.Couchbase.Services;

namespace IdentityServer4.Couchbase.Filters
{
    public class ScopeFilter : IDocumentFilter<CouchbaseWrapper<Scope>>
    {
        public int Priority { get; set; }

        public IQueryable<CouchbaseWrapper<Scope>> ApplyFilter(IQueryable<CouchbaseWrapper<Scope>> source)
        {
            return source.Where(p => p.Discriminator == typeof(Scope).Name.ToLower());
        }
    }
}
