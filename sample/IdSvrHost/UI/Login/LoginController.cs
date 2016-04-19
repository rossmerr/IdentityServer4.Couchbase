using IdentityModel;
using IdentityServer4.Core;
using IdentityServer4.Core.Services;
using Microsoft.AspNet.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace IdSvrHost.UI.Login
{
    public class LoginController : Controller
    {
        readonly SignInManager<CouchbaseUser> _loginService;
        readonly UserManager<CouchbaseUser> _userManager;
        readonly SignInInteraction _signInInteraction;

        public LoginController(SignInManager<CouchbaseUser> loginService, 
            SignInInteraction signInInteraction, 
            UserManager<CouchbaseUser> userManager)
        {
            _loginService = loginService;
            _signInInteraction = signInInteraction;
            _userManager = userManager;
        }

        [HttpGet(Constants.RoutePaths.Login, Name = "Login")]
        public async Task<IActionResult> Index(string id)
        {
            var vm = new LoginViewModel();

            if (id != null)
            {
                var request = await _signInInteraction.GetRequestAsync(id);
                if (request != null)
                {
                    vm.Username = request.LoginHint;
                    vm.SignInId = id;
  
                }
            }

            return View(vm);
        }

        [HttpPost(Constants.RoutePaths.Login)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginInputModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _loginService.PasswordSignInAsync(model.Username, model.Password, false, false);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(model.Username);
                    
                    var name =
                        user.Claims.Where(x => x.Type == JwtClaimTypes.Name).Select(x => x.Value).FirstOrDefault() ??
                        user.Username;

                    var claims = new[]
                    {
                        new Claim(JwtClaimTypes.Subject, user.SubjectId),
                        new Claim(JwtClaimTypes.Name, name),
                        new Claim(JwtClaimTypes.IdentityProvider, "idsvr"),
                        new Claim(JwtClaimTypes.AuthenticationTime, DateTime.UtcNow.ToEpochTime().ToString()),
                    };
                    var ci = new ClaimsIdentity(claims, "password", JwtClaimTypes.Name, JwtClaimTypes.Role);
                    var cp = new ClaimsPrincipal(ci);

                    await HttpContext.Authentication.SignInAsync(Constants.PrimaryAuthenticationType, cp);

                    if (model.SignInId != null)
                    {
                        return new SignInResult(model.SignInId);
                    }

                    return Redirect("~/");
                }

                ModelState.AddModelError("", "Invalid username or password.");
            }

            var vm = new LoginViewModel(model);
            return View(vm);
        }
    }
}
