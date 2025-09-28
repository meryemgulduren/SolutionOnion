using MediatR;

namespace SO.Application.Features.Queries.AccountModule.Authentication.GetCurrentUser
{
    public class GetCurrentUserQueryRequest : IRequest<GetCurrentUserQueryResponse>
    {
        // Parametresiz - HttpContext'ten alınacak
    }
}
