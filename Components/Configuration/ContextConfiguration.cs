using ForestChurches.Components.Roles;
using ForestChurches.Components.Users;
using ForestChurches.Data;
using Microsoft.EntityFrameworkCore;

namespace ForestChurches.Components.Configuration
{
    public class ContextConfiguration
    {
        internal static void ConfigureDatabaseContext(IServiceCollection services, string connectionString)
        {
            // Register the DBContext
            services.AddDbContext<ForestChurchesContext>(options => options
                .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString),
                    mysqloptions => mysqloptions.EnableStringComparisonTranslations())
                .EnableDetailedErrors()
                    .EnableSensitiveDataLogging());

            // Register the ASP.NET Identity 'ChurchAccount' and 'ChurchRole'
            services.AddDefaultIdentity<ChurchAccount>(options => options.
                SignIn.RequireConfirmedAccount = true)
            .AddRoles<ChurchRoles>().AddEntityFrameworkStores<ForestChurchesContext>();
        }
    }
}
