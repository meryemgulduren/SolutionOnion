using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SO.Application.Features.Commands.ProposalModule.Proposal.CreateProposal;
using SO.Application.Features.Commands.ProposalModule.Proposal.UpdateProposalSummary;
using SO.Application.Features.Queries.AccountModule.Account.GetAllAccount;
using SO.Application.Features.Queries.ProposalModule.Proposal.GetAllProposal;
using SO.Application.Features.Queries.ProposalModule.Proposal.GetByIdProposal; // YENİ
using SO.Application.Interfaces.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SO.Web.Controllers
{
    public class ProposalsController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IDocumentExportService _documentExportService;

        public ProposalsController(IMediator mediator, IDocumentExportService documentExportService)
        {
            _mediator = mediator;
            _documentExportService = documentExportService;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _mediator.Send(new GetAllProposalQueryRequest());
            return View(response);
        }
       

        
        public async Task<IActionResult> Create()
        {
            var accountsResponse = await _mediator.Send(new GetAllAccountQueryRequest());
            var accounts = accountsResponse.Result as System.Collections.Generic.List<SO.Application.DTOs.AccountModule.Account.ListAccount>;
            ViewBag.Accounts = new SelectList(accounts, "Id", "CompanyName");
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProposalCommandRequest request)
        {
            

            if (ModelState.IsValid)
            {
                // Bu komut taslak teklifi oluşturacak.
                var response = await _mediator.Send(request);

                // TODO: Handler'dan dönen ID'yi al. Şimdilik son ekleneni buluyoruz.
                // Bu geçici bir çözümdür.
                var allProposals = await _mediator.Send(new GetAllProposalQueryRequest());
                var proposalsList = allProposals.Result as System.Collections.Generic.List<SO.Application.DTOs.ProposalModule.Proposal.ListProposal>;
                var newProposalId = proposalsList.OrderByDescending(p => p.ProposalDate).First().Id;

                // Kullanıcıyı yeni oluşan teklifin sihirbaz sayfasına yönlendir.
                return RedirectToAction(nameof(Edit), new { id = newProposalId });
            }

            var accountsResponse = await _mediator.Send(new GetAllAccountQueryRequest());
            var accounts = accountsResponse.Result as System.Collections.Generic.List<SO.Application.DTOs.AccountModule.Account.ListAccount>;
            ViewBag.Accounts = new SelectList(accounts, "Id", "CompanyName", request.AccountId);
            return View(request);
        }

       
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var response = await _mediator.Send(new GetByIdProposalQueryRequest { Id = id });
            if (response.Result == null)
            {
                return NotFound();
            }

            return View(response);
        }
        public async Task<IActionResult> GetStepView(string proposalId, string stepName)
        {
            // İlgili teklifin verilerini tekrar çekiyoruz.
            var response = await _mediator.Send(new GetByIdProposalQueryRequest { Id = proposalId });
            if (response.Result == null)
            {
                return Content("<div class='alert alert-danger'>Teklif bulunamadı.</div>");
            }

            // Gelen stepName'e göre ilgili Partial View'i, teklif verileriyle birlikte döndürüyoruz.
            // Partial View'lerin isimlerinin alt çizgi (_) ile başlaması bir standarttır.
            return PartialView(stepName, response);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateSummary(string id, UpdateProposalSummaryCommandRequest request)
        {
          
            request.Id = id;

            if (ModelState.IsValid)
            {
                await _mediator.Send(request);

                
                return RedirectToAction(nameof(Edit), new { id = id });
            }

            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateInitiatorSponsor(string id, [FromForm] SO.Application.Features.Commands.ProposalModule.Proposal.UpdateInitiatorSponsor.UpdateInitiatorSponsorCommandRequest request)
        {
            request.Id = id;
            if (ModelState.IsValid)
            {
                await _mediator.Send(request);
                return RedirectToAction(nameof(Edit), new { id = id });
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCustomersAndDeliverables(string id, [FromForm] SO.Application.Features.Commands.ProposalModule.Proposal.UpdateCustomersAndDeliverables.UpdateCustomersAndDeliverablesCommandRequest request)
        {
            request.Id = id;
            if (ModelState.IsValid)
            {
                await _mediator.Send(request);
                return RedirectToAction(nameof(Edit), new { id = id });
            }
            return RedirectToAction(nameof(Index)); // Hata durumunda ana listeye dön
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProjectApproach(string id, [FromForm] SO.Application.Features.Commands.ProposalModule.Proposal.UpdateProjectApproach.UpdateProjectApproachCommandRequest request)
        {
            request.Id = id;
            if (ModelState.IsValid)
            {
                await _mediator.Send(request);
                return RedirectToAction(nameof(Edit), new { id = id });
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateResourceRequirements(string id, [FromForm] SO.Application.Features.Commands.ProposalModule.Proposal.UpdateResourceRequirements.UpdateResourceRequirementsCommandRequest request)
        {
            request.Id = id;
            if (ModelState.IsValid)
            {
                await _mediator.Send(request);
                return RedirectToAction(nameof(Edit), new { id = id });
            }
            return RedirectToAction("Edit", new { id = request.Id });
        }
        public async Task<IActionResult> CreateSummary()
        {
            var accountsResponse = await _mediator.Send(new GetAllAccountQueryRequest());
            var accounts = accountsResponse.Result as System.Collections.Generic.List<SO.Application.DTOs.AccountModule.Account.ListAccount>;
            ViewBag.Accounts = new SelectList(accounts, "Id", "CompanyName");

            // Yeni bir teklif DTO'su döndürülüyor
            var model = new CreateProposalCommandRequest();
            return View("CreateSummary", model); // Eğer View ismi CreateSummary.cshtml ise
        }

        [HttpGet]
        public async Task<IActionResult> ExportToWord(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest("Proposal ID is required");
                }

                if (!Guid.TryParse(id, out Guid proposalId))
                {
                    return BadRequest("Invalid Proposal ID format");
                }

                var wordDocument = await _documentExportService.ExportProposalToWordAsync(proposalId);
                
                var proposalData = await _documentExportService.GetProposalExportDataAsync(proposalId);
                var fileName = $"Proje_Teklifi_{proposalData.Title?.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd_HHmmss}.docx";

                return File(wordDocument, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", fileName);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, "An error occurred while generating the document");
            }
        }

    }
}