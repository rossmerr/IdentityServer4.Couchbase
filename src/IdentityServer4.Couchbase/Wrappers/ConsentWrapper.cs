using Couchbase.Linq.Filters;
using IdentityServer4.Core.Models;

namespace IdentityServer4.Couchbase.Wrappers
{
    [DocumentTypeFilter("consent")]
    public class ConsentWrapper : CouchbaseWrapper<Consent>
    {
        public ConsentWrapper(string id, Consent model) : base(id, model)
        {
        }
    }
}
