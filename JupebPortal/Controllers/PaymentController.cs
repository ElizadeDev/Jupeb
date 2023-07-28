using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using JupebPortal.Models;
using JupebPortal.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace JupebPortal.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public PaymentController(IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager, ApplicationDbContext context, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _context = context;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;
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

        [HttpPost]
        public async Task<IActionResult> UpdateTransactionStatus(string transactionId, string paymentReference, DateTime paymentDate, decimal amount, string paymentState)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound(); // User not found
            }
            if (paymentState != "SUCCESSFUL")
            {
                return BadRequest("Payment not completed");
            }
            var fee = await _context.Payments.FirstOrDefaultAsync(f => f.UserId == user.Id && f.TransactionID == transactionId);
            if (fee == null)
            {
                return NotFound(); // User's fee record not found
            }

            // Update the fee record
            fee.PaymentRef = paymentReference;
            fee.IsSuccess = true;
            fee.Amount = amount;
            fee.PaymentDate = paymentDate;

            _context.Payments.Update(fee);
            await _context.SaveChangesAsync();

            // Assign applicant role on successful payment
            await _userManager.AddToRoleAsync(user, "Applicant");
            await _signInManager.RefreshSignInAsync(user);

            return Ok(); // Success
        }

        [HttpGet]

        public async Task<IActionResult> GetTransactionStatus(string transactionId)
        {
            // Retrieve the privateKey securely from appsettings.json configuration file
            string privateKey = _configuration.GetSection("Remita:SecretKey").Value;

            // Create the SHA-512 hash of the transaction ID and privateKey
            var hashValue = "";
            using (var shaObj = SHA512.Create())
            {
                var hashValueBytes = shaObj.ComputeHash(Encoding.UTF8.GetBytes(transactionId + privateKey));
                hashValue = BitConverter.ToString(hashValueBytes).Replace("-", string.Empty);
            }
            // Construct the URL and headers for the request
            string url = _configuration.GetSection("Remita:BaseStatusUrl").Value + transactionId;
            string publicKey = _configuration.GetSection("Remita:PublicKey").Value;

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("publicKey", publicKey);
                //client.DefaultRequestHeaders.Add("Content-Type", "application/json");
                client.DefaultRequestHeaders.Add("TXN_HASH", hashValue);

                // Make the GET request to the remita API
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    //var responseObject = JsonConvert.DeserializeObject<RemitaStatusResponse>(responseData);

                    RemitaStatusResponseRoot responseObject = JsonConvert.DeserializeObject<RemitaStatusResponseRoot>(responseData);

                    if (responseObject != null && responseObject.ResponseData.Count > 0)
                    {
                        string paymentReference = responseObject.ResponseData[0].paymentReference;
                        var paymentDate = responseObject.ResponseData[0].paymentDate;
                        var amount = responseObject.ResponseData[0].amount;
                        string paymentState = responseObject.ResponseData[0].paymentState;
                        string trId = responseObject.ResponseData[0].transactionId;

                        var result = await UpdateTransactionStatus(trId, paymentReference, paymentDate, amount, paymentState);
                        if (result is BadRequestObjectResult || result is NotFoundObjectResult)
                        {
                            return BadRequest("Payment not completed.");
                        }

                        return Ok(responseData);
                    }
                }

                return BadRequest();
            }

        }


    }
}



