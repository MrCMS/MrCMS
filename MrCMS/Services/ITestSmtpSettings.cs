using System;
using System.Net.Mail;
using Elmah;
using MrCMS.Entities.Multisite;
using MrCMS.Models;
using MrCMS.Services.Resources;
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
        private readonly IStringResourceProvider _resourceProvider;
        private readonly Site _site;

        public TestSmtpSettings(IGetSmtpClient getSmtpClient, ErrorSignal errorSignal, IStringResourceProvider resourceProvider, Site site)
        {
            _getSmtpClient = getSmtpClient;
            _errorSignal = errorSignal;
            _resourceProvider = resourceProvider;
            _site = site;
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
                        Body = _resourceProvider.GetValue("Admin - Test Email - Content", "Testing email functionality from " + _site.DisplayName),
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