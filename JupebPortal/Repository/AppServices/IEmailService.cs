using JupebPortal.Models;
using System.Threading.Tasks;


namespace JupebPortal.Repository.AppServices
{
    public interface IEmailService
    {
        //Task SendTestEmail(UserEmailOptions userEmailOptions);

        //Task SendEmailForEmailConfirmation(UserEmailOptions userEmailOptions);

        //Task SendEmailForForgotPassword(UserEmailOptions userEmailOptions);
        Task<bool> PDSendEmailRegistrationSuccess(UserEmailOptions userEmailOptions);
        Task<bool> PDSendEmailForForgotPassword(UserEmailOptions userEmailOptions);
        Task<bool> SendErrorMail(string errorTitle, string errorMsg, string errorSource);
    }

}