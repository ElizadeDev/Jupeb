using JupebPortal.Data;
using JupebPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace JupebPortal.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ILogger<HomeController> logger,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
        }
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // There is no logged in user
            if (userId == null)
            {
                return View();
            }

            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Admin");
            }
            // Check if userId exists in the "Payments" table
            bool userIdExists = await _context.Payments.AnyAsync(p => p.UserId == userId);

            // Check if userId exists in the "Application Form" table
            bool userFormExists = await _context.ApplicationForms.AnyAsync(p => p.UserId == userId);
            bool IsSubmitted = await _context.ApplicationForms.AnyAsync(p => p.UserId == userId && p.isSubmitted == true);

            if (!userIdExists)
            {
                // User has no payment record. Redirect to New Applicant Index
                return RedirectToAction("NewApplicantIndex", "Home", new { id = userId });
            }
            // Retrieve the records from the "Payments" table for the userId
            var FeeRecord = await _context.Payments
                .Where(p => p.UserId == userId)
                .ToListAsync();


            // Check the value of the "IsSuccess" field for each record
            foreach (var fee in FeeRecord)
            {
                bool isSuccess = fee.IsSuccess == true;

                // If there is a successful payment
                if (isSuccess)
                {

                    if (!userFormExists)
                    {
                        // User doesn't have form record yet. Redirect to the Applicant Form page
                        return RedirectToAction("Create", "ApplicationForm", new { id = userId });
                    }

                    if (!IsSubmitted)
                    {

                        // User has not submitted form yet. Redirect to the Applicant Form page
                        return RedirectToAction("Create", "ApplicationForm", new { id = userId });
                    }

                    // User has completed payment. Proceed to home page
                    return View("ApplicationSuccess");
                }
            }
            // User has no successful payment record. Redirect to New Applicant Index
            return RedirectToAction("NewApplicantIndex", "Home", new { id = userId });
        }

        public async Task<IActionResult> NewApplicantIndex(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            return View(user);
        }
        public IActionResult ApplicationSuccess(string id)
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}