using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MediatR;
using SO.Application.Abstractions.Services;
using SO.Application.Features.Commands.AccountModule.Authentication.ConfirmEmail;
using SO.Application.Features.Commands.AccountModule.Authentication.DeleteUser;
using SO.Application.Features.Commands.AccountModule.Authentication.ForgotPassword;
using SO.Application.Features.Commands.AccountModule.Authentication.LoginUser;
using SO.Application.Features.Commands.AccountModule.Authentication.LogoutUser;
using SO.Application.Features.Commands.AccountModule.Authentication.RegisterUser;
using SO.Application.Features.Commands.AccountModule.Authentication.ResendConfirmation;
using SO.Application.Features.Commands.AccountModule.Authentication.ResetPassword;
using SO.Application.Features.Commands.AccountModule.Profile.ChangePassword;
using SO.Application.Features.Commands.AccountModule.Profile.EditProfile;
using SO.Application.Features.Commands.AdminModule.Export.ExportAllProposalsExcel;
using SO.Application.Features.Commands.AdminModule.Export.ExportAllProposalsWord;
using SO.Application.Features.Commands.AdminModule.Export.ExportProposalWord;
using SO.Application.Features.Queries.AccountModule.Account.GetAllAccount;
using SO.Application.Features.Queries.AccountModule.Address.GetAllAddress;
using SO.Application.Features.Queries.AccountModule.Authentication.GetCurrentUserId;
using SO.Application.Features.Queries.ProposalModule.Proposal.GetAllProposal;
using SO.Domain.Entities.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using AuthService = SO.Application.Abstractions.Services.IAuthorizationService;
using SO.Web.Authorization;

namespace SO.Web.Controllers
{
    [Authorize]
    [Route("users")] // konsolide route kökü
    public class UsersController : Controller
    {
        private readonly IMediator _mediator;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly AuthService _authorizationService;
        private readonly IDocumentExportService _documentExportService;

        public UsersController(
            IMediator mediator,
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager,
            AuthService authorizationService,
            IDocumentExportService documentExportService)
        {
            _mediator = mediator;
            _userManager = userManager;
            _roleManager = roleManager;
            _authorizationService = authorizationService;
            _documentExportService = documentExportService;
        }

        // =============== AUTH (eski AccountController) ===============

        [AllowAnonymous]
        [HttpGet("login")]
        [HttpGet("account/login")] // legacy route relative
        [HttpGet("/Account/Login")] // legacy route absolute
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View("~/Views/Account/Login.cshtml");
        }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost("login")]
        [HttpPost("account/login")] // legacy route relative
        [HttpPost("/Account/Login")] // legacy route absolute
        public async Task<IActionResult> LoginPost(LoginUserCommandRequest model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var command = new LoginUserCommandRequest
                {
                    Email = model.Email,
                    Password = model.Password,
                    RememberMe = model.RememberMe,
                    ReturnUrl = returnUrl
                };
                var response = await _mediator.Send(command);
                if (response.Succeeded)
                {
                    if (!string.IsNullOrEmpty(response.RedirectUrl) && Url.IsLocalUrl(response.RedirectUrl))
                        return Redirect(response.RedirectUrl);
                    return RedirectToAction("Index", "Home");
                }
                if (response.IsLockedOut)
                    ModelState.AddModelError(string.Empty, "Your account is locked. Please try again later.");
                else
                    ModelState.AddModelError(string.Empty, response.ErrorMessage ?? "Invalid login attempt.");
            }
            return View("~/Views/Account/Login.cshtml", model);
        }

        [AllowAnonymous]
        [HttpGet("register")]
        [HttpGet("account/register")] // legacy route relative
        [HttpGet("/Account/Register")] // legacy route absolute
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View("~/Views/Account/Register.cshtml");
        }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost("register")]
        [HttpPost("account/register")] // legacy route relative
        [HttpPost("/Account/Register")] // legacy route absolute
        public async Task<IActionResult> RegisterPost(RegisterUserCommandRequest model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var command = new RegisterUserCommandRequest
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    FullName = model.FullName,
                    Password = model.Password,
                    ReturnUrl = returnUrl,
                    BaseUrl = $"{Request.Scheme}://{Request.Host}"
                };
                var response = await _mediator.Send(command);
                if (response.Succeeded)
                {
                    if (!string.IsNullOrEmpty(response.RedirectUrl) && Url.IsLocalUrl(response.RedirectUrl))
                        return Redirect(response.RedirectUrl);
                    return RedirectToAction("Index", "Home");
                }
                if (response.Errors.Any())
                    foreach (var error in response.Errors) ModelState.AddModelError(string.Empty, error);
                else if (!string.IsNullOrEmpty(response.ErrorMessage))
                    ModelState.AddModelError(string.Empty, response.ErrorMessage);
            }
            return View("~/Views/Account/Register.cshtml", model);
        }

        [HttpGet("logout")]
        [HttpGet("account/logout")] // legacy route relative
        [HttpGet("/Account/Logout")] // legacy route absolute
        public async Task<IActionResult> Logout()
        {
            var command = new LogoutUserCommandRequest();
            var response = await _mediator.Send(command);
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        [HttpGet("access-denied")]
        [HttpGet("account/accessdenied")] // legacy route relative
        [HttpGet("/Account/AccessDenied")] // legacy route absolute
        public IActionResult AccessDenied()
        {
            return View("~/Views/Account/AccessDenied.cshtml");
        }

        [AllowAnonymous]
        [HttpGet("resend-confirmation")]
        [HttpGet("account/resendconfirmation")] // legacy route relative
        [HttpGet("/Account/ResendConfirmation")] // legacy route absolute
        public IActionResult ResendConfirmation()
        {
            return View("~/Views/Account/ResendConfirmation.cshtml");
        }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost("resend-confirmation")]
        [HttpPost("account/resendconfirmation")] // legacy route relative
        [HttpPost("/Account/ResendConfirmation")] // legacy route absolute
        public async Task<IActionResult> ResendConfirmationPost(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                ModelState.AddModelError(string.Empty, "E-posta gerekli.");
                return View("~/Views/Account/ResendConfirmation.cshtml");
            }
            var command = new ResendConfirmationCommandRequest
            {
                Email = email,
                BaseUrl = $"{Request.Scheme}://{Request.Host}"
            };
            var response = await _mediator.Send(command);
            if (response.Succeeded) ViewBag.Info = response.Message; else ModelState.AddModelError(string.Empty, response.ErrorMessage ?? "Bir hata oluştu.");
            return View("~/Views/Account/ResendConfirmation.cshtml");
        }

        [AllowAnonymous]
        [HttpGet("confirm-email")]
        [HttpGet("account/confirmemail")] // legacy route relative
        [HttpGet("/Account/ConfirmEmail")] // legacy route absolute
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token)) return BadRequest();
            var command = new ConfirmEmailCommandRequest { UserId = userId, Token = token };
            var response = await _mediator.Send(command);
            ViewBag.EmailConfirmed = response.Succeeded;
            if (!response.Succeeded) ViewBag.ErrorMessage = response.ErrorMessage;
            return View("~/Views/Account/ConfirmEmail.cshtml");
        }

        [AllowAnonymous]
        [HttpGet("forgot-password")]
        [HttpGet("account/forgotpassword")] // legacy route relative
        [HttpGet("/Account/ForgotPassword")] // legacy route absolute
        public IActionResult ForgotPassword()
        {
            return View("~/Views/Account/ForgotPassword.cshtml");
        }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost("forgot-password")]
        [HttpPost("account/forgotpassword")] // legacy route relative
        [HttpPost("/Account/ForgotPassword")] // legacy route absolute
        public async Task<IActionResult> ForgotPasswordPost(ForgotPasswordCommandRequest model)
        {
            if (!ModelState.IsValid) return View("~/Views/Account/ForgotPassword.cshtml", model);
            var command = new ForgotPasswordCommandRequest { Email = model.Email, BaseUrl = $"{Request.Scheme}://{Request.Host}" };
            var response = await _mediator.Send(command);
            if (response.Succeeded) return View("~/Views/Account/ForgotPasswordConfirmation.cshtml");
            ModelState.AddModelError(string.Empty, response.ErrorMessage ?? "Bir hata oluştu.");
            return View("~/Views/Account/ForgotPassword.cshtml", model);
        }

        [AllowAnonymous]
        [HttpGet("reset-password")]
        [HttpGet("account/resetpassword")] // legacy route relative
        [HttpGet("/Account/ResetPassword")] // legacy route absolute
        public IActionResult ResetPassword(string email, string token)
        {
            var model = new ResetPasswordCommandRequest { Email = email, Token = token };
            return View("~/Views/Account/ResetPassword.cshtml", model);
        }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost("reset-password")]
        [HttpPost("account/resetpassword")] // legacy route relative
        [HttpPost("/Account/ResetPassword")] // legacy route absolute
        public async Task<IActionResult> ResetPasswordPost(ResetPasswordCommandRequest model)
        {
            if (!ModelState.IsValid) return View("~/Views/Account/ResetPassword.cshtml", model);
            var command = new ResetPasswordCommandRequest { Email = model.Email, Token = model.Token, Password = model.Password, ConfirmPassword = model.ConfirmPassword };
            var response = await _mediator.Send(command);
            if (response.Succeeded) return RedirectToAction("ResetPasswordConfirmation");
            if (response.Errors.Any()) foreach (var error in response.Errors) ModelState.AddModelError(string.Empty, error); else if (!string.IsNullOrEmpty(response.ErrorMessage)) ModelState.AddModelError(string.Empty, response.ErrorMessage);
            return View("~/Views/Account/ResetPassword.cshtml", model);
        }

        [AllowAnonymous]
        [HttpGet("reset-password/confirmation")]
        [HttpGet("account/resetpasswordconfirmation")] // legacy route relative
        [HttpGet("/Account/ResetPasswordConfirmation")] // legacy route absolute
        public IActionResult ResetPasswordConfirmation()
        {
            return View("~/Views/Account/ResetPasswordConfirmation.cshtml");
        }

        [HttpGet("profile/edit")]
        [HttpGet("account/editprofile")] // legacy route relative
        [HttpGet("/Account/EditProfile")] // legacy route absolute
        public async Task<IActionResult> EditProfile()
        {
            var userIdQuery = new GetCurrentUserIdQueryRequest();
            var userIdResponse = await _mediator.Send(userIdQuery);
            if (!userIdResponse.IsAuthenticated || string.IsNullOrEmpty(userIdResponse.UserId)) return RedirectToAction("Login");
            var query = new SO.Application.Features.Queries.AccountModule.Authentication.GetUserProfile.GetUserProfileQueryRequest { UserId = userIdResponse.UserId };
            var response = await _mediator.Send(query);
            if (string.IsNullOrEmpty(response.Id)) return NotFound();
            var model = new EditProfileCommandRequest { UserId = response.Id, UserName = response.UserName, Email = response.Email, FullName = response.FullName, ProfilePicture = response.ProfilePicture };
            return View("~/Views/Account/EditProfile.cshtml", model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost("profile/edit")]
        [HttpPost("account/editprofile")] // legacy route relative
        [HttpPost("/Account/EditProfile")] // legacy route absolute
        public async Task<IActionResult> EditProfilePost(EditProfileCommandRequest model)
        {
            if (!ModelState.IsValid) return View("~/Views/Account/EditProfile.cshtml", model);
            var userIdQuery = new GetCurrentUserIdQueryRequest();
            var userIdResponse = await _mediator.Send(userIdQuery);
            if (!userIdResponse.IsAuthenticated || string.IsNullOrEmpty(userIdResponse.UserId) || userIdResponse.UserId != model.UserId) return RedirectToAction("Login");
            if (model.ProfilePictureFile != null)
            {
                var fileUploadCommand = new SO.Application.Features.Commands.AccountModule.Profile.FileUpload.FileUploadCommandRequest { File = model.ProfilePictureFile, ContainerName = "profile-pictures" };
                var uploadResponse = await _mediator.Send(fileUploadCommand);
                if (uploadResponse.Succeeded && !string.IsNullOrEmpty(uploadResponse.FilePath)) model.ProfilePicture = uploadResponse.FilePath;
            }
            var command = new EditProfileCommandRequest { UserId = userIdResponse.UserId, UserName = model.UserName, Email = model.Email, FullName = model.FullName, ProfilePicture = model.ProfilePicture };
            var response = await _mediator.Send(command);
            if (response.Succeeded)
            {
                TempData["SuccessMessage"] = "Profil başarıyla güncellendi.";
                return RedirectToAction("Profile");
            }
            if (response.Errors.Any()) foreach (var error in response.Errors) ModelState.AddModelError(string.Empty, error); else ModelState.AddModelError(string.Empty, response.ErrorMessage ?? "Profil güncellenirken hata oluştu.");
            return View("~/Views/Account/EditProfile.cshtml", model);
        }

        [HttpGet("profile")]
        [HttpGet("account/profile")] // legacy route relative
        [HttpGet("/Account/Profile")] // legacy route absolute
        public async Task<IActionResult> Profile()
        {
            var userIdQuery = new GetCurrentUserIdQueryRequest();
            var userIdResponse = await _mediator.Send(userIdQuery);
            if (!userIdResponse.IsAuthenticated || string.IsNullOrEmpty(userIdResponse.UserId)) return RedirectToAction("Login");
            var query = new SO.Application.Features.Queries.AccountModule.Authentication.GetUserProfile.GetUserProfileQueryRequest { UserId = userIdResponse.UserId };
            var response = await _mediator.Send(query);
            if (string.IsNullOrEmpty(response.Id)) return NotFound();
            return View("~/Views/Account/Profile.cshtml", response);
        }

        [HttpGet("change-password")]
        [HttpGet("account/changepassword")] // legacy route relative
        [HttpGet("/Account/ChangePassword")] // legacy route absolute
        public IActionResult ChangePassword()
        {
            return View("~/Views/Account/ChangePassword.cshtml");
        }

        [ValidateAntiForgeryToken]
        [HttpPost("change-password")]
        [HttpPost("account/changepassword")] // legacy route relative
        [HttpPost("/Account/ChangePassword")] // legacy route absolute
        public async Task<IActionResult> ChangePasswordPost(ChangePasswordCommandRequest model)
        {
            if (!ModelState.IsValid) return View("~/Views/Account/ChangePassword.cshtml", model);
            var userIdQuery = new GetCurrentUserIdQueryRequest();
            var userIdResponse = await _mediator.Send(userIdQuery);
            if (!userIdResponse.IsAuthenticated || string.IsNullOrEmpty(userIdResponse.UserId)) return RedirectToAction("Login");
            var command = new ChangePasswordCommandRequest { UserId = userIdResponse.UserId, CurrentPassword = model.CurrentPassword, NewPassword = model.NewPassword };
            var response = await _mediator.Send(command);
            if (response.Succeeded)
            {
                TempData["SuccessMessage"] = "Şifre başarıyla değiştirildi.";
                return RedirectToAction("Profile");
            }
            if (response.Errors.Any()) foreach (var error in response.Errors) ModelState.AddModelError(string.Empty, error); else ModelState.AddModelError(string.Empty, response.ErrorMessage ?? "Şifre değiştirilirken hata oluştu.");
            return View("~/Views/Account/ChangePassword.cshtml", model);
        }

        [HttpGet("delete")]
        [HttpGet("account/delete")] // legacy route relative
        [HttpGet("/Account/Delete")] // legacy route absolute
        public IActionResult Delete()
        {
            return View("~/Views/Account/Delete.cshtml");
        }

        [ValidateAntiForgeryToken]
        [HttpPost("delete")]
        [HttpPost("account/deleteconfirmed")] // legacy route relative
        [HttpPost("/Account/DeleteConfirmed")] // legacy route absolute
        public async Task<IActionResult> DeleteConfirmed(string currentPassword)
        {
            var userIdQuery = new GetCurrentUserIdQueryRequest();
            var userIdResponse = await _mediator.Send(userIdQuery);
            if (!userIdResponse.IsAuthenticated || string.IsNullOrEmpty(userIdResponse.UserId)) return RedirectToAction("Login");
            var command = new DeleteUserCommandRequest { UserId = userIdResponse.UserId, CurrentPassword = currentPassword };
            var response = await _mediator.Send(command);
            if (response.Succeeded) return Redirect(response.RedirectUrl ?? "/Home/Index");
            ModelState.AddModelError(string.Empty, response.ErrorMessage ?? "Hesap silme başarısız");
            return View("~/Views/Account/Delete.cshtml");
        }

        // =============== ADMIN (eski AdminController) ===============

        [RequirePermission(Permission.Users_Admin_Dashboard)]
        [HttpGet("admin/dashboard")]
        [HttpGet("admin")] // legacy relative
        [HttpGet("/Admin/Dashboard")]
        [HttpGet("/Admin")] // legacy absolute
        public async Task<IActionResult> AdminDashboard()
        {
            try
            {
                var proposalsResponse = await _mediator.Send(new GetAllProposalQueryRequest { CurrentUserId = null, IsAdmin = true });
                var accountsResponse = await _mediator.Send(new GetAllAccountQueryRequest { CurrentUserId = null, IsAdmin = true });
                var addressesResponse = await _mediator.Send(new GetAllAddressQueryRequest { CurrentUserId = null, IsAdmin = true });
                var proposals = proposalsResponse.Result as System.Collections.ICollection;
                var accounts = accountsResponse.Result as System.Collections.ICollection;
                var addresses = addressesResponse.Result as System.Collections.ICollection;
                ViewBag.TotalProposals = proposals?.Count ?? 0;
                ViewBag.TotalAccounts = accounts?.Count ?? 0;
                ViewBag.TotalAddresses = addresses?.Count ?? 0;
                ViewBag.IsAdmin = true;
                return View("~/Views/Admin/Dashboard.cshtml");
            }
            catch
            {
                return View("Error");
            }
        }

        [RequirePermission(Permission.Users_Admin_View)]
        [HttpGet("admin/proposals")]
        [HttpGet("/Admin/Proposals")]
        public async Task<IActionResult> AdminProposals()
        {
            try
            {
                var currentUserId = (await _mediator.Send(new SO.Application.Features.Queries.AccountModule.Authentication.GetCurrentUser.GetCurrentUserQueryRequest())).UserId;
                var request = new GetAllProposalQueryRequest { CurrentUserId = currentUserId, IsAdmin = false };
                var response = await _mediator.Send(request);
                return View("~/Views/Admin/Proposals.cshtml", response);
            }
            catch
            {
                return View("Error");
            }
        }

        [RequirePermission(Permission.Users_Admin_View)]
        [HttpGet("admin/accounts")]
        [HttpGet("/Admin/Accounts")]
        public async Task<IActionResult> AdminAccounts()
        {
            try
            {
                var response = await _mediator.Send(new GetAllAccountQueryRequest { CurrentUserId = null, IsAdmin = true });
                return View("~/Views/Admin/Accounts.cshtml", response);
            }
            catch
            {
                return View("Error");
            }
        }

        [RequirePermission(Permission.Users_Admin_View)]
        [HttpGet("admin/addresses")]
        [HttpGet("/Admin/Addresses")]
        public async Task<IActionResult> AdminAddresses()
        {
            try
            {
                var response = await _mediator.Send(new GetAllAddressQueryRequest { CurrentUserId = null, IsAdmin = true });
                return View("~/Views/Admin/Addresses.cshtml", response);
            }
            catch
            {
                return View("Error");
            }
        }

        [RequirePermission(Permission.Users_Admin_Export)]
        [HttpGet("admin/export/proposal-word")]
        [HttpGet("/Admin/ExportProposalWord")]
        public async Task<IActionResult> AdminExportProposalWord(Guid id)
        {
            try
            {
                var command = new ExportProposalWordCommandRequest { ProposalId = id };
                var response = await _mediator.Send(command);
                if (response.Succeeded && response.FileBytes != null)
                    return File(response.FileBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", response.FileName);
                return BadRequest(response.ErrorMessage ?? "Error exporting proposal to Word");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error exporting proposal to Word: {ex.Message}");
            }
        }

        [RequirePermission(Permission.Users_Admin_Export)]
        [HttpPost("admin/export/all-word")]
        [HttpPost("/Admin/ExportAllWord")]
        public async Task<IActionResult> AdminExportAllWord([FromBody] AdminExportRequest request)
        {
            try
            {
                var command = new ExportAllProposalsWordCommandRequest { ProposalIds = request.ProposalIds };
                var response = await _mediator.Send(command);
                if (response.Succeeded && response.FileBytes != null)
                    return File(response.FileBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", response.FileName);
                return BadRequest(response.ErrorMessage ?? "Error exporting all proposals to Word");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error exporting all proposals to Word: {ex.Message}");
            }
        }

        [RequirePermission(Permission.Users_Admin_Export)]
        [HttpPost("admin/export/all-excel")]
        [HttpPost("/Admin/ExportAllExcel")]
        public async Task<IActionResult> AdminExportAllExcel([FromBody] AdminExportRequest request)
        {
            try
            {
                var command = new ExportAllProposalsExcelCommandRequest { ProposalIds = request.ProposalIds };
                var response = await _mediator.Send(command);
                if (response.Succeeded && response.FileBytes != null)
                    return File(response.FileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", response.FileName);
                return BadRequest(response.ErrorMessage ?? "Error exporting all proposals to Excel");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error exporting all proposals to Excel: {ex.Message}");
            }
        }

        // Admin - AJAX endpoint for getting all proposals
        [RequirePermission(Permission.Users_Admin_Dashboard)]
        [HttpGet("admin/get-all-proposals")]
        [HttpGet("/Admin/GetAllProposals")]
        public async Task<IActionResult> AdminGetAllProposals()
        {
            try
            {
                var response = await _mediator.Send(new GetAllProposalQueryRequest { CurrentUserId = null, IsAdmin = true });
                
                if (response.Result != null)
                {
                    var proposals = response.Result as System.Collections.Generic.List<SO.Application.DTOs.ProposalModule.Proposal.ListProposal>;
                    
                    var result = proposals?.Select(p => new
                    {
                        id = p.Id,
                        proposalName = p.ProposalName,
                        companyName = p.CompanyName,
                        preparedBy = p.PreparedBy,
                        proposalDate = p.ProposalDate,
                        status = p.Status,
                        totalAmount = p.TotalAmount
                    }).ToList();

                    return Json(result);
                }
                
                return Json(new List<object>());
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        // =============== SUPERADMIN (eski SuperAdminController) ===============

        [RequirePermission(Permission.Users_SuperAdmin_Dashboard)]
        [HttpGet("superadmin/dashboard")]
        [HttpGet("/SuperAdmin/Dashboard")]
        public async Task<IActionResult> SuperAdminDashboard()
        {
            try
            {
                var proposalsResponse = await _mediator.Send(new GetAllProposalQueryRequest { CurrentUserId = null, IsAdmin = true });
                var accountsResponse = await _mediator.Send(new GetAllAccountQueryRequest { CurrentUserId = null, IsAdmin = true });
                var addressesResponse = await _mediator.Send(new GetAllAddressQueryRequest { CurrentUserId = null, IsAdmin = true });
                var proposals = proposalsResponse.Result as System.Collections.ICollection;
                var accounts = accountsResponse.Result as System.Collections.ICollection;
                var addresses = addressesResponse.Result as System.Collections.ICollection;
                var totalUsers = await _userManager.Users.CountAsync();
                var adminUsers = await _userManager.GetUsersInRoleAsync("Admin");
                var managerUsers = await _userManager.GetUsersInRoleAsync("Manager");
                var superAdminUsers = await _userManager.GetUsersInRoleAsync("SuperAdmin");
                ViewBag.TotalProposals = proposals?.Count ?? 0;
                ViewBag.TotalAccounts = accounts?.Count ?? 0;
                ViewBag.TotalAddresses = addresses?.Count ?? 0;
                ViewBag.TotalUsers = totalUsers;
                ViewBag.AdminUsers = adminUsers.Count;
                ViewBag.ManagerUsers = managerUsers.Count;
                ViewBag.SuperAdminUsers = superAdminUsers.Count;
                ViewBag.IsSuperAdmin = true;
                return View("~/Views/SuperAdmin/Dashboard.cshtml");
            }
            catch
            {
                return View("Error");
            }
        }

        [RequirePermission(Permission.Users_SuperAdmin_ManageUsers)]
        [HttpGet("superadmin/users")]
        [HttpGet("/SuperAdmin/Users")]
        public async Task<IActionResult> SuperAdminUsers()
        {
            try
            {
                var users = await _userManager.Users.ToListAsync();
                var vm = new System.Collections.Generic.List<object>();
                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    vm.Add(new { Id = user.Id, UserName = user.UserName, Email = user.Email, FullName = user.FullName, CreatedDate = user.CreatedDate, IsActive = user.IsActive, Roles = string.Join(", ", roles) });
                }
                return View("~/Views/SuperAdmin/Users.cshtml", vm);
            }
            catch
            {
                return View("Error");
            }
        }

        [RequirePermission(Permission.Users_SuperAdmin_ManageUsers)]
        [ValidateAntiForgeryToken]
        [HttpPost("superadmin/change-role")]
        [HttpPost("/SuperAdmin/ChangeUserRole")]
        public async Task<IActionResult> SuperAdminChangeUserRole(string userId, string newRole)
        {
            var response = await _mediator.Send(new SO.Application.Features.Commands.AdminModule.User.ChangeUserRole.ChangeUserRoleCommandRequest { UserId = userId, NewRole = newRole });
            if (response.Succeeded) TempData["SuccessMessage"] = response.Message; else TempData["ErrorMessage"] = response.Message;
            return RedirectToAction("SuperAdminUsers");
        }

        [RequirePermission(Permission.Users_SuperAdmin_ManageUsers)]
        [ValidateAntiForgeryToken]
        [HttpPost("superadmin/toggle-status")]
        [HttpPost("/SuperAdmin/ToggleUserStatus")]
        public async Task<IActionResult> SuperAdminToggleUserStatus(string userId)
        {
            var response = await _mediator.Send(new SO.Application.Features.Commands.AdminModule.User.ToggleUserStatus.ToggleUserStatusCommandRequest { UserId = userId });
            if (response.Succeeded) TempData["SuccessMessage"] = response.Message; else TempData["ErrorMessage"] = response.Message;
            return RedirectToAction("SuperAdminUsers");
        }

        // SuperAdmin - Müşteriler (Accounts)
        [RequirePermission(Permission.Users_SuperAdmin_Dashboard)]
        [HttpGet("superadmin/accounts")]
        [HttpGet("/SuperAdmin/Accounts")]
        public async Task<IActionResult> SuperAdminAccounts()
        {
            try
            {
                var response = await _mediator.Send(new GetAllAccountQueryRequest { CurrentUserId = null, IsAdmin = true });
                return View("~/Views/SuperAdmin/Accounts.cshtml", response);
            }
            catch
            {
                return View("Error");
            }
        }

        // SuperAdmin - AJAX endpoint for getting all accounts
        [RequirePermission(Permission.Users_SuperAdmin_Dashboard)]
        [HttpGet("superadmin/get-all-accounts")]
        [HttpGet("/SuperAdmin/GetAllAccounts")]
        public async Task<IActionResult> SuperAdminGetAllAccounts()
        {
            try
            {
                var response = await _mediator.Send(new GetAllAccountQueryRequest { CurrentUserId = null, IsAdmin = true });
                
                if (response.Result != null)
                {
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
                        isActive = a.IsActive,
                        createdDate = a.CreatedDate
                    }).ToList();

                    return Json(result);
                }
                
                return Json(new List<object>());
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        // SuperAdmin - Adresler (Addresses)
        [RequirePermission(Permission.Users_SuperAdmin_Dashboard)]
        [HttpGet("superadmin/addresses")]
        [HttpGet("/SuperAdmin/Addresses")]
        public async Task<IActionResult> SuperAdminAddresses()
        {
            try
            {
                var response = await _mediator.Send(new GetAllAddressQueryRequest { CurrentUserId = null, IsAdmin = true });
                return View("~/Views/SuperAdmin/Addresses.cshtml", response);
            }
            catch
            {
                return View("Error");
            }
        }

        // SuperAdmin - AJAX endpoint for getting all addresses
        [RequirePermission(Permission.Users_SuperAdmin_Dashboard)]
        [HttpGet("superadmin/get-all-addresses")]
        [HttpGet("/SuperAdmin/GetAllAddresses")]
        public async Task<IActionResult> SuperAdminGetAllAddresses()
        {
            try
            {
                var response = await _mediator.Send(new GetAllAddressQueryRequest { CurrentUserId = null, IsAdmin = true });
                
                if (response.Result != null)
                {
                    var addresses = response.Result as System.Collections.Generic.List<SO.Application.DTOs.AccountModule.Address.ListAddress>;
                    
                    var result = addresses?.Select(a => new
                    {
                        id = a.Id,
                        accountId = a.AccountId,
                        accountName = a.CompanyName, // CompanyName kullan
                        addressType = a.AddressType,
                        addressLine1 = a.AddressLine1,
                        addressLine2 = a.AddressLine2,
                        city = a.City,
                        state = a.State,
                        postalCode = a.PostalCode,
                        country = a.Country,
                        isActive = a.Active, // Active kullan
                        createdDate = a.CreatedDate
                    }).ToList();

                    return Json(result);
                }
                
                return Json(new List<object>());
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        // SuperAdmin - Teklifler (Proposals)
        [RequirePermission(Permission.Users_SuperAdmin_Dashboard)]
        [HttpGet("superadmin/proposals")]
        [HttpGet("/SuperAdmin/Proposals")]
        public async Task<IActionResult> SuperAdminProposals()
        {
            try
            {
                var response = await _mediator.Send(new GetAllProposalQueryRequest { CurrentUserId = null, IsAdmin = true });
                return View("~/Views/SuperAdmin/Proposals.cshtml", response);
            }
            catch
            {
                return View("Error");
            }
        }

        // SuperAdmin - AJAX endpoint for getting all proposals
        [RequirePermission(Permission.Users_SuperAdmin_Dashboard)]
        [HttpGet("superadmin/get-all-proposals")]
        [HttpGet("/SuperAdmin/GetAllProposals")]
        public async Task<IActionResult> SuperAdminGetAllProposals()
        {
            try
            {
                var response = await _mediator.Send(new GetAllProposalQueryRequest { CurrentUserId = null, IsAdmin = true });
                
                if (response.Result != null)
                {
                    var proposals = response.Result as System.Collections.Generic.List<SO.Application.DTOs.ProposalModule.Proposal.ListProposal>;
                    
                    var result = proposals?.Select(p => new
                    {
                        id = p.Id,
                        proposalName = p.ProposalName,
                        companyName = p.CompanyName,
                        preparedBy = p.PreparedBy,
                        proposalDate = p.ProposalDate,
                        status = p.Status,
                        totalAmount = p.TotalAmount
                    }).ToList();

                    return Json(result);
                }
                
                return Json(new List<object>());
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [RequirePermission(Permission.Users_SuperAdmin_ManageUsers)]
        [ValidateAntiForgeryToken]
        [HttpPost("superadmin/delete-user")]
        [HttpPost("/SuperAdmin/DeleteUser")]
        public async Task<IActionResult> SuperAdminDeleteUser(string userId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var response = await _mediator.Send(new SO.Application.Features.Commands.AdminModule.User.DeleteUser.DeleteUserCommandRequest { UserId = userId, CurrentUserId = currentUser?.Id ?? string.Empty });
            if (response.Succeeded) TempData["SuccessMessage"] = response.Message; else TempData["ErrorMessage"] = response.Message;
            return RedirectToAction("SuperAdminUsers");
        }

        [RequirePermission(Permission.Users_SuperAdmin_Export)]
        [HttpGet("superadmin/export/proposal-word")]
        [HttpGet("/SuperAdmin/ExportProposalWord")]
        public async Task<IActionResult> SuperAdminExportProposalWord(Guid id)
        {
            try
            {
                var proposalsResponse = await _mediator.Send(new GetAllProposalQueryRequest { CurrentUserId = null, IsAdmin = true });
                var proposals = proposalsResponse.Result as System.Collections.Generic.List<SO.Application.DTOs.ProposalModule.Proposal.ListProposal>;
                var proposal = proposals?.FirstOrDefault(p => p.Id == id);
                if (proposal == null) return NotFound();
                var wordBytes = await _documentExportService.ExportProposalToWordAsync(proposal.Id);
                return File(wordBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", $"proposal-{proposal.ProposalName.Replace(" ", "-")}.docx");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error exporting proposal: {ex.Message}");
            }
        }

        [RequirePermission(Permission.Users_SuperAdmin_Export)]
        [HttpGet("superadmin/export/all-word")]
        [HttpGet("/SuperAdmin/ExportAllWord")]
        public async Task<IActionResult> SuperAdminExportAllWord()
        {
            try
            {
                var proposalsResponse = await _mediator.Send(new GetAllProposalQueryRequest { CurrentUserId = null, IsAdmin = true });
                var proposals = proposalsResponse.Result as System.Collections.Generic.List<SO.Application.DTOs.ProposalModule.Proposal.ListProposal>;
                if (proposals == null || !proposals.Any()) return BadRequest("No proposals found to export");
                var wordBytes = await _documentExportService.ExportAllProposalsToWordAsync(proposals.Select(p => p.Id).ToList());
                return File(wordBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "all-proposals.docx");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error exporting proposals: {ex.Message}");
            }
        }

        [RequirePermission(Permission.Users_SuperAdmin_Export)]
        [HttpGet("superadmin/export/all-excel")]
        [HttpGet("/SuperAdmin/ExportAllExcel")]
        public async Task<IActionResult> SuperAdminExportAllExcel()
        {
            try
            {
                var proposalsResponse = await _mediator.Send(new GetAllProposalQueryRequest { CurrentUserId = null, IsAdmin = true });
                var proposals = proposalsResponse.Result as System.Collections.Generic.List<SO.Application.DTOs.ProposalModule.Proposal.ListProposal>;
                if (proposals == null || !proposals.Any()) return BadRequest("No proposals found to export");
                var excelBytes = await _documentExportService.ExportAllProposalsToExcelAsync(proposals.Select(p => p.Id).ToList());
                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "all-proposals.xlsx");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error exporting proposals: {ex.Message}");
            }
        }
    }

    public class AdminExportRequest
    {
        public System.Collections.Generic.List<Guid> ProposalIds { get; set; } = new System.Collections.Generic.List<Guid>();
    }
}


