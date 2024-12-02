using ForestChurches.Components.LogReader;
using ForestChurches.Components.Roles;
using ForestChurches.Components.Users;
using ForestChurches.Data;
using ForestChurches.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ForestChurches.Pages.Admin.Users
{
    [Authorize(Policy = "UserManagement.Edit")]
    public class EditModel : PageModel
    {
        private ILogger<EditModel> _logger;
        private ForestChurchesContext _context;
        private UserManager<ChurchAccount> _userManager;
        private RoleManager<ChurchRoles> _roleManager;

        public ChurchAccount currentUser { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [TempData]
        public Guid UserID { get; set; }

        [BindProperty]
        public Profile Input { get; set; }
        public IList<ChurchRoles> Roles { get; set; }

        public EditModel(
            UserManager<ChurchAccount> userManager,
            ForestChurchesContext context,
            ILogger<EditModel> logger,
            RoleManager<ChurchRoles> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public IActionResult OnGet(Guid id)
        {
            UserID = id;

            currentUser = _userManager
                .FindByIdAsync(id.ToString()).Result;

            if (currentUser == null)
            {
                return NotFound($"Unable to load user with ID '{id}'.");
            }

            var userRoles = _userManager
                .GetRolesAsync(currentUser).Result;

            Input = new Profile
            {
                Email = currentUser.Email,
                Phone = currentUser.PhoneNumber,
                Roles = userRoles.ToList() ?? new List<string>()
            };

            Roles = _roleManager.Roles.ToList();

            return Page();
        }

        // Method for updated profile information
        public async Task<IActionResult> OnPostUpdateUser(Guid id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id.ToString());

                if (user == null)
                {
                    _logger.LogError("User not found: " + Input.Email);
                    StatusMessage = $"User '{id} not found...";
                    return RedirectToPage("./Index");
                }

                // Adds user to role...
                var userRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, userRoles);

                if (Input.Roles != null)
                {
                    await _userManager.AddToRolesAsync(user, Input.Roles);
                    user.Role = string.Join(", ", Input.Roles ?? null);
                }

                // Update the properties that have changed
                user.Email = Input.Email ?? string.Empty;
                user.PhoneNumber = Input.Phone ?? string.Empty;

                if (!string.IsNullOrEmpty(Input.Password))
                {
                    user.PasswordHash = new PasswordHasher<ChurchAccount>().HashPassword(user, Input.Password);
                }

                // Save the changes
                await _userManager.UpdateAsync(user);

                StatusMessage = $"User '{user.Email}' successfully updated...";
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }

            return RedirectToPage("./Index");
        }
    }
}