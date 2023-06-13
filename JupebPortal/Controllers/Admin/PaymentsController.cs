using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using JupebPortal.Data;
using JupebPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using JupebPortal.ViewModels;

namespace JupebPortal.Controllers.Admin
{
    [Authorize(Roles = "Admin,Bursary officer")]
    public class PaymentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PaymentsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Payments
        public async Task<IActionResult> Index()
        {
            if (_context.Payments == null)
            {
                return Problem("Entity set 'ApplicationDbContext.ApplicationFee'  is null.");
            }
            var payments = await _context.Payments.ToListAsync();
            var paymentViewModels = new List<PaymentsViewModel>();

            foreach (var payment in payments)
            {
                var user = await _userManager.FindByIdAsync(payment.UserId);
                var userEmail = user.Email;
                var userName = user.Surname + " " + user.FirstName;
                var paymentViewModel = new PaymentsViewModel
                {
                    Payment = payment,
                    UserEmail = userEmail,
                    UserName = userName
                };

                paymentViewModels.Add(paymentViewModel);
            }

            return View(paymentViewModels);
        }

        // GET: Payments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Payments == null)
            {
                return NotFound();
            }

            var applicationFeeModel = await _context.Payments
                .FirstOrDefaultAsync(m => m.Id == id);
            if (applicationFeeModel == null)
            {
                return NotFound();
            }

            return View(applicationFeeModel);
        }

        // GET: Payments/Create
        public async Task<IActionResult> Create()
        {
            var applicants = (await _userManager.Users.ToListAsync())
                .OrderBy(u => u.Surname)
                .Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = u.Surname + " " + u.FirstName + " (" + u.Email + ")"
                })
                .ToList();
            ViewBag.applicants = new SelectList(applicants, "Value", "Text");
            return View();
        }

        // POST: Payments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,PaymentRef,TransactionID,Amount,Purpose,IsSuccess,PaymentDate")] Payment payment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(payment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(payment);
        }

        // GET: Payments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Payments == null)
            {
                return NotFound();
            }

            var applicationFeeModel = await _context.Payments.FindAsync(id);
            if (applicationFeeModel == null)
            {
                return NotFound();
            }
            return View(applicationFeeModel);
        }

        // POST: Payments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,PaymentRef,TransactionID,Amount,Purpose,IsSuccess,PaymentDate")] Payment payment)
        {
            if (id != payment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(payment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApplicationFeeModelExists(payment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(payment);
        }

        // GET: Payments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Payments == null)
            {
                return NotFound();
            }

            var applicationFeeModel = await _context.Payments
                .FirstOrDefaultAsync(m => m.Id == id);
            if (applicationFeeModel == null)
            {
                return NotFound();
            }

            return View(applicationFeeModel);
        }

        // POST: Payments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Payments == null)
            {
                return Problem("Entity set 'ApplicationDbContext.ApplicationFee'  is null.");
            }
            var applicationFeeModel = await _context.Payments.FindAsync(id);
            if (applicationFeeModel != null)
            {
                _context.Payments.Remove(applicationFeeModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ApplicationFeeModelExists(int id)
        {
            return (_context.Payments?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public async Task<IActionResult> GetSuccessfulPayments()
        {
            if (_context.Payments == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Payments'  is null.");
            }
            var payments = await _context.Payments.Where(m => m.IsSuccess == true).ToListAsync();
            var paymentViewModels = new List<PaymentsViewModel>();

            foreach (var payment in payments)
            {
                var user = await _userManager.FindByIdAsync(payment.UserId);
                var userEmail = user.Email;
                var userName = user.Surname + " " + user.FirstName;
                var paymentViewModel = new PaymentsViewModel
                {
                    Payment = payment,
                    UserEmail = userEmail,
                    UserName = userName
                };

                paymentViewModels.Add(paymentViewModel);
            }

            return View(nameof(Index), paymentViewModels);
        }
    }
}
