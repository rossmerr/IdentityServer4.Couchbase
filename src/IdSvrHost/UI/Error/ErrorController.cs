using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Mvc;

namespace IdSvrHost.UI.Error
{
    public class ErrorController : Controller
    {
        private readonly IUserInteractionService _errorInteraction;

        public ErrorController(IUserInteractionService errorInteraction)
        {
            _errorInteraction = errorInteraction;
        }

        [Route("ui/error", Name = "Error")]
        public async Task<IActionResult> Index(string id)
        {
            var vm = new ErrorViewModel();

            if (id != null)
            {
                var message = await _errorInteraction.GetErrorContextAsync(id);
                if (message != null)
                {
                    vm.Error = message;
                }
            }

            return View("Error", vm);
        }
    }
}
