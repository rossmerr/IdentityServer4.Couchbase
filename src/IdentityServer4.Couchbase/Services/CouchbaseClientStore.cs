using System.Linq;
using System.Threading.Tasks;
using Couchbase.Linq;
using IdentityServer4.Core.Models;
using IdentityServer4.Core.Services;
using IdentityServer4.Couchbase.Wrappers;

namespace IdentityServer4.Couchbase.Services
{
    /// <summary>
    /// Couchbase client store
    /// </summary>
    public class CouchbaseClientStore : IClientStore
    {
        readonly IBucketContext _context;
        
        public CouchbaseClientStore(IBucketContext context)
        {
            _context = context;
        }


        /// <summary>
        /// Finds a client by id
        /// </summary>
        /// <param name="clientId">The client id</param>
        /// <returns>
        /// The client  
        /// </returns>
        public Task<Client> FindClientByIdAsync(string clientId)
        {
            var query =
                from client in _context.Query<ClientWrapper>()
                where client.Model.ClientId == clientId && client.Model.Enabled
                select client.Model;
            
            return Task.FromResult(query.SingleOrDefault());
        }
    }
}