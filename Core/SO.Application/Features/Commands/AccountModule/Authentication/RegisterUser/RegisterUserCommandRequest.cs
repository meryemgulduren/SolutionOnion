using MediatR;
using System.ComponentModel.DataAnnotations;

namespace SO.Application.Features.Commands.AccountModule.Authentication.RegisterUser
{
    public class RegisterUserCommandRequest : IRequest<RegisterUserCommandResponse>
    {
        [Required(ErrorMessage = "Ad Soyad gereklidir.")]
        [Display(Name = "Ad Soyad")]
        [StringLength(100, ErrorMessage = "{0} en fazla {1} karakter olabilir.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kullanıcı adı gereklidir.")]
        [Display(Name = "Kullanıcı Adı")]
        [StringLength(50, ErrorMessage = "{0} en fazla {1} karakter olabilir.")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Kullanıcı adı sadece harf, rakam ve alt çizgi içerebilir.")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta adresi gereklidir.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [Display(Name = "E-posta")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre gereklidir.")]
        [StringLength(100, ErrorMessage = "{0} en az {2} karakter olmalıdır.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Şifre Tekrar")]
        [Compare("Password", ErrorMessage = "Şifreler eşleşmiyor.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public string? ReturnUrl { get; set; }
        public string BaseUrl { get; set; } = string.Empty;
    }
}
