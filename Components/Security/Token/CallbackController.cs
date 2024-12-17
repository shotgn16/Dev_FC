using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ForestChurches.Components.Token
{
    [Route("Callback")]
    public class CallbackController : Controller
    {
        private readonly CallbackTokenDataFormat _tokenDataFormat;
        private readonly ILogger<CallbackController> _logger;

        public CallbackController(IDataProtectionProvider dataProtectionProvider, ILogger<CallbackController> logger)
        {
            _logger = logger;
            _tokenDataFormat = new CallbackTokenDataFormat(dataProtectionProvider, "User Registration");
        }

        [HttpGet("ValidateToken")]
        public IActionResult ValidateToken(string token)
        {
            try
            {
                _logger.LogInformation("Received token: {Token}", token);
                var callbackToken = _tokenDataFormat.Unprotect(token);

                _logger.LogInformation("Unprotected token: {CallbackToken}", JsonConvert.SerializeObject(callbackToken));

                if (callbackToken.ExpirationTime < DateTime.UtcNow)
                {
                    return BadRequest("Token has expired.");
                }

                return Ok(callbackToken.RedirectUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Token validation failed.");
                return BadRequest("Token is invalid.");
            }
        }
    }
}
