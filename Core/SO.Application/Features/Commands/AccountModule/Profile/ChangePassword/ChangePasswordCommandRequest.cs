using MediatR;
using System.ComponentModel.DataAnnotations;

namespace SO.Application.Features.Commands.AccountModule.Profile.ChangePassword
{
    public class ChangePasswordCommandRequest : IRequest<ChangePasswordCommandResponse>
    {
        public string UserId { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Mevcut şifre gereklidir.")]
        [DataType(DataType.Password)]
        [Display(Name = "Mevcut Şifre")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Yeni şifre gereklidir.")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Şifre en az 8 karakter olmalıdır.")]
        [Display(Name = "Yeni Şifre")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre onayı gereklidir.")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Yeni şifreler eşleşmiyor.")]
        [Display(Name = "Yeni Şifre Tekrar")]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}
