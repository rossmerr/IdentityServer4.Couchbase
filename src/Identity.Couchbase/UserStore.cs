using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Couchbase.Core;
using Couchbase.Linq;
using Microsoft.AspNet.Identity;

namespace Identity.Couchbase
{

    /// <summary>
    /// Represents a new instance of a persistence store for the specified user and role types.
    /// </summary>
    /// <typeparam name="TUser">The type representing a user.</typeparam>
    /// <typeparam name="TRole">The type representing a role.</typeparam>
    /// <typeparam name="TKey">The type of the primary key for a role.</typeparam>
    public class UserStore<TUser, TRole, TKey> :
        IUserLoginStore<TUser>,
        IUserRoleStore<TUser>,
        IUserPasswordStore<TUser>
        where TUser : class, IIdentityUser<TKey>
        where TRole : IIdentityRole
        where TKey : IEquatable<TKey>
    {

        readonly IBucket _bucket;
        readonly IBucketContext _context;

        /// <summary>
        /// Creates a new instance of <see cref="UserStore"/>.
        /// </summary>
        /// <param name="bucket">The bucket used to access the store.</param>
        /// <param name="context">The context used to access the store.</param>
        public UserStore(IBucket bucket, IBucketContext context)
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
            _bucket = bucket;

        }


        bool _disposed;
        
        protected void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        public void Dispose()
        {
            _disposed = true;
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
                user.Logins.Add(new IdentityUserLogin
                {
                    ProviderKey = login.ProviderKey,
                    LoginProvider = login.LoginProvider,
                    ProviderDisplayName = login.ProviderDisplayName
                });
            }           

            return _bucket.UpsertAsync(user.ConvertUserToId(), user);
        }

     


        public Task RemoveLoginAsync(TUser user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (loginProvider == null || providerKey == null)
            {
                throw new ArgumentNullException("Login");
            }

            var matchedLogins = user.Logins.First(x => x.ProviderKey == providerKey && x.LoginProvider == loginProvider);

            if (matchedLogins != null)
            {
                user.Logins.Remove(matchedLogins);
            }

            return _bucket.UpsertAsync(user.ConvertUserToId(), user);
        }

        public  Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.Factory.StartNew<IList<UserLoginInfo>>(() =>
            {
                return user.Logins.Select(login => new UserLoginInfo(login.ProviderKey, login.LoginProvider, login.ProviderDisplayName)).ToList();
            }, cancellationToken);
        }

        public Task<TUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            var results = from user in _context.Query<TUser>()
                from login in user.Logins
                where login.LoginProvider == loginProvider
                where login.ProviderKey == providerKey
                select user;

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

            return Task.FromResult(user.ConvertUserToId());
        }

        public Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.UserName);
        }

        public Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.UserName = userName;
            _bucket.UpsertAsync(user.ConvertUserToId(), user);
            return Task.FromResult(0);
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
            _bucket.UpsertAsync(user.ConvertUserToId(), user);
            return Task.FromResult(0);
        }

        public async Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var result = await _bucket.InsertAsync(user.ConvertUserToId(), user);
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

            var result = await _bucket.UpsertAsync(user.ConvertUserToId(), user);

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

            var result = await _bucket.RemoveAsync(user.ConvertUserToId());

            return result.Success ? IdentityResult.Success : IdentityResult.Failed();
        }

        public async Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            var result = await _bucket.GetAsync<TUser>(userId);
            return result.Value;
        }

        public Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            return Task.Factory.StartNew(() =>
            {
                var results = from user in _context.Query<TUser>()
                    where user.NormalizedUserName == normalizedUserName
                    select user;

                return results.FirstOrDefault();
            }, cancellationToken);
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
            _bucket.UpsertAsync(user.ConvertUserToId(), user);
            return Task.FromResult(0);
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

            var roleEntity = (from role in _context.Query<TRole>()
                          where role.NormalizedName == normalizedRoleName
                          select role).FirstOrDefault();

            if (roleEntity == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resource.RoleNotFound, normalizedRoleName));
            }

            var ur = Activator.CreateInstance<TRole>();
            ur.RoleId = roleEntity.RoleId;
            user.Roles.Add(ur);
            return _bucket.UpsertAsync(user.ConvertUserToId(), user);
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

            var roleEntity = (from role in _context.Query<TRole>()
                              where role.NormalizedName == normalizedRoleName
                              select role).FirstOrDefault();

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
            
            return _bucket.UpsertAsync(user.ConvertUserToId(), user);
        }

        public Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.Factory.StartNew<IList<string>>(() =>
            {
                var results = from role in _context.Query<TRole>()
                    from userRole in user.Roles
                    where userRole.RoleId == role.RoleId
                    select role.Name;

                return results.ToList();
            }, cancellationToken);
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

            return Task.Factory.StartNew<IList<TUser>>(() =>
            {

                var role = (from roles in _context.Query<TRole>()
                    where roles.NormalizedName == normalizedRoleName
                    select roles).FirstOrDefault();

                if (role == null)
                {
                    return new List<TUser>();
                }

                var results = from user in _context.Query<TUser>()
                    from ur in user.Roles
                    where ur.RoleId == role.RoleId
                    select user;

                return results.ToList();
            }, cancellationToken);
        }
    }
}