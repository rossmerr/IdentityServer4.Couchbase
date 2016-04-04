using System;
using System.Threading.Tasks;
using Couchbase.Core;
using Microsoft.AspNet.Authentication;
using Microsoft.AspNet.Authentication.Cookies;
using Microsoft.AspNet.Identity;
using Microsoft.Extensions.OptionsModel;

namespace Identity.Couchbase.Stores
{
    public class SessionStore : ITicketStore
    {
        readonly IDataSerializer<AuthenticationTicket> _ticketSerializer;
        readonly IBucket _bucket;
        readonly TimeSpan _timeout;

        public SessionStore(IBucket bucket,
            IDataSerializer<AuthenticationTicket> ticketSerializer,
            IOptions<IdentityOptions> options)
        {
            _bucket = bucket;
            _timeout = options.Value.Cookies.ApplicationCookie.ExpireTimeSpan;
            _ticketSerializer = ticketSerializer;
        }

        public async Task RemoveAsync(string key)
        {
            await _bucket.RemoveAsync(key);
        }

        public async Task RenewAsync(string key, AuthenticationTicket ticket)
        {
            await _bucket.UpsertAsync(key, Serialize(ticket), _timeout);
        }

        public async Task<AuthenticationTicket> RetrieveAsync(string key)
        {
            var doc = await _bucket.GetAndTouchAsync<string>(key, _timeout);
            if (!doc.Success) return null;
            var ticket = Deserialize(doc.Value);
            return ticket;
        }

        public async Task<string> StoreAsync(AuthenticationTicket ticket)
        {
            var key = nameof(SessionStore) + ":" + Guid.NewGuid() + ticket.GetHashCode();
            await _bucket.InsertAsync(key, Serialize(ticket), _timeout);
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