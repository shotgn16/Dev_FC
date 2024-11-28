using ForestChurches.Components.Roles;
using ForestChurches.Data;
using ForestChurches.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ForestChurches.Pages.Admin.Roles
{
    [Authorize(Policy = "RoleManagement.Read")]
    public class EditModel : PageModel
    {
        private readonly RoleManager<ChurchRoles> _roleManager;
        private ForestChurchesContext _context;

        public ChurchRoles Role { get; set; }
        public List<Permission> AllPermissions { get; set; }
        
        [BindProperty]
        public string[] SelectedPermissions { get; set; }

        public EditModel(RoleManager<ChurchRoles> roleManager, ForestChurchesContext context)
        {
            _roleManager = roleManager;
            _context = context;
        }

        public IActionResult OnGet(string roleId)
        {
            Role = _roleManager.Roles
                .Include(r => r.RolePermissions)
                .FirstOrDefault(r => r.Id == roleId);

            if (Role == null)
            {
                return NotFound($"Unable to load role with ID '{roleId}'...");
            }

            // Fetch all permissions
            AllPermissions = _context.Permissions.ToList();

            return Page();
        }

        [Authorize(Policy = "RoleManagement.Write")]
        public async Task<IActionResult> OnPostAsync(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                return Page();
            }

            // Clear existing permissions
            var currentPermissions = _context.RolePermissions.Where(rp => rp.RoleId == roleId);
            _context.RolePermissions.RemoveRange(currentPermissions);

            // Add new permissions
            foreach (var permissionId in SelectedPermissions)
            {
                var rolePermission = new RolePermission
                {
                    RoleId = roleId,
                    PermissionId = permissionId,
                    PermissionName = _context.Permissions.Find(permissionId).Name
                };
                _context.RolePermissions.Add(rolePermission);
            }

            _context.SaveChanges();
            return RedirectToPage("./Index");
        }
    }
}
