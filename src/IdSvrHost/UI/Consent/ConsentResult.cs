using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using IdentityServer4.Models;

namespace IdSvrHost.UI.Consent
{
    public class ConsentResult : IActionResult
    {
        private readonly string _requestId;
        private readonly ConsentResponse _response;

        public ConsentResult(string requestId, ConsentResponse response)
        {
            _requestId = requestId;
            _response = response;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            //var interaction = context.HttpContext.RequestServices.GetRequiredService<ConsentInteraction>();
            //await interaction.ProcessResponseAsync(_requestId, _response);
        }
    }
}
