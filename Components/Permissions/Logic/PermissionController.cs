using ForestChurches.Components.Permissions;
using ForestChurches.Components.Roles;
using ForestChurches.Data;
using ForestChurches.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ForestChurches.Components.Permissions.Logic
{
    public class PermissionController : PermissionsInterface
    {
        private readonly ForestChurchesContext _context;
        private readonly RoleManager<ChurchRoles> _roleManager;

        public PermissionController(ForestChurchesContext context, RoleManager<ChurchRoles> roleManager)
        {
            _context = context;
            _roleManager = roleManager;
        }

        public async Task AddPermissionsAsync()
        {
            List<string> appPermissions = new();

            var baseType = typeof(AppPermissions);
            var assemply = baseType.Assembly;

            var nestedTypes = baseType.GetNestedTypes(BindingFlags.Public); // Retrieve all public nested types

            foreach (var nestedType in nestedTypes)
            {
                var permissions = nestedType.GetFields(BindingFlags.Public | BindingFlags.Static)
                    .Where(f => f.FieldType == typeof(string))
                    .Select(f => (string)f.GetValue(null));

                appPermissions.AddRange(permissions);
            }

            // Retrieve all existing permissions asynchronously
            var allPermissions = await _context.Permissions.ToListAsync();

            foreach (var name in appPermissions)
            {
                // Check if permission already exists
                if (allPermissions.All(p => p.Name != name))
                {
                    // Add new permission
                    _context.Permissions.Add(new Permission
                    {
                        Name = name,
                        Id = Guid.NewGuid().ToString(),
                        Description = name
                    });
                }
            }

            // Save changes asynchronously
            await _context.SaveChangesAsync();
        }
    }
}
