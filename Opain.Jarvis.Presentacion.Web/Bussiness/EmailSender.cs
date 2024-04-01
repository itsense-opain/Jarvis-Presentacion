using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;
using System.Configuration;
using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Net.Mime;
using System.Net.Http;
using System.Net;
using System.Net.Mail;
using System.Diagnostics;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Runtime.Serialization.Json;
using Microsoft.Extensions.Logging;
using Opain.Jarvis.Presentacion.Web.Bussiness;

namespace Opain.Jarvis.EnvioCorreos
{

    public class EmailSender : IEmail
    {
        private IConfiguration config;
        private object p;
        private readonly ILogger _logger;


        public IConfiguration Configuration { get; }
        public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor, IConfiguration conf, ILogger<EmailSender> logger)
        {
            Options = optionsAccessor.Value;
            Configuration = conf;
            _logger = logger;
        }

        public EmailSender(IConfiguration config, object p, ILogger<EmailSender> logger)
        {
            this.config = config;
            this.p = p;
            _logger = logger;
        }

        public AuthMessageSenderOptions Options { get; }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            if (Configuration.GetSection("SendGrid:EnviarCorreos").Value.Equals("true"))
            {
                if (Configuration.GetSection("SendGrid:EnviarCorreosOffice365").Value.Equals("true"))
                {
                    //ExecuteApiOffice365(subject, message, email);
                    SendEmail(subject, message, email);
                }
                else
                {
                    await Execute(Options.Key, subject, message, email);
                }
            }
        }


        static async Task Execute()
        {
            var apiKey = "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";
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("jarvis.opain@componenteserviex.com", "prueba");
            var subject = "Sending with SendGrid is Fun";
            var to = new EmailAddress("ingluiscantin@gmail.com", "ING");
            var plainTextContent = "and easy to do anywhere, even with C#";
            var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

            msg.MailSettings = new MailSettings
            {
                SandboxMode = new SandboxMode
                {
                    Enable = true
                }
            };

            var response = await client.SendEmailAsync(msg);
        }

        public async Task Execute(string apiKey, string subject, string message, string email)
        {

            //var apiKey = "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";
            //var client = new SendGridClient(apiKey);
            //var from = new EmailAddress("edvard.rodriguez@componenteserviex.com", "Example User");
            //var subject = "Sending with SendGrid is Fun";
            //var to = new EmailAddress("luis.cantin@componenteserviex.com", "Example User");
            //var plainTextContent = "and easy to do anywhere, even with C#";
            //var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
            //var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            //var response = await client.SendEmailAsync(msg);

            //var client = new SendGridClient(apiKey);

            //var msg = new SendGridMessage()
            //{
            //    // From = new EmailAddress("noreply@opain.co", "JARVIS - OPAIN"),
            //    From = new EmailAddress(Configuration.GetSection("SendGrid:RemitenteEmail").Value, Configuration.GetSection("SendGrid:RemitenteNombre").Value),
            //    Subject = subject
            //};

            //msg.MailSettings = new MailSettings
            //{
            //    SandboxMode = new SandboxMode
            //    {
            //        Enable = true
            //    }
            //}; 

            //msg.AddContent(MimeType.Html, message);


            //msg.AddTo(new EmailAddress(email));

            //msg.SetClickTracking(false, false);

            try
            {

                //await Execute();
                _logger.LogInformation("Iniciar. el envio  correo desde SendGrid : ");
                //string cadena = "{\"personalizations\": [{\"to\": [{\"email\": \"" + Configuration.GetSection("SendGrid:RemitenteEmail").Value.ToString().Replace("\x022\x022", "") + "\"}]}],\"from\": {\"email\": \"" + email.ToString().Replace("\x022\x022", "") + "\"},\"subject\": \"" + subject.ToString().Replace("\x022\x022", "") + "\",\"content\": [{\"type\": \"text/plain\", \"value\": \"" + message.ToString().Replace("\x022\x022", "") + "\"}]}";
                string cadena = "{\"personalizations\": [{\"to\": [{\"email\": \"" + email.ToString().Replace("\x022\x022", "").ToString().Replace("\x022\x022", "") + "\"}]}],\"from\": {\"email\": \"" + Configuration.GetSection("SendGrid:RemitenteEmail").Value.ToString().Replace("\x022\x022", "") + "\"},\"subject\": \"" + subject.ToString().Replace("\x022\x022", "") + "\",\"content\": [{\"type\": \"text/html\", \"value\": \"" + message.ToString().Replace("\x022\x022", "") + "\"}]}";
                //{\"personalizations\": [{\"to\": [{\"email\": \"viral_haker@hotmail.com\"}]}],\"from\": {\"email\": \"luis.cantin@componenteserviex.com\"},\"subject\": \"Sendgri nuevo!\",\"content\": [{\"type\": \"text/plain\", \"value\": \"Contenido!\"}]}

                using (var httpClient = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://api.sendgrid.com/v3/mail/send"))
                    {
                        request.Headers.TryAddWithoutValidation("Authorization", "Bearer " + apiKey.ToString());

                        //request.Content = new StringContent("{\"personalizations\": [{\"to\": [{\"email\": \"viral_haker@hotmail.com\"}]}],\"from\": {\"email\": \"luis.cantin@componenteserviex.com\"},\"subject\": \"Sendgri nuevo!\",\"content\": [{\"type\": \"text/plain\", \"value\": \"Contenido!\"}]}");
                        request.Content = new StringContent(cadena);
                        request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

                        var response = await httpClient.SendAsync(request);


                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation("ERROR. al enviar el correo desde SendGrid : " + ex.Message);
                throw;
            }

        }


        public void SendEmail(string subject, string message, string email)
        {
            try
            {
                _logger.LogInformation("Iniciar. el envio  correo desde Office : ");

                // Configuración del cliente de correo electrónico // outlook.office365.com 
                var smtpClient = new SmtpClient("smtp.office365.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(Configuration.GetSection("SendGrid:RemitenteEmailOffice365").Value, Configuration.GetSection("SendGrid:PasswordeOffice365").Value),
                    EnableSsl = true
                };

                // Crear correo electrónico
                var _emailMessage = new MailMessage();
                _emailMessage.From = new MailAddress(Configuration.GetSection("SendGrid:RemitenteEmailOffice365").Value);
                _emailMessage.To.Add(new MailAddress(email.ToString()));
                _emailMessage.CC.Add(new MailAddress("luis.cantin@componenteserviex.com"));
                _emailMessage.Subject = subject;
                _emailMessage.Body = message; //new MessageBody(BodyType.HTML, message); //message;
                _emailMessage.IsBodyHtml = true;
                // Envío de correo electrónico
                smtpClient.Send(_emailMessage);
            }
            catch (Exception ex)
            {
                _logger.LogInformation("ERROR. al enviar el correo desde Office 365 : " + ex.Message);
                throw;
            }

        }

    }
}
