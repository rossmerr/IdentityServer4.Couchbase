using Microsoft.AspNet.Identity;

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
