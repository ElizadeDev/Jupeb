using JupebPortal.Data;
using JupebPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace JupebPortal.Controllers.Admin
{
    [Authorize(Roles = "Admin,Admission officer,Bursary officer")]
    public class AdminController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            // Get the current user
            var currentUser = await _userManager.GetUserAsync(User);
            var numForms = await _context.ApplicationForms.CountAsync();
            var Users = await _userManager.Users.CountAsync();
            var payments = await _context.Payments.Where(m => m.IsSuccess == true).CountAsync();

            var totalApplicationFees = await _context.Payments
                 .Where(m => m.IsSuccess == true)
                    .SumAsync(m => m.Amount);



            string totalApplicationFee = totalApplicationFees.ToString("N0");

            ViewBag.numForms = numForms;
            ViewBag.Users = Users;
            ViewBag.Payment = payments;
            ViewBag.AppFee = totalApplicationFee;
            
            if (currentUser != null)
            {
                // Access the user's unique identifier
                var userId = currentUser.Id;
                return View(currentUser);

            }
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

        public async Task<IActionResult> GetAllApplicants(string id)
        {

            var applicationDbContext = _context.ApplicationForms
                .Include(a => a.Programme1).Include(a => a.Programme2);
            var entries = await applicationDbContext.ToListAsync();
            return View(entries);
        }
        public IActionResult VerifyPayment(string? transactionId)
        {

            ViewBag.TransactionID = transactionId;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> VerifyPayment(string transactionId, string paymentReference, string paymentDate, decimal amount, string paymentState)
        {
            if (paymentState != "SUCCESSFUL")
            {
                return Problem("Payment not completed");
            }
            var fee = await _context.Payments.FirstOrDefaultAsync(f => f.TransactionID == transactionId);
            if (fee == null)
            {
                return NotFound(); // User's fee record not found
            }
            var userId = fee.UserId;
            var user = await _userManager.FindByIdAsync(userId);
            // Convert paymentDate from string to DateTime
            DateTime.TryParse(paymentDate, out DateTime parsedPaymentDate);

            // Update the fee record
            fee.PaymentRef = paymentReference;
            fee.IsSuccess = true;
            fee.Amount = amount;
            fee.PaymentDate = parsedPaymentDate;

            _context.Payments.Update(fee);
            await _context.SaveChangesAsync();


            // Assign applicant role on successful payment
            await _userManager.AddToRoleAsync(user, "Applicant");

            return Ok(); // Success
        }
    }

}