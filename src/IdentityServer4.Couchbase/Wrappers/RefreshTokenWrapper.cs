using Couchbase.Linq.Filters;
using IdentityServer4.Core.Models;

namespace IdentityServer4.Couchbase.Wrappers
{
    [DocumentTypeFilter(nameof(RefreshToken))]
    public class RefreshTokenWrapper
    {
        public RefreshTokenWrapper(string id, RefreshToken model) : this()
        {
            Id = id;
            Model = model;
        }

        public RefreshTokenWrapper()
        {
            Type = nameof(RefreshToken);
        }

        public string Id { get; set; }

        public RefreshToken Model { get; set; }

        public string Type { get; set; }
    }
}
