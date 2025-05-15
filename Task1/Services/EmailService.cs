using System.Net.Mail;
using Task1.DTO.Auth;
using System.Net;


namespace Task1.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<bool> SendEmailAsync(EmailSendDto emailSend)
        {
            Console.WriteLine(emailSend.To);
            Console.WriteLine(emailSend.Body);
            Console.WriteLine(emailSend.Subject);

            string MailServer = _config["EmailSettings:MailServer"];
            string FromEmail = _config["EmailSettings:FromEmail"];
            string Password = _config["EmailSettings:Password"];
            string SenderName = _config["EmailSettings:SenderName"];
            int Port = Convert.ToInt32(_config["EmailSettings:MailPort"]);

            try
            {
                var client = new SmtpClient(MailServer, Port)
                {
                    Credentials = new NetworkCredential(FromEmail, Password),
                    EnableSsl = true,
                };

                MailAddress fromAddress = new MailAddress(FromEmail, SenderName);

                MailMessage mailMessage = new MailMessage
                {
                    From = fromAddress,
                    Subject = emailSend.Subject,
                    Body = emailSend.Body,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(emailSend.To);
                await client.SendMailAsync(mailMessage);
                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SendEmailAsync] Error sending email: {ex.Message}");
                return false;
            }
        }

    }

}
