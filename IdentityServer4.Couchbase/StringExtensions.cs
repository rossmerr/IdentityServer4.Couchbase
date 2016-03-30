using System;

namespace IdentityServer4.Couchbase
{
    public static class StringExtensions
    {
        public static string GetOrigin(this string url)
        {
            if (url == null ||
                (!url.StartsWith("http://", StringComparison.Ordinal) &&
                 !url.StartsWith("https://", StringComparison.Ordinal))) return null;

            var idx = url.IndexOf("//", StringComparison.Ordinal);
            if (idx <= 0) return null;

            idx = url.IndexOf("/", idx + 2, StringComparison.Ordinal);
            if (idx >= 0)
            {
                url = url.Substring(0, idx);
            }
            return url;
        }
    }
}
