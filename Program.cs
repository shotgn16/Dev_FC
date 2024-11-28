using ForestChurches.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ForestChurches;
using ForestChurches.Components.Users;
using Azure.Identity;
using System.Security.Cryptography.X509Certificates;
using Azure.Security.KeyVault.Secrets;
using ForestChurches.Components.Permissions.Logic;
using ForestChurches.Components.Configuration;
using ForestChurches.Components.Roles.logic;
using Serilog;
using ForestChurches.Components.Logging;
using Serilog.Sinks.AspNetCore.App.SignalR.Extensions;
using ForestChurches.Components.Roles;
using ServiceStack.Jobs;
using System.Reflection;
using static System.Formats.Asn1.AsnWriter;
internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        try
        {
            builder.Services.AddSession();

            // Configure Serilog logger
            // Define SignalR (Realtime data) & Serilog (Log manager)
            builder.Services.AddSignalR();
            builder.Services.AddSerilogHub<LoggingHub>();
                LoggingConfiguration.ConfigureLogging(builder.Services);

            builder.Services.AddHttpLogging(x => {
                x.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.All;
            });

            string ConnectionString = "";
            var Enviroment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            // Load certificate
            var keyVaultConfig = builder.Configuration.GetSection("KeyVault");

            var certificatePath = Enviroment == "Production" ? keyVaultConfig["CertificatePath_Production"] : keyVaultConfig["CertificatePath_Development"];

            var fullPath = Path.Combine(directory, certificatePath);
            var certificatePassword = keyVaultConfig["CertificatePassword"];
            var certificate = new X509Certificate2(fullPath, certificatePassword);

            // Use certificate for authentication
            var keyVaultURL = keyVaultConfig["KeyVaultUrl"];
            var clientId = keyVaultConfig["ClientID"];
            var tenantId = keyVaultConfig["DirectoryID"];

            var clientSecretCredential = new ClientCertificateCredential(tenantId, clientId, certificate);
            builder.Configuration.AddAzureKeyVault(new Uri(keyVaultURL), clientSecretCredential);

            var clientCertificateCredential = new ClientCertificateCredential(tenantId, clientId, certificate);
            builder.Configuration.AddAzureKeyVault(new Uri(keyVaultURL), clientCertificateCredential);

            var secretClient = new SecretClient(new Uri(keyVaultURL), clientCertificateCredential);

            if (Enviroment == "Production")
            {
                ConnectionString = secretClient.GetSecretAsync("db-connection-live").Result.Value.Value ?? throw new InvalidOperationException("Connection string not found.");
            }

            else if (Enviroment == "Development")
            {
                //string host = Environment.GetEnvironmentVariable("MYSQL_HOST");
                //string database = Environment.GetEnvironmentVariable("MYSQL_DATABASE");
                //string user = Environment.GetEnvironmentVariable("MYSQL_USER");
                //string password = Environment.GetEnvironmentVariable("MYSQL_PASSWORD");

                ConnectionString = builder.Configuration.GetConnectionString("DockerConfiguration");

                if (string.IsNullOrEmpty(ConnectionString))
                {
                    var secret = await secretClient.GetSecretAsync("db-local-testing");
                    ConnectionString = secret.Value.Value;
                }
            }

            // Configure Database Context & Identity
            ContextConfiguration.ConfigureDatabaseContext(builder.Services, ConnectionString);

            // Application user & group policies
            PolicyConfiguration.AddPolicies(builder.Services);

            // Configure session cache
            builder.Services.AddSession(options => {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // Register the ErrorController for error code handling
            builder.Services.AddControllersWithViews(options => { 
                options.Filters.Add(typeof(ErrorController)); 
            })
            .AddSessionStateTempDataProvider();

            builder.Services.AddControllers();

            builder.Services.AddLogging();

            // Register application services
            ServiceRegistration.RegisterServices(builder.Services);

            // Add services to the container.
            builder.Services.AddRazorPages()
                .AddSessionStateTempDataProvider();

            var app = builder.Build();

            // Apply Migration to database at project boot
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ForestChurchesContext>();

                IEnumerable<string> migrations = db.Database.GetAppliedMigrations();

                if (migrations.Count() == 0)
                {
                    db.Database.Migrate();
                }
            }

            // Seeding Role Permissions
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var loggerFactory = services.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger("Permission Logger");
                var permissionManager = services.GetRequiredService<PermissionsInterface>();

                try
                {
                    await permissionManager.AddPermissionsAsync();
                    logger.LogInformation("Updating permissions complete...");
                }

                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred updating permissions...");
                }
            }

            // Seeding Default Roles and Users
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var loggerFactory = services.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger("Identity Seeder");

                try
                {
                    var userManager = services.GetRequiredService<UserManager<ChurchAccount>>();
                    var roleManager = services.GetRequiredService<RoleManager<ChurchRoles>>();

                    var Roles = services.GetRequiredService<RolesInterface>();
                    var Users = services.GetRequiredService<UserInterface>();

                    await Roles.SeedRolesAsync(userManager, roleManager);
                    await Users.SeedChurchUserAsync(userManager, roleManager);
                    await Users.SeedSuperAdminAsync(userManager, roleManager);

                    logger.LogInformation("Seeding complete...");
                }

                catch (Exception ex)
                {
                    logger.LogWarning(ex, "An error occurred the database setup...");
                }
            }

            // Configure the HTTP request pipeline.
            //if (!app.Environment.IsDevelopment())
            //{
            app.UseExceptionHandler("/Error");
            app.UseHsts();
            //}

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();

            app.UseAuthentication(); // This must come first 
            app.UseAuthorization();

            // Configure Signal
            app.UseEndpoints(e =>
            {
                e.MapHub<LoggingHub>("realtimeLogging");
            });

            app.MapRazorPages();

            app.Run();
        }

        catch (Exception ex)
        {
            Log.Error(ex.Message, ex);
        }

        finally
        {
            Log.CloseAndFlush();
        }
    }
}