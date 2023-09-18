using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Validation;

namespace FreeCourse.IdentityServer.Services
{
    public class TokenExchangeExtensionGrantValidator : IExtensionGrantValidator
    {
        private readonly ITokenValidator _tokenValidator;

        public TokenExchangeExtensionGrantValidator(ITokenValidator tokenValidator)
        {
            _tokenValidator = tokenValidator;
        }


        public string GrantType => "urn:ietf:params:oauth:grant-type:token-exchange";

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            var requestRaw = context.Request.Raw;

            var token = context.Request.Raw.Get("subject_token");

            if (string.IsNullOrEmpty(token))
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest, "token missing");
                return;
            }

            var tokenValidateResult = await _tokenValidator.ValidateAccessTokenAsync(token);

            if (tokenValidateResult.IsError)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "token invalid");
                return;
            }

            var subjectClaim = tokenValidateResult.Claims.FirstOrDefault(x => x.Type == "sub");

            if (subjectClaim is null)
            {
                context.Result =
                    new GrantValidationResult(TokenRequestErrors.InvalidGrant, "token must contain sub value");
                return;
            }

            context.Result = new GrantValidationResult(subjectClaim.Value, "access_token", tokenValidateResult.Claims);

            return;
        }
    }
}