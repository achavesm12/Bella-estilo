using BelaEstilo.Application.Config;
using BelaEstilo.Application.Services.Interfaces;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BelaEstilo.Application.Services.Implementations
{
    public class ServiceEmail : IServiceEmail
    {
        private readonly EmailSettings _emailSettings;

        public ServiceEmail(IOptions<EmailSettings> options)
        {
            _emailSettings = options.Value;
        }

        public async Task EnviarFacturaPorCorreoAsync(string destinatario, string asunto, string mensaje,
            byte[] adjuntoPdf, string nombreAdjunto)
        {
            using (var mail = new MailMessage())
            {
                mail.From = new MailAddress(_emailSettings.FromEmail, _emailSettings.FromName);
                mail.To.Add(destinatario);
                mail.Subject = asunto;
                mail.Body = mensaje;
                mail.IsBodyHtml = true;

                // Adjuntar el PDF
                if (adjuntoPdf != null)
                {
                    var adjunto = new Attachment(new System.IO.MemoryStream(adjuntoPdf), nombreAdjunto, "application/pdf");
                    mail.Attachments.Add(adjunto);
                }

                using (var smtp = new SmtpClient(_emailSettings.Host, _emailSettings.Port))
                {
                    smtp.Credentials = new NetworkCredential(_emailSettings.FromEmail, _emailSettings.Password);
                    smtp.EnableSsl = _emailSettings.EnableSSL;
                    await smtp.SendMailAsync(mail);
                }
            }
        }

    }
}
