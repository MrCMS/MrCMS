using System;
using System.Net.Mail;
using Elmah;
using MrCMS.Models;
using MrCMS.Settings;

namespace MrCMS.Services
{
    public interface ITestSmtpSettings
    {
        bool TestSettings(MailSettings settings, TestEmailInfo info);
    }

    public class TestSmtpSettings : ITestSmtpSettings
    {
        private readonly IGetSmtpClient _getSmtpClient;
        private readonly ErrorSignal _errorSignal;

        public TestSmtpSettings(IGetSmtpClient getSmtpClient, ErrorSignal errorSignal)
        {
            _getSmtpClient = getSmtpClient;
            _errorSignal = errorSignal;
        }
        public bool TestSettings(MailSettings settings, TestEmailInfo info)
        {
            try
            {
                using (var smtpClient = _getSmtpClient.GetClient(settings))
                {
                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(settings.SystemEmailAddress),
                        Subject = "SMTP Test",
                        Body = info.Content,
                    };
                    mailMessage.To.Add(new MailAddress(info.Email));

                    smtpClient.Send(mailMessage);
                }
                return true;
            }
            catch (Exception ex)
            {
                _errorSignal.Raise(ex);
                return false;
            }
        }
    }
}