using SO.Domain.Entities.Common;
using SO.Domain.Entities.Identity;

namespace SO.Application.Abstractions.Services
{
    public interface IAuthorizationService
    {
        Task<bool> CanUserAccessEntityAsync<T>(AppUser user, T entity) where T : BaseEntity;
        Task<bool> CanUserModifyEntityAsync<T>(AppUser user, T entity) where T : BaseEntity;
        Task<bool> CanUserDeleteEntityAsync<T>(AppUser user, T entity) where T : BaseEntity;
        Task<bool> IsUserAdminAsync(AppUser user);
        Task<bool> IsUserInRoleAsync(AppUser user, string roleName);
        Task<IEnumerable<T>> FilterEntitiesByUserAccessAsync<T>(AppUser user, IEnumerable<T> entities) where T : BaseEntity;
    }
}
