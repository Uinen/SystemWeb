using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace GestioniDirette.Mail
{
    public class Message
    {
        private const int Timeout = 180000;
        private readonly string _host;
        private readonly int _port;
        private readonly string _user;
        private readonly string _pass;
        private readonly bool _ssl;

        public string Sender { get; set; }
        public string Recipient { get; set; }
        public string RecipientCC { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string AttachmentFile { get; set; }

        public Message()
        {
            _host = ConfigurationManager.AppSettings["Server"];
            _port = int.Parse(ConfigurationManager.AppSettings["Porta"]);
            _user = ConfigurationManager.AppSettings["ServerUserName"];
            _pass = ConfigurationManager.AppSettings["ServerUserPassword"];
            _ssl = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSSL"]);
        }

        public void Send()
        {
            try
            {
                Attachment att = null;
                var message = new MailMessage(Sender, Recipient, Subject, Body) { IsBodyHtml = true };
                if (RecipientCC != null)
                {
                    message.Bcc.Add(RecipientCC);
                }
                var smtp = new SmtpClient(_host, _port);

                if (!string.IsNullOrEmpty(AttachmentFile))
                {
                    if (File.Exists(AttachmentFile))
                    {
                        att = new Attachment(AttachmentFile);
                        message.Attachments.Add(att);
                    }
                }

                if (_user.Length > 0 && _pass.Length > 0)
                {
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(_user, _pass);
                    smtp.EnableSsl = _ssl;
                }

                smtp.Send(message);

                if (att != null)
                    att.Dispose();
                message.Dispose();
                smtp.Dispose();
            }

            catch (Exception ex)
            {
                string.Format("{0}, {1}", ex.Message, ex.Data);
            }
        }

    }
}
