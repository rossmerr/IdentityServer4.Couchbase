using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Core.Validation;

namespace IdentityServer4.Couchbase.Services
{
    public class CouchbaseResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly List<CouchbaseUser> _users;

        public CouchbaseResourceOwnerPasswordValidator(List<CouchbaseUser> users)
        {
            _users = users;
        }

        public Task<CustomGrantValidationResult> ValidateAsync(string userName, string password, ValidatedTokenRequest request)
        {
            var query =
                from u in _users
                where u.Username == userName && u.Password == password
                select u;

            var user = query.SingleOrDefault();
            if (user != null)
            {
                return Task.FromResult(new CustomGrantValidationResult(user.Subject, "password"));
            }

            return Task.FromResult(new CustomGrantValidationResult("Invalid username or password"));
        }
    }
}
