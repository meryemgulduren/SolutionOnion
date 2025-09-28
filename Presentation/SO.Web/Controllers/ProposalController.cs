using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using SO.Application.Abstractions.Services;
using SO.Domain.Entities.Identity;
using SO.Application.Features.Commands.ProposalModule.Proposal.CreateProposal;
using SO.Application.Features.Commands.ProposalModule.Proposal.UpdateProposalSummary;
using SO.Application.Features.Commands.ProposalModule.Proposal.UpdateProposal;
using SO.Application.Features.Commands.ProposalModule.Proposal.DeleteProposal;
using SO.Application.Features.Queries.AccountModule.Account.GetAllAccount;
using SO.Application.Features.Queries.ProposalModule.Proposal.GetAllProposal;
using SO.Application.Features.Queries.ProposalModule.Proposal.GetByIdProposal;
using SO.Application.Abstractions.Storage;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using SO.Web.Authorization;
using AuthService = SO.Application.Abstractions.Services.IAuthorizationService;
using SO.Application.DTOs.ProposalModule.Proposal;
using SO.Application.DTOs.ProposalModule.CompetitionCompany;
using SO.Application.DTOs.ProposalModule.BusinessPartner;
using SO.Application.Repositories;
using SO.Application.Abstractions.Services.ProposalModule;

namespace SO.Web.Controllers
{
    public class ProposalsController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IDocumentExportService _documentExportService;
        private readonly ILogger<ProposalsController> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly AuthService _authorizationService;
        private readonly IBusinessPartnerService _businessPartnerService;
        private readonly ICompetitionCompanyService _competitionCompanyService;
        private readonly IAppAuthorizationService _appAuthorization;

        public ProposalsController(
            IMediator mediator, 
            IDocumentExportService documentExportService,
            ILogger<ProposalsController> logger,
            UserManager<AppUser> userManager,
            AuthService authorizationService,
            IBusinessPartnerService businessPartnerService,
            ICompetitionCompanyService competitionCompanyService,
            IAppAuthorizationService appAuthorization)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _documentExportService = documentExportService ?? throw new ArgumentNullException(nameof(documentExportService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _authorizationService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));
            _businessPartnerService = businessPartnerService ?? throw new ArgumentNullException(nameof(businessPartnerService));
            _competitionCompanyService = competitionCompanyService ?? throw new ArgumentNullException(nameof(competitionCompanyService));
            _appAuthorization = appAuthorization ?? throw new ArgumentNullException(nameof(appAuthorization));
        }

        #region Helper Methods

        private async Task<(string? userId, bool canViewAll, AppUser? user)> GetCurrentUserInfoAsync()
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return (null, false, null);
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return (null, false, null);
            }

            // Admin veya SuperAdmin kontrolü
            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin") || 
                         await _userManager.IsInRoleAsync(currentUser, "SuperAdmin");

            return (currentUser.Id, isAdmin, currentUser);
        }

        private async Task<SelectList> GetAccountListAsync(string? currentUserId, bool canViewAll)
        {
            var accountRequest = new GetAllAccountQueryRequest
            {
                CurrentUserId = currentUserId,
                IsAdmin = canViewAll
            };
            
            var accountsResponse = await _mediator.Send(accountRequest);
            var accounts = accountsResponse.Result as System.Collections.Generic.List<SO.Application.DTOs.AccountModule.Account.ListAccount>;
            return new SelectList(accounts, "Id", "CompanyName");
        }

        #endregion

        [HttpGet]
        [RequirePermission(Permission.Proposals_ReadOwn)]
        public async Task<IActionResult> Index()
        {
            try
            {
                var (currentUserId, canViewAll, currentUser) = await GetCurrentUserInfoAsync();
                
                ViewBag.IsAdmin = canViewAll;
                ViewBag.CurrentUser = currentUser?.UserName;
                ViewBag.IsAuthenticated = User.Identity?.IsAuthenticated ?? false;
                
                var request = new GetAllProposalQueryRequest
                {
                    CurrentUserId = currentUserId,
                    IsAdmin = canViewAll
                };
                
                var response = await _mediator.Send(request);
                return View(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading proposals index page");
                return View("Error");
            }
        }

        [HttpGet]
        [RequirePermission(Permission.Proposals_ReadOwn)]
        public async Task<IActionResult> GetAllProposals()
        {
            try
            {
                var (currentUserId, canViewAll, _) = await GetCurrentUserInfoAsync();
                
                var request = new GetAllProposalQueryRequest
                {
                    CurrentUserId = currentUserId,
                    IsAdmin = canViewAll
                };
                
                var response = await _mediator.Send(request);
                var proposals = response.Result as System.Collections.Generic.List<SO.Application.DTOs.ProposalModule.Proposal.ListProposal>;
                
                var result = proposals?.Select(p => new
                {
                    id = p.Id,
                    proposalName = p.ProposalName,
                    companyName = p.CompanyName,
                    proposalDate = p.ProposalDate,
                    totalAmount = p.TotalAmount,
                    currency = p.Currency,
                    status = p.Status.ToString(),
                    preparedBy = p.PreparedBy
                }).ToList();

                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all proposals");
                return Json(new { error = "An error occurred while retrieving proposals" });
            }
        }


        [HttpGet]
        [RequirePermission(Permission.Proposals_Create)]
        public async Task<IActionResult> Create()
        {
            try
            {
                var (currentUserId, canViewAll, currentUser) = await GetCurrentUserInfoAsync();
                ViewBag.CurrentUserName = currentUser?.UserName;
                ViewBag.Accounts = await GetAccountListAsync(currentUserId, canViewAll);
                
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading create proposal page");
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(Permission.Proposals_Create)]
        public async Task<IActionResult> Create(CreateProposalCommandRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var (currentUserId, canViewAll, _) = await GetCurrentUserInfoAsync();
                    ViewBag.Accounts = await GetAccountListAsync(currentUserId, canViewAll);
                    return View(request);
                }

                await _mediator.Send(request);

                // Get the newly created proposal ID
                var (currentUserIdForProposal, canViewAllForProposal, _) = await GetCurrentUserInfoAsync();
                var allProposals = await _mediator.Send(new GetAllProposalQueryRequest 
                { 
                    CurrentUserId = currentUserIdForProposal, 
                    IsAdmin = canViewAllForProposal 
                });
                var proposalsList = allProposals.Result as System.Collections.Generic.List<SO.Application.DTOs.ProposalModule.Proposal.ListProposal>;
                var newProposalId = proposalsList?.OrderByDescending(p => p.ProposalDate).First()?.Id;
                
                return RedirectToAction(nameof(Edit), new { id = newProposalId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating proposal: {Message}", ex.Message);
                
                // Hata durumunda account listesini tekrar yükle
                var (currentUserId, canViewAll, _) = await GetCurrentUserInfoAsync();
                ViewBag.Accounts = await GetAccountListAsync(currentUserId, canViewAll);
                
                // Daha spesifik hata mesajları
                if (ex.Message.Contains("Validation error"))
                {
                    ModelState.AddModelError("", ex.Message);
                }
                else if (ex.Message.Contains("Account ID must be a valid GUID"))
                {
                    ModelState.AddModelError("AccountId", "Please select a valid client");
                }
                else
                {
                    ModelState.AddModelError("", "An error occurred while creating the proposal. Please try again.");
                }
                
                return View(request);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(Permission.Proposals_ReadOwn)]
        public async Task<IActionResult> CreateAjax([FromBody] CreateProposalCommandRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, message = "Validation failed", errors = ModelState });
                }

                await _mediator.Send(request);

                // Get the newly created proposal ID
                var (currentUserIdForProposal, canViewAllForProposal, _) = await GetCurrentUserInfoAsync();
                var allProposals = await _mediator.Send(new GetAllProposalQueryRequest 
                { 
                    CurrentUserId = currentUserIdForProposal, 
                    IsAdmin = canViewAllForProposal 
                });
                var proposalsList = allProposals.Result as System.Collections.Generic.List<SO.Application.DTOs.ProposalModule.Proposal.ListProposal>;
                var newProposalId = proposalsList?.OrderByDescending(p => p.ProposalDate).First()?.Id;

                return Json(new { success = true, message = "Proposal created successfully", id = newProposalId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating proposal via AJAX");
                return Json(new { success = false, message = "An error occurred while creating the proposal" });
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(Permission.Proposals_ReadOwn)]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Proposal ID is required");
            }

            try
            {
                var (currentUserId, canViewAll, _) = await GetCurrentUserInfoAsync();

                // Proposal'ı getir ve authorization kontrolü yap
                var proposalResponse = await _mediator.Send(new GetByIdProposalQueryRequest { Id = id });
                if (proposalResponse.Result == null)
                {
                    return Json(new { success = false, message = "Proposal not found" });
                }

                var proposal = proposalResponse.Result as SO.Application.DTOs.ProposalModule.Proposal.SingleProposal;
                
                // Admin değilse ve kendi proposal'ı değilse silmeye izin verme
                if (!canViewAll)
                {
                    if (string.IsNullOrEmpty(proposal.CreatedById) || proposal.CreatedById != currentUserId)
                    {
                        return Json(new { success = false, message = "You don't have permission to delete this proposal" });
                    }
                }

                await _mediator.Send(new DeleteProposalCommandRequest { Id = id });
                return Json(new { success = true, message = "Proposal deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting proposal: {ProposalId}", id);
                return Json(new { success = false, message = $"An error occurred while deleting the proposal: {ex.Message}" });
            }
        }

        [HttpGet]
        [RequirePermission(Permission.Proposals_ReadOwn)]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound("Proposal ID is required");
            }

            try
            {
                _logger.LogInformation("Loading proposal for edit: {ProposalId}", id);
                var response = await _mediator.Send(new GetByIdProposalQueryRequest { Id = id });
                
                if (response.Result == null)
                {
                    _logger.LogWarning("Proposal not found: {ProposalId}", id);
                    return NotFound("Proposal not found");
                }

                _logger.LogInformation("Proposal loaded successfully: {ProposalId}", id);
                return View(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading edit proposal page: {ProposalId}", id);
                return View("Error");
            }
        }

        // Eski adım bazlı partial yükleme yapısı kaldırıldı.

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(Permission.Proposals_ReadOwn)]
        public async Task<IActionResult> UpdateSummary(string id, UpdateProposalSummaryCommandRequest request)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Proposal ID is required");
            }

            try
            {
                var form = await Request.ReadFormAsync();
                
                request.Id = id;
                request.ProjectDescription = form["ProjectDescription"].ToString();
                // Gün sayıları için esnek parsing
                var offerDaysStr = form["OfferDurationDays"].ToString().Replace(",", ".").Replace(" ", "");
                request.OfferDurationDays = int.TryParse(offerDaysStr, out var offerDays) ? offerDays : null;
                
                var deliveryDaysStr = form["DeliveryDurationDays"].ToString().Replace(",", ".").Replace(" ", "");
                request.DeliveryDurationDays = int.TryParse(deliveryDaysStr, out var deliveryDays) ? deliveryDays : null;
                request.OfferOwner = form["OfferOwner"].ToString();
                // QuantityValue için esnek parsing
                var quantityStr = form["QuantityValue"].ToString().Replace(",", ".").Replace(" ", "").Replace("'", "");
                request.QuantityValue = decimal.TryParse(quantityStr, out var quantity) ? quantity : null;
                request.QuantityUnit = form["QuantityUnit"].ToString();
                request.GeneralNote = form["GeneralNote"].ToString();
                request.AddressId = form["AddressId"].ToString();
                
                // Ticari alanlar
                // TargetPrice için esnek parsing
                var targetPriceStr = form["TargetPrice"].ToString().Replace(",", ".").Replace(" ", "").Replace("'", "");
                request.TargetPrice = decimal.TryParse(targetPriceStr, out var targetPrice) ? targetPrice : null;
                request.PaymentMethod = form["PaymentMethod"].ToString();
                request.PaymentTerm = form["PaymentTerm"].ToString();
                request.ValidUntilDate = DateTime.TryParse(form["ValidUntilDate"], out var validDate) ? validDate : null;
                request.CommercialNote = form["CommercialNote"].ToString();

                await _mediator.Send(request);

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { 
                        success = true, 
                        message = "Ticari bilgiler başarıyla kaydedildi!"
                    });
                }
                
                // Normal form submit için başarılı mesaj ile yönlendir
                TempData["SuccessMessage"] = "Ticari bilgiler başarıyla kaydedildi!";
                var from = Request.Form["from"].ToString();
                var open = from == "commercial" ? "commercial" : null;
                return RedirectToAction(nameof(Edit), new { id = id, open = open });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating proposal summary: {ProposalId}", id);
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Bir hata oluştu: " + ex.Message });
                }
                return RedirectToAction(nameof(Edit), new { id = id });
            }
        }

        [HttpGet]
        [RequirePermission(Permission.Proposals_Export)]
        public async Task<IActionResult> ExportToWord(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Proposal ID is required");
            }

            if (!Guid.TryParse(id, out Guid proposalId))
            {
                return BadRequest("Invalid Proposal ID format");
            }

            try
            {
                var wordDocument = await _documentExportService.ExportProposalToWordAsync(proposalId);
                var proposalData = await _documentExportService.GetProposalExportDataAsync(proposalId);
                var fileName = $"Proje_Teklifi_{proposalData.Title?.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd_HHmmss}.docx";

                return File(wordDocument, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting proposal to Word: {ProposalId}", id);
                return BadRequest("An error occurred while exporting the proposal");
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetCompetitionCompanies(string proposalId)
        {
            if (string.IsNullOrEmpty(proposalId) || !Guid.TryParse(proposalId, out Guid proposalGuid))
            {
                return BadRequest("Invalid proposal ID");
            }

            try
            {
                var result = await _competitionCompanyService.GetCompetitionCompaniesByProposalIdAsync(proposalId);
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting competition companies for proposal: {ProposalId}", proposalId);
                return Json(new { error = "An error occurred while retrieving competition companies" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCompetitionCompany([FromBody] CreateCompetitionCompany request)
        {
            try
            {
                await _competitionCompanyService.CreateCompetitionCompanyAsync(request);
                return Json(new { success = true, message = "Competition company created successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating competition company");
                return Json(new { success = false, message = "An error occurred while creating the competition company" });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCompetitionCompany([FromBody] UpdateCompetitionCompany request)
        {
            try
            {
                await _competitionCompanyService.UpdateCompetitionCompanyAsync(request);
                return Json(new { success = true, message = "Competition company updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating competition company: {Id}", request.Id);
                return Json(new { success = false, message = "An error occurred while updating the competition company" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCompetitionCompany(string id)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid companyId))
            {
                return BadRequest("Invalid company ID");
            }

            try
            {
                await _competitionCompanyService.DeleteCompetitionCompanyAsync(id);
                return Json(new { success = true, message = "Competition company deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting competition company: {Id}", id);
                return Json(new { success = false, message = "An error occurred while deleting the competition company" });
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetBusinessPartners(string proposalId)
        {
            if (string.IsNullOrEmpty(proposalId) || !Guid.TryParse(proposalId, out Guid proposalGuid))
            {
                return BadRequest("Invalid proposal ID");
            }

            try
            {
                var result = await _businessPartnerService.GetBusinessPartnersByProposalIdAsync(proposalId);
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting business partners for proposal: {ProposalId}", proposalId);
                return Json(new { error = "An error occurred while retrieving business partners" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateBusinessPartner([FromBody] CreateBusinessPartner request)
        {
            try
            {
                await _businessPartnerService.CreateBusinessPartnerAsync(request);
                return Json(new { success = true, message = "Business partner created successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating business partner");
                return Json(new { success = false, message = "An error occurred while creating the business partner" });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateBusinessPartner([FromBody] UpdateBusinessPartner request)
        {
            try
            {
                await _businessPartnerService.UpdateBusinessPartnerAsync(request);
                return Json(new { success = true, message = "Business partner updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating business partner: {Id}", request.Id);
                return Json(new { success = false, message = "An error occurred while updating the business partner" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteBusinessPartner(string id)
        {
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid partnerId))
            {
                return BadRequest("Invalid partner ID");
            }

            try
            {
                await _businessPartnerService.DeleteBusinessPartnerAsync(id);
                return Json(new { success = true, message = "Business partner deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting business partner: {Id}", id);
                return Json(new { success = false, message = "An error occurred while deleting the business partner" });
            }
        }

    }
}