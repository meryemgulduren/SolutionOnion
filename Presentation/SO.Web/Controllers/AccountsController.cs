using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using SO.Application.Abstractions.Services;
using SO.Domain.Entities.Identity;
using SO.Application.Features.Commands.AccountModule.Account.CreateAccount;
using SO.Application.Features.Commands.AccountModule.Account.UpdateAccount;
using SO.Application.Features.Commands.AccountModule.Account.DeleteAccount;
using SO.Application.Features.Queries.AccountModule.Account.GetAllAccount;
using SO.Application.Features.Queries.AccountModule.Account.GetByIdAccount;
using System.Threading.Tasks;
using System.Linq;
using SO.Web.Authorization;

namespace SO.Web.Controllers
{
    public class AccountsController : Controller
    {
        private readonly IMediator _mediator;
        private readonly UserManager<AppUser> _userManager;
        private readonly IAuthorizationService _authorizationService;

        public AccountsController(
            IMediator mediator,
            UserManager<AppUser> userManager,
            IAuthorizationService authorizationService)
        {
            _mediator = mediator;
            _userManager = userManager;
            _authorizationService = authorizationService;
        }

        [RequirePermission(Permission.Accounts_Manage)]
        public async Task<IActionResult> Index()
        {
            // Kullanıcı bilgilerini al
            string? currentUserId = null;
            bool isAdmin = false;
            
            if (User.Identity?.IsAuthenticated == true)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser != null)
                {
                    currentUserId = currentUser.Id;
                    isAdmin = await _authorizationService.IsUserAdminAsync(currentUser);
                    
                    // SuperAdmin veya Admin olsa bile normal user gibi davran (sadece kendi account'larını görsün)
                    if (await _userManager.IsInRoleAsync(currentUser, "SuperAdmin") || 
                        await _userManager.IsInRoleAsync(currentUser, "Admin"))
                    {
                        isAdmin = false; // SuperAdmin ve Admin'i normal user gibi davranmaya zorla
                    }
                    
                    ViewBag.IsAdmin = isAdmin;
                    ViewBag.CurrentUser = currentUser.UserName;
                }
            }
            
            ViewBag.IsAuthenticated = User.Identity?.IsAuthenticated ?? false;
            
            var request = new GetAllAccountQueryRequest
            {
                CurrentUserId = currentUserId,
                IsAdmin = isAdmin
            };
            
            GetAllAccountQueryResponse response = await _mediator.Send(request);
            return View(response);
        }

        // AJAX Endpoints
        [HttpGet]
        [RequirePermission(Permission.Accounts_Manage)]
        public async Task<IActionResult> GetAllAccounts()
        {
            // Kullanıcı bilgilerini al
            string? currentUserId = null;
            bool isAdmin = false;
            
            if (User.Identity?.IsAuthenticated == true)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser != null)
                {
                    currentUserId = currentUser.Id;
                    isAdmin = await _authorizationService.IsUserAdminAsync(currentUser);
                    
                    // SuperAdmin veya Admin olsa bile normal user gibi davran (sadece kendi account'larını görsün)
                    if (await _userManager.IsInRoleAsync(currentUser, "SuperAdmin") || 
                        await _userManager.IsInRoleAsync(currentUser, "Admin"))
                    {
                        isAdmin = false; // SuperAdmin ve Admin'i normal user gibi davranmaya zorla
                    }
                }
            }
            
            var request = new GetAllAccountQueryRequest
            {
                CurrentUserId = currentUserId,
                IsAdmin = isAdmin
            };
            
            var response = await _mediator.Send(request);
            var accounts = response.Result as System.Collections.Generic.List<SO.Application.DTOs.AccountModule.Account.ListAccount>;
            
            var result = accounts?.Select(a => new
            {
                id = a.Id,
                companyName = a.CompanyName,
                contactPerson = a.ContactPerson,
                email = a.Email,
                phoneNumber = a.PhoneNumber,
                taxOffice = a.TaxOffice,
                taxNumber = a.TaxNumber,
                isActive = a.IsActive
            }).ToList();

            return Json(result);
        }

        [HttpGet]
        [RequirePermission(Permission.Accounts_Manage)]
        public async Task<IActionResult> GetById(string id)
        {
            var response = await _mediator.Send(new GetByIdAccountQueryRequest { Id = id });
            if (response.Result == null)
                return NotFound();

            var account = response.Result as SO.Application.DTOs.AccountModule.Account.SingleAccount;
            var result = new
            {
                id = account.Id,
                companyName = account.CompanyName,
                contactPerson = account.ContactPerson,
                email = account.Email,
                phoneNumber = account.PhoneNumber,
                taxOffice = account.TaxOffice,
                taxNumber = account.TaxNumber,
                isActive = account.IsActive,
                createdDate = account.CreatedDate
            };

            return Json(result);
        }

        [HttpGet]
        [RequirePermission(Permission.Accounts_Manage)]
        public async Task<IActionResult> Details(string id)
        {
            try
            {
                var response = await _mediator.Send(new GetByIdAccountQueryRequest { Id = id });
                if (response.Result == null)
                {
                    return NotFound("Account not found");
                }
                return View(response);
            }
            catch (Exception ex)
            {
                return View("Error");
            }
        }

        // AJAX Create endpoint
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(Permission.Accounts_Create)]
        public async Task<IActionResult> Create([FromBody] CreateAccountCommandRequest request)
        {
            if (ModelState.IsValid)
            {
                await _mediator.Send(request);
                return Json(new { success = true, message = "Account created successfully" });
            }
            return Json(new { success = false, message = "Invalid data" });
        }

        // AJAX Update endpoint
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(Permission.Accounts_Edit)]
        public async Task<IActionResult> Update([FromBody] UpdateAccountCommandRequest request)
        {
            if (ModelState.IsValid)
            {
                await _mediator.Send(request);
                return Json(new { success = true, message = "Account updated successfully" });
            }
            return Json(new { success = false, message = "Invalid data" });
        }

        // AJAX Delete endpoint
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(Permission.Accounts_Delete)]
        public async Task<IActionResult> Delete([FromForm] string id)
        {
            await _mediator.Send(new DeleteAccountCommandRequest { Id = id });
            return Json(new { success = true, message = "Account deleted successfully" });
        }

    }
}
