using Microsoft.AspNetCore.Identity;
using System;

namespace Identity.Couchbase
{
    public class LookupNormalizer : ILookupNormalizer
    {
        public string Normalize(string key)
        {
            return key.Normalize().ToLowerInvariant();
        }
    }
}
