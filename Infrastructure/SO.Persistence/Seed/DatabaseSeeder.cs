using Microsoft.AspNetCore.Identity;
using SO.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using SO.Persistence.Contexts;

namespace SO.Persistence.Seed
{
    public class DatabaseSeeder
    {
        private readonly SODbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;

        public DatabaseSeeder(
            SODbContext context,
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task SeedAsync()
        {
            // Rolleri seed et
            await SeedRolesAsync();
            
            // Varsayılan kullanıcıları oluştur
            await SeedUsersAsync();
            
            // Permission'ları seed et
            await SeedPermissionsAsync();
            
            Console.WriteLine("Database seeding completed successfully!");
        }

        private async Task SeedRolesAsync()
        {
            string[] roles = new[] { "User", "Manager", "Admin", "SuperAdmin" };
            
            foreach (var roleName in roles)
            {
                var exists = await _roleManager.RoleExistsAsync(roleName);
                if (!exists)
                {
                    await _roleManager.CreateAsync(new AppRole { Name = roleName });
                    Console.WriteLine($"Role '{roleName}' created.");
                }
            }
        }

        private async Task SeedUsersAsync()
        {
            // SuperAdmin kullanıcısı
            await CreateUserIfNotExistsAsync(
                email: "superadmin@solutiononion.com",
                username: "superadmin",
                fullName: "Super Administrator",
                password: "SuperAdmin123!",
                role: "SuperAdmin"
            );

            // Admin kullanıcısı
            await CreateUserIfNotExistsAsync(
                email: "admin@solutiononion.com",
                username: "admin",
                fullName: "Administrator",
                password: "Admin123!",
                role: "Admin"
            );

            // Test User
            await CreateUserIfNotExistsAsync(
                email: "user@solutiononion.com",
                username: "user",
                fullName: "Test User",
                password: "User123!",
                role: "User"
            );
        }

        private async Task CreateUserIfNotExistsAsync(string email, string username, string fullName, string password, string role)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new AppUser
                {
                    UserName = username,
                    Email = email,
                    FullName = fullName,
                    EmailConfirmed = true,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, role);
                    Console.WriteLine($"User '{email}' created with role '{role}'.");
                }
                else
                {
                    Console.WriteLine($"Failed to create user '{email}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
        }

        private async Task SeedPermissionsAsync()
        {
            // Permission'ları oluştur
            var permissions = new List<AppPermission>
            {
                // Users
                new AppPermission { Name = "Users.Admin.Dashboard", Description = "Admin Dashboard Access", Module = "Users" },
                new AppPermission { Name = "Users.Admin.View", Description = "Admin View Access", Module = "Users" },
                new AppPermission { Name = "Users.Admin.Export", Description = "Admin Export Access", Module = "Users" },
                new AppPermission { Name = "Users.SuperAdmin.Dashboard", Description = "SuperAdmin Dashboard Access", Module = "Users" },
                new AppPermission { Name = "Users.SuperAdmin.ManageUsers", Description = "SuperAdmin User Management", Module = "Users" },
                new AppPermission { Name = "Users.SuperAdmin.Export", Description = "SuperAdmin Export Access", Module = "Users" },
                new AppPermission { Name = "Users.Account.Profile.Edit", Description = "User Profile Edit", Module = "Users" },

                // Proposals
                new AppPermission { Name = "Proposals.ReadOwn", Description = "Read Own Proposals", Module = "Proposals" },
                new AppPermission { Name = "Proposals.ReadAll", Description = "Read All Proposals", Module = "Proposals" },
                new AppPermission { Name = "Proposals.Create", Description = "Create Proposals", Module = "Proposals" },
                new AppPermission { Name = "Proposals.Edit", Description = "Edit Proposals", Module = "Proposals" },
                new AppPermission { Name = "Proposals.Delete", Description = "Delete Proposals", Module = "Proposals" },
                new AppPermission { Name = "Proposals.Export", Description = "Export Proposals", Module = "Proposals" },

                // Accounts
                new AppPermission { Name = "Accounts.Manage", Description = "Manage Accounts", Module = "Accounts" },
                new AppPermission { Name = "Accounts.Create", Description = "Create Accounts", Module = "Accounts" },
                new AppPermission { Name = "Accounts.Edit", Description = "Edit Accounts", Module = "Accounts" },
                new AppPermission { Name = "Accounts.Delete", Description = "Delete Accounts", Module = "Accounts" },

                // Addresses
                new AppPermission { Name = "Addresses.Manage", Description = "Manage Addresses", Module = "Addresses" },
                new AppPermission { Name = "Addresses.Create", Description = "Create Addresses", Module = "Addresses" },
                new AppPermission { Name = "Addresses.Edit", Description = "Edit Addresses", Module = "Addresses" },
                new AppPermission { Name = "Addresses.Delete", Description = "Delete Addresses", Module = "Addresses" }
            };

            foreach (var permission in permissions)
            {
                var existingPermission = await _context.Permissions
                    .FirstOrDefaultAsync(p => p.Name == permission.Name);

                if (existingPermission == null)
                {
                    _context.Permissions.Add(permission);
                    Console.WriteLine($"Permission '{permission.Name}' created.");
                }
            }

            await _context.SaveChangesAsync();

            // Permission-Role ilişkilerini oluştur
            await SeedPermissionRolesAsync();
        }

        private async Task SeedPermissionRolesAsync()
        {
            var permissionRoleMappings = new Dictionary<string, string[]>
            {
                // Users
                ["Users.Admin.Dashboard"] = new[] { "Admin", "SuperAdmin" },
                ["Users.Admin.View"] = new[] { "Admin", "SuperAdmin" },
                ["Users.Admin.Export"] = new[] { "Admin", "SuperAdmin" },
                ["Users.SuperAdmin.Dashboard"] = new[] { "SuperAdmin" },
                ["Users.SuperAdmin.ManageUsers"] = new[] { "SuperAdmin" },
                ["Users.SuperAdmin.Export"] = new[] { "SuperAdmin" },
                ["Users.Account.Profile.Edit"] = new[] { "User", "Manager", "Admin", "SuperAdmin" },

                // Proposals
                ["Proposals.ReadOwn"] = new[] { "User", "Manager", "Admin", "SuperAdmin" },
                ["Proposals.ReadAll"] = new[] { "Admin", "SuperAdmin" },
                ["Proposals.Create"] = new[] { "User", "Manager", "Admin", "SuperAdmin" },
                ["Proposals.Edit"] = new[] { "User", "Manager", "Admin", "SuperAdmin" },
                ["Proposals.Delete"] = new[] { "User", "Manager", "Admin", "SuperAdmin" },
                ["Proposals.Export"] = new[] { "Admin", "SuperAdmin" },

                // Accounts
                ["Accounts.Manage"] = new[] { "User", "Manager", "Admin", "SuperAdmin" },
                ["Accounts.Create"] = new[] { "User", "Manager", "Admin", "SuperAdmin" },
                ["Accounts.Edit"] = new[] { "User", "Manager", "Admin", "SuperAdmin" },
                ["Accounts.Delete"] = new[] { "User", "Manager", "Admin", "SuperAdmin" },

                // Addresses
                ["Addresses.Manage"] = new[] { "User", "Manager", "Admin", "SuperAdmin" },
                ["Addresses.Create"] = new[] { "User", "Manager", "Admin", "SuperAdmin" },
                ["Addresses.Edit"] = new[] { "User", "Manager", "Admin", "SuperAdmin" },
                ["Addresses.Delete"] = new[] { "User", "Manager", "Admin", "SuperAdmin" }
            };

            foreach (var mapping in permissionRoleMappings)
            {
                var permission = await _context.Permissions
                    .FirstOrDefaultAsync(p => p.Name == mapping.Key);

                if (permission == null) continue;

                foreach (var roleName in mapping.Value)
                {
                    var role = await _roleManager.FindByNameAsync(roleName);
                    if (role == null) continue;

                    var existingMapping = await _context.PermissionRoles
                        .FirstOrDefaultAsync(pr => pr.PermissionId == permission.Id && pr.RoleId == role.Id);

                    if (existingMapping == null)
                    {
                        _context.PermissionRoles.Add(new PermissionRole
                        {
                            PermissionId = permission.Id,
                            RoleId = role.Id,
                            IsActive = true
                        });
                        Console.WriteLine($"Permission '{permission.Name}' assigned to role '{roleName}'.");
                    }
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
