using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CourseFlow.Api;

public static class DatabaseExtensions
{
    public static async Task ApplyMigrationsAsync(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        await using ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await using ApplicationIdentityDbContext identityDbContext = scope.ServiceProvider.GetRequiredService<ApplicationIdentityDbContext>();

        try
        {
            await dbContext.Database.MigrateAsync();
            app.Logger.LogInformation("Application Database migrations applied successfully.");

            await identityDbContext.Database.MigrateAsync();
            app.Logger.LogInformation("Identity Database migrations applied successfully.");
        }
        catch (Exception ex)
        {
            app.Logger.LogError(ex, "An error ocurred while applying database migrations.");
            throw;
        }
    }

    public static async Task SeedInitialDataAsync(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        RoleManager<IdentityRole> roleManager =
        scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        UserManager<IdentityUser> userManager =
        scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

        app.Logger.LogInformation("Trying to execute initial seed...");

        try
        {
            string[] roles = {Roles.Admin, Roles.Instructor, Roles.Student};
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var result = await roleManager.CreateAsync(new IdentityRole(role));

                    if (!result.Succeeded)
                    {
                        throw new Exception($"Failed creating role {role}");
                    }
                }
            }

            app.Logger.LogInformation("Initial data seeded successfully.");
        }
        catch (Exception ex)
        {
            app.Logger.LogError(ex, "An error occured while seeding initial data.");
            throw;
        }

        app.Logger.LogInformation("Trying to create new admin user...");

       try
        {
            var adminEmail = "admin@couseflow.com";
            var adminPassword = "Admin@123";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);

                if (!result.Succeeded)
                {
                    throw new Exception("Failed to create admin user");
                }

                await userManager.AddToRoleAsync(adminUser, Roles.Admin);
            }

            app.Logger.LogInformation("Admin user created successfully!");
        }
        catch (Exception ex)
        {
            app.Logger.LogError(ex, "An error occured while creating admin user.");
            throw;
        }
    }
}
