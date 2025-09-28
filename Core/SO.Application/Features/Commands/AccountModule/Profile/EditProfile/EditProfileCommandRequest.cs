using MediatR;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace SO.Application.Features.Commands.AccountModule.Profile.EditProfile
{
    public class EditProfileCommandRequest : IRequest<EditProfileCommandResponse>
    {
        public string UserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kullanıcı adı gereklidir.")]
        [StringLength(50, ErrorMessage = "Kullanıcı adı en fazla 50 karakter olabilir.")]
        [Display(Name = "Kullanıcı Adı")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta gereklidir.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [Display(Name = "E-posta")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ad Soyad gereklidir.")]
        [StringLength(100, ErrorMessage = "Ad Soyad en fazla 100 karakter olabilir.")]
        [Display(Name = "Ad Soyad")]
        public string FullName { get; set; } = string.Empty;

        [Display(Name = "Profil Resmi")]
        public string? ProfilePicture { get; set; }
        
        [Display(Name = "Profil Resmi Dosyası")]
        public IFormFile? ProfilePictureFile { get; set; }
    }
}
