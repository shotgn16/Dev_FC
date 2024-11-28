using ForestChurches.Components.Roles;
using ForestChurches.Components.Users;
using ForestChurches.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ForestChurches.Pages.Admin.Roles
{
    [Authorize(Policy = "RoleManagement.Read")]
    public class IndexModel : PageModel
    {
        private readonly RoleManager<ChurchRoles> _roleManager;
        private readonly UserManager<ChurchAccount> _userManager;
        private readonly ForestChurchesContext _context;
        private readonly ILogger<IndexModel> _logger;

        public IList<ChurchRoles> Roles;

        [TempData]
        public string StatusMessage { get; set; }

        public IndexModel(RoleManager<ChurchRoles> roleManager, ILogger<IndexModel> logger, UserManager<ChurchAccount> userManager, ForestChurchesContext context)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        public void OnGet()
        {
            Roles = GetRoles().Result;
        }

        public async Task<IList<ChurchRoles>> GetRoles()
        {
            var roles = await _roleManager.Roles
                .Include(r => r.RolePermissions)
                    .ToListAsync();

            return roles;
        }

        [Authorize(Policy = "RoleManagement.Write", Roles = "SuperAdmin")]
        public async Task<IActionResult> OnPostCreateRole(string roleName)
        {
            IdentityResult actionResult = new();

            try
            {
                bool x = await _roleManager.RoleExistsAsync(roleName);
                if (!x)
                {
                    var role = new ChurchRoles();
                    role.Name = roleName;
                    actionResult = await _roleManager.CreateAsync(role);
                }

                Roles = await GetRoles();
            }

            catch (Exception ex)
            {
                // Logger
            }

            StatusMessage = actionResult.Succeeded == true ? "Role successfully created!" : "An error occurred while creating the role.";
            return Page();
        }

        [Authorize(Policy = "RoleManagement.Delete", Roles = "SuperAdmin")]
        public IActionResult OnPostDeleteRole(string value)
        {
            try
            {
                var role = _roleManager.FindByIdAsync(value).Result;
                var result = _roleManager.DeleteAsync(role).Result;

                var usersInRole = _userManager.GetUsersInRoleAsync(role.Name).Result;

                if (usersInRole.Count > 0)
                {
                    RedirectToPage("./Index");
                    StatusMessage = $"Role '{role.Name}' is in use by {usersInRole.Count} user(s) and cannot be deleted...";
                }

                if (result.Succeeded)
                {
                    _logger.LogInformation($"Role '{role.Name}' successfully deleted...");
                    StatusMessage = $"Role '{role.Name}' successfully deleted...";
                }
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }

            return RedirectToPage();
        }
    }
}
