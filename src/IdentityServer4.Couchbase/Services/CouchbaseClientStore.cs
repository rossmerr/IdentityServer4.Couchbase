using System.Linq;
using System.Threading.Tasks;
using Couchbase.Core;
using Couchbase.Linq;
using IdentityServer4.Couchbase.Wrappers;
using IdentityServer4.Models;
using IdentityServer4.Services;

namespace IdentityServer4.Couchbase.Services
{
    public interface ICouchbaseClientStore : IClientStore
    {
        Task StoreClientAsync(Client client);
    }

    /// <summary>
    /// Couchbase client store
    /// </summary>
    public class CouchbaseClientStore : ICouchbaseClientStore
    {
        readonly IBucketContext _context;
        readonly IBucket _bucket;

        public CouchbaseClientStore(IBucketContext context, IBucket bucket)
        {
            _context = context;
            _bucket = bucket;
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
            var id = ClientWrapper.ClientWrapperId(clientId);
            var query =
                from client in _context.Query<ClientWrapper>()
                where client.Id == id && client.Model.Enabled
                select client.Model;

            var first = query.SingleOrDefault();
            return Task.FromResult(first);
        }

        public Task StoreClientAsync(Client client)
        {            
            return _bucket.InsertAsync(ClientWrapper.ClientWrapperId(client.ClientId), new ClientWrapper(client.ClientId, client));
        }
    }
}