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
        // --- User Constants for Easy Referencing ---
        private const string AdminUserName = "admin@easypay.local";
        private const string AdminPhoneNumber = "+989000000000";

        private const string User1UserName = "+989123456789";
        private const string User1Email = "john.doe@example.com";

        private const string User2UserName = "+989129876543";
        private const string User2Email = "jane.smith@example.com";

        private const string User3UserName = "+989120000000";
        private const string User3Email = "new.user@example.com";

        public static async Task InitializeAsync(IServiceProvider serviceProvider, bool clearFirst = false)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var logger = serviceProvider.GetRequiredService<ILogger<DbInitializer>>();

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

            // Seeding order is crucial to respect foreign key constraints
            await SeedRolesAndPermissionsAsync(roleManager, logger);
            await SeedCoreDataAsync(context, logger);
            await SeedUsersAsync(userManager, logger);
            await SeedAccountTypeRequirementsAsync(context, logger); // Link core data
            await SeedUserSpecificDataAsync(context, userManager, logger); // Seed data for each user
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
            Permissions.Bills.Delete,

            Permissions.AuthItem.View
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


        private static async Task SeedCoreDataAsync(ApplicationDbContext context, ILogger logger)
        {
            if (!await context.AuthItems.AnyAsync())
            {
                logger.LogInformation("Seeding AuthItems...");
                await context.AuthItems.AddRangeAsync(
                    new AuthItem { Title = "National ID Card", Description = "A government-issued identification card.", AuthItemType = AuthItemType.Document, AuthItemValueType = AuthItemValueType.String },
                    new AuthItem { Title = "Proof of Address", Description = "A recent utility bill or bank statement.", AuthItemType = AuthItemType.Document, AuthItemValueType = AuthItemValueType.String },
                    new AuthItem { Title = "Student ID Card", Description = "A valid ID from an accredited educational institution.", AuthItemType = AuthItemType.Document, AuthItemValueType = AuthItemValueType.String },
                    new AuthItem { Title = "Signature Sample", Description = "A clear image of your handwritten signature.", AuthItemType = AuthItemType.Document, AuthItemValueType = AuthItemValueType.String }
                );
                await context.SaveChangesAsync();
            }

            if (!await context.AccountTypes.AnyAsync())
            {
                logger.LogInformation("Seeding AccountTypes...");
                context.AccountTypes.AddRange(
                    new AccountType { Title = "Standard Current Account", Code = "SCA", Description = "For daily transactions.", MinimumOpeningBalance = 50, IsActive = true, CreatedBy = "System", ModifiedBy = "System" },
                    new AccountType { Title = "High-Interest Savings", Code = "HIS", Description = "Grow your savings.", MinimumOpeningBalance = 500, InterestRate = 2.5m, IsActive = true, CreatedBy = "System", ModifiedBy = "System" },
                    new AccountType { Title = "Student Advantage Account", Code = "SAA", Description = "No fees for students.", MinimumOpeningBalance = 0, MaximumAgeRequirement = 25, IsActive = true, CreatedBy = "System", ModifiedBy = "System" }
                );
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager, ILogger logger)
        {
            // Seed SuperAdmin User
            if (await userManager.FindByNameAsync(AdminUserName) == null)
            {
                var adminUser = new AdminUser { UserName = AdminUserName, Email = AdminUserName, FirstName = "Super", LastName = "Admin", EmailConfirmed = true, PhoneNumber = AdminPhoneNumber, PhoneNumberConfirmed = true, IsActive = true, CurrentStep = RegistrationStep.Completed, UserType = UserType.Admin, EmployeeCode = "EASY-001", Department = "IT", AdminLevel = AdminLevel.SuperAdmin };
                var result = await userManager.CreateAsync(adminUser, "AdminPa$$w0rd!");
                if (result.Succeeded) await userManager.AddToRoleAsync(adminUser, SystemRoles.SuperAdmin);
                logger.LogInformation("SuperAdmin user created.");
            }

            // Seed User 1: Fully Registered, The "Happy Path"
            if (await userManager.FindByNameAsync(User1UserName) == null)
            {
                var basicUser1 = new NaturalUser { UserName = User1UserName, Email = User1Email, PhoneNumber = User1UserName, PhoneNumberConfirmed = true, FirstName = "John", LastName = "Doe", NationalCode = "1234567890", FatherName = "Richard", BirthDate = new DateTime(1990, 5, 15), Gender = GenderType.Male, IsActive = true, UserType = UserType.Natural, CurrentStep = RegistrationStep.Completed };
                var result = await userManager.CreateAsync(basicUser1, "UserPa$$w0rd1!");
                if (result.Succeeded) await userManager.AddToRoleAsync(basicUser1, SystemRoles.BasicUser);
                logger.LogInformation("User 'John Doe' created.");
            }

            // Seed User 2: Pending Verification, for testing admin panel
            if (await userManager.FindByNameAsync(User2UserName) == null)
            {
                var basicUser2 = new NaturalUser { UserName = User2UserName, Email = User2Email, PhoneNumber = User2UserName, PhoneNumberConfirmed = true, FirstName = "Jane", LastName = "Smith", NationalCode = "0987654321", FatherName = "Robert", BirthDate = new DateTime(1995, 8, 20), Gender = GenderType.Female, IsActive = true, UserType = UserType.Natural, CurrentStep = RegistrationStep.Completed };
                var result = await userManager.CreateAsync(basicUser2, "UserPa$$w0rd2!");
                if (result.Succeeded) await userManager.AddToRoleAsync(basicUser2, SystemRoles.BasicUser);
                logger.LogInformation("User 'Jane Smith' (for verification testing) created.");
            }

            // Seed User 3: New User, for testing onboarding
            if (await userManager.FindByNameAsync(User3UserName) == null)
            {
                var basicUser3 = new NaturalUser { UserName = User3UserName, Email = User3Email, PhoneNumber = User3UserName, PhoneNumberConfirmed = true, IsActive = false, UserType = UserType.Natural, CurrentStep = RegistrationStep.PhoneNumberVerification };
                var result = await userManager.CreateAsync(basicUser3); // No password yet
                if (result.Succeeded) await userManager.AddToRoleAsync(basicUser3, SystemRoles.BasicUser);
                logger.LogInformation("User 'New User' (for onboarding testing) created.");
            }
        }

        private static async Task SeedAccountTypeRequirementsAsync(ApplicationDbContext context, ILogger logger)
        {
            if (!await context.AccountTypeDocumentRequirements.AnyAsync())
            {
                logger.LogInformation("Seeding AccountTypeDocumentRequirements...");
                var savingsAccount = await context.AccountTypes.SingleAsync(at => at.Code == "HIS");
                var studentAccount = await context.AccountTypes.SingleAsync(at => at.Code == "SAA");
                var nationalId = await context.AuthItems.SingleAsync(ai => ai.Title == "National ID Card");
                var proofOfAddress = await context.AuthItems.SingleAsync(ai => ai.Title == "Proof of Address");
                var studentId = await context.AuthItems.SingleAsync(ai => ai.Title == "Student ID Card");

                context.AccountTypeDocumentRequirements.AddRange(
                    new AccountTypeDocumentRequirement { AccountType = savingsAccount, AuthItem = nationalId, IsRequired = true, Order = 1 },
                    new AccountTypeDocumentRequirement { AccountType = savingsAccount, AuthItem = proofOfAddress, IsRequired = true, Order = 2 },
                    new AccountTypeDocumentRequirement { AccountType = studentAccount, AuthItem = nationalId, IsRequired = true, Order = 1 },
                    new AccountTypeDocumentRequirement { AccountType = studentAccount, AuthItem = studentId, IsRequired = true, Order = 2 }
                );
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedUserSpecificDataAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ILogger logger)
        {
            var adminUser = await userManager.FindByNameAsync(AdminUserName);
            await SeedDataForJohnDoeAsync(context, userManager, adminUser, logger);
            await SeedDataForJaneSmithAsync(context, userManager, adminUser, logger);
        }

        private static async Task SeedDataForJohnDoeAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ApplicationUser adminUser, ILogger logger)
        {
            var user = await userManager.FindByNameAsync(User1UserName);
            if (user == null || await context.Accounts.AnyAsync(a => a.OwnerUserId == user.Id)) return;

            logger.LogInformation("Seeding data for John Doe...");

            // --- Accounts ---
            var standardAccType = await context.AccountTypes.SingleAsync(at => at.Code == "SCA");
            var savingsAccType = await context.AccountTypes.SingleAsync(at => at.Code == "HIS");
            var studentAccType = await context.AccountTypes.SingleAsync(at => at.Code == "SAA");

            var account1 = new Account { Title = "Primary Checking", AccountNumber = "6270000001", AccountType = standardAccType, OwnerUser = user, Status = AccountStatus.Active, CurrentBalance = 1250.50m };
            var account2 = new Account { Title = "High-Yield Savings", AccountNumber = "6270000002", AccountType = savingsAccType, OwnerUser = user, Status = AccountStatus.Active, CurrentBalance = 15000.75m };
            var account3 = new Account { Title = "College Fund", AccountNumber = "6270000003", AccountType = studentAccType, OwnerUser = user, Status = AccountStatus.Active, CurrentBalance = 800.00m };
            context.Accounts.AddRange(account1, account2, account3);

            // --- Bank Cards ---
            context.BankCards.AddRange(
                new BankCard { Title = "Main Visa Debit", CardNumber = "4242424242424242", AccountNumber = "1234567890", OwnerUserId = user.Id },
                new BankCard { Title = "Business Mastercard", CardNumber = "5588558855885588", AccountNumber = "0987654321", OwnerUserId = user.Id }
            );

            // --- Approved Documents (required for his accounts) ---
            var nationalIdItem = await context.AuthItems.SingleAsync(ai => ai.Title == "National ID Card");
            var proofOfAddressItem = await context.AuthItems.SingleAsync(ai => ai.Title == "Proof of Address");
            var studentIdItem = await context.AuthItems.SingleAsync(ai => ai.Title == "Student ID Card");
            context.AuthItemValues.AddRange(
                new AuthItemValue(nationalIdItem.Id, user.Id, "/uploads/placeholder/national-id.pdf") { Status = VerificationStatus.Approved, VerificationDate = DateTime.UtcNow.AddDays(-10), VerifiedByUserId = adminUser.Id },
                new AuthItemValue(proofOfAddressItem.Id, user.Id, "/uploads/placeholder/utility-bill.pdf") { Status = VerificationStatus.Approved, VerificationDate = DateTime.UtcNow.AddDays(-10), VerifiedByUserId = adminUser.Id },
                new AuthItemValue(studentIdItem.Id, user.Id, "/uploads/placeholder/student-id.pdf") { Status = VerificationStatus.Approved, VerificationDate = DateTime.UtcNow.AddDays(-10), VerifiedByUserId = adminUser.Id }
            );

            // --- Rich Transaction History ---
            var rnd = new Random();
            var transactions = new List<Transaction>
            {
                CreateTransaction(account1, 2000, TransactionType.Deposit, "Paycheck Deposit", -30),
                CreateTransaction(account1, -25.50m, TransactionType.BillPayment, "Spotify Subscription", -28),
                CreateTransaction(account1, -85.20m, TransactionType.Withdrawal, "ATM Withdrawal", -25),
                CreateTransaction(account2, 5000, TransactionType.Deposit, "Initial Savings Deposit", -20),
                CreateTransaction(account1, -250, TransactionType.TransferOut, "Transfer to Savings", -15, account2.Id.ToString()),
                CreateTransaction(account2, 250, TransactionType.TransferIn, "Transfer from Checking", -15, account1.Id.ToString()),
                CreateTransaction(account1, -12.75m, TransactionType.Withdrawal, "Coffee Shop", -5),
            };
            context.Transaction.AddRange(transactions);

            await context.SaveChangesAsync();
        }

        private static async Task SeedDataForJaneSmithAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ApplicationUser adminUser, ILogger logger)
        {
            var user = await userManager.FindByNameAsync(User2UserName);
            if (user == null || await context.Accounts.AnyAsync(a => a.OwnerUserId == user.Id)) return;

            logger.LogInformation("Seeding data for Jane Smith...");

            // --- Account (She can only open one without approved docs) ---
            var standardAccType = await context.AccountTypes.SingleAsync(at => at.Code == "SCA");
            context.Accounts.Add(new Account { Title = "Everyday Spending", AccountNumber = "6270000004", AccountType = standardAccType, OwnerUser = user, Status = AccountStatus.Active, CurrentBalance = 540.80m });

            // --- Bank Card ---
            context.BankCards.Add(new BankCard { Title = "Primary Debit", CardNumber = "4111111111111111", AccountNumber = "5555555555", OwnerUserId = user.Id });

            // --- Documents for Verification Queue ---
            var nationalIdItem = await context.AuthItems.SingleAsync(ai => ai.Title == "National ID Card");
            var proofOfAddressItem = await context.AuthItems.SingleAsync(ai => ai.Title == "Proof of Address");
            context.AuthItemValues.AddRange(
                new AuthItemValue(nationalIdItem.Id, user.Id, "/uploads/placeholder/jane-national-id.pdf") { Status = VerificationStatus.Pending }, // PENDING
                new AuthItemValue(proofOfAddressItem.Id, user.Id, "/uploads/placeholder/jane-address.pdf") { Status = VerificationStatus.Rejected, RejectionReason = "Document is blurry and out of date.", VerificationDate = DateTime.UtcNow.AddDays(-1), VerifiedByUserId = adminUser.Id } // REJECTED
            );

            await context.SaveChangesAsync();
        }

        private static Transaction CreateTransaction(Account account, decimal amount, TransactionType type, string description, int daysAgo, string? refId = null)
        {
            return new Transaction(
                account.Id,
                Math.Abs(amount),
                type,
                refId ?? Guid.NewGuid().ToString(), 
                DateTime.UtcNow.AddDays(daysAgo),
                new TransactionMetadata("127.0.0.1", "SeedData"),
                description
            );
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
    }

    public class DbInitializer { } // Dummy class for typed ILogger
}