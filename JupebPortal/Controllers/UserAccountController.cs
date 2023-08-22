using JupebPortal.Data;
using JupebPortal.Models;
using JupebPortal.Repository.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JupebPortal.Controllers
{
    public class UserAccountController : Controller
    {
        private readonly IUserAccountService _services;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserAccountController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IUserAccountService services)
        {
            _context = context;
            _userManager = userManager;
            _services = services;
        }



        [AllowAnonymous, HttpGet("forgot-password")]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [AllowAnonymous, HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPassword model)
        {
            if (ModelState.IsValid)
            {
                // code here
                var user = await _services.GetUserByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.Clear();
                    model.EmailSent = false;
                    return View(model);
                }
                await _services.GenerateForgotPasswordTokenAsync(user);
                ModelState.Clear();
                model.EmailSent = true;
            }
            return View(model);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult ResetPassword(string uid, string token)
        {
            ResetPassword resetPasswordModel = new ResetPassword
            {
                Token = token,
                UserId = uid
            };
            return View(resetPasswordModel);
        }

        [AllowAnonymous, HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPassword model)
        {
            if (ModelState.IsValid)
            {
                model.Token = model.Token.Replace(' ', '+');
                var result = await _services.ResetPasswordAsync(model);
                if (result.Succeeded)
                {
                    ModelState.Clear();
                    model.IsSuccess = true;
                    return View(model);
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }
    }
}