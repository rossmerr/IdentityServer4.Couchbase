using System;
using System.Linq;
using Couchbase.Linq.Filters;
using IdentityServer4.Core.Models;
using IdentityServer4.Couchbase.Services;

namespace IdentityServer4.Couchbase.Filters
{
    public class AuthorizationCodeFilter : IDocumentFilter<CouchbaseWrapper<AuthorizationCode>>
    {
        public int Priority { get; set; }

        public IQueryable<CouchbaseWrapper<AuthorizationCode>> ApplyFilter(IQueryable<CouchbaseWrapper<AuthorizationCode>> source)
        {
            return source.Where(p => p.Discriminator == typeof(AuthorizationCode).Name.ToLower());
        }
    }
}
