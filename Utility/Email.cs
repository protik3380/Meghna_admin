using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace EFreshStore.Utility
{
    public class Email
    {
        public static void SendEmail(string subject, string body, MailAddress toMailAddress)
        {
            var fromAddress = new MailAddress("meghna.ecommerce@gmail.com", "Meghna e-Commerce");
            //var toAddress = new MailAddress("to@example.com", "To Name");
            var receivers = new List<MailAddress>
            {
                toMailAddress
            };
            const string fromPassword = "Meghna2019";
            //const string subject = "Subject";
            //const string body = "Body";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            foreach (var toAddress in receivers)
            {
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    smtp.Send(message);
                }
            }
            //using (var message = new MailMessage(fromAddress, toAddress)
            //{
            //    Subject = subject,
            //    Body = body
            //})
            //{
            //    smtp.Send(message);
            //}
        }
    }
}