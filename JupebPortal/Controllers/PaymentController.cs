using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using JupebPortal.Models;
using JupebPortal.Data;
using Microsoft.EntityFrameworkCore;

namespace JupebPortal.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public PaymentController(IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager, ApplicationDbContext context, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _context = context;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }


        private string GenerateString()
        {
            //DateTime now = DateTime.Now;
            TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
            DateTimeOffset now = DateTimeOffset.UtcNow.ToOffset(timeZone.GetUtcOffset(DateTimeOffset.UtcNow));

            string year = now.Year.ToString().Substring(2); ;
            string month = now.Month.ToString().PadLeft(2, '0');
            string day = now.Day.ToString().PadLeft(2, '0');
            string hour = now.Hour.ToString().PadLeft(2, '0');
            string minute = now.Minute.ToString().PadLeft(2, '0');
            string second = now.Second.ToString().PadLeft(2, '0');

            Random random = new Random();
            int randomNumber = random.Next(10000, 99999);

            string result = $"{year}{month}{day}{hour}{minute}{second}{randomNumber}";

            return result;
        }

        public async Task<IActionResult> AddPayment(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (id != null)
            {
                ViewBag.FirstName = user.FirstName;
                ViewBag.LastName = user.Surname;
                ViewBag.Email = user.Email;
                ViewBag.Purpose = "Jupeb Application Fee";
                ViewBag.Amount = "10500";

            }
            return View();

        }


        [HttpPost]
        public async Task<IActionResult> saveTransactionId()
        {
			var TransactionID = GenerateString();
			var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var payment = new Payment
            {
                PaymentRef = null,
                TransactionID = TransactionID,
                UserId = user,
                Amount = 10500,
                Purpose = "Jupeb Application Fee"
            };
            var res = _context.Payments.Add(payment);
            if(res == null)
            {
                return BadRequest("");
            }
            await _context.SaveChangesAsync();
            return Ok(TransactionID);
        }



		[HttpPost]
        public async Task<IActionResult> Success(string paymentReference, string processorId, string transactionId, string message, decimal amount)
        {
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userDetails = await _userManager.FindByIdAsync(user.ToString());

            var existingPayment = await _context.Payments.FirstOrDefaultAsync(p => p.TransactionID == transactionId);

            if (existingPayment != null)
            {
                // Update the existing record with the passed fields
                existingPayment.PaymentRef = paymentReference;
                existingPayment.IsSuccess = true;
                existingPayment.Amount = 10500;
                existingPayment.Purpose = "Jupeb Application Fee";
                existingPayment.PaymentDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }

            // Assign applicant role on successful payment
            if (await _roleManager.RoleExistsAsync("Applicant") == false)
            {
                await _roleManager.CreateAsync(new IdentityRole("Applicant"));
            }
            await _userManager.AddToRoleAsync(userDetails, "Applicant");
            await _signInManager.RefreshSignInAsync(userDetails);

            return Content("ok");
        }

        public async Task<IActionResult> VerifyPayment()
        {
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var existingPayments = await _context.Payments.Where(p => p.UserId == user).ToListAsync();
            return View(existingPayments);
        }


    }
}



