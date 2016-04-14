using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Couchbase.Core;
using Couchbase.Linq;
using Microsoft.AspNet.Identity;

namespace Identity.Couchbase.Stores
{
    public interface ISubject<TUser> where TUser : class, IUser
    {
        Task<string> GetSubjectAsync(TUser user);
    }

    public class UserStore<TUser, TRole> :
        IUserLoginStore<TUser>,
        IUserRoleStore<TUser>,
        IUserPasswordStore<TUser>,
        IUserSecurityStampStore<TUser>,
        IUserClaimStore<TUser>,
        IUserLockoutStore<TUser>,
        ISubject<TUser>
        where TUser : class, IUser
        where TRole : class, IRole
    {

        readonly IBucket _bucket;
        readonly IBucketContext _context;
        readonly ILookupNormalizer _lookupNormalizer;

        bool _disposed;

        public UserStore(IBucket bucket, IBucketContext context, ILookupNormalizer lookupNormalizer)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            if (bucket == null)
            {
                throw new ArgumentNullException(nameof(bucket));
            }
            _context = context;
            _lookupNormalizer = lookupNormalizer;
            _bucket = bucket;
        }
        
        protected void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        public void Dispose()
        {
           // _disposed = true;
        }


        public Task AddLoginAsync(TUser user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (login == null)
            {
                throw new ArgumentNullException(nameof(login));
            }

            var matchedLogin = user.Logins.First(x => x.ProviderKey == login.ProviderKey && x.LoginProvider == login.LoginProvider && x.ProviderDisplayName == login.ProviderDisplayName);

            if (matchedLogin == null)
            {
                user.Logins.Add(new UserLogin
                {
                    ProviderKey = login.ProviderKey,
                    LoginProvider = login.LoginProvider,
                    ProviderDisplayName = login.ProviderDisplayName
                });
            }           

            return _bucket.UpsertAsync(user.ConvertUserToId(_lookupNormalizer), new UserWrapper<TUser>(user));
        }
     
        public Task RemoveLoginAsync(TUser user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (loginProvider == null)
            {
                throw new ArgumentNullException(nameof(loginProvider));
            }

            if (providerKey == null)
            {
                throw new ArgumentNullException(nameof(providerKey));
            }

            var matchedLogins = user.Logins.First(x => x.ProviderKey == providerKey && x.LoginProvider == loginProvider);

            if (matchedLogins != null)
            {
                user.Logins.Remove(matchedLogins);
            }

            return _bucket.UpsertAsync(user.ConvertUserToId(_lookupNormalizer), new UserWrapper<TUser>(user));
        }

        public  Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var logins = user.Logins.Select(login => new UserLoginInfo(login.ProviderKey, login.LoginProvider, login.ProviderDisplayName));

            return Task.FromResult<IList<UserLoginInfo>>(logins.ToList());
        }

        public Task<TUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            var results = from user in _context.Query<UserWrapper<TUser>>()
                from login in user.User.Logins
                where login.LoginProvider == loginProvider
                && login.ProviderKey == providerKey
                select user.User;

            return Task.FromResult(results.FirstOrDefault());
        }

        public Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.ConvertUserToId(_lookupNormalizer));
        }

        public Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.Username);
        }

        public Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.Username = userName;
            return _bucket.UpsertAsync(user.ConvertUserToId(_lookupNormalizer), new UserWrapper<TUser>(user));
        }

        public Task<string> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.NormalizedUserName);
        }

        public Task SetNormalizedUserNameAsync(TUser user, string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            user.NormalizedUserName = normalizedName;
            return _bucket.UpsertAsync(user.ConvertUserToId(_lookupNormalizer), new UserWrapper<TUser>(user));
        }

        public async Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var userWrapper = new UserWrapper<TUser>(user)
            {
                Subject = Guid.NewGuid().ToString()
            };

            var result = await _bucket.UpsertAsync(user.ConvertUserToId(_lookupNormalizer), userWrapper);
            return result.Success ? IdentityResult.Success : IdentityResult.Failed();
        }

        public async Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            
            user.ConcurrencyStamp = Guid.NewGuid().ToString();

            var result = await _bucket.UpsertAsync(user.ConvertUserToId(_lookupNormalizer), new UserWrapper<TUser>(user));

            return result.Success ? IdentityResult.Success : IdentityResult.Failed();         
        }

        public async Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var result = await _bucket.RemoveAsync(user.ConvertUserToId(_lookupNormalizer));

            return result.Success ? IdentityResult.Success : IdentityResult.Failed();
        }

        public async Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            var result = await _bucket.GetAsync<UserWrapper<TUser>>(IdentityExtensions.ConvertEmailToId(userId, _lookupNormalizer));
            return result.Value.User;
        }

        public Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            
            var results = from user in _context.Query<UserWrapper<TUser>>()
                where user.User.NormalizedUserName == normalizedUserName
                select user.User;

            var u = results.FirstOrDefault();

            return Task.FromResult(u);
        }

        public Task SetPasswordHashAsync(TUser user, string passwordHash, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.PasswordHash = passwordHash;
            return _bucket.UpsertAsync(user.ConvertUserToId(_lookupNormalizer), new UserWrapper<TUser>(user));
        }

        public Task<string> GetPasswordHashAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(user.PasswordHash != null);
        }

        public Task AddToRoleAsync(TUser user, string normalizedRoleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (string.IsNullOrWhiteSpace(normalizedRoleName))
            {                
                throw new ArgumentException(Resource.ValueCannotBeNullOrEmpty, nameof(normalizedRoleName));
            }

            var roleEntity = (from role in _context.Query<RoleWrapper<TRole>>()
                          where role.Role.NormalizedName == normalizedRoleName
                          select role.Role).FirstOrDefault();

            if (roleEntity == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resource.RoleNotFound, normalizedRoleName));
            }

            var ur = Activator.CreateInstance<TRole>();
            ur.RoleId = roleEntity.RoleId;
            user.Roles.Add(ur);

            return _bucket.UpsertAsync(user.ConvertUserToId(_lookupNormalizer), new UserWrapper<TUser>(user));
        }

        public Task RemoveFromRoleAsync(TUser user, string normalizedRoleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (string.IsNullOrWhiteSpace(normalizedRoleName))
            {
                throw new ArgumentException(Resource.ValueCannotBeNullOrEmpty, nameof(normalizedRoleName));
            }

            var roleEntity = (from role in _context.Query<RoleWrapper<TRole>>()
                              where role.Role.NormalizedName == normalizedRoleName
                              select role.Role).FirstOrDefault();

            if (roleEntity == null)
            {
                return Task.FromResult(0);
            }

            var userRole = user.Roles.FirstOrDefault(role => role.RoleId.Equals(roleEntity.RoleId));

            if (userRole == null)
            {
                return Task.FromResult(0);
            }

            user.Roles.Remove(userRole);

            return _bucket.UpsertAsync(user.ConvertUserToId(_lookupNormalizer), new UserWrapper<TUser>(user));
        }

        public Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            var list = new List<string>();

            if (user.Roles == null)
                return Task.FromResult<IList<string>>(list);

            foreach (var role in user.Roles)
            {                
                var results = from r in _context.Query<RoleWrapper<TRole>>()
                              where r.Role.RoleId == role.RoleId
                              select r.Role.Name;

                list.AddRange(results);
            }
            
            return Task.FromResult<IList<string>>(list);
        }

        public async Task<bool> IsInRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (string.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentException(nameof(roleName));
            }


            var roles = await GetRolesAsync(user, cancellationToken);

            return roles.Any(p => p == roleName);
        }

        public Task<IList<TUser>> GetUsersInRoleAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (string.IsNullOrEmpty(normalizedRoleName))
            {
                throw new ArgumentNullException(nameof(normalizedRoleName));
            }
            
            var role = (from roles in _context.Query<RoleWrapper<TRole>>()
                where roles.Role.NormalizedName == normalizedRoleName
                select roles.Role).FirstOrDefault();

            if (role == null)
            {
                return Task.FromResult<IList<TUser>>(new List<TUser>());
            }

            var results = from user in _context.Query<UserWrapper<TUser>>()
                from ur in user.User.Roles
                where ur.RoleId == role.RoleId
                select user.User;

            return Task.FromResult<IList<TUser>>(results.ToList());
        }

        public Task<IList<Claim>> GetClaimsAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult<IList<Claim>>(user.Claims?.ToList() ?? new List<Claim>());
        }

        public Task AddClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (claims == null)
            {
                throw new ArgumentNullException(nameof(claims));
            }

            foreach (var claim in claims)
            {
                user.Claims.Add(claim);
            }

            return _bucket.UpsertAsync(user.ConvertUserToId(_lookupNormalizer), new UserWrapper<TUser>(user));
        }

        public Task ReplaceClaimAsync(TUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (claim == null)
            {
                throw new ArgumentNullException(nameof(claim));
            }

            if (newClaim == null)
            {
                throw new ArgumentNullException(nameof(newClaim));
            }

            if (user.Claims.Contains(claim))
            {
                user.Claims.Remove(claim);
            }

            user.Claims.Add(newClaim);

            return _bucket.UpsertAsync(user.ConvertUserToId(_lookupNormalizer), new UserWrapper<TUser>(user));
        }

        public Task RemoveClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (claims == null)
            {
                throw new ArgumentNullException(nameof(claims));
            }

            foreach (var claim in claims)
            {
                if (user.Claims.Contains(claim))
                {
                    user.Claims.Remove(claim);
                }
            }

            return _bucket.UpsertAsync(user.ConvertUserToId(_lookupNormalizer), new UserWrapper<TUser>(user));
        }

        public Task<IList<TUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (claim == null)
            {
                throw new ArgumentNullException(nameof(claim));
            }

            var query = from user in _context.Query<UserWrapper<TUser>>()
                from userclaims in user.User.Claims
                where userclaims.Value == claim.Value
                      && userclaims.Type == claim.Type
                select user.User;

            return Task.FromResult<IList<TUser>>(query.ToList());
        }

        public Task SetSecurityStampAsync(TUser user, string stamp, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.SecurityStamp = stamp;
            return Task.FromResult(0);
        }

        public Task<string> GetSecurityStampAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.SecurityStamp);
        }

        public Task<DateTimeOffset?> GetLockoutEndDateAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.LockoutEnd);
        }

        public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.LockoutEnd = lockoutEnd;
            return Task.FromResult(0);
        }

        public Task<int> IncrementAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.AccessFailedCount++;
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task ResetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.AccessFailedCount = 0;
            return Task.FromResult(0);
        }

        public Task<int> GetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.AccessFailedCount);
        }

        public Task<bool> GetLockoutEnabledAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.LockoutEnabled);
        }

        public Task SetLockoutEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.LockoutEnabled = enabled;
            return Task.FromResult(0);
        }

        public async Task<string> GetSubjectAsync(TUser user)
        {
            ThrowIfDisposed();
            
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var result = await _bucket.GetAsync<UserWrapper<TUser>>(IdentityExtensions.ConvertEmailToId(user.Email, _lookupNormalizer));

            return await Task.FromResult(result.Value.Subject);
        }
    }
}