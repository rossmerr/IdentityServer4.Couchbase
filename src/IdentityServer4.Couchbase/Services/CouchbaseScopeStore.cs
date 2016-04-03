using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Couchbase.Linq;
using IdentityServer4.Core.Models;
using IdentityServer4.Core.Services;
using IdentityServer4.Couchbase.Wrappers;

namespace IdentityServer4.Couchbase.Services
{
    /// <summary>
    /// Couchbase scope store
    /// </summary>
    public class CouchbaseScopeStore : IScopeStore
    {
        readonly IBucketContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="CouchbaseScopeStore"/> class.
        /// </summary>
        /// <param name="scopes">The scopes.</param>
        public CouchbaseScopeStore(IBucketContext context)
        {
            _context = context;
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
            
            var scopes = from s in _context.Query<ScopeWrapper>()
                         where scopeNames.ToList().Contains(s.Model.Name)
                         select s.Model;

            return Task.FromResult<IEnumerable<Scope>>(scopes.ToList());
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

                return Task.FromResult<IEnumerable<Scope>>(scopes.ToList());
            }

            return Task.FromResult<IEnumerable<Scope>>(_context.Query<ScopeWrapper>().Select(p => p.Model).ToList());
        }
    }
}