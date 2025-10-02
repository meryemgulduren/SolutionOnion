using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using SO.Application.Abstractions.Services;
using SO.Domain.Entities.Identity;
using SO.Application.Features.Commands.AccountModule.Address.CreateAddress;
using SO.Application.Features.Commands.AccountModule.Address.UpdateAddress;
using SO.Application.Features.Commands.AccountModule.Address.DeleteAddress;
using SO.Application.Features.Queries.AccountModule.Address.GetAllAddress;
using SO.Application.Features.Queries.AccountModule.Address.GetByIdAddress;
using SO.Application.Features.Queries.AccountModule.Account.GetAllAccount;
using System;
using System.Linq;
using System.Threading.Tasks;
using SO.Web.Authorization;

namespace SO.Web.Controllers
{
    public class AddressesController : Controller
    {
        private readonly IMediator _mediator;
        private readonly UserManager<AppUser> _userManager;
        private readonly IAuthorizationService _authorizationService;

        public AddressesController(
            IMediator mediator,
            UserManager<AppUser> userManager,
            IAuthorizationService authorizationService)
        {
            _mediator = mediator;
            _userManager = userManager;
            _authorizationService = authorizationService;
        }

        [RequirePermission(Permission.Addresses_Manage)]
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
                    
                    // SuperAdmin veya Admin olsa bile normal user gibi davran (sadece kendi address'lerini görsün)
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
            
            var addressRequest = new GetAllAddressQueryRequest
            {
                CurrentUserId = currentUserId,
                IsAdmin = isAdmin
            };
            
            var response = await _mediator.Send(addressRequest);

            var accountRequest = new GetAllAccountQueryRequest
            {
                CurrentUserId = currentUserId,
                IsAdmin = isAdmin
            };
            
            var accountsResponse = await _mediator.Send(accountRequest);
            var accounts = accountsResponse.Result as System.Collections.Generic.List<SO.Application.DTOs.AccountModule.Account.ListAccount>;
            ViewBag.Accounts = accounts ?? new System.Collections.Generic.List<SO.Application.DTOs.AccountModule.Account.ListAccount>();

            return View(response);
        }

        [RequirePermission(Permission.Addresses_Manage)]
        public async Task<IActionResult> Create()
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
                    
                    // SuperAdmin veya Admin olsa bile normal user gibi davran (sadece kendi address'lerini görsün)
                    if (await _userManager.IsInRoleAsync(currentUser, "SuperAdmin") || 
                        await _userManager.IsInRoleAsync(currentUser, "Admin"))
                    {
                        isAdmin = false; // SuperAdmin ve Admin'i normal user gibi davranmaya zorla
                    }
                }
            }
            
            var accountRequest = new GetAllAccountQueryRequest
            {
                CurrentUserId = currentUserId,
                IsAdmin = isAdmin
            };
            
            var accountsResponse = await _mediator.Send(accountRequest);
            var accounts = accountsResponse.Result as System.Collections.Generic.List<SO.Application.DTOs.AccountModule.Account.ListAccount>;
            ViewBag.Accounts = accounts ?? new System.Collections.Generic.List<SO.Application.DTOs.AccountModule.Account.ListAccount>();

            return View(new CreateAddressCommandRequest());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(Permission.Addresses_Create)]
        public async Task<IActionResult> Create(CreateAddressCommandRequest request)
        {
            if (ModelState.IsValid)
            {
                await _mediator.Send(request);
                return RedirectToAction(nameof(Index));
            }

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
                    
                    // SuperAdmin veya Admin olsa bile normal user gibi davran (sadece kendi address'lerini görsün)
                    if (await _userManager.IsInRoleAsync(currentUser, "SuperAdmin") || 
                        await _userManager.IsInRoleAsync(currentUser, "Admin"))
                    {
                        isAdmin = false; // SuperAdmin ve Admin'i normal user gibi davranmaya zorla
                    }
                }
            }
            
            var accountRequest = new GetAllAccountQueryRequest
            {
                CurrentUserId = currentUserId,
                IsAdmin = isAdmin
            };
            
            var accountsResponse = await _mediator.Send(accountRequest);
            var accounts = accountsResponse.Result as System.Collections.Generic.List<SO.Application.DTOs.AccountModule.Account.ListAccount>;
            ViewBag.Accounts = accounts ?? new System.Collections.Generic.List<SO.Application.DTOs.AccountModule.Account.ListAccount>();

            return View(request);
        }

        [HttpGet]
        [RequirePermission(Permission.Addresses_Manage)]
        public async Task<IActionResult> GetAllAddresses()
        {
            try
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
                    }
                }
                
                var request = new GetAllAddressQueryRequest
                {
                    CurrentUserId = currentUserId,
                    IsAdmin = isAdmin
                };
                
                var response = await _mediator.Send(request);
                var addresses = response.Result as System.Collections.Generic.List<SO.Application.DTOs.AccountModule.Address.ListAddress>;

                var result = addresses?.Select(a => new
                {
                    id = a.Id,
                    accountId = a.AccountId,
                    companyName = a.CompanyName,
                    addressName = a.AddressName,
                    addressType = a.AddressType,
                    addressLine1 = a.AddressLine1,
                    addressLine2 = a.AddressLine2,
                    city = a.City,
                    state = a.State,
                    country = a.Country,
                    phone = a.Phone,
                    fax = a.Fax,
                    isDefault = a.isDefault,
                    active = a.Active
                }).ToList();

                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpGet]
        [RequirePermission(Permission.Addresses_Manage)]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var response = await _mediator.Send(new GetByIdAddressQueryRequest { Id = id });

                var result = new
                {
                    id = response.Id,
                    accountId = response.AccountId,
                    addressName = response.AddressName,
                    addressType = response.AddressType,
                    addressLine1 = response.AddressLine1,
                    addressLine2 = response.AddressLine2,
                    city = response.City,
                    state = response.State,
                    country = response.Country,
                    phone = response.Phone,
                    fax = response.Fax,
                    isDefault = response.isDefault,
                    active = response.Active,
                    createdDate = response.CreatedDate
                };

                return Json(result);
            }
            catch (ArgumentException ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpGet]
        [RequirePermission(Permission.Addresses_Manage)]
        public async Task<IActionResult> Details(string id)
        {
            try
            {
                var response = await _mediator.Send(new GetByIdAddressQueryRequest { Id = id });
                if (response == null)
                {
                    return NotFound("Address not found");
                }
                return View(response);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        // Yeni: Belirli bir Account'a ait adresleri JSON olarak döndür
        [HttpGet]
        [RequirePermission(Permission.Addresses_Manage)]
        public async Task<IActionResult> GetByAccount(string accountId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(accountId))
                    return BadRequest("accountId zorunludur");

                // Mevcut query handler tüm adresleri döndürüyor; burada filtreleyelim
                var allResp = await _mediator.Send(new SO.Application.Features.Queries.AccountModule.Address.GetAllAddress.GetAllAddressQueryRequest());
                var all = allResp.Result as System.Collections.Generic.List<SO.Application.DTOs.AccountModule.Address.ListAddress>;
                var list = (all ?? new System.Collections.Generic.List<SO.Application.DTOs.AccountModule.Address.ListAddress>())
                    .Where(a => a.AccountId == accountId)
                    .Select(a => new {
                        id = a.Id,
                        addressName = a.AddressName,
                        addressLine = a.AddressLine,
                        addressLine1 = a.AddressLine1,
                        city = a.City,
                        state = a.State,
                        postalCode = a.PostalCode,
                        country = a.Country,
                        phone = a.Phone,
                        fax = a.Fax,
                        mail = a.Mail,
                        isDefault = a.isDefault
                    })
                    .ToList();

                return Json(list);
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(Permission.Addresses_Create)]
        public async Task<IActionResult> CreateAjax([FromBody] CreateAddressCommandRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return Json(new { success = false, message = "Validation errors: " + string.Join(", ", errors) });
                }

                if (request.AccountId == Guid.Empty)
                    return Json(new { success = false, message = "Account ID is required" });

                await _mediator.Send(request);
                return Json(new { success = true, message = "Address created successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error creating address: " + ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(Permission.Addresses_Edit)]
        public async Task<IActionResult> Update([FromBody] UpdateAddressCommandRequest request)
        {
            if (ModelState.IsValid)
            {
                await _mediator.Send(request);
                return Json(new { success = true, message = "Address updated successfully" });
            }
            return Json(new { success = false, message = "Invalid data" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(Permission.Addresses_Delete)]
        public async Task<IActionResult> Delete([FromForm] string id)
        {
            await _mediator.Send(new DeleteAddressCommandRequest { Id = id });
            return Json(new { success = true, message = "Address deleted successfully" });
        }
    }
}
