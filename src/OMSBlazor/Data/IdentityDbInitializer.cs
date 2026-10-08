using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace OMSBlazor.Data
{
    /// <summary>
    /// Applies pending migrations for the Identity database (creates the AspNet* tables)
    /// and seeds the default admin user.
    /// </summary>
    public static class IdentityDbInitializer
    {
        public const string DefaultAdminEmail = "admin@email.com";
        public const string DefaultAdminPassword = "1q2w3E*";

        public static async Task InitializeIdentityDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<ApplicationDbContext>>();

            var dbContext = services.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.MigrateAsync();

            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            if (await userManager.FindByEmailAsync(DefaultAdminEmail) is not null)
            {
                return;
            }

            var admin = new ApplicationUser
            {
                UserName = DefaultAdminEmail,
                Email = DefaultAdminEmail,
                // Account confirmation is required to sign in (RequireConfirmedAccount = true)
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(admin, DefaultAdminPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to seed default admin user: {errors}");
            }

            logger.LogInformation("Seeded default admin user {Email}", DefaultAdminEmail);
        }
    }
}
