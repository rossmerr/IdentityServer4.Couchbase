using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Couchbase.Core;
using Couchbase.Linq;
using IdentityServer4.Core.Models;
using IdentityServer4.Core.Services;

namespace IdentityServer4.Couchbase.Services
{
    /// <summary>
    /// Couchbase consent store
    /// </summary>
    public class CouchbaseConsentStore : IConsentStore
    {
        readonly IBucket _bucket;
        readonly IBucketContext _context;

        public CouchbaseConsentStore(IBucket bucket, IBucketContext context)
        {
            _bucket = bucket;
            _context = context;
        }


        /// <summary>
        /// Loads all permissions the subject has granted to all clients.
        /// </summary>
        /// <param name="subject">The subject.</param>
        /// <returns>The permissions.</returns>
        public Task<IEnumerable<Consent>> LoadAllAsync(string subject)
        {
            var query =
                from c in _context.Query<CouchbaseWrapper<Consent>>()
                where c.Model.Subject == subject
                select c.Model;
            return Task.FromResult<IEnumerable<Consent>>(query.ToArray());
        }

        /// <summary>
        /// Loads the subject's prior consent for the client.
        /// </summary>
        /// <param name="subject">The subject.</param>
        /// <param name="client">The client.</param>
        /// <returns>The persisted consent.</returns>
        public Task<Consent> LoadAsync(string subject, string client)
        {
            var query =
                from c in _context.Query<CouchbaseWrapper<Consent>>()
                where c.Model.Subject == subject && c.Model.ClientId == client
                select c.Model;
            return Task.FromResult(query.SingleOrDefault());
        }


        /// <summary>
        /// Persists the subject's consent.
        /// </summary>
        /// <param name="consent">The consent.</param>
        /// <returns></returns>
        public Task UpdateAsync(Consent consent)
        {
            // makes a snapshot as a DB would
            consent.Scopes = consent.Scopes.ToArray();

            var query =
                from c in _context.Query<CouchbaseWrapper<Consent>>()
                where c.Model.Subject == consent.Subject && c.Model.ClientId == consent.ClientId
                select c.Model;

            var item = query.SingleOrDefault();
            if (item != null)
            {
                item.Scopes = consent.Scopes;
            }
            else
            {
                var key = Guid.NewGuid().ToString();
                _bucket.InsertAsync(key, new CouchbaseWrapper<Consent>(key, consent));
            }
            return Task.FromResult(0);
        }

        /// <summary>
        /// Revokes all permissions the subject has given to a client.
        /// </summary>
        /// <param name="subject">The subject.</param>
        /// <param name="client">The client.</param>
        /// <returns></returns>
        public Task RevokeAsync(string subject, string client)
        {
            var query =
                from c in _context.Query<CouchbaseWrapper<Consent>>()
                where c.Model.Subject == subject && c.Model.ClientId == client
                select c.Id;
            var item = query.SingleOrDefault();
            if (item != null)
            {
                _bucket.RemoveAsync(item);
            }
            return Task.FromResult(0);
        }
    }
}