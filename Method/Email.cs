using MimeKit;
using MimeKit.Text;
using MailKit.Net.Smtp;
using MailKit.Security;


namespace rentalcar_backend.Method
{
    public class Email
    {
        public static IConfiguration _configuration;
        private static string _email;
        private static string _password;

        //Get appsettings.json variable config
        public static void Init(IConfiguration configuration)
        {
            _configuration = configuration;
            _email = configuration["Email:Email"];
            _password = configuration["Email:Password"];
        }

        //Sending Email Verification
        public static bool SendEmail(string emailTo, string subjectText, string bodyHtml)
        {
            try
            {
                // MimeKit - sebuah pensil untuk menulis surat
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(_email));
                email.To.Add(MailboxAddress.Parse(emailTo));
                email.Subject = subjectText;
                email.Body = new TextPart(TextFormat.Html) { Text = bodyHtml };

                // MailKit - sebuah kantor pos untuk mengirim surat
                using (var smtp = new SmtpClient())
                {
                    // gmail smtp port: https://support.google.com/mail/answer/7126229?hl=en#zippy=%2Cstep-change-smtp-other-settings-in-your-email-client
                    smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                    smtp.Authenticate(_email, _password);
                    smtp.Send(email);
                    smtp.Disconnect(true);
                }

                return true;
            }
            catch(Exception e)
            {
                throw new Exception (e.Message);
            }
        }


    }
}
