using MediatR;

namespace SO.Application.Features.Queries.AccountModule.Authentication.GetCurrentUserId
{
    public class GetCurrentUserIdQueryRequest : IRequest<GetCurrentUserIdQueryResponse>
    {
        // Bu query için parametre gerekmez, HttpContext'ten alınacak
    }
}
