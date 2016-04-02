namespace Identity.Couchbase
{
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