using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ScibuAPIConnector.CustomFunctions
{
    public class HKV
    {
        public string _mailTo = "maarten@hkvochten.nl";
        public string _mailFrom = "support@qubics.nl";

        public void SendMail(string body, string subject)
        {
            MailMessage mail = new MailMessage(_mailFrom, _mailTo)
            {
                Subject = subject,
                Body = body,

                IsBodyHtml = true
            };

            SmtpClient smtp = new SmtpClient
            {
                Host = "smtp.office365.com", //Or Your SMTP Server Address
                Credentials = new System.Net.NetworkCredential("t.bizot@qubics.nl", "f48N885jngj8"),

                EnableSsl = true
            };

            smtp.Send(mail);
        }
    }
}
