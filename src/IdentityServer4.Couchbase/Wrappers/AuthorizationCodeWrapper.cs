using Couchbase.Linq.Filters;
using IdentityServer4.Core.Models;

namespace IdentityServer4.Couchbase.Wrappers
{
    [DocumentTypeFilter("authorizationCode")]
    public class AuthorizationCodeWrapper : CouchbaseWrapper<AuthorizationCode>
    {
        public AuthorizationCodeWrapper(string id, AuthorizationCode model) : base(id, model)
        {
        }
    }
}
