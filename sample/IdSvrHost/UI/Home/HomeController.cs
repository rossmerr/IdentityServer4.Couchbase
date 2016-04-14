using Microsoft.AspNet.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Couchbase;
using Couchbase.Linq;
using IdentityServer4.Couchbase.Wrappers;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace IdSvrHost.UI.Home
{
    public class HomeController : Controller
    {
        [Route("/")]
        public IActionResult Index()
        {
            return View();
        }
    }
}