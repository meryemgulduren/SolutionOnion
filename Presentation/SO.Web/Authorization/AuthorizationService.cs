using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using SO.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using SO.Persistence.Contexts;

namespace SO.Web.Authorization
{
    public interface IAppAuthorizationService
    {
        bool IsAllowed(ClaimsPrincipal user, string permission, object? resourceId = null);
        void EnsureAllowed(ClaimsPrincipal user, string permission, object? resourceId = null);
    }

    public class AppAuthorizationService : IAppAuthorizationService
    {
        private readonly PermissionOptions _options;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly SODbContext _context;

        public AppAuthorizationService(
            IOptions<PermissionOptions> options, 
            IHttpContextAccessor httpContextAccessor,
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager,
            SODbContext context)
        {
            _options = options.Value;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<bool> IsAllowedAsync(ClaimsPrincipal user, string permission, object? resourceId = null)
        {
            if (permission == Permission.Users_Account_Profile_Edit)
                return user?.Identity?.IsAuthenticated == true;

            // Önce DB'den kontrol et
            var hasDbPermission = await CheckDatabasePermissionAsync(user, permission);
            if (hasDbPermission.HasValue)
                return hasDbPermission.Value;

            // DB'de yoksa appsettings'ten kontrol et (fallback)
            if (_options.PermissionRoles.TryGetValue(permission, out var roles))
            {
                foreach (var role in roles)
                {
                    if (user.IsInRole(role)) return true;
                }
            }
            return false;
        }

        public bool IsAllowed(ClaimsPrincipal user, string permission, object? resourceId = null)
        {
            return IsAllowedAsync(user, permission, resourceId).GetAwaiter().GetResult();
        }

        private async Task<bool?> CheckDatabasePermissionAsync(ClaimsPrincipal user, string permission)
        {
            try
            {
                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId)) return false;

                // Kullanıcının rollerini al
                var userRoles = await _userManager.GetRolesAsync(await _userManager.FindByIdAsync(userId));
                if (!userRoles.Any()) return false;

                // Permission'ı DB'de ara
                var permissionEntity = await _context.Permissions
                    .FirstOrDefaultAsync(p => p.Name == permission);

                if (permissionEntity == null) return null; // DB'de yok, appsettings'e bak

                // Kullanıcının rollerinden herhangi biri bu permission'a sahip mi?
                var hasPermission = await _context.PermissionRoles
                    .AnyAsync(pr => pr.PermissionId == permissionEntity.Id 
                                 && userRoles.Contains(pr.Role.Name));

                return hasPermission;
            }
            catch
            {
                return null; // Hata durumunda appsettings'e fallback
            }
        }

        public void EnsureAllowed(ClaimsPrincipal user, string permission, object? resourceId = null)
        {
            if (!IsAllowed(user, permission, resourceId))
            {
                throw new UnauthorizedAccessException($"Permission denied: {permission}");
            }
        }
    }
}


