using MediatR;
using Microsoft.AspNetCore.Mvc;
using SO.Application.Features.Commands.AccountModule.Address.CreateAddress;
using SO.Application.Features.Queries.AccountModule.Address.GetAllAddress;
using System.Threading.Tasks;

namespace SO.Web.Controllers
{
    public class AddressesController : Controller
    {
        private readonly IMediator _mediator;

        public AddressesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> Index()
        {
            // Application katmanına isteği gönderiyoruz.
            GetAllAddressQueryResponse response = await _mediator.Send(new GetAllAddressQueryRequest());

            // Dönen sonucu View'e model olarak gönderiyoruz.
            return View(response);

        }
        public IActionResult Create()
        {
            return View(new CreateAddressCommandRequest());
        }
        // Bu metodun, bir formdan POST isteği ile geldiğini belirtiyoruz.
        [HttpPost]
        // Formdaki verilerin sahte olmadığından emin olmak için bir güvenlik önlemi.
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Application.Features.Commands.AccountModule.Address.CreateAddress.CreateAddressCommandRequest request)
        {
            // ModelState.IsValid, formdan gelen verilerin DTO'daki kurallara
            // (örn: zorunlu alanlar) uyup uymadığını kontrol eder.
            if (ModelState.IsValid)
            {
                // Gelen form verilerini MediatR aracılığıyla ilgili Handler'a gönderiyoruz.
                await _mediator.Send(request);

                // İşlem bittikten sonra kullanıcıyı adres listesi sayfasına geri yönlendiriyoruz.
                return RedirectToAction(nameof(Index));
            }

            // Eğer formda bir hata varsa, kullanıcıyı aynı form sayfasına,
            // girdiği veriler ve hata mesajlarıyla birlikte geri gönderiyoruz.
            return View(request);
        }
    }
}