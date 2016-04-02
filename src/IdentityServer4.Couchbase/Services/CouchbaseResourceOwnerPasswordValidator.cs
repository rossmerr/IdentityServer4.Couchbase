using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Couchbase.Linq;
using IdentityServer4.Core.Validation;

namespace IdentityServer4.Couchbase.Services
{
    public class CouchbaseResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        readonly IBucketContext _context;

        public CouchbaseResourceOwnerPasswordValidator(IBucketContext context)
        {
            _context = context;
        }

        public Task<CustomGrantValidationResult> ValidateAsync(string userName, string password, ValidatedTokenRequest request)
        {
            var query =
                from u in _context.Query<CouchbaseWrapper<CouchbaseUser>>()
                where u.Model.Username == userName && u.Model.Password == password
                select u.Model;

            var user = query.SingleOrDefault();
            if (user != null)
            {
                return Task.FromResult(new CustomGrantValidationResult(user.Subject, "password"));
            }

            return Task.FromResult(new CustomGrantValidationResult("Invalid username or password"));
        }
    }
}
