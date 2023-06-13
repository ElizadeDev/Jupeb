using Microsoft.AspNetCore.Identity;
using JupebPortal.Models;

namespace JupebPortal.Repository.Interface
{
    public interface IUserAccountService
    {
        Task<ApplicationUser> GetUserByEmailAsync(string email);

        Task GenerateForgotPasswordTokenAsync(ApplicationUser user);

        Task<IdentityResult> ResetPasswordAsync(ResetPassword model);
    }
}