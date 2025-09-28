namespace SO.Application.Features.Queries.AccountModule.Authentication.GetCurrentUserId
{
    public class GetCurrentUserIdQueryResponse
    {
        public string? UserId { get; set; }
        public bool IsAuthenticated { get; set; }
    }
}
