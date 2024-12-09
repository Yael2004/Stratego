using log4net;
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using StrategoServices.Logic.Interfaces;
using System;
using System.Configuration;
using System.Net.Sockets;

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
        private static readonly string _logMessage = "Sending email error: ";
        private static readonly string _gameName = "Stratego";

        private EmailSender()
        {
            _mailHost = ConfigurationManager.AppSettings["SmtpHost"];
            _port = int.Parse(ConfigurationManager.AppSettings["SmtpPort"]);
            _userMail = ConfigurationManager.AppSettings["EmailFromAddress"];
            _password = ConfigurationManager.AppSettings["EmailFromPassword"];
        }

        public static EmailSender Instance => _instance.Value;

        public bool SendVerificationEmail(string destinationAddress, string code)
        {
            bool result = false;
            try
            {
                var message = MakeVerificationMessage(destinationAddress, code);
                _smtpClient = ConfigureMailClient();
                AuthenticateSmtpClient(_smtpClient);
                _smtpClient.Send(message);
                result = true;
            }
            catch (ProtocolException pex)
            {
                log.Error(_logMessage, pex);
            }
            catch (Exception ex)
            {
                log.Error(_logMessage, ex);
            }
            finally
            {
                DisconnectMailClient();
            }
            return result;
        }

        public bool SendInvitationEmail(string destinationAddress, string code)
        {
            bool result = false;
            try
            {
                var mailMessage = MakeInvitationMessage(destinationAddress, code);
                _smtpClient = ConfigureMailClient();
                AuthenticateSmtpClient(_smtpClient);
                _smtpClient.Send(mailMessage);
                result = true;
            }
            catch (ProtocolException pex)
            {
                log.Error(_logMessage, pex);
            }
            catch (Exception ex)
            {
                log.Error(_logMessage, ex);
            }
            finally
            {
                DisconnectMailClient();
            }
            return result;
        }

        private MimeMessage MakeVerificationMessage(string destinationAddress, string verificationCode)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_gameName, _userMail));
            message.To.Add(new MailboxAddress(_gameName, destinationAddress));
            message.Subject = _gameName;
            message.Body = new TextPart("plain")
            {
                Text = $"Verification code: {verificationCode}"
            };
            return message;
        }

        private MimeMessage MakeInvitationMessage(string destinationAddress, string roomCode)
        {
            var mailMessage = new MimeMessage();
            mailMessage.From.Add(new MailboxAddress(_gameName, _userMail));
            mailMessage.To.Add(new MailboxAddress(_gameName, destinationAddress));
            mailMessage.Subject = _gameName;
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
            try
            {
                client.Connect(_mailHost, _port, MailKit.Security.SecureSocketOptions.StartTls);
            }
            catch (SocketException sex)
            {
                log.Error("Connecting to mail server error: ", sex);
            }
            catch (Exception ex)
            {
                log.Error("Connecting to mail server error: ", ex);
            }
            return client;
        }

        private void AuthenticateSmtpClient(SmtpClient smtpClient)
        {
            try
            {
                smtpClient.Authenticate(_userMail, _password);
            }
            catch (AuthenticationException aex)
            {
                log.Error("Authentication error: ", aex);
            }
            catch (Exception ex)
            {
                log.Error("Authentication error: ", ex);
            }
        }

        private void DisconnectMailClient()
        {
            try
            {
                if (_smtpClient != null && _smtpClient.IsConnected)
                {
                    _smtpClient.Disconnect(true);
                    _smtpClient.Dispose();
                    _smtpClient = null;
                }
            }
            catch (Exception ex)
            {
                log.Error("Disconnecting from mail server error: ", ex);
            }
        }

        public void Dispose()
        {
            DisconnectMailClient();
        }

    }
}
