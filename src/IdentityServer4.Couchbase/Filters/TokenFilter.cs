using System;
using System.Linq;
using Couchbase.Linq.Filters;
using IdentityServer4.Core.Models;
using IdentityServer4.Couchbase.Services;

namespace IdentityServer4.Couchbase.Filters
{
    public class TokenFilter : IDocumentFilter<CouchbaseWrapper<Token>>
    {
        public int Priority { get; set; }

        public IQueryable<CouchbaseWrapper<Token>> ApplyFilter(IQueryable<CouchbaseWrapper<Token>> source)
        {
            return source.Where(p => p.Discriminator == typeof(Token).Name.ToLower());
        }
    }
}
