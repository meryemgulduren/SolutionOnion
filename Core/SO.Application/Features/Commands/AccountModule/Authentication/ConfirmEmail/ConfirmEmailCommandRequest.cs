using MediatR;
using System.ComponentModel.DataAnnotations;

namespace SO.Application.Features.Commands.AccountModule.Authentication.ConfirmEmail
{
    public class ConfirmEmailCommandRequest : IRequest<ConfirmEmailCommandResponse>
    {
        [Required(ErrorMessage = "User ID is required")]
        public string UserId { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Token is required")]
        public string Token { get; set; } = string.Empty;
    }
}
