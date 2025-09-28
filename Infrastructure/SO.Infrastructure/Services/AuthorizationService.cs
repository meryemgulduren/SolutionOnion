using Microsoft.AspNetCore.Identity;
using SO.Application.Abstractions.Services;
using SO.Domain.Entities.Common;
using SO.Domain.Entities.Identity;

namespace SO.Infrastructure.Services
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly UserManager<AppUser> _userManager;

        public AuthorizationService(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> CanUserAccessEntityAsync<T>(AppUser user, T entity) where T : BaseEntity
        {
            if (user == null || entity == null)
                return false;

            // Admin tüm entity'lere erişebilir
            if (await IsUserAdminAsync(user))
                return true;

            // User sadece kendi oluşturduğu entity'lere erişebilir
            return entity.CreatedById == user.Id;
        }

        public async Task<bool> CanUserModifyEntityAsync<T>(AppUser user, T entity) where T : BaseEntity
        {
            if (user == null || entity == null)
                return false;

            // Admin tüm entity'leri düzenleyebilir
            if (await IsUserAdminAsync(user))
                return true;

            // User sadece kendi oluşturduğu entity'leri düzenleyebilir
            return entity.CreatedById == user.Id;
        }

        public async Task<bool> CanUserDeleteEntityAsync<T>(AppUser user, T entity) where T : BaseEntity
        {
            if (user == null || entity == null)
                return false;

            // Admin tüm entity'leri silebilir
            if (await IsUserAdminAsync(user))
                return true;

            // User sadece kendi oluşturduğu entity'leri silebilir
            return entity.CreatedById == user.Id;
        }

        public async Task<bool> IsUserAdminAsync(AppUser user)
        {
            if (user == null)
                return false;

            return await _userManager.IsInRoleAsync(user, "Admin");
        }

        public async Task<bool> IsUserInRoleAsync(AppUser user, string roleName)
        {
            if (user == null)
                return false;

            return await _userManager.IsInRoleAsync(user, roleName);
        }

        public async Task<IEnumerable<T>> FilterEntitiesByUserAccessAsync<T>(AppUser user, IEnumerable<T> entities) where T : BaseEntity
        {
            if (user == null || entities == null)
                return Enumerable.Empty<T>();

            // Admin tüm entity'leri görebilir
            if (await IsUserAdminAsync(user))
                return entities;

            // User sadece kendi oluşturduğu entity'leri görebilir
            return entities.Where(e => e.CreatedById == user.Id);
        }
    }
}
