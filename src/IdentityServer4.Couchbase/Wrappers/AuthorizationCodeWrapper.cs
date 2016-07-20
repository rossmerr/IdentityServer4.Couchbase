using Couchbase.Linq.Filters;
using IdentityServer4.Models;

namespace IdentityServer4.Couchbase.Wrappers
{
    [DocumentTypeFilter(nameof(AuthorizationCode))]
    public class AuthorizationCodeWrapper 
    {
        public static string AuthorizationCodeId(string id)
        {
            return $"AuthorizationCode:{id}".ToLowerInvariant();
        }

        public AuthorizationCodeWrapper(string id, AuthorizationCode model) : this()
        {
            Id = AuthorizationCodeId(id);
            Model = model;
        }
        
        public AuthorizationCodeWrapper()
        {
            Type = nameof(AuthorizationCode);
        }

        public string Id { get; set; }

        public AuthorizationCode Model { get; set; }

        public string Type { get; set; }
    }
}
