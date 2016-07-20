using System;
using System.Threading.Tasks;
using IdentityServer4.Validation;

namespace IdSvrHost
{
    public class DeviceGrantValidator : ICustomGrantValidator
    {
        public Task<CustomGrantValidationResult> ValidateAsync(ValidatedTokenRequest request)
        {
            // this is a sample custom grant
            var deviceIdentifier = request.Raw.Get("deviceIdentifier");

            if (!string.IsNullOrWhiteSpace(deviceIdentifier))
            {
                // valid credential
                return Task.FromResult(new CustomGrantValidationResult(deviceIdentifier, "device"));
            }
            else
            {
                // custom error message
                return Task.FromResult(new CustomGrantValidationResult("invalid device credential"));
            }
        }

        public string GrantType => "device";
    }
}
