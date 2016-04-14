using System;
using System.Linq;
using System.Threading.Tasks;
using Identity.Couchbase;
using IdentityServer4.Core.Models;
using IdentityServer4.Core.Services;
using Microsoft.AspNet.Identity;


namespace IdentityServer4.Couchbase
{
    public class AspNetIdentityProfileService<TUser>  : IProfileService
        where TUser : class, IUser
    {
        public string DisplayNameClaimType { get; set; }
        public bool EnableSecurityStamp { get; set; }

        readonly UserManager<TUser> _userManager;

        public AspNetIdentityProfileService(UserManager<TUser> userManager)
        {
            _userManager = userManager;
        }


        /// <summary>
        /// This method is called whenever claims about the user are requested (e.g. during token creation or via the userinfo endpoint)
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subject = context.Subject;
            var requestedClaimTypes = context.RequestedClaimTypes;

            if (subject == null) throw new ArgumentNullException("subject");

            var user = await _userManager.FindByIdAsync(subject.GetSubjectId());
            if (user == null)
            {
                throw new ArgumentException("Invalid subject identifier");
            }
            
            var claims = user.Claims;
            if (requestedClaimTypes != null && requestedClaimTypes.Any())
            {
                claims = claims.Where(x => requestedClaimTypes.Contains(x.Type)).ToList();
            }

            context.IssuedClaims = claims;
        }

        /// <summary>
        /// This method gets called whenever identity server needs to determine if the user is valid or active (e.g. during token issuance or validation)
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">subject</exception>
        public async Task IsActiveAsync(IsActiveContext context)
        {

            var subject = context.Subject;

            if (subject == null) throw new ArgumentNullException("subject");

            var user = await _userManager.FindByIdAsync(subject.GetSubjectId());

            context.IsActive = false;

            if (user != null)
            {
                if (EnableSecurityStamp && _userManager.SupportsUserSecurityStamp)
                {
                    var security_stamp = subject.Claims.Where(x => x.Type == "security_stamp").Select(x => x.Value).SingleOrDefault();
                    if (security_stamp != null)
                    {
                        var db_security_stamp = await _userManager.GetSecurityStampAsync(user);
                        if (db_security_stamp != security_stamp)
                        {
                            return;
                        }
                    }
                }

                context.IsActive = true;
            }
        }
    }
}