using System;
using System.Linq;
using Couchbase.Linq.Filters;
using IdentityServer4.Core.Models;
using IdentityServer4.Couchbase.Services;

namespace IdentityServer4.Couchbase.Filters
{
    public class ClientFilter : IDocumentFilter<CouchbaseWrapper<Client>>
    {
        public int Priority { get; set; }

        public IQueryable<CouchbaseWrapper<Client>> ApplyFilter(IQueryable<CouchbaseWrapper<Client>> source)
        {
            return source.Where(p => p.Discriminator == typeof(Client).Name.ToLower());
        }
    }
}
