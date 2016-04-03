using System;
using System.Linq;
using Couchbase.Linq.Filters;
using IdentityServer4.Core.Models;
using IdentityServer4.Couchbase.Services;

namespace IdentityServer4.Couchbase.Filters
{
    public class RefreshTokenFilter : IDocumentFilter<CouchbaseWrapper<RefreshToken>>
    {
        public int Priority { get; set; }

        public IQueryable<CouchbaseWrapper<RefreshToken>> ApplyFilter(IQueryable<CouchbaseWrapper<RefreshToken>> source)
        {
            return source.Where(p => p.Discriminator == typeof(RefreshToken).Name.ToLower());
        }
    }
}
