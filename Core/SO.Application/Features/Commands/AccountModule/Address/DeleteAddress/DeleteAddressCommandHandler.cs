using SO.Application.Abstractions.Services.AccountModule;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SO.Application.Features.Commands.AccountModule.Address.DeleteAddress
{
    public class DeleteAddressCommandHandler : IRequestHandler<DeleteAddressCommandRequest, DeleteAddressCommandResponse>
    {
        readonly IAddressService _addressService;

        public DeleteAddressCommandHandler(IAddressService addressService)
        {
            _addressService = addressService;
        }

        public async Task<DeleteAddressCommandResponse> Handle(DeleteAddressCommandRequest request, CancellationToken cancellationToken)
        {
            await _addressService.DeleteAddressAsync(request.Id);
            return new()
            {
                Succeeded = true
            };
        }
    }
}
