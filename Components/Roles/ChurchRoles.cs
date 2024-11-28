using ForestChurches.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;

namespace ForestChurches.Components.Roles
{
    public class ChurchRoles : IdentityRole
    {
        public ChurchRoles() : base() { }

        public ChurchRoles(string roleName) : base(roleName) { }

        public List<RolePermission> RolePermissions { get; set; }
    }
}
