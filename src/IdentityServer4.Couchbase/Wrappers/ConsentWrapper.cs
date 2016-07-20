using Couchbase.Linq.Filters;
using IdentityServer4.Models;

namespace IdentityServer4.Couchbase.Wrappers
{
    [DocumentTypeFilter(nameof(Consent))]
    public class ConsentWrapper
    {
        public static string ConsentWrapperId(string id)
        {
            return $"Consent:{id}".ToLowerInvariant();
        }

        public ConsentWrapper(string id, Consent model) : this()
        {
            Id = ConsentWrapperId(id);
            Model = model;
        }

        public ConsentWrapper()
        {
            Type = nameof(Consent);
        }

        public string Id { get; set; }

        public Consent Model { get; set; }

        public string Type { get; set; }
    }
}
