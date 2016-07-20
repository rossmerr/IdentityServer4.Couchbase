using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityServer4;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Mvc.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Secure()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> CallApi()
        {
            var token = User.FindFirst("access_token").Value;

            var client = new HttpClient();
            client.SetBearerToken(token);

            var response = await client.GetStringAsync("http://localhost:3860/identity");
            ViewBag.Json = JArray.Parse(response).ToString();

            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.Authentication.SignOutAsync("Cookies");
            await HttpContext.Authentication.SignOutAsync("oidc");
            HttpContext.Response.Cookies.Delete("idsvr");
            return Redirect("~/");
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}