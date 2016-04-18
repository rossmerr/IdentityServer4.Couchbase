using Couchbase.Linq.Filters;
using IdentityServer4.Core.Models;

namespace IdentityServer4.Couchbase.Wrappers
{
    [DocumentTypeFilter(nameof(Scope))]
    public class ScopeWrapper
    {
        public static string ScopeWrapperId(string id)
        {
            return $"ScopeWrapper:{id}".ToLowerInvariant();
        }

        public ScopeWrapper(string id, Scope model) : this()
        {
            Id = id;
            Model = model;
        }

        public ScopeWrapper()
        {
            Type = nameof(Scope);
        }

        public string Id { get; set; }

        public Scope Model { get; set; }

        public string Type { get; set; }
    }
}
