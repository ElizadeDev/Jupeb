using JupebPortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PD.EmailSender.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace JupebPortal.Repository.AppServices
{
    public class EmailService : IEmailService
    {
        private const string templatePath = @"EmailTemplate/{0}.html";
        private readonly SMTPConfig _smtpConfig;

        public async Task<bool> PDSendEmailRegistrationSuccess(UserEmailOptions userEmailOptions)
        {
            userEmailOptions.Subject = UpdatePlaceHolders("ELIZADE UNIVERSITY - Application Successful.", userEmailOptions.PlaceHolders);

            userEmailOptions.Body = UpdatePlaceHolders(GetEmailBody("RegistrationSuccessful"), userEmailOptions.PlaceHolders);
            var Isauthenticated = await SendMail.AuthenticateSenderDomain("info.admission@elizadeuniversity.edu.ng", "xkqchlmyfhppchsf");

            // WITHOUT USING DEFAULT TEMPLATE
            var res = SendMail.SendSingleEmail(new PD.EmailSender.Helpers.Model.MessageModel
            {
                Subject = userEmailOptions.Subject,
                EmailAddresses = userEmailOptions.ToEmails.ToArray(),
                EmailDisplayName = "Elizade University",
                Message = userEmailOptions.Body

            }, Isauthenticated.Settings);
            
            return res;
        }

        public EmailService(IOptions<SMTPConfig> smtpConfig)
        {
            _smtpConfig = smtpConfig.Value;
        }


        private string GetEmailBody(string templateName)
        {
            var body = File.ReadAllText(string.Format(templatePath, templateName));
            return body;
        }

        private string UpdatePlaceHolders(string text, List<KeyValuePair<string, string>> keyValuePairs)
        {
            if (!string.IsNullOrEmpty(text) && keyValuePairs != null)
            {
                foreach (var placeholder in keyValuePairs)
                {
                    if (text.Contains(placeholder.Key))
                    {
                        text = text.Replace(placeholder.Key, placeholder.Value);
                    }
                }
            }

            return text;
        }

        public async Task<bool> PDSendEmailForForgotPassword(UserEmailOptions userEmailOptions)
        {
            userEmailOptions.Subject = UpdatePlaceHolders("Hello {{UserName}}, reset your password.", userEmailOptions.PlaceHolders);
            userEmailOptions.Body = UpdatePlaceHolders(GetEmailBody("ForgotPassword"), userEmailOptions.PlaceHolders);
            //var Isauthenticated = await SendMail.AuthenticateSenderDomain("admin@projectdriveng.com.ng", "nimda9876@Elo");
            var Isauthenticated = await SendMail.AuthenticateSenderDomain("info.admission@elizadeuniversity.edu.ng", "xkqchlmyfhppchsf");

            //USING TEMPLATE
            //var res = await SendMail.SendSingleEmailAsync(new PD.EmailSender.Helpers.Model.MessageModel
            //{
            //    Subject = userEmailOptions.Subject,
            //    EmailAddresses = userEmailOptions.ToEmails.ToArray(),
            //    Message = userEmailOptions.Body

            //}, Isauthenticated.Settings,"templateone");
            //return res;

            // WITHOUT USING DEFAULT TEMPLATE
            var res = SendMail.SendSingleEmail(new PD.EmailSender.Helpers.Model.MessageModel
            {
                Subject = userEmailOptions.Subject,
                EmailAddresses = userEmailOptions.ToEmails.ToArray(),
                EmailDisplayName = "no-reply@elizadeuniversity.edu.ng",
                Message = userEmailOptions.Body

            }, Isauthenticated.Settings);
            return res;
        }

    }
    }