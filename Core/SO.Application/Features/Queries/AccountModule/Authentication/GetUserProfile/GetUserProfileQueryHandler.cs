using MediatR;
using Microsoft.AspNetCore.Identity;
using SO.Domain.Entities.Identity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SO.Application.Features.Queries.AccountModule.Authentication.GetUserProfile
{
    public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQueryRequest, GetUserProfileQueryResponse>
    {
        private readonly UserManager<AppUser> _userManager;

        public GetUserProfileQueryHandler(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<GetUserProfileQueryResponse> Handle(GetUserProfileQueryRequest request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                return new GetUserProfileQueryResponse();
            }

            var roles = await _userManager.GetRolesAsync(user);

            return new GetUserProfileQueryResponse
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
                FullName = user.FullName ?? string.Empty,
                UserName = user.UserName ?? string.Empty,
                CreatedDate = user.CreatedDate,
                LastLoginDate = user.LastLoginDate,
                IsActive = user.IsActive,
                Roles = roles.ToList(),
                ProfilePicture = user.ProfilePicture
            };
        }
    }
}
