using EasyPay.Common;
using EasyPay.Domain.Entities.AccountManagement;
using EasyPay.Domain.Entities.Identity;
using EasyPay.Domain.Entities.Report;
using EasyPay.Domain.ValueObjects.Report;
using EasyPay.Shared.Enums.AccountManagement;
using EasyPay.Shared.Enums.Identity;
using EasyPay.Shared.Enums.Report;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EasyPay.Infrastructure.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider, bool clearFirst = false)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var logger = serviceProvider.GetRequiredService<ILogger<DbInitializer>>();

            // Apply any pending migrations
            await context.Database.MigrateAsync();
            logger.LogInformation("Database migrations applied.");

            if (clearFirst)
            {
                await ClearDatabaseAsync(context, logger);
            }

            await RunSeedingTasksAsync(serviceProvider);
        }

        private static async Task RunSeedingTasksAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var logger = serviceProvider.GetRequiredService<ILogger<DbInitializer>>();

            await SeedRolesAndPermissionsAsync(roleManager, logger);
            await SeedUsersAsync(userManager, logger);
            await SeedCoreDataAsync(context, logger);
            await SeedUserSpecificDataAsync(context, userManager, logger);
        }

        private static async Task ClearDatabaseAsync(ApplicationDbContext context, ILogger logger)
        {
            logger.LogInformation("CLEARING database tables...");

            // The order of deletion is crucial to avoid foreign key constraint violations.
            var tablesToClear = new[]
            {
                "\"Transaction\"",
                "\"BillBase\"",
                "\"AccountTypeDocumentRequirements\"",
                "\"AuthItemValues\"",
                "\"VerifyPasswordHistory\"",
                "\"account\".\"Accounts\"",
                "\"BankCards\"",
                "\"AspNetUserTokens\"",
                "\"AspNetUserRoles\"",
                "\"AspNetUserLogins\"",
                "\"AspNetUserClaims\"",
                "\"AspNetRoleClaims\"",
                "\"AdminUser\"",
                "\"NaturalUser\"",
                "\"AspNetUsers\"",
                "\"AspNetRoles\"",
                "\"AccountTypes\"",
                "\"AuthItems\""
            };

            foreach (var table in tablesToClear)
            {
                try
                {
                    await context.Database.ExecuteSqlRawAsync($"DELETE FROM {table}");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Could not clear table {TableName}", table);
                }
            }

            logger.LogInformation("All specified tables cleared successfully.");
        }

        private static async Task SeedRolesAndPermissionsAsync(RoleManager<IdentityRole> roleManager, ILogger logger)
        {
            // Seed Roles
            if (!await roleManager.RoleExistsAsync(SystemRoles.SuperAdmin))
            {
                await roleManager.CreateAsync(new IdentityRole(SystemRoles.SuperAdmin));
                logger.LogInformation("'{RoleName}' role created.", SystemRoles.SuperAdmin);
            }

            if (!await roleManager.RoleExistsAsync(SystemRoles.BasicUser))
            {
                await roleManager.CreateAsync(new IdentityRole(SystemRoles.BasicUser));
                logger.LogInformation("'{RoleName}' role created.", SystemRoles.BasicUser);
            }

            // Seed Permissions for SuperAdmin
            var adminRole = await roleManager.FindByNameAsync(SystemRoles.SuperAdmin);
            if (adminRole == null)
            {
                logger.LogError("SuperAdmin role not found. Cannot seed permissions.");
                return;
            }

            var allPermissions = new List<string>();
            var nestedTypes = typeof(Permissions).GetNestedTypes(BindingFlags.Public);
            foreach (var type in nestedTypes)
            {
                var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                                 .Where(fi => fi.IsLiteral && !fi.IsInitOnly);
                allPermissions.AddRange(fields.Select(fi => (string)fi.GetRawConstantValue()));
            }

            var currentClaims = await roleManager.GetClaimsAsync(adminRole);
            var permissionsToAdd = allPermissions.Where(p => !currentClaims.Any(c => c.Type == "Permission" && c.Value == p)).ToList();

            foreach (var permission in permissionsToAdd)
            {
                await roleManager.AddClaimAsync(adminRole, new Claim("Permission", permission));
            }
            if (permissionsToAdd.Any())
            {
                logger.LogInformation("{Count} permissions added to role '{RoleName}'.", permissionsToAdd.Count, adminRole.Name);
            }

            var basicUserRole = await roleManager.FindByNameAsync(SystemRoles.BasicUser);
            if (basicUserRole != null)
            {
                // Define the specific permissions for a basic user
                var basicPermissions = new List<string>
        {
            Permissions.Accounts.View,
            Permissions.Accounts.Create,
            Permissions.Accounts.Edit, // To change account title
            Permissions.Accounts.Delete, // Soft delete their own accounts
            
            Permissions.AccountTypes.View, // To see available account types when creating an account
            
            Permissions.BankCards.View,
            Permissions.BankCards.Create,
            Permissions.BankCards.Edit,
            Permissions.BankCards.Delete,

            Permissions.Bills.View,
            Permissions.Bills.Create,
            Permissions.Bills.Edit,
            Permissions.Bills.Delete
        };

                var currentBasicClaims = await roleManager.GetClaimsAsync(basicUserRole);
                var basicPermissionsToAdd = basicPermissions.Where(p => !currentBasicClaims.Any(c => c.Type == "Permission" && c.Value == p)).ToList();

                foreach (var permission in basicPermissionsToAdd)
                {
                    await roleManager.AddClaimAsync(basicUserRole, new Claim("Permission", permission));
                }

                if (basicPermissionsToAdd.Any())
                {
                    logger.LogInformation("{Count} permissions added to role '{RoleName}'.", basicPermissionsToAdd.Count, basicUserRole.Name);
                }
            }
            else
            {
                logger.LogError("BasicUser role not found. Cannot seed permissions.");
            }
        }

        private static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager, ILogger logger)
        {
            // Seed SuperAdmin User
            if (await userManager.FindByNameAsync("admin@easypay.local") == null)
            {
                var adminUser = new AdminUser
                {
                    UserName = "admin@easypay.local",
                    Email = "admin@easypay.local",
                    FirstName = "Super",
                    LastName = "Admin",
                    EmailConfirmed = true,
                    PhoneNumber = "09000000000",
                    PhoneNumberConfirmed = true,
                    IsActive = true,
                    CurrentStep = RegistrationStep.Completed,
                    UserType = UserType.Admin,
                    EmployeeCode = "EASY-001",
                    Department = "IT",
                    AdminLevel = AdminLevel.SuperAdmin
                };
                var result = await userManager.CreateAsync(adminUser, "AdminPa$$w0rd!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, SystemRoles.SuperAdmin);
                    logger.LogInformation("SuperAdmin user created and assigned to role.");
                }
            }

            // Seed Basic User 1 (Fully registered)
            if (await userManager.FindByNameAsync("09123456789") == null)
            {
                var basicUser1 = new NaturalUser
                {
                    UserName = "09123456789",
                    PhoneNumber = "09123456789",
                    PhoneNumberConfirmed = true,
                    FirstName = "John",
                    LastName = "Doe",
                    NationalCode = "1234567890",
                    FatherName = "Richard",
                    BirthDate = new DateTime(1990, 5, 15),
                    Gender = GenderType.Male,
                    IsActive = true,
                    UserType = UserType.Natural,
                    CurrentStep = RegistrationStep.Completed
                };
                var result = await userManager.CreateAsync(basicUser1, "UserPa$$w0rd1!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(basicUser1, SystemRoles.BasicUser);
                    logger.LogInformation("Basic user 'John Doe' created.");
                }
            }
        }

        private static async Task SeedCoreDataAsync(ApplicationDbContext context, ILogger logger)
        {
            // Seed AuthItems
            if (!await context.AuthItems.AnyAsync())
            {
                await context.AuthItems.AddRangeAsync(
                    new AuthItem { Title = "National ID Card", Description = "A government-issued identification card.", AuthItemType = AuthItemType.Document, AuthItemValueType = AuthItemValueType.String },
                    new AuthItem { Title = "Proof of Address", Description = "A utility bill or bank statement showing your address.", AuthItemType = AuthItemType.Document, AuthItemValueType = AuthItemValueType.String }
                );
                await context.SaveChangesAsync();
                logger.LogInformation("AuthItems seeded.");
            }

            // Seed AccountTypes
            if (!await context.AccountTypes.AnyAsync())
            {
                context.AccountTypes.AddRange(
                    new AccountType
                    {
                        Title = "Standard Current Account",
                        Code = "SCA",
                        Description = "A standard account for daily transactions.",
                        MinimumOpeningBalance = 50,
                        IsActive = true,
                        CreatedBy = "System",
                        ModifiedBy = "System"
                    },
                    new AccountType
                    {
                        Title = "High-Interest Savings",
                        Code = "HIS",
                        Description = "An account for savings with a competitive interest rate.",
                        MinimumOpeningBalance = 500,
                        InterestRate = 2.5m,
                        IsActive = true,
                        CreatedBy = "System",
                        ModifiedBy = "System"
                    }
                );
                await context.SaveChangesAsync();
                logger.LogInformation("AccountTypes seeded.");
            }

            // Seed AccountTypeDocumentRequirements
            if (!await context.AccountTypeDocumentRequirements.AnyAsync())
            {
                var savingsAccount = await context.AccountTypes.FirstOrDefaultAsync(at => at.Code == "HIS");
                var nationalId = await context.AuthItems.FirstOrDefaultAsync(ai => ai.Title == "National ID Card");
                if (savingsAccount != null && nationalId != null)
                {
                    context.AccountTypeDocumentRequirements.Add(new AccountTypeDocumentRequirement
                    {
                        AccountTypeId = savingsAccount.Id,
                        AuthItemId = nationalId.Id,
                        IsRequired = true,
                        Order = 1,
                        ValidationRules = ""
                    });
                    await context.SaveChangesAsync();
                    logger.LogInformation("AccountTypeDocumentRequirements seeded.");
                }
            }
        }

        private static async Task SeedUserSpecificDataAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ILogger logger)
        {
            var user1 = await userManager.FindByNameAsync("09123456789") as NaturalUser;
            if (user1 == null) return;

            // Seed Account for User 1
            if (!await context.Accounts.AnyAsync(a => a.OwnerUserId == user1.Id))
            {
                var standardAccountType = await context.AccountTypes.SingleAsync(at => at.Code == "SCA");

                var account = new Account
                {
                    Title = "Primary Account",
                    AccountNumber = "6271234567",
                    AccountTypeId = standardAccountType.Id,
                    OwnerUserId = user1.Id,
                    Status = AccountStatus.Active,
                    OpeningDate = DateTime.UtcNow.AddDays(-30),
                    CurrentBalance = 5000,
                    LastActivityDate = DateTime.UtcNow
                };

                var initialDeposit = new Transaction(
                    accountId: Guid.Empty, 
                    amount: 5000,
                    transactionType: TransactionType.Deposit,
                    referenceId: Guid.NewGuid().ToString(),
                    metadata: new TransactionMetadata("127.0.0.1", "SeedData"),
                    description: "Initial deposit"
                );

                initialDeposit.Account = account;

                await context.Accounts.AddAsync(account);
                await context.Transaction.AddAsync(initialDeposit);

                await context.SaveChangesAsync();
                logger.LogInformation("Account and initial transaction created for {UserName}.", user1.UserName);
            }
        }
    }

    public class DbInitializer { } // Dummy class for typed ILogger
}