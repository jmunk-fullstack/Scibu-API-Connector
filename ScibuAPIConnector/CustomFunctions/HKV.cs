﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ScibuAPIConnector.CustomFunctions
{
    public class HKV
    {
        public string _mailTo = UploadSettings.HkvMailTo;
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
                Host = "smtprelay.wearehostingyou.com", //Or Your SMTP Server Address
                //EnableSsl = true,
                Port = 25,
            };

            smtp.Send(mail);
        }
    }
}
