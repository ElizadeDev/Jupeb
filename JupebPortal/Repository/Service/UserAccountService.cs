using JupebPortal.Models;
using JupebPortal.Repository.AppServices;
using JupebPortal.Repository.Interface;
using Microsoft.AspNetCore.Identity;

namespace JupebPortal.Repository.Service
{
    public class UserAccountService : IUserAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public UserAccountService(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IUserService userService,
            IEmailService emailService,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _userService = userService;
            _emailService = emailService;
            _configuration = configuration;
        }


        public async Task<ApplicationUser> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }


        

        public async Task GenerateForgotPasswordTokenAsync(ApplicationUser user)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            if (!string.IsNullOrEmpty(token))
            {
                await SendForgotPasswordEmail(user, token);
            }
        }



        public async Task<IdentityResult> ConfirmEmailAsync(string uid, string token)
        {
            return await _userManager.ConfirmEmailAsync(await _userManager.FindByIdAsync(uid), token);
        }

        public async Task<IdentityResult> ResetPasswordAsync(ResetPassword model)
        {
            return await _userManager.ResetPasswordAsync(await _userManager.FindByIdAsync(model.UserId), model.Token, model.NewPassword);
        }


        private async Task SendForgotPasswordEmail(ApplicationUser user, string token)
        {
            try
            {
                string appDomain = _configuration.GetSection("Application:AppDomain").Value;
                string confirmationLink = _configuration.GetSection("Application:ForgotPassword").Value;

                UserEmailOptions options = new UserEmailOptions
                {
                    ToEmails = new List<string>() { user.Email },
                    PlaceHolders = new List<KeyValuePair<string, string>>()
                        {
                            new KeyValuePair<string, string>("{{UserName}}", user.FirstName),
                            new KeyValuePair<string, string>("{{Link}}",
                                string.Format(appDomain + confirmationLink, user.Id, token))
                        }
                };
          
                    await _emailService.PDSendEmailForForgotPassword(options);

            }
            catch
            {

            }
        }
    }
}