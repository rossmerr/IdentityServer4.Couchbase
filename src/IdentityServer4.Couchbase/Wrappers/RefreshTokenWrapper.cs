using Couchbase.Linq.Filters;
using IdentityServer4.Core.Models;

namespace IdentityServer4.Couchbase.Wrappers
{
    [DocumentTypeFilter("refreshToken")]
    public class RefreshTokenWrapper : CouchbaseWrapper<RefreshToken>
    {
        public RefreshTokenWrapper(string id, RefreshToken model) : base(id, model)
        {
        }
    }
}
