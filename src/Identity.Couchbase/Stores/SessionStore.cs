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
        readonly ILookupNormalizer _lookupNormalizer;

        public SessionStore(IBucket bucket,
            IDataSerializer<AuthenticationTicket> ticketSerializer,
            ILookupNormalizer lookupNormalizer,
            IOptions<IdentityOptions> options)
        {
            _bucket = bucket;
            _timeout = options.Value.Cookies.ApplicationCookie.ExpireTimeSpan;
            _ticketSerializer = ticketSerializer;
            _lookupNormalizer = lookupNormalizer;
        }

        public async Task RemoveAsync(string key)
        {
            await _bucket.RemoveAsync(key.ConvertSessionKeyToId(_lookupNormalizer));
        }

        public async Task RenewAsync(string key, AuthenticationTicket ticket)
        {            
            await _bucket.UpsertAsync(key.ConvertSessionKeyToId(_lookupNormalizer), new Session(Serialize(ticket)), _timeout);
        }

        public async Task<AuthenticationTicket> RetrieveAsync(string key)
        {
            var doc = await _bucket.GetAndTouchAsync<Session>(key, _timeout);
            return !doc.Success ? null : Deserialize(doc.Value.Data);
        }

        public async Task<string> StoreAsync(AuthenticationTicket ticket)
        {
            var key = Guid.NewGuid().ToString() + ticket.GetHashCode();
            await _bucket.InsertAsync(key.ConvertSessionKeyToId(_lookupNormalizer), new Session(Serialize(ticket)), _timeout);
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