using System;
using System.Collections.Generic;

namespace SO.Application.Features.Queries.AccountModule.Authentication.GetUserProfile
{
    public class GetUserProfileQueryResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public bool IsActive { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public string? ProfilePicture { get; set; }
    }
}
