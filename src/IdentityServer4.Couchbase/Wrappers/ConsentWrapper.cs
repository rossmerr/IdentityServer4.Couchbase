using Couchbase.Linq.Filters;
using IdentityServer4.Core.Models;

namespace IdentityServer4.Couchbase.Wrappers
{
    [DocumentTypeFilter(nameof(Consent))]
    public class ConsentWrapper
    {
        public ConsentWrapper(string id, Consent model) : this()
        {
            Id = id;
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
