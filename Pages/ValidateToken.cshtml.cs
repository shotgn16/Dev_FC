using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ForestChurches.Pages
{
    public class ValidateTokenModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ValidateTokenModel> _logger;

        public ValidateTokenModel(IHttpClientFactory httpClientFactory, ILogger<ValidateTokenModel> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        [BindProperty(SupportsGet = true)]
        public string Token { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool TokenValidated { get; set; } = false;

        public bool IsValidating { get; private set; } = true;
        public bool IsTokenValid { get; private set; } = false;

        public async Task<IActionResult> OnGetAsync()
        {
            if (string.IsNullOrEmpty(Token))
            {
                _logger.LogWarning("Token is null or empty.");
                IsValidating = false;
                return Page();
            }

            if (TokenValidated)
            {
                _logger.LogInformation("Token has already been validated.");
                return Page();
            }

            var request = HttpContext.Request;
            var baseAddress = $"{request.Scheme}://{request.Host}";

            var client = _httpClientFactory.CreateClient();

            var requestUri = $"/Callback/ValidateToken?token={Token}";
            _logger.LogInformation("Requesting URL: {RequestUri}", requestUri);

            var response = await client.GetAsync(baseAddress + requestUri);

            if (response.IsSuccessStatusCode)
            {
                IsTokenValid = true;
                var redirectUrl = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Token is valid. Redirecting to: {RedirectUrl}", redirectUrl);
                return Redirect(redirectUrl);
            }

            _logger.LogWarning("Token validation failed with status code: {StatusCode}", response.StatusCode);
            IsValidating = false;
            return Page();
        }
    }
}
