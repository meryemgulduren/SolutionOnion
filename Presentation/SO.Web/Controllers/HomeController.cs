using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using MediatR;
using SO.Application.Features.Queries.DashboardModule.GetDashboardStatistics;
using System;
using Microsoft.AspNetCore.Identity;
using SO.Domain.Entities.Identity;
using SO.Application.Abstractions.Services;

namespace SO.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMediator _mediator;
        private readonly UserManager<AppUser> _userManager;
        private readonly IAuthorizationService _authorizationService;

        public HomeController(ILogger<HomeController> logger, IMediator mediator, UserManager<AppUser> userManager, IAuthorizationService authorizationService)
        {
            _logger = logger;
            _mediator = mediator;
            _userManager = userManager;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            // If user is authenticated, redirect to dashboard
            if (User.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToAction("Dashboard");
            }

            // Show landing page for guests
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            // Require authentication for dashboard
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                _logger.LogInformation("Loading dashboard statistics for user: {UserName}", User.Identity?.Name);
                
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
                
                // Get dashboard statistics with user context
                var dashboardResponse = await _mediator.Send(new GetDashboardStatisticsQueryRequest 
                { 
                    CurrentUserId = currentUserId,
                    IsAdmin = isAdmin
                });
                
                ViewBag.UserName = User.Identity?.Name;
                ViewBag.WelcomeMessage = $"Welcome back, {User.Identity?.Name}!";
                ViewBag.IsAdmin = isAdmin;

                return View(dashboardResponse.Statistics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dashboard statistics");
                
                // In case of error, return empty model
                ViewBag.UserName = User.Identity?.Name;
                ViewBag.WelcomeMessage = $"Welcome back, {User.Identity?.Name}!";
                
                return View(new SO.Application.DTOs.Dashboard.DashboardStatisticsDto());
            }
        }
    }
}