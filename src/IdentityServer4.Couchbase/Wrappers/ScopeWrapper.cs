using Couchbase.Linq.Filters;
using IdentityServer4.Core.Models;

namespace IdentityServer4.Couchbase.Wrappers
{
    [DocumentTypeFilter("scope")]
    public class ScopeWrapper : CouchbaseWrapper<Scope>
    {
        public ScopeWrapper(string id, Scope model) : base(id, model)
        {
        }
    }
}
