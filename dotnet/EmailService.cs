using sib_api_v3_sdk.Api;
using sib_api_v3_sdk.Client;
using sib_api_v3_sdk.Model;
using Sabio.Data.Providers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Sabio.Services.Interfaces;
using Sabio.Models.AppSettings;
using Sabio.Models.Requests.EmailRequests;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using System.Xml.Linq;
using System.IO;
using Task = System.Threading.Tasks.Task;
using System.Reflection;
using Sabio.Models.Domain;
using Sabio.Models.Requests.Users;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Drawing.Text;
using Sabio.Models.Domain.Users;
using Sabio.Data;


namespace Sabio.Services
{

    public class EmailService : IEmailService
    {
        private BrevoApi _brevo;
        private IWebHostEnvironment _environment;
        private AppKeys _appKeys;
        IDataProvider _data;

        public EmailService(IWebHostEnvironment environment, IOptions<BrevoApi> brevo, IOptions<AppKeys> appKeys, IDataProvider data)
        {
            _brevo = brevo.Value;
            _environment = environment;
            _appKeys = appKeys.Value;
            _data = data;
        }

        public async Task TestEmail()
        {
            string emailOfSender = _brevo.SenderEmail;
            string nameOfSender = _brevo.SenderName;
            SendSmtpEmailSender senderInfo = new SendSmtpEmailSender(nameOfSender, emailOfSender);

            SendSmtpEmailTo smtpEmailTo = new SendSmtpEmailTo("cclliiffttoonniigg@dispostable.com", "Bob");

            List<SendSmtpEmailTo> emailRecipients = new List<SendSmtpEmailTo>();
            emailRecipients.Add(smtpEmailTo);

            var sendSmtpEmail = new SendSmtpEmail(senderInfo, emailRecipients)
            {
                Subject = "This is a test.",
                TextContent = "Testing. Testing. 123. Can you read me?",
            };

            await SendEmailAsync(sendSmtpEmail);

        }

        public async Task AutoEmailReceiver(EmailAddRequest model)
        {
            string emailOfSender = _brevo.SenderEmail;
            string nameOfSender = _brevo.SenderName;
            SendSmtpEmailSender senderInfo = new SendSmtpEmailSender(nameOfSender, emailOfSender);

            string emailOfReceiver = model.ReceiverEmail;
            string nameOfReceiver = model.ReceiverName;
            SendSmtpEmailTo smtpEmailTo = new SendSmtpEmailTo(emailOfReceiver, nameOfReceiver);

            List<SendSmtpEmailTo> emailRecipients = new List<SendSmtpEmailTo>();
            emailRecipients.Add(smtpEmailTo);

            var sendSmtpEmail = new SendSmtpEmail(senderInfo, emailRecipients)
            {
                Subject = $"Thank you {model.ReceiverName}, for contacting us!",
                TextContent = LoadHtmlTemplate("testTemplate.html"),
            };

            await SendEmailAsync(sendSmtpEmail);

        }

        public async Task AutoEmailAdmin(EmailAddRequest model)
        {
            string emailOfSender = model.ReceiverEmail;
            string nameOfSender = model.ReceiverName;
            SendSmtpEmailSender senderInfo = new SendSmtpEmailSender(nameOfSender, emailOfSender);

            string emailOfReceiver = _brevo.SenderEmail;
            string nameOfReceiver = _brevo.SenderName;
            SendSmtpEmailTo smtpEmailTo = new SendSmtpEmailTo(emailOfReceiver, nameOfReceiver);

            List<SendSmtpEmailTo> emailRecipients = new List<SendSmtpEmailTo>();
            emailRecipients.Add(smtpEmailTo);

            var sendSmtpEmail = new SendSmtpEmail(senderInfo, emailRecipients)
            {
                Subject = $"{model.ReceiverName} is trying to contact us!",
                TextContent = $"{model.ReceiverName} said {model.Message}",
            };

            await SendEmailAsync(sendSmtpEmail);

        }

        public async Task EmailConfirm(UserAddRequest model, string tokenId)
        {
            string emailOfSender = _brevo.SenderEmail;
            string nameOfSender = _brevo.SenderName;
            SendSmtpEmailSender senderInfo = new SendSmtpEmailSender(nameOfSender, emailOfSender);

            string emailOfReceiver = model.Email;
            string nameOfReceiver = model.FirstName;
            SendSmtpEmailTo smtpEmailTo = new SendSmtpEmailTo(emailOfReceiver, nameOfReceiver);

            List<SendSmtpEmailTo> emailRecipients = new List<SendSmtpEmailTo>() {smtpEmailTo};
            
            var sendSmtpEmail = new SendSmtpEmail(senderInfo, emailRecipients)
            {

                Subject = "Confirm your Email Address",
                TextContent = LoadHtmlTemplate("emailConfirmation.html", null, model.FirstName, tokenId)

            };
            await SendEmailAsync(sendSmtpEmail);
        }

        public bool EmailCheck(User model, string tokenId)
        {
            string procName = "[dbo].[User_EmailCheck]";

            int rowsAffected = _data.ExecuteNonQuery(procName, delegate (SqlParameterCollection inputCollection)
            {
                inputCollection.AddWithValue("@Email", model.Email);
                inputCollection.AddWithValue("@Token", tokenId);
            });
            
            if (rowsAffected == 1) { return true; }
            return false;             
        }

        public async Task AutoEmailPasswordReset(User model, string tokenId)
        {

            string emailOfSender = _brevo.SenderEmail;
            string nameOfSender = _brevo.SenderName;
            SendSmtpEmailSender senderInfo = new SendSmtpEmailSender(nameOfSender, emailOfSender);

            string emailOfReceiver = model.Email;
            SendSmtpEmailTo smtpEmailTo = new SendSmtpEmailTo(emailOfReceiver);

            List<SendSmtpEmailTo> emailRecipients = new List<SendSmtpEmailTo>() { smtpEmailTo };

            var sendSmtpEmail = new SendSmtpEmail(senderInfo, emailRecipients)
            {

                Subject = "Reset your Tabi Password",
                TextContent = LoadHtmlTemplatePasswordReset("passwordReset.html", tokenId)

            };
            await SendEmailAsync(sendSmtpEmail);
        }
        private string LoadHtmlTemplatePasswordReset(string templateFileName, string tokenId)
        {
            try
            {
                string templatePath = Path.Combine(_environment.WebRootPath, "EmailTemplates", templateFileName);
                if (File.Exists(templatePath))
                {
                    //For development
                    string customLink = $"https://localhost:3000/changepassword?token={tokenId}";
                    //For live Site
                    //string customLink = $"{_appKeys.DomainUrl}/changepassword?token={tokenId}"; 
                    string customScript = File.ReadAllText(templatePath).Replace("Reset-Link-Insert", customLink);

                    return customScript;
                }
                else
                {
                    return string.Empty;
                }
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }

        private async Task SendEmailAsync(SendSmtpEmail email)
        {
            try
            {
                Configuration.Default.ApiKey["api-key"] = _brevo.ApiKey;
                var apiInstance = new TransactionalEmailsApi();
                CreateSmtpEmail result = await apiInstance.SendTransacEmailAsync(email);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message, "An error occurred while sending the email.");
                throw;
            }

        }

        private string LoadHtmlTemplate(string templateFileName, string businessName = null, string firstName = null, string tokenId = null)
        {
            try
            {
                string templatePath = Path.Combine(_environment.WebRootPath, "EmailTemplates", templateFileName);
                if (File.Exists(templatePath))
                {
                    object newkey = _appKeys;
                    string url = _appKeys.DomainUrl;

                    if (tokenId != null)
                    {
                        //LiveUrl
                        string customLink = $"{url}/confirmuser?tokenId={tokenId}";
                        
                        //TestUrl
                        //string customLink = $"https://localhost:3000/confirmuser?tokenId={tokenId}";
                        string customScript = File.ReadAllText(templatePath).Replace("Confirm-Link-Insert", customLink).Replace("Users-First-Name", firstName);

                        return customScript;
                    }
                    else if (businessName != null)
                    {
                        return File.ReadAllText(templatePath).Replace("Business-Name", businessName);
                    }
                    else { return File.ReadAllText(templatePath); }
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

    }
}
