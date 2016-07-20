using System;
using System.Collections.Generic;
using System.Linq;
using IdentityServer4.Models;

namespace IdSvrHost.UI.Logout
{
    public class LoggedOutViewModel
    {
        public string PostLogoutRedirectUri { get; set; }
        public string ClientName { get; set; }
        public string SignOutIframeUrl { get; set; }
    }
}
