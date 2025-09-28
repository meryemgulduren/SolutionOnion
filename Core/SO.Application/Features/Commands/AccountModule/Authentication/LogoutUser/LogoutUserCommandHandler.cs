using MediatR;
using Microsoft.AspNetCore.Identity;
using SO.Domain.Entities.Identity;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SO.Application.Features.Commands.AccountModule.Authentication.LogoutUser
{
    public class LogoutUserCommandHandler : IRequestHandler<LogoutUserCommandRequest, LogoutUserCommandResponse>
    {
        private readonly SignInManager<AppUser> _signInManager;

        public LogoutUserCommandHandler(SignInManager<AppUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public async Task<LogoutUserCommandResponse> Handle(LogoutUserCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                await _signInManager.SignOutAsync();

                return new LogoutUserCommandResponse
                {
                    Succeeded = true
                };
            }
            catch (Exception ex)
            {
                return new LogoutUserCommandResponse
                {
                    Succeeded = false,
                    ErrorMessage = $"Çıkış sırasında hata oluştu: {ex.Message}"
                };
            }
        }
    }
}
