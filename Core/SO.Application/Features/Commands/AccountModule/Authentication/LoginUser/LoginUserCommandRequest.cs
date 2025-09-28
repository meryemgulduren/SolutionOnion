using MediatR;
using System.ComponentModel.DataAnnotations;

namespace SO.Application.Features.Commands.AccountModule.Authentication.LoginUser
{
    public class LoginUserCommandRequest : IRequest<LoginUserCommandResponse>
    {
        [Required(ErrorMessage = "E-posta adresi gereklidir.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [Display(Name = "E-posta")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre gereklidir.")]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Beni Hatırla")]
        public bool RememberMe { get; set; }
        
        public string? ReturnUrl { get; set; }
    }
}
