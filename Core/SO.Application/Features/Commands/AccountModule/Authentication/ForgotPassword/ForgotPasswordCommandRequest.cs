using MediatR;
using System.ComponentModel.DataAnnotations;

namespace SO.Application.Features.Commands.AccountModule.Authentication.ForgotPassword
{
    public class ForgotPasswordCommandRequest : IRequest<ForgotPasswordCommandResponse>
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;
        
        public string? BaseUrl { get; set; }
    }
}
