using System;
using System.Linq;
using Couchbase.Linq.Filters;
using IdentityServer4.Core.Models;
using IdentityServer4.Couchbase.Services;

namespace IdentityServer4.Couchbase.Filters
{
    public class ConsentFilter : IDocumentFilter<CouchbaseWrapper<Consent>>
    {
        public int Priority { get; set; }

        public IQueryable<CouchbaseWrapper<Consent>> ApplyFilter(IQueryable<CouchbaseWrapper<Consent>> source)
        {
            return source.Where(p => p.Discriminator == typeof(Consent).Name.ToLower());
        }
    }
}
