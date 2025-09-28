using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace SO.Application.Features.Queries.AccountModule.Authentication.GetCurrentUser
{
    public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQueryRequest, GetCurrentUserQueryResponse>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetCurrentUserQueryHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<GetCurrentUserQueryResponse> Handle(GetCurrentUserQueryRequest request, CancellationToken cancellationToken)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var isAuthenticated = user?.Identity?.IsAuthenticated ?? false;

            if (!isAuthenticated)
            {
                return Task.FromResult(new GetCurrentUserQueryResponse
                {
                    IsAuthenticated = false
                });
            }

            var userId = user?.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = user?.FindFirstValue(ClaimTypes.Name);
            var email = user?.FindFirstValue(ClaimTypes.Email);

            return Task.FromResult(new GetCurrentUserQueryResponse
            {
                UserId = userId,
                UserName = userName,
                Email = email,
                IsAuthenticated = true
            });
        }
    }
}
