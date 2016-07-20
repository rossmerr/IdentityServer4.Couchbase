using Couchbase.Linq.Filters;
using System.ComponentModel.DataAnnotations;
using IdentityServer4.Models;
using Newtonsoft.Json;

namespace IdentityServer4.Couchbase.Wrappers
{
    [DocumentTypeFilter(nameof(Token))]
    public class TokenWrapper 
    {
        public static string TokenWrapperId(string id)
        {
            return $"Token:{id}".ToLowerInvariant();
        }

        public TokenWrapper()
        {
            Type = nameof(Token);
        }

        public string Id { get; set; }

        public Token Model { get; set; }

        public string Type { get; set; }
    }
}
