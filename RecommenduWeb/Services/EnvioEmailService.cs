using System.Net;
using System.Net.Mail;

namespace RecommenduWeb.Services
{
    public class EnvioEmailService
    {
        public bool EnvioEmail(string destinatario, string assunto, string mensagem)
        {
            try
            {
                MailMessage mailMessage = new MailMessage();

                mailMessage.From = new MailAddress("no-reply-recommendu@outlook.com");
                mailMessage.CC.Add(destinatario);
                mailMessage.Subject = assunto;
                mailMessage.IsBodyHtml = true;
                mailMessage.Body = mensagem;

                SmtpClient smtpClient = new SmtpClient("smtp-mail.outlook.com", Convert.ToInt32("587"));
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential("no-reply-recommendu@outlook.com", "Tcc2023!");
                smtpClient.EnableSsl = true;
                smtpClient.Send(mailMessage);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
