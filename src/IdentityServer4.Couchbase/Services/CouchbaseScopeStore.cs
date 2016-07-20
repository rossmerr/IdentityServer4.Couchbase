using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Couchbase.Core;
using Couchbase.Linq;
using IdentityServer4.Couchbase.Wrappers;
using IdentityServer4.Models;
using IdentityServer4.Services;

namespace IdentityServer4.Couchbase.Services
{
    public interface ICouchbaseScopeStore : IScopeStore
    {
        Task StoreScopeAsync(Scope client);
    }


    /// <summary>
    /// Couchbase scope store
    /// </summary>
    public class CouchbaseScopeStore : ICouchbaseScopeStore
    {
        readonly IBucketContext _context;
        readonly IBucket _bucket;

        /// <summary>
        /// Initializes a new instance of the <see cref="CouchbaseScopeStore"/> class.
        /// </summary>
        /// <param name="scopes">The scopes.</param>
        public CouchbaseScopeStore(IBucketContext context, IBucket bucket)
        {
            _context = context;
            _bucket = bucket;
        }

        /// <summary>
        /// Gets all scopes.
        /// </summary>
        /// <returns>
        /// List of scopes
        /// </returns>
        public Task<IEnumerable<Scope>> FindScopesAsync(IEnumerable<string> scopeNames)
        {
            if (scopeNames == null) throw new ArgumentNullException("scopeNames");
            
            var list = new List<Scope>();

            foreach (var scopeName in scopeNames)
            {
                var scopes = from s in _context.Query<ScopeWrapper>()
                             where s.Model.Name == scopeName
                             select s.Model;

                list.AddRange(scopes);
            }

            return Task.FromResult<IEnumerable<Scope>>(list);
        }


        /// <summary>
        /// Gets all defined scopes.
        /// </summary>
        /// <param name="publicOnly">if set to <c>true</c> only public scopes are returned.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Task<IEnumerable<Scope>> GetScopesAsync(bool publicOnly = true)
        {
            if (publicOnly)
            {
                var scopes = from s in _context.Query<ScopeWrapper>()
                             where s.Model.ShowInDiscoveryDocument
                             select s.Model;

                if (scopes.Any())
                {
                    return Task.FromResult<IEnumerable<Scope>>(scopes.ToList());
                }
                return Task.FromResult(Enumerable.Empty<Scope>());

            }
            var results = from s in _context.Query<ScopeWrapper>()
                select s.Model;

            return Task.FromResult<IEnumerable<Scope>>(results.ToList());
        }

        public Task StoreScopeAsync(Scope client)
        {
            return _bucket.InsertAsync(ScopeWrapper.ScopeWrapperId(client.Name), new ScopeWrapper(client.Name, client));
        }
    }
}