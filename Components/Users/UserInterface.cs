using ForestChurches.Components.Roles;
using Microsoft.AspNetCore.Identity;

namespace ForestChurches.Components.Users
{
    public interface UserInterface
    {
        Task SeedChurchUserAsync(UserManager<ChurchAccount> userManager, RoleManager<ChurchRoles> roleManager);
        Task SeedSuperAdminAsync(UserManager<ChurchAccount> userManager, RoleManager<ChurchRoles> roleManager);
    }
}
