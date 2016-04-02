using System;

namespace Identity.Couchbase
{
    public static class IdentityUserExtensions
    {

        public static string ConvertEmailToId(string email)
        {
            return $"IdentityUser:{email}";
        }

        public static string ConvertUserToId<TKey>(this IIdentityUser<TKey> user) where TKey : IEquatable<TKey>
        {
            return ConvertEmailToId(user.Email);
        }
    }
}
