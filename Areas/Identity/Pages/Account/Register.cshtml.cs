// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.Diagnostics.CodeAnalysis;
using ForestChurches.Components.ImageHandler;
using ForestChurches.Components.Email;
using Newtonsoft.Json;
using ForestChurches.Components.Configuration;
using ForestChurches.Components.UserRegistration;
using ForestChurches.Components.Users;
using ForestChurches.Data;
using Microsoft.EntityFrameworkCore;
using ServiceStack;

namespace ForestChurches.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ChurchAccount> _signInManager;
        private readonly UserManager<ChurchAccount> _userManager;
        private readonly IUserStore<ChurchAccount> _userStore;
        private readonly IUserEmailStore<ChurchAccount> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly ForestChurchesContext _context;
        private readonly iEmail _mailRepository;
        private readonly iMailSender _mailSender;
        internal readonly Configuration _configuration;
        private readonly iRegistrationGenerate _registrationGenerator; 

        private ImageInterface _iImage;

        public RegisterModel(
            UserManager<ChurchAccount> userManager,
            IUserStore<ChurchAccount> userStore,
            SignInManager<ChurchAccount> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            iMailSender mailSender,
            ImageInterface iImage,
            iEmail mailRepository,
            ForestChurchesContext context,
            Configuration configuration,
            iRegistrationGenerate registrationGenerator)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _mailSender = mailSender;
            _emailSender = emailSender;
            _iImage = iImage;
            _mailRepository = mailRepository;
            _configuration = configuration;
            _configuration = configuration;
            _context = context;
            _registrationGenerator = registrationGenerator;
        }

        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }
        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public List<string> whitelistedUsers { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }
        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>        
        public bool isTokenValid { get; set; } = true;
        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            // Custom Parameters
            [AllowNull]
            [PersonalData]
            public byte[] ImageArray { get; set; }

            [AllowNull]
            [PersonalData]
            public IFormFile Image { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null, string email = "")
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            Input = new InputModel();
            Input.Email = email;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            try
            {
                whitelistedUsers = await _context.WhitelistedUsers.Select(a => a.Email).ToListAsync();

                returnUrl ??= Url.Content("~/");
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
                if (ModelState.IsValid && _registrationGenerator.ValidateEmail(Input.Email) == true && whitelistedUsers.Contains(Input.Email))
                {
                    var user = CreateUser();

                    user.CreatedDate = DateTime.UtcNow;

                    if (Input.Image == null)
                    {
                        user.ImageArray = await _iImage.ConvertToByteArrayFromUrl("https://i.imgur.com/oLC9RcU.png");
                    }

                    else if (Input.Image != null)
                    {
                        user.ImageArray = await _iImage.ConvertToByteArray(Input.Image);
                    }

                    await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                    await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                    var result = await _userManager.CreateAsync(user, Input.Password);

                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User created a new account with password.");
                        StatusMessage = $"User '{user.UserName}' successfully registered!";

                        var userId = await _userManager.GetUserIdAsync(user);
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callbackUrl = Url.Page(
                            "/Account/ConfirmEmail",
                            pageHandler: null,
                            values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                            protocol: Request.Scheme);

                        // Mark regisration as a success.
                        _registrationGenerator.MarkAsCompleted(Input.Email, DateTime.UtcNow);

                        var id = _userManager.FindByEmailAsync(Input.Email);
                        _mailSender.SendEmailConfirmAccount(id.Id.ConvertTo<Guid>(), Input.Email, callbackUrl);

                        //var userData = new Dictionary<string, string>()
                        //{ { "{confirm_email_link}", HtmlEncoder.Default.Encode(callbackUrl) } };
                        //await _mailRepository.StartEmailAsync(Input.Email, userData, "Confirm your email", "./templates/confirm_email.html");


                        if (_userManager.Options.SignIn.RequireConfirmedAccount)
                        {
                            return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                        }
                        else
                        {
                            await _signInManager.SignInAsync(user, isPersistent: false);
                            return LocalRedirect(returnUrl);
                        }
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }

                else if (_registrationGenerator.ValidateEmail(Input.Email) == false)
                {
                    ModelState.AddModelError("Email", "Email is already registered or the registration has expired...");
                    StatusMessage = "Error: Email is already registered or the registration has expired...";
                    return Page();
                }

                else if (!whitelistedUsers.Contains(Input.Email))
                {
                    throw new Exception("Error! Unknown email address. Please try again or contact us for further information using the contact forum or email us at 'info@forestchurches.co.uk'.");
                }
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());

                StatusMessage = ex.Message;
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private ChurchAccount CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ChurchAccount>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ChurchAccount)}'. " +
                    $"Ensure that '{nameof(ChurchAccount)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<ChurchAccount> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ChurchAccount>)_userStore;
        }

        private bool HashExpired(DateTime timestamp, int timeoutMinutes)
        {
            return DateTime.UtcNow > timestamp.AddMinutes(timeoutMinutes);
        }

    }
}
