using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using SO.Domain.Entities.Identity;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace SO.Application.Features.Queries.AccountModule.Authentication.GetCurrentUserId
{
    public class GetCurrentUserIdQueryHandler : IRequestHandler<GetCurrentUserIdQueryRequest, GetCurrentUserIdQueryResponse>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<AppUser> _userManager;

        public GetCurrentUserIdQueryHandler(IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public async Task<GetCurrentUserIdQueryResponse> Handle(GetCurrentUserIdQueryRequest request, CancellationToken cancellationToken)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated != true)
            {
                return new GetCurrentUserIdQueryResponse
                {
                    UserId = null,
                    IsAuthenticated = false
                };
            }

            var userId = _userManager.GetUserId(httpContext.User);
            return new GetCurrentUserIdQueryResponse
            {
                UserId = userId,
                IsAuthenticated = !string.IsNullOrEmpty(userId)
            };
        }
    }
}
