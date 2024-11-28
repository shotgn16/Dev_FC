namespace ForestChurches.Components.Configuration
{
    public class PolicyConfiguration
    {
        public static void AddPolicies(IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin.Read", policy => policy.Requirements.Add(new CustomAuthorizeAttribute(Permissions.AppPermissions.Admin.Read)));
                options.AddPolicy("UserEvents.View", policy => policy.Requirements.Add(new CustomAuthorizeAttribute(Permissions.AppPermissions.UserEvents.Read)));
                options.AddPolicy("UserEvents.Edit", policy => policy.Requirements.Add(new CustomAuthorizeAttribute(Permissions.AppPermissions.UserEvents.Edit)));
                options.AddPolicy("UserEvents.Add", policy => policy.Requirements.Add(new CustomAuthorizeAttribute(Permissions.AppPermissions.UserEvents.Add)));
                options.AddPolicy("UserManagement.Read", policy => policy.Requirements.Add(new CustomAuthorizeAttribute(Permissions.AppPermissions.UserManagement.Read)));
                options.AddPolicy("UserManagement.Write", policy => policy.Requirements.Add(new CustomAuthorizeAttribute(Permissions.AppPermissions.UserManagement.Write)));
                options.AddPolicy("UserManagement.Edit", policy => policy.Requirements.Add(new CustomAuthorizeAttribute(Permissions.AppPermissions.UserManagement.Edit)));
                options.AddPolicy("RoleManagement.Read", policy => policy.Requirements.Add(new CustomAuthorizeAttribute(Permissions.AppPermissions.RoleManagement.Read)));
                options.AddPolicy("RoleManagement.Write", policy => policy.Requirements.Add(new CustomAuthorizeAttribute(Permissions.AppPermissions.RoleManagement.Write)));
                options.AddPolicy("RoleManagement.Delete", policy => policy.Requirements.Add(new CustomAuthorizeAttribute(Permissions.AppPermissions.RoleManagement.Delete)));
                options.AddPolicy("RoleManagement.Edit", policy => policy.Requirements.Add(new CustomAuthorizeAttribute(Permissions.AppPermissions.RoleManagement.Edit)));
                options.AddPolicy("EventManagement.Read", policy => policy.Requirements.Add(new CustomAuthorizeAttribute(Permissions.AppPermissions.EventManagement.Read)));
                options.AddPolicy("EventManagement.Write", policy => policy.Requirements.Add(new CustomAuthorizeAttribute(Permissions.AppPermissions.EventManagement.Write)));
                options.AddPolicy("EventManagement.Edit", policy => policy.Requirements.Add(new CustomAuthorizeAttribute(Permissions.AppPermissions.EventManagement.Edit)));
                options.AddPolicy("EventManagement.Delete", policy => policy.Requirements.Add(new CustomAuthorizeAttribute(Permissions.AppPermissions.EventManagement.Delete)));
            });
        }
    }
}
