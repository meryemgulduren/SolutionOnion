using MediatR;
using Microsoft.AspNetCore.Mvc;
using SO.Application.Features.Commands.AccountModule.Account.CreateAccount;
using SO.Application.Features.Queries.AccountModule.Account.GetAllAccount;
using System.Threading.Tasks;

namespace SO.Web.Controllers
{
    public class AccountsController : Controller
    {
        private readonly IMediator _mediator;

        public AccountsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> Index()
        {
            GetAllAccountQueryResponse response = await _mediator.Send(new GetAllAccountQueryRequest());
            return View(response);
        }

        // GET: /Accounts/Create
        // YENİ EKLENEN METOD (Boş formu göstermek için)
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Accounts/Create
        // YENİ EKLENEN METOD (Dolu formu kaydetmek için)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAccountCommandRequest request)
        {
            if (ModelState.IsValid)
            {
                await _mediator.Send(request);
                return RedirectToAction(nameof(Index));
            }
            return View(request);
        }
    }
}
