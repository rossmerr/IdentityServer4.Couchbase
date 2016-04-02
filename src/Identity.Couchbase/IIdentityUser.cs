using System;
using System.Collections.Generic;

namespace Identity.Couchbase
{
    public interface IIdentityUser<TKey> where TKey : IEquatable<TKey>
    {
        TKey Id { get; }
        string Email { get; set; }
        string UserName { get; set; }
        string NormalizedUserName { get; set; }
        string PasswordHash { get; set; }
        ICollection<IIdentityRole> Roles { get; }
        ICollection<IdentityUserLogin> Logins { get; }
        string ConcurrencyStamp { get; set; }
    }

    public interface IIdentityRole
    {
        string RoleId { get; set; }

        string NormalizedName { get; }
        string Name { get; }
    }

    public class IdentityUserLogin
    {
        /// <summary>
        /// The login provider for the login (i.e. facebook, google)
        /// </summary>
        public string LoginProvider { get; set; }

        /// <summary>
        /// Key representing the login for the provider
        /// </summary>
        public string ProviderKey { get; set; }

        /// <summary>
        /// Display name for the login
        /// </summary>
        public string ProviderDisplayName { get; set; }
    }
}
