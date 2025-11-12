using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Authentication.API.Middleware
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string Permission { get; }

        public PermissionRequirement(string permission)
        {
            Permission = permission;
        }
    }

    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            var permissionClaims = context.User.Claims
                .Where(c => c.Type == "permission")
                .Select(c => c.Value);

            if (permissionClaims.Contains(requirement.Permission))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }

    public class RoleRequirement : IAuthorizationRequirement
    {
        public string Role { get; }

        public RoleRequirement(string role)
        {
            Role = role;
        }
    }

    public class RoleHandler : AuthorizationHandler<RoleRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            RoleRequirement requirement)
        {
            var userRole = context.User.FindFirst("role")?.Value;

            if (userRole == requirement.Role)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }

    public static class AuthorizationPolicyExtensions
    {
        public static void AddRoleBasedPolicies(this IServiceCollection services)
        {
            services.AddScoped<IAuthorizationHandler, PermissionHandler>();
            services.AddScoped<IAuthorizationHandler, RoleHandler>();

            services.AddAuthorization(options =>
            {
                // Role-based policies
                options.AddPolicy("CustomerOnly", policy => policy.Requirements.Add(new RoleRequirement("Customer")));
                options.AddPolicy("BillerOnly", policy => policy.Requirements.Add(new RoleRequirement("Biller")));
                options.AddPolicy("OperatorOnly", policy => policy.Requirements.Add(new RoleRequirement("Operator")));
                options.AddPolicy("WorkerOnly", policy => policy.Requirements.Add(new RoleRequirement("Worker")));
                options.AddPolicy("DeliveryAgentOnly", policy => policy.Requirements.Add(new RoleRequirement("DeliveryAgent")));
                options.AddPolicy("DeveloperOnly", policy => policy.Requirements.Add(new RoleRequirement("Developer")));
                options.AddPolicy("TesterOnly", policy => policy.Requirements.Add(new RoleRequirement("Tester")));
                options.AddPolicy("NetworkAdminOnly", policy => policy.Requirements.Add(new RoleRequirement("NetworkAdmin")));
                options.AddPolicy("DatabaseAdminOnly", policy => policy.Requirements.Add(new RoleRequirement("DatabaseAdmin")));

                // Combined role policies
                options.AddPolicy("RestaurantStaff", policy =>
                    policy.RequireAssertion(context =>
                        context.User.HasClaim("role", "Biller") ||
                        context.User.HasClaim("role", "Operator") ||
                        context.User.HasClaim("role", "Worker")));

                options.AddPolicy("ITStaff", policy =>
                    policy.RequireAssertion(context =>
                        context.User.HasClaim("role", "Developer") ||
                        context.User.HasClaim("role", "Tester") ||
                        context.User.HasClaim("role", "NetworkAdmin") ||
                        context.User.HasClaim("role", "DatabaseAdmin")));

                options.AddPolicy("ManagementLevel", policy =>
                    policy.RequireAssertion(context =>
                        context.User.HasClaim("role", "Biller") ||
                        context.User.HasClaim("role", "Operator")));

                // Permission-based policies
                options.AddPolicy("CanReceivePayments", policy => policy.Requirements.Add(new PermissionRequirement("receive_payments")));
                options.AddPolicy("CanConfirmOrders", policy => policy.Requirements.Add(new PermissionRequirement("confirm_orders")));
                options.AddPolicy("CanPrepareFood", policy => policy.Requirements.Add(new PermissionRequirement("prepare_food")));
                options.AddPolicy("CanConfirmDelivery", policy => policy.Requirements.Add(new PermissionRequirement("confirm_delivery")));
                options.AddPolicy("CanAccessAllEndpoints", policy => policy.Requirements.Add(new PermissionRequirement("access_all_endpoints")));
                options.AddPolicy("CanAccessHealthcheck", policy => policy.Requirements.Add(new PermissionRequirement("access_healthcheck_api")));
                options.AddPolicy("CanAccessDatabase", policy => policy.Requirements.Add(new PermissionRequirement("access_database")));
            });
        }
    }

    // Custom authorization attributes
    public class RequirePermissionAttribute : AuthorizeAttribute
    {
        public RequirePermissionAttribute(string permission) : base($"RequirePermission_{permission}")
        {
        }
    }

    public class RequireRoleAttribute : AuthorizeAttribute
    {
        public RequireRoleAttribute(string role) : base($"{role}Only")
        {
        }
    }
}