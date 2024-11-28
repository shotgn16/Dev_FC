using ForestChurches.Components.ImageHandler;
using ForestChurches.Components.Roles;
using ForestChurches.Components.Users;
using ForestChurches.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ForestChurches.Pages.Admin.Users
{
    [Authorize(Policy = "UserManagement.Write")]
    public class RegisterModel : PageModel
    {
        [BindProperty]
        public Registration Input { get; set; }

        private readonly SignInManager<ChurchAccount> _signInManager;
        private readonly UserManager<ChurchAccount> _userManager;
        private readonly IUserStore<ChurchAccount> _userStore;
        private readonly IUserEmailStore<ChurchAccount> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private RoleManager<ChurchRoles> _roleManager;
        private readonly IEmailSender _emailSender;
        private ImageInterface _iImage;

        public IList<ChurchRoles> Roles;

        [TempData]
        public string StatusMessage { get; set; }

        public RegisterModel(
            UserManager<ChurchAccount> userManager,
            IUserStore<ChurchAccount> userStore,
            SignInManager<ChurchAccount> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            ImageInterface iImage,
            RoleManager<ChurchRoles> roleManager)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _iImage = iImage;
            _roleManager = roleManager;

            Input = new Registration();
        }

        public void OnGet(string username)
        {
            Input.Email = username;
            Roles = _roleManager.Roles.ToList() ?? null;
        }

        public async Task<ActionResult> OnPostRegisterUser()
        {
            bool b = false;

            if (Input != null)
            {
                var user = CreateUser();

                // Profile image
                if (Input.Image == null) {
                    user.ImageArray = await _iImage.ConvertToByteArrayFromUrl("https://i.imgur.com/oLC9RcU.png");
                }

                else if (Input.Image != null) {
                    user.ImageArray = await _iImage.ConvertToByteArray(Input.Image);
                }

                // User roles
                if (Input.Roles != null) {
                    await _userManager.AddToRolesAsync(user, Input.Roles);
                    user.Role = string.Join(", ", Input.Roles ?? null);
                }

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);

                b = result.Succeeded;

                if (result.Succeeded == true) 
                {
                    _logger.LogInformation($"User: '{user.Email}' created!");

                    var confirmAccountResult = await 
                        _userManager.ConfirmEmailAsync(
                            user, await _userManager
                                .GenerateEmailConfirmationTokenAsync(user));

                    _logger.LogInformation($"Account: '{user.Email}' successfully confirmed!");

                    StatusMessage = $"User '{user.Email}' successfully created...";
                    return RedirectToPage();
                }

                else if (result.Succeeded == false)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);

                        _logger.LogError(error.Code, error.Description);
                    }

                    StatusMessage = $"An error occurred while creating user '{user.Email}'...";
                }
            }

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
    }
}
