using IdentityModel;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.Services;
using IdentityServer4.Services.InMemory;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdSvrHost.UI.Login
{
    public class LoginController : Controller
    {
        private readonly IUserInteractionService _interaction;
        readonly SignInManager<CouchbaseUser> _loginService;
        readonly UserManager<CouchbaseUser> _userManager;

        public LoginController(
            IUserInteractionService interaction, 
            SignInManager<CouchbaseUser> loginService, 
            UserManager<CouchbaseUser> userManager)
        {
            _interaction = interaction;
            _loginService = loginService;
            _userManager = userManager;
        }

        [HttpGet("ui/login", Name = "Login")]
        public async Task<IActionResult> Index(string returnUrl)
        {
            var vm = new LoginViewModel();

            if (returnUrl != null)
            {
                var context = await _interaction.GetLoginContextAsync();
                if (context != null)
                {
                    vm.Username = context.LoginHint;
                    vm.ReturnUrl = returnUrl;
                }
            }

            return View(vm);
        }

        [HttpPost("ui/login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginInputModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _loginService.PasswordSignInAsync(model.Username, model.Password, false, false);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(model.Username);

                    await IssueCookie(user, "idsvr", "password");

                    if (model.ReturnUrl != null && _interaction.IsValidReturnUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }

                    return Redirect("~/");
                }

                ModelState.AddModelError("", "Invalid username or password.");
            }

            var vm = new LoginViewModel(model);
            return View(vm);
        }

        private async Task IssueCookie(
            CouchbaseUser user,
            string idp,
            string amr)
        {
            var name = user.Claims.Where(x => x.Type == JwtClaimTypes.Name).Select(x => x.Value).FirstOrDefault() ?? user.Username;

            var claims = new Claim[] {
                new Claim(JwtClaimTypes.Subject, user.SubjectId),
                new Claim(JwtClaimTypes.Name, name),
                new Claim(JwtClaimTypes.IdentityProvider, idp),
                new Claim(JwtClaimTypes.AuthenticationTime, DateTime.UtcNow.ToEpochTime().ToString()),
            };
            var ci = new ClaimsIdentity(claims, amr, JwtClaimTypes.Name, JwtClaimTypes.Role);
            var cp = new ClaimsPrincipal(ci);

            await HttpContext.Authentication.SignInAsync(Constants.DefaultCookieAuthenticationScheme, cp);
        }

        [HttpGet("/ui/external/{provider}", Name = "External")]
        public IActionResult External(string provider, string returnUrl)
        {
            return new ChallengeResult(provider, new AuthenticationProperties
            {
                RedirectUri = "/ui/external-callback?returnUrl=" + returnUrl
            });
        }

        //[HttpGet("/ui/external-callback")]
        //public async Task<IActionResult> ExternalCallback(string returnUrl)
        //{
        //    var tempUser = await HttpContext.Authentication.AuthenticateAsync("Temp");
        //    if (tempUser == null)
        //    {
        //        throw new Exception();
        //    }

        //    var claims = tempUser.Claims.ToList();

        //    var userIdClaim = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Subject);
        //    if (userIdClaim == null)
        //    {
        //        userIdClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
        //    }
        //    if (userIdClaim == null)
        //    {
        //        throw new Exception("Unknown userid");
        //    }

        //    claims.Remove(userIdClaim);

        //    var provider = userIdClaim.Issuer;
        //    var userId = userIdClaim.Value;
            
        //    var user = _loginService.FindByExternalProvider(provider, userId);
        //    if (user == null)
        //    {
        //        user = _loginService.AutoProvisionUser(provider, userId, claims);
        //    }

        //    await IssueCookie(user, provider, "external");
        //    await HttpContext.Authentication.SignOutAsync("Temp");

        //    if (returnUrl != null)
        //    {
        //        // todo: signin
        //        //return new SignInResult(signInId);
        //    }

        //    return Redirect("~/");

        //}
    }
}