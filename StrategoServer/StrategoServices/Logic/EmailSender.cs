using MailKit.Net.Smtp;
using MimeKit;
using StrategoServices.Logic.Interfaces;
using System;
using System.Configuration;

namespace StrategoServices.Logic
{
    public class EmailSender : IEmailSender
    {
        private static EmailSender _instance;
        private string _mailHost;
        private int _port;
        private string _userMail;
        private string _password;

        private EmailSender()
        {
            _mailHost = ConfigurationManager.AppSettings["SmtpHost"];
            _port = int.Parse(ConfigurationManager.AppSettings["SmtpPort"]);
            _userMail = ConfigurationManager.AppSettings["EmailFromAddress"];
            _password = ConfigurationManager.AppSettings["EmailFromPassword"];
        }

        public static EmailSender Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new EmailSender();
                }
                return _instance;
            }
        }

        public void SendEmail(string destinationAddress)
        {
            SmtpClient smtpClient = null;
            try
            {
                var message = MakeMessage(destinationAddress);
                smtpClient = ConfigureMailClient();
                AuthenticateSmtpClient(smtpClient);
                SendMessage(smtpClient, message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al enviar el correo: {ex.Message}");
            }
            finally
            {
                if (smtpClient != null && smtpClient.IsConnected)
                {
                    DisconnectMailClient(smtpClient);
                }
            }
        }

        private BodyBuilder MakeMessageBody()
        {
            BodyBuilder body = new BodyBuilder
            {
                HtmlBody = "<h1>Bienvenido, tu contraseña se encuentra en el siguiente enlace: </h1>"
            };
            return body;
        }

        private MimeMessage MakeMessage(string destinationAddress)
        {
            MimeMessage message = new MimeMessage();
            message.From.Add(new MailboxAddress("Stratego", _userMail));
            message.To.Add(new MailboxAddress("Stratego", destinationAddress));
            message.Subject = "Recuperación de contraseña";
            message.Body = MakeMessageBody().ToMessageBody();
            return message;
        }

        private SmtpClient ConfigureMailClient()
        {
            var client = new SmtpClient
            {
                CheckCertificateRevocation = false
            };
            client.Connect(_mailHost, _port, MailKit.Security.SecureSocketOptions.StartTls);
            return client;
        }

        private void AuthenticateSmtpClient(SmtpClient smtpClient)
        {
            smtpClient.Authenticate(_userMail, _password);
        }

        private void SendMessage(SmtpClient smtpClient, MimeMessage message)
        {
            smtpClient.Send(message);
        }

        private void DisconnectMailClient(SmtpClient smtpClient)
        {
            smtpClient.Disconnect(true);
        }

    }
}