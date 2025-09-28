namespace SO.Application.Features.Queries.AccountModule.Authentication.GetCurrentUser
{
    public class GetCurrentUserQueryResponse
    {
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public bool IsAuthenticated { get; set; }
    }
}
