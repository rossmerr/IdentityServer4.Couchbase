using Couchbase.Linq.Filters;
using IdentityServer4.Core.Models;

namespace IdentityServer4.Couchbase.Wrappers
{
    [DocumentTypeFilter("token")]
    public class TokenWrapper : CouchbaseWrapper<Token>
    {
        public TokenWrapper(string id, Token model) : base(id, model)
        {
        }
    }
}
