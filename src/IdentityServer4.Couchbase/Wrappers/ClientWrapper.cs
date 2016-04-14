using Couchbase.Linq.Filters;
using IdentityServer4.Core.Models;

namespace IdentityServer4.Couchbase.Wrappers
{
    [DocumentTypeFilter(nameof(Client))]
    public class ClientWrapper
    {
        public ClientWrapper(string id, Client model) : this()
        {
            Id = id;
            Model = model;
        }

        public ClientWrapper()
        {
            Type = nameof(Client);
        }

        public string Id { get; set; }

        public Client Model { get; set; }

        public string Type { get; set; }
    }

}
