using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace RMS.UI.View.Utility
{
    public class MyUtility
    {
        public static string BuildReceiptBody(List<string> lstContent)
        {
            string body = string.Empty;
            foreach (string msg in lstContent)
            {
                body = body + msg + "<br/>" +Environment.NewLine;
            }
            return body;
        }

        public static string BuildPrintReceipt(List<string> lstContent)
        {
            string body = string.Empty;
            body = "RMS Restaurant" + Environment.NewLine;
            body = body + "-----------------------------------------------" + Environment.NewLine;
            foreach (string msg in lstContent)
            {
                body = body + msg + Environment.NewLine;
            }
            return body;
        }

        public static void SendEmailMailToCustomer(string from, string to, string bcc, string cc, string subject, string body)
        {
            MailMessage mMailMessage = new MailMessage();
            mMailMessage.From = new MailAddress(from);
            mMailMessage.To.Add(new MailAddress(to));
            if ((bcc != null) && (bcc != string.Empty))
            {
                mMailMessage.Bcc.Add(new MailAddress(bcc));
            }
            if ((cc != null) && (cc != string.Empty))
            {
                mMailMessage.CC.Add(new MailAddress(cc));
            }
            mMailMessage.Subject = subject;
            mMailMessage.Body = body;
            mMailMessage.IsBodyHtml = true;
            mMailMessage.Priority = MailPriority.High;

            //use yahoo mail to send email
            //MailMessage oMail = new MailMessage(new MailAddress("username@yahoo.com"), new MailAddress("username@yahoo.com"));
            SmtpClient oSmtp = new SmtpClient();
            oSmtp.Host = "smtp.mail.yahoo.com";
            oSmtp.Credentials = new NetworkCredential("rms_restaurant@yahoo.com", "cs@4444");
            oSmtp.EnableSsl = true;
            oSmtp.Port = 25; // 587; for security purpose
            //oSmtp.Port = 465;

            // detect SSL type automatically
            
            oSmtp.Send(mMailMessage);         
        }

    }
}
