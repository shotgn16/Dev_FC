using ForestChurches.Components;
using ForestChurches.Components.Roles;
using ForestChurches.Data;
using ForestChurches.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace ForestChurches.Components.Users
{
    public class UsersController : Controller, UserInterface
    {
        private ForestChurchesContext _context;
        private Configuration.Configuration _configuration;

        public UsersController(ForestChurchesContext context, Configuration.Configuration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task SeedChurchUserAsync(UserManager<ChurchAccount> userManager, RoleManager<ChurchRoles> roleManager)
        {
            // Create the 'Demo' User
            var defaultUser = new ChurchAccount
            {
                Email = _configuration.Client.GetSecret("sys-demo-username").Value.Value,
                UserName = _configuration.Client.GetSecret("sys-demo-username").Value.Value,
                EmailConfirmed = true,
                ImageArray = await ConvertLinkToByteArray("https://i.imgur.com/oLC9RcU.png"),
                Role = Roles.Roles.AuthorizedChurch.ToString()
            };

            if (userManager.Users.All(u => u.Id != defaultUser.Id))
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);

                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, _configuration.Client.GetSecret("sys-demo-password").Value.Value);

                    await userManager.AddToRoleAsync(defaultUser, Roles.Roles.AuthorizedChurch.ToString());

                    // Create a ChurchInformation object
                    var churchInformation1 = new ChurchInformation
                    {
                        ID = Guid.NewGuid().ToString(),
                        ChurchAccountId = defaultUser.Id,
                        Name = "St. John's Church",
                        Denominaion = "Baptist",
                        Website = "https://stjohnsbaptist.co.uk",
                        Status = "Open",
                        Address = "4725 Sed St.",
                        Description = "A local baptist church in the center of quiet town.",
                        Activities = new List<string> { "Totts - Toddler group", "Worship Evening", "Church Meals" },
                        WheelchairAccess = true,
                        Wifi = true,
                        Parking = true,
                        Refreshments = true,
                        Restrooms = true,
                        ServiceTimes = new List<ServiceTimes>
                        {
                            new ServiceTimes { Time = new TimeOnly(10,30), Note = "Morning Communion Service" },
                            new ServiceTimes { Time = new TimeOnly(11,00), Note = "Morning Traditional Service" }
                        },
                        Churchsuite = "https://login.churchsuite.com/?account=demo_church",
                        Congregation = "100-200",
                        Phone = "076 2934 8138"
                    };

                    _context.ChurchInformation.Add(churchInformation1);
                    await _context.SaveChangesAsync();
                }
            }
        }

        // Create the 'SuperAdmin' User
        public async Task SeedSuperAdminAsync(UserManager<ChurchAccount> userManager, RoleManager<ChurchRoles> roleManager)
        {
            // Seed Super Admin User
            var superAdminUser = new ChurchAccount
            {
                Email = _configuration.Client.GetSecret("sys-admin-username").Value.Value,
                UserName = _configuration.Client.GetSecret("sys-admin-username").Value.Value,
                EmailConfirmed = true,
                ImageArray = await ConvertLinkToByteArray("https://i.imgur.com/oLC9RcU.png"),
                Role = Roles.Roles.SuperAdmin.ToString()
            };

            if (userManager.Users.All(u => u.Id != superAdminUser.Id))
            {
                var user = await userManager.FindByEmailAsync(superAdminUser.Email);

                if (user == null)
                {
                    await userManager.CreateAsync(superAdminUser, _configuration.Client.GetSecret("sys-admin-password").Value.Value);
                    await userManager.AddToRoleAsync(superAdminUser, "SuperAdmin");

                    // Create a ChurchInformation object
                    var churchInformation2 = new ChurchInformation
                    {
                        ID = Guid.NewGuid().ToString(),
                        ChurchAccountId = superAdminUser.Id,
                        Name = "St. Paul's Church",
                        Denominaion = "Anglican",
                        Website = "https://stpaulsanglican.co.uk",
                        Status = "Open",
                        Description = "A local anglican church in the center of our busy city.",
                        Activities = new List<string> { "Baby & Toddler Group", "Friday Fitness", "Home groups" },
                        WheelchairAccess = true,
                        Wifi = true,
                        Parking = true,
                        Refreshments = true,
                        Restrooms = true,
                        ServiceTimes = new List<ServiceTimes>
                        {
                            new ServiceTimes { Time = new TimeOnly(9,00), Note = "Early Morning Service" },
                            new ServiceTimes { Time = new TimeOnly(11,00), Note = "Morning Traditional Service" }
                        },
                        Address = "7250 Red St.",
                        Churchsuite = "https://login.churchsuite.com/?account=demo_church",
                        Congregation = "50-100",
                        Phone = "077 3454 9108"
                    };

                    _context.ChurchInformation.Add(churchInformation2);
                    await _context.SaveChangesAsync();
                }
            }
        }

        // Private Methods
        private async Task<byte[]> ConvertLinkToByteArray(string url)
        {
            using (WebClient client = new WebClient())
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                byte[] imagebytes = client.DownloadData(url);

                return imagebytes;
            }
        }

        public async Task AddPermissionClaim(RoleManager<ChurchRoles> roleManager, ChurchRoles role, string module)
        {
            var allPermissions = new List<string>
            {
                $"Permissions.{module}.View" ,
                $"Permissions.{module}.View" ,
                $"Permissions.{module}.Edit",
                $"Permissions.{module}.Delete"
            };

            foreach (var permission in allPermissions)
            {
                if (role.RolePermissions.Any(p => p.Permission.Name == permission))
                {
                    var claimValue = permission;
                    var claim = new Claim("Permission", claimValue);
                    await roleManager.AddClaimAsync(role, claim);
                }
            }
        }
    }
}
