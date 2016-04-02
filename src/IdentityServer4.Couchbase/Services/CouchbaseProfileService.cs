using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Couchbase.Linq;
using IdentityModel;
using IdentityServer4.Core.Models;
using IdentityServer4.Core.Services;

namespace IdentityServer4.Couchbase.Services
{
    /// <summary>
    /// Couchbase user service
    /// </summary>
    public class CouchbaseProfileService : IProfileService
    {
        readonly IBucketContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="CouchbaseProfileService"/> class.
        /// </summary>
        /// <param name="users">The users.</param>
        public CouchbaseProfileService(IBucketContext context)
        {
            _context = context;
        }

        /// <summary>
        /// This method is called whenever claims about the user are requested (e.g. during token creation or via the userinfo endpoint)
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var query =
                from u in _context.Query<CouchbaseWrapper<CouchbaseUser>>()
                where u.Model.Subject == context.Subject.GetSubjectId()
                select u.Model;
            var user = query.Single();

            var claims = new List<Claim>{
                new Claim(JwtClaimTypes.Subject, user.Subject),
            };

            claims.AddRange(user.Claims);
            if (!context.AllClaimsRequested)
            {
                claims = claims.Where(x => context.RequestedClaimTypes.Contains(x.Type)).ToList();
            }

            context.IssuedClaims = claims;

            return Task.FromResult(0);
        }

        /// <summary>
        /// This method gets called whenever identity server needs to determine if the user is valid or active (e.g. during token issuance or validation)
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">subject</exception>
        public Task IsActiveAsync(IsActiveContext context)
        {
            if (context.Subject == null) throw new ArgumentNullException("subject");

            var query =
                from u in _context.Query<CouchbaseWrapper<CouchbaseUser>>()
                where
                    u.Model.Subject == context.Subject.GetSubjectId()
                select u.Model;

            var user = query.SingleOrDefault();
            
            context.IsActive = (user != null) && user.Enabled;

            return Task.FromResult(0);
        }
    }
}