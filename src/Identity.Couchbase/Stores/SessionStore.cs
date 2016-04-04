using System;
using System.Threading.Tasks;
using Couchbase.Core;
using Microsoft.AspNet.Authentication;
using Microsoft.AspNet.Authentication.Cookies;
using Microsoft.AspNet.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.OptionsModel;

namespace Identity.Couchbase.Stores
{
    public class SessionStore : ITicketStore
    {
        readonly IDataSerializer<AuthenticationTicket> _ticketSerializer;
        readonly IBucket _bucket;
        readonly ILogger _logger;
        readonly double _timeout;

        public SessionStore(IBucket bucket,
            IDataSerializer<AuthenticationTicket> ticketSerializer,
            IOptions<IdentityOptions> options, 
            ILogger<SessionStore> logger)
        {
            _logger = logger;
            _bucket = bucket;
            _timeout = options.Value.Cookies.ApplicationCookie.ExpireTimeSpan.TotalMinutes;
            _ticketSerializer = ticketSerializer;
        }

        public async Task RemoveAsync(string key)
        {
            var result = await _bucket.ExistsAsync(key);
            if (!result) return;
            _logger.LogInformation($"{nameof(RemoveAsync)}: {key}");
            _bucket.Remove(key);
        }

        public async Task RenewAsync(string key, AuthenticationTicket ticket)
        {
            if (await _bucket.ExistsAsync(key))
            {
                var ttl = TimeSpan.FromMinutes(_timeout);
                await _bucket.UpsertAsync(key, Serialize(ticket), ttl);
                _logger.LogInformation($"{nameof(RenewAsync)} {key} { ticket.Principal.Identity.Name} {ttl}");
            }
            else
            {
                _logger.LogInformation($"{nameof(RenewAsync)} Failed: {key}");
            }
        }

        public async Task<AuthenticationTicket> RetrieveAsync(string key)
        {
            if (await _bucket.ExistsAsync(key))
            {
                var ttl = TimeSpan.FromMinutes(_timeout);
                var doc = await _bucket.GetAndTouchAsync<string>(key, ttl);
                if (string.IsNullOrWhiteSpace(doc.Value)) return null;
                var ticket = Deserialize(doc.Value);
                _logger.LogInformation($"{nameof(RetrieveAsync)}: {key} { ticket.Principal.Identity.Name}");
                return ticket;
            }

            _logger.LogInformation($"{nameof(RetrieveAsync)} Failes: {key}");
            return null;
        }

        public async Task<string> StoreAsync(AuthenticationTicket ticket)
        {
            var key = nameof(SessionStore) + ":" + Guid.NewGuid() + ticket.GetHashCode();
            var ttl = TimeSpan.FromMinutes(_timeout);
            _logger.LogInformation($"{nameof(StoreAsync)} {key} { ticket.Principal.Identity.Name} {ttl}");
            await _bucket.InsertAsync(key, Serialize(ticket), ttl);
            return key;      
        }

        AuthenticationTicket Deserialize(string text)
        {
            var orginalBytes = Convert.FromBase64String(text);
            return _ticketSerializer.Deserialize(orginalBytes);
        }

        string Serialize(AuthenticationTicket ticket)
        {
            var bytes = _ticketSerializer.Serialize(ticket);
            return Convert.ToBase64String(bytes);
        }
    }

}