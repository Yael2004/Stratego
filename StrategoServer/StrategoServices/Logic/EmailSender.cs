using log4net;
using MailKit.Net.Smtp;
using MimeKit;
using StrategoServices.Logic.Interfaces;
using System;
using System.Configuration;

namespace StrategoServices.Logic
{
    public class EmailSender : IEmailSender, IDisposable
    {
        private static readonly Lazy<EmailSender> _instance = new Lazy<EmailSender>(() => new EmailSender());
        private readonly string _mailHost;
        private readonly int _port;
        private readonly string _userMail;
        private readonly string _password;
        private SmtpClient _smtpClient;
        private static readonly ILog log = LogManager.GetLogger(typeof(EmailSender));

        private EmailSender()
        {
            _mailHost = ConfigurationManager.AppSettings["SmtpHost"];
            _port = int.Parse(ConfigurationManager.AppSettings["SmtpPort"]);
            _userMail = ConfigurationManager.AppSettings["EmailFromAddress"];
            _password = ConfigurationManager.AppSettings["EmailFromPassword"];
        }

        public static EmailSender Instance => _instance.Value;

        public void SendVerificationEmail(string destinationAddress, string verificationCode)
        {
            try
            {
                var message = MakeVerificationMessage(destinationAddress, verificationCode);
                _smtpClient = ConfigureMailClient();
                AuthenticateSmtpClient(_smtpClient);
                _smtpClient.Send(message);
            }
            catch (Exception ex)
            {
                log.Error("Sending email error: ", ex);
            }
            finally
            {
                DisconnectMailClient();
            }
        }

        public void SendInvitationEmail(string destinationAddress, string message)
        {
            try
            {
                var mailMessage = MakeInvitationMessage(destinationAddress, message);
                _smtpClient = ConfigureMailClient();
                AuthenticateSmtpClient(_smtpClient);
                _smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                log.Error("Sending email error: ", ex);
            }
            finally
            {
                DisconnectMailClient();
            }
        }

        private MimeMessage MakeVerificationMessage(string destinationAddress, string verificationCode)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Stratego", _userMail));
            message.To.Add(new MailboxAddress("Stratego", destinationAddress));
            message.Subject = "Stratego";
            message.Body = new TextPart("plain")
            {
                Text = $"Verification code: {verificationCode}"
            };
            return message;
        }

        private MimeMessage MakeInvitationMessage(string destinationAddress, string roomCode)
        {
            var mailMessage = new MimeMessage();
            mailMessage.From.Add(new MailboxAddress("Stratego", _userMail));
            mailMessage.To.Add(new MailboxAddress("Stratego", destinationAddress));
            mailMessage.Subject = "Stratego";
            mailMessage.Body = new TextPart("plain")
            {
                Text = $"Let's play, join to room: {roomCode}"
            };
            return mailMessage;
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

        private void DisconnectMailClient()
        {
            if (_smtpClient != null && _smtpClient.IsConnected)
            {
                _smtpClient.Disconnect(true);
                _smtpClient.Dispose();
                _smtpClient = null;
            }
        }

        public void Dispose()
        {
            DisconnectMailClient();
        }

    }
}
