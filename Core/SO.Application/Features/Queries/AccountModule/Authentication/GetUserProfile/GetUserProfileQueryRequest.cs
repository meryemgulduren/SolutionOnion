using MediatR;

namespace SO.Application.Features.Queries.AccountModule.Authentication.GetUserProfile
{
    public class GetUserProfileQueryRequest : IRequest<GetUserProfileQueryResponse>
    {
        public string UserId { get; set; } = string.Empty;
    }
}
