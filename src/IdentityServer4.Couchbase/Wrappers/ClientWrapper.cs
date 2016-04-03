using Couchbase.Linq.Filters;
using IdentityServer4.Core.Models;

namespace IdentityServer4.Couchbase.Wrappers
{
    [DocumentTypeFilter("client")]
    public class ClientWrapper : CouchbaseWrapper<Client>
    {
        public ClientWrapper(string id, Client model) : base(id, model)
        {
        }
    }

}
