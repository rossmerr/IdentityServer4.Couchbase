// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Core.Models;
using IdentityServer4.Core.Services;

namespace IdentityServer4.Couchbase.Services
{
    /// <summary>
    /// In-memory client store
    /// </summary>
    public class CouchbaseClientStore : IClientStore
    {
        readonly IEnumerable<Client> _clients;

        /// <summary>
        /// Initializes a new instance of the <see cref="CouchbaseClientStore"/> class.
        /// </summary>
        /// <param name="clients">The clients.</param>
        public CouchbaseClientStore(IEnumerable<Client> clients)
        {
            _clients = clients;
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
                from client in _clients
                where client.ClientId == clientId && client.Enabled
                select client;
            
            return Task.FromResult(query.SingleOrDefault());
        }
    }
}